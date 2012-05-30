using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace SpriteEditor
{
    public partial class frmMain : Form
    {
        //private static Regex _stateName = new Regex(@"\d+$");
        //private static Regex _stateNumber = new Regex(@"\d$");
        private string _request = "";
        private Image _imageOriginal;
        private List<string> _imageLocations;
        private List<Bitmap> _images;
        private readonly string[] _possibleParams = {
                                                        "uri", "flipX", "sizeMultiplier", "sizeDivider", "frameDelay",
                                                        "cropX", "cropY", "cropW", "cropH",
                                                        "offsX", "offsY", "isChain", "usePrevious", "autoClose",
                                                        "transparent", "walkMultiplier"
                                                    };
        private readonly string[] _possibleStates = {
                                                        "SPRITE_META_DATA", "SPRITE_STATE_DEFAULT",
                                                        "SPRITE_STATE_STAND_LEFT", "SPRITE_STATE_STAND_RIGHT",
                                                        "SPRITE_STATE_WALK_LEFT", "SPRITE_STATE_WALK_RIGHT",
                                                        "SPRITE_STATE_JUMP_LEFT", "SPRITE_STATE_JUMP_RIGHT",
                                                        "SPRITE_STATE_DESTROY_LEFT", "SPRITE_STATE_DESTROY_RIGHT",
                                                        "SPRITE_STATE_RUN_LEFT", "SPRITE_STATE_RUN_RIGHT",
                                                        "SPRITE_STATE_FLY_LEFT", "SPRITE_STATE_FLY_RIGHT"
                                                    };

        public frmMain() { InitializeComponent(); }

        // application start
        private void frmMain_Load(object sender, EventArgs e)
        {
            _images = new List<Bitmap>();
            _imageLocations = new List<string>();
            imageDisplay.Width = imageDisplay.Image.Width;
            imageDisplay.Height = imageDisplay.Image.Height;
            _imageOriginal = imageDisplay.Image;
            populateRecentFiles();

            foreach (string s in _possibleParams)
            {
                ToolStripMenuItem addThis = new ToolStripMenuItem(s, null, addParameter_Click);
                ToolStripMenuItem thisToo = new ToolStripMenuItem(s, null, addParameter_Click);
                addParameterToolStripMenuItem.DropDownItems.Add(addThis);
                addParameterMenuItem.DropDownItems.Add(thisToo);
            }

            foreach (string s in _possibleStates)
            {
                ToolStripMenuItem addThis = new ToolStripMenuItem(s, null, addState_Click);
                ToolStripMenuItem thisToo = new ToolStripMenuItem(s, null, addState_Click);
                addStateMenuItem.DropDownItems.Add(addThis);
                addStateToolStripMenuItem.DropDownItems.Add(thisToo);
            }
        }

        // event handler for add parameter
        private void addParameter_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem paramToAdd = sender as ToolStripMenuItem;
            if (treeView.SelectedNode != null && treeView.SelectedNode.Tag.Equals("State") && paramToAdd != null)
            {
                treeView.SelectedNode.Nodes.Add(getNewNode(paramToAdd.Text, "Parameter"));
                treeView.SelectedNode.Expand();
            }
            else
                MessageBox.Show("Please select a STATE in the tree to add the parameter to.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // file > import
        private void menuImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog
                                              {
                                                  InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                  Filter = "Sprite Files(*.spr;*.spi)|*.spr;*.spi|All files (*.*)|*.*",
                                                  RestoreDirectory = true,
                                                  Multiselect = false
                                              };
                DialogResult result = openFile.ShowDialog();
                if (result == DialogResult.OK)
                    loadSprite(openFile.FileName);
                treeView.TreeViewNodeSorter = new NodeSorter();
            }
            catch (FileNotFoundException ex)
            {
                statusLabel.Text = "Error: File not found.";
                MessageBox.Show("Error: File not found.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // helper to load spr files and populate controls
        private void loadSprite(string fileLocation)
        {
            treeView.Nodes.Clear();

            //read file into string and close
            StreamReader myFile = new StreamReader(fileLocation);
            string jsonString = myFile.ReadToEnd();
            myFile.Close();

            // deserialize jsonString into Sprite instance and add tree root
            Sprite sprizite = JsonConvert.DeserializeObject<Sprite>(jsonString);
            sprizite.fileName = fileLocation;
            sprizite.safeFileName = getFileNameFromPath(fileLocation);
            treeView.Nodes.Add(getNewNode(sprizite.safeFileName, "File"));

            // get list of states and iterate over states
            List<string> validatedStateList = getStatesFromJSONString(jsonString);
            int stateNodeIndex = 0;
            foreach (string state in validatedStateList)
            {
                // add SPRITE_* nodes
                treeView.Nodes[0].Nodes.Add(getNewNode(state, "State"));

                // Special Case: Preloaded STATES ( META and DEFAULT )
                if (state.Equals("SPRITE_META_DATA") || state.Equals("SPRITE_STATE_DEFAULT"))
                {
                    // list the properties to populate
                    List<string> propList = sprizite.getNonNullProperties(state.Equals("SPRITE_META_DATA") ? "meta" : "default", sprizite);

                    // add parameterName nodes for each state
                    foreach (string vs in propList)
                    {
                        if (vs.Equals("fixtures") || vs.Equals("credits") || vs.Equals("spawn"))
                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes.Add(getNewNode(vs, "Index"));
                        else
                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes.Add(getNewNode(vs, "Parameter"));
                    }

                    // additional processing for subnodes in each root depending on if property is: value, array, or subarray
                    int k = 0;
                    foreach (TreeNode n in treeView.Nodes[0].Nodes[stateNodeIndex].Nodes)
                    {
                        string prop = n.Text;

                        //process values
                        string val = fetchParameter(sprizite, prop);

                        // contains array or sub array - extra processing
                        if (val.Equals("||ERROR||"))
                        {
                            // BEGIN process sub arrays (credits, fixtures, spawn)
                            if (prop.Equals("credits"))
                            {
                                for (int z = 0; z < sprizite.SPRITE_META_DATA.credits.Count; z++)
                                {
                                    treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode("[" + (z + 1) + "]", "Index"));

                                    if (sprizite.SPRITE_META_DATA.credits[z].author != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("author", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[0].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.credits[z].author, "Value"));
                                    }
                                    if (sprizite.SPRITE_META_DATA.credits[z].description != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("description", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[1].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.credits[z].description, "Value"));
                                    }
                                    if (sprizite.SPRITE_META_DATA.credits[z].url != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("url", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[2].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.credits[z].url, "Value"));
                                    }
                                }
                                k++;
                                continue;
                            }
                            if (prop.Equals("fixtures"))
                            {
                                for (int z = 0; z < sprizite.SPRITE_STATE_DEFAULT.fixtures.Count; z++)
                                {
                                    treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode("[" + (z + 1) + "]", "Index"));

                                    if (sprizite.SPRITE_STATE_DEFAULT.fixtures[z].x != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("x", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[0].Nodes.Add(getNewNode(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].x, "Value"));
                                    }
                                    if (sprizite.SPRITE_STATE_DEFAULT.fixtures[z].y != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("y", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[1].Nodes.Add(getNewNode(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].y, "Value"));
                                    }
                                    if (sprizite.SPRITE_STATE_DEFAULT.fixtures[z].w != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("w", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[2].Nodes.Add(getNewNode(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].w, "Value"));
                                    }
                                    if (sprizite.SPRITE_STATE_DEFAULT.fixtures[z].h != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("h", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[3].Nodes.Add(getNewNode(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].h, "Value"));
                                    }
                                }
                                k++;
                                continue;
                            }
                            if (prop.Equals("spawn"))
                            {
                                for (int z = 0; z < sprizite.SPRITE_META_DATA.spawn.Count; z++)
                                {
                                    treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode("[" + (z + 1) + "]", "Index"));
                                    
                                    if (sprizite.SPRITE_META_DATA.spawn[z].uri != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("uri", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[0].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.spawn[z].uri, "Value"));
                                    }
                                    if (sprizite.SPRITE_META_DATA.spawn[z].spawnX != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("spawnX", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[1].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.spawn[z].spawnX, "Value"));
                                    }
                                    if (sprizite.SPRITE_META_DATA.spawn[z].spawnY != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("spawnY", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[2].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.spawn[z].spawnY, "Value"));
                                    }
                                    if (sprizite.SPRITE_META_DATA.spawn[z].spawnExplode != null)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("spawnExplode", "Parameter"));
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[3].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.spawn[z].spawnExplode, "Value"));
                                    }
                                }
                                k++;
                                continue;
                            }

                            // process arrays
                            if (prop.Equals("flags") || prop.Equals("actions"))
                            {
                                int arrayCount = 0;
                                if (prop.Equals("flags"))
                                    arrayCount = sprizite.SPRITE_META_DATA.flags.Count;
                                else if (prop.Equals("actions"))
                                    arrayCount = sprizite.SPRITE_META_DATA.actions.Count;

                                for (int z = 0; z < arrayCount; z++)
                                {
                                    if (prop.Equals("flags"))
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.flags[z], "Parameter"));
                                    else if (prop.Equals("actions"))
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.actions[z], "Value"));
                                }
                                k++; continue;
                            }
                        }
                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode(val, "Value"));
                        k++;
                    }
                    stateNodeIndex++;
                    continue;
                } // end processing of META and DEFAULT

                // SPRITE_STATE_* detected - start processing using JTokens
                SpriteState addThis = new SpriteState(state);

                // Special Case: check if state is empty
                if (!JObject.Parse(jsonString)[state].Any()) { stateNodeIndex++; continue; }

                JToken iteratorToken = JObject.Parse(jsonString)[state].First();

                int j = 0;
                // check each token and parse
                while (iteratorToken != null)
                {
                    string propName = ((JProperty)iteratorToken).Name;
                    string propValue = ((JProperty)iteratorToken).Value.ToString();
                    treeView.Nodes[0].Nodes[stateNodeIndex].Nodes.Add(getNewNode(propName, "Parameter"));
                    treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[j].Nodes.Add(getNewNode(propValue, "Value"));
                    addThis.setParameter(propName, propValue);

                    j++;
                    iteratorToken = iteratorToken.Next;
                }
                sprizite.addNewState(addThis);
                stateNodeIndex++;
            }

            // prepare controls by clearing them
            zoomSlider.Value = 1;
            _images.Clear();
            _imageLocations.Clear();
            dependenciesToolStripMenuItem.DropDownItems.Clear();

            // extract images from uri params and load them
            TreeNode[] imagesToLoad = treeView.Nodes.Find("uri", true);
            foreach (TreeNode u in imagesToLoad)
            {
                string imageLoc = u.FirstNode.Text;
                string fullImagePath = String.Concat(sprizite.fileName.Substring(0, sprizite.fileName.LastIndexOf("\\", StringComparison.Ordinal)), "\\", imageLoc);
                if (_imageLocations.Contains(fullImagePath))
                    continue;

                // add *.spi files to associated files list
                if (fullImagePath.EndsWith("spi") || fullImagePath.EndsWith("spr"))
                {
                    ToolStripMenuItem dynamicMenuItem = new ToolStripMenuItem(getFileNameFromPath(fullImagePath));
                    dynamicMenuItem.Click += dynamicMenuItem_Click;
                    dynamicMenuItem.Tag = fullImagePath;
                    dependenciesToolStripMenuItem.DropDownItems.Add(dynamicMenuItem);
                    continue;
                }
                if (File.Exists(fullImagePath))
                    _images.Add(new Bitmap(fullImagePath));
                _imageLocations.Add(fullImagePath);
            }

            // display DEFAULT image in picturebox
            if (_imageLocations.Any() && File.Exists(_imageLocations[0]))
            {
                imageDisplay.Image = new Bitmap(_imageLocations[0]);
                imageDisplay.Tag = sprizite.SPRITE_STATE_DEFAULT.uri;
                imageDisplay.Width = imageDisplay.Image.Width;
                imageDisplay.Height = imageDisplay.Image.Height;
                _imageOriginal = imageDisplay.Image;
            }

            // add spr to associated files list
            ToolStripMenuItem originalFile = new ToolStripMenuItem(getFileNameFromPath(fileLocation));
            originalFile.Click += dynamicMenuItem_Click;
            originalFile.Tag = fileLocation;
            dependenciesToolStripMenuItem.DropDownItems.Add(originalFile);

            // all done populating controls - finalize
            statusLabel.Text = sprizite.safeFileName + " was imported successfully.";
            treeView.Nodes[0].Expand();
            addMRU(sprizite.fileName);
            populateRecentFiles();
        }

        public static string getFileNameFromPath(string filePath)
        {
            return filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
        }

        // used for processing META_DATA and STATE_DEFAULT
        private static string fetchParameter(Sprite sprizite, string prop)
        {
            string val = "||ERROR||";
            if (prop.Equals("version"))
                val = sprizite.SPRITE_META_DATA.version;
            if (prop.Equals("uri"))
                val = sprizite.SPRITE_STATE_DEFAULT.uri;
            if (prop.Equals("cropX"))
                val = sprizite.SPRITE_STATE_DEFAULT.cropX;
            if (prop.Equals("cropY"))
                val = sprizite.SPRITE_STATE_DEFAULT.cropY;
            if (prop.Equals("cropW"))
                val = sprizite.SPRITE_STATE_DEFAULT.cropW;
            if (prop.Equals("cropH"))
                val = sprizite.SPRITE_STATE_DEFAULT.cropH;
            if (prop.Equals("transparent"))
                val = sprizite.SPRITE_STATE_DEFAULT.transparent;
            if (prop.Equals("frameDelay"))
                val = sprizite.SPRITE_STATE_DEFAULT.frameDelay;
            if (prop.Equals("sizeMultiplier"))
                val = sprizite.SPRITE_STATE_DEFAULT.sizeMultiplier;
            if (prop.Equals("autoClose"))
                val = sprizite.SPRITE_STATE_DEFAULT.autoClose;
            if (prop.Equals("isChain"))
                val = sprizite.SPRITE_STATE_DEFAULT.isChain;
            if (prop.Equals("flipX"))
                val = sprizite.SPRITE_STATE_DEFAULT.flipX;
            if (prop.Equals("sizeDivider"))
                val = sprizite.SPRITE_STATE_DEFAULT.sizeDivider;
            if (prop.Equals("offsX"))
                val = sprizite.SPRITE_STATE_DEFAULT.offsX;
            if (prop.Equals("offsY"))
                val = sprizite.SPRITE_STATE_DEFAULT.offsY;
            if (prop.Equals("usePrevious"))
                val = sprizite.SPRITE_STATE_DEFAULT.usePrevious;
            if (prop.Equals("walkMultiplier"))
                val = sprizite.SPRITE_STATE_DEFAULT.walkMultiplier;
            return val;
        }

        // file > exit
        private void menuExit_Click(object sender, EventArgs e) { Application.Exit(); }  // TO DO:  Save App settings

        // file > new
        private void newFileMenuItem_Click(object sender, EventArgs e)
        {

            // confirm with user that unsaved changed will be lost
            if (treeView.Nodes.Count != 0)
            {
                DialogResult result = MessageBox.Show("You will lose any unsaved changes to " + treeView.Nodes[0].Text + ".", "New File", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                    return;
            }

            // display sprite wizard, get values, process values
            using (var form = new frmWizard())
            {
                DialogResult result2 = form.ShowDialog();
                if (result2 == DialogResult.OK)
                {
                    // clear controls for new sprite
                    treeView.Nodes.Clear();
                    _images.Clear();
                    _imageLocations.Clear();
                    imageDisplay.Image = Properties.Resources.favicon;
                    imageDisplay.Height = imageDisplay.Image.Height;
                    imageDisplay.Width = imageDisplay.Image.Width;

                    // store values from Sprite Wizard
                    Dictionary<string, string> sprInfo = form._spriteInfo;
                    List<string> actions = form._actions;
                    List<string> flags = form._flags;
                    List<string> states = form._states;

                    // add required nodes
                    treeView.Nodes.Add(getNewNode(sprInfo["name"], "File"));
                    treeView.Nodes[0].Nodes.Add(getNewNode("SPRITE_META_DATA", "State"));
                    treeView.Nodes[0].Nodes.Add(getNewNode("SPRITE_STATE_DEFAULT", "State"));

                    foreach (KeyValuePair<string, string> i in sprInfo)
                    {
                        switch (i.Key)
                        {
                            case "name":
                                break;
                            case "version":
                                treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes.Add(getNewNode("version", "Parameter"));
                                treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["version"].Nodes.Add(getNewNode(i.Value, "Value"));
                                break;
                            case "author":
                            case "description":
                            case "url":
                                if (treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["credits"] == null)
                                {
                                    treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes.Add(getNewNode("credits", "Index"));
                                    treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["credits"].Nodes.Add(getNewNode("[1]", "Index"));
                                }
                                treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["credits"].Nodes["[1]"].Nodes.Add(getNewNode(i.Key, "Value"));
                                treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["credits"].Nodes["[1]"].Nodes[i.Key].Nodes.Add(getNewNode(i.Value, "Value"));
                                break;
                        }
                    }

                    if (actions.Count != 0)
                    {
                        treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes.Add(getNewNode("actions", "Parameter"));
                        foreach (string a in actions)
                            treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["actions"].Nodes.Add(getNewNode(a, "Value"));
                    }

                    if (flags.Count != 0)
                    {
                        treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes.Add(getNewNode("flags", "Parameter"));
                        foreach (string f in flags)
                            treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["flags"].Nodes.Add(getNewNode(f, "Value"));
                    }

                    if (states.Count != 0)
                    {
                        foreach (string s in states)
                            treeView.Nodes[0].Nodes.Add(getNewNode(s, "State"));
                    }
                    treeView.TreeViewNodeSorter = new NodeSorter();
                    treeView.Nodes[0].Expand();
                }
            }
        }

        // image click
        private void imageDisplay_Click(object sender, EventArgs e)
        {
            if (_request.Equals("grabHTML"))
            {
                string[] coords = statusCoordsLabel.Text.Split(new[] { 'X', 'Y', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Bitmap getThisColor = (Bitmap)imageDisplay.Image;
                Color chosenColor = getThisColor.GetPixel(int.Parse(coords[0]), int.Parse(coords[1]));
                if (treeView.SelectedNode == null) { MessageBox.Show("Please select a valid Tree Node to place the value in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                string chosenColorHTML = hexConverter(chosenColor);
                TreeNode addHere = treeView.SelectedNode;
                if (addHere.Tag.Equals("Value"))
                    addHere.Text = chosenColorHTML;
                else
                {
                    if (addHere.Tag.Equals("Parameter") && addHere.Nodes.Count == 0)
                        addHere.Nodes.Add(getNewNode(chosenColorHTML, "Value"));
                    else
                    {
                        addHere.Nodes[0].Remove();
                        addHere.Nodes.Add(getNewNode(chosenColorHTML, "Value"));
                    }
                }
                _request = "";
                statusLabel.Text = chosenColorHTML + " was selected.";
            }
            else if (_request.Equals("grabX"))
            {
                string[] coords = statusCoordsLabel.Text.Split(new[] { 'X', 'Y', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (treeView.SelectedNode == null) { MessageBox.Show("Please select a valid Tree Node to place the value in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                TreeNode addHere = treeView.SelectedNode;
                if (addHere.Tag.Equals("Value"))
                    addHere.Text = coords[0];
                else
                {
                    if (addHere.Tag.Equals("Parameter") && addHere.Nodes.Count == 0)
                        addHere.Nodes.Add(getNewNode(coords[0], "Value"));
                    else
                    {
                        addHere.Nodes[0].Remove();
                        addHere.Nodes.Add(getNewNode(coords[0], "Value"));
                    }
                }
                statusLabel.Text = treeView.SelectedNode.FullPath + " set to " + coords[0];
                _request = "";
            }
            else if (_request.Equals("grabY"))
            {
                string[] coords = statusCoordsLabel.Text.Split(new[] { 'X', 'Y', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (treeView.SelectedNode == null) { MessageBox.Show("Please select a valid Tree Node to place the value in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                TreeNode addHere = treeView.SelectedNode;
                if (addHere.Tag.Equals("Value"))
                    addHere.Text = coords[1];
                else
                {
                    if (addHere.Tag.Equals("Parameter") && addHere.Nodes.Count == 0)
                        addHere.Nodes.Add(getNewNode(coords[1], "Value"));
                    else
                    {
                        addHere.Nodes[0].Remove();
                        addHere.Nodes.Add(getNewNode(coords[1], "Value"));
                    }
                }
                statusLabel.Text = treeView.SelectedNode.FullPath + " set to " + coords[0];
                _request = "";
            }
        }

        // tree clicked
        private void treeView_OnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            Point p = new Point(e.X, e.Y);
            TreeNode node = treeView.GetNodeAt(p);
            switch (Convert.ToString(node.Tag))
            {
                case "File":
                    treeFileContextMenu.Show(treeView, p);
                    break;
                case "State":
                    treeStateContextMenu.Show(treeView, p);
                    break;
                case "Parameter":
                case "Value":
                    treeParamContextMenu.Show(treeView, p);
                    break;
                case "Index":
                    treeIndexContextMenu.Show(treeView, p);
                    break;
            }
            treeView.SelectedNode = node;
            Debug.WriteLine(node.Text + " is " + node.Tag);
        }

        void dynamicMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem sent = sender as ToolStripMenuItem;
            if (sent != null)
                loadSprite(sent.Tag.ToString());
        }

        // help > visit avian website
        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://avian.netne.net/index.php?p=programming&pid=7");
        }

        // help > submit sprite
        private void visitSpriteForumsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://sprites.caustik.com/forum/10-character-submissions/");
        }

        // tree context menu > expand all
        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e) { treeView.ExpandAll(); }

        // tree context menu > collapse all
        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e) { treeView.CollapseAll(); }

        // tools > prettify mouseover
        private void prettifySPRToolStripMenuItem_OnMouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Give SPR file proper indentation, spacing, and line breaks.";
        }

        // tools > prettify mouseleave
        private void prettifySPRToolStripMenuItem_OnMouseLeave(object sender, EventArgs e) { statusLabel.Text = "Sprite Editor By Avian"; }

        // tools > preffity clicked
        private void prettifySPRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog
                                              {
                                                  InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                  Filter = "Sprite Files(*.spr;*.spi;*.json)|*.spr;*.spi;*.json|All files (*.*)|*.*",
                                                  RestoreDirectory = true,
                                                  Multiselect = false
                                              };
                DialogResult result = openFile.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //read file to string then close
                    StreamReader myFile = new StreamReader(openFile.FileName);
                    string jsonString = myFile.ReadToEnd();
                    myFile.Close();

                    // parse into JSON object, serialize, save file
                    File.WriteAllText(openFile.FileName, prettifyJSON(jsonString));
                }
                statusLabel.Text = openFile.SafeFileName + " was cleaned and updated.";
                openFile.Dispose();
            }
            catch (FileNotFoundException ex)
            {
                statusLabel.Text = "Error: File not found!";
                MessageBox.Show("Error: File not found.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // helper method for tree population
        public TreeNode getNewNode(string nameText, string tag)
        {
            TreeNode node = new TreeNode(nameText)
                           {Name = nameText, Tag = tag};
            return node;
        }

        // helper method to Format JSON
        public static string prettifyJSON(string formatThis) { return (JsonConvert.SerializeObject(JObject.Parse(formatThis), Formatting.Indented)); }

        // helper method to convert Color to Hex
        public static String hexConverter(Color c) { return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2"); }

        // helper method for file > import
        public static List<string> getStatesFromJSONString(string jsonString)
        {
            JObject root = JObject.Parse(jsonString);
            IList<JToken> states = root.Children().ToList();

            List<string> rootStates = new List<string>();

            JToken jtoken = states.First();

            while (jtoken != null)
            {
                rootStates.Add(((JProperty)jtoken).Name);
                jtoken = jtoken.Next;
            }
            return rootStates;
        }

        // file > save as clicked
        private void saveFileMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0)
            {
                statusLabel.Text = "Cannot save. Empty file!";
                return;
            }
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
                                                 {
                                                     Filter = "Sprite Files(*.spr;*.spi)|*.spr;*.spi|All files (*.*)|*.*",
                                                     FilterIndex = 1,
                                                     RestoreDirectory = true,
                                                     FileName = treeView.Nodes[0].Text
                                                 };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter file = new StreamWriter(saveFileDialog1.FileName);
                file.WriteLine(writeJsonFromTree(treeView));
                file.Close();
            }
            Debug.WriteLine(writeJsonFromTree(treeView));
        }

        // helper for saving files
        private static string writeJsonFromTree(TreeView tree)
        {
            StringWriter sw = new StringWriter(new StringBuilder());
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            jsonWriter.Formatting = Formatting.Indented;

            // START WRITING JSON
            jsonWriter.WriteStartObject();

            if (tree.Nodes.Count <= 0) // TREE EMPTY! SHOW ERROR!
                return "Cannot write empty sprite!";

            if (tree.Nodes[0].Nodes.Count < 2)
                return "Sprite Requires at least META_DATA and STATE_DEFAULT!";

            TreeNode stateNode = tree.Nodes[0].Nodes[0];

            // loop through each SPRITE_* node and process
            while (stateNode != null)
            {
                jsonWriter.WritePropertyName(stateNode.Text);
                jsonWriter.WriteStartObject();

                // Check if state node has no subnodes
                if (stateNode.Nodes.Count == 0) { jsonWriter.WriteEndObject(); stateNode = stateNode.NextNode; continue; }

                // check properties and write arrays / subarrays
                TreeNode parameterNode = stateNode.Nodes[0];
                while (parameterNode != null)
                {
                    // handle properties with sub arrays
                    if (parameterNode.Text.Equals("credits") || parameterNode.Text.Equals("fixtures") || parameterNode.Text.Equals("spawn"))
                    {

                        int credCount = parameterNode.Nodes.Count;
                        if (credCount == 0) { stateNode = stateNode.NextNode; continue; }
                        TreeNode indexNode = parameterNode.Nodes[0];

                        jsonWriter.WritePropertyName(parameterNode.Text);
                        jsonWriter.WriteStartArray();
                        while (indexNode != null)
                        {
                            if (indexNode.Nodes.Count == 0) { indexNode = indexNode.NextNode; }
                            TreeNode credProps = indexNode.Nodes[0];
                            jsonWriter.WriteStartObject();
                            int i = 0;
                            while (credProps != null)
                            {
                                if (indexNode.Nodes[i].Nodes.Count == 0) { i++; credProps = credProps.NextNode; continue; }
                                jsonWriter.WritePropertyName(indexNode.Nodes[i].Text);
                                jsonWriter.WriteValue(indexNode.Nodes[i].Nodes[0].Text);
                                i++;
                                credProps = credProps.NextNode;
                            }
                            jsonWriter.WriteEndObject();
                            indexNode = indexNode.NextNode;
                        }
                        jsonWriter.WriteEndArray();
                        parameterNode = parameterNode.NextNode;
                        continue;
                    }
                    if (parameterNode.Text.Equals("actions") || parameterNode.Text.Equals("flags")) // handle array properties
                    {
                        if (parameterNode.Nodes.Count == 0) { parameterNode = parameterNode.NextNode; continue; }
                        jsonWriter.WritePropertyName(parameterNode.Text);
                        jsonWriter.WriteStartArray();
                        TreeNode propName = parameterNode.Nodes[0];
                        while (propName != null)
                        {
                            jsonWriter.WriteValue(propName.Text);
                            propName = propName.NextNode;
                        }
                        jsonWriter.WriteEndArray();
                        parameterNode = parameterNode.NextNode;
                        continue;
                    }

                    // singular property / value
                    TreeNode stateValue = parameterNode.Nodes.Count != 0 ? parameterNode.Nodes[0] : new TreeNode("");
                    jsonWriter.WritePropertyName(parameterNode.Text);
                    jsonWriter.WriteValue(stateValue.Text);
                    parameterNode = parameterNode.NextNode;
                }
                jsonWriter.WriteEndObject();
                stateNode = stateNode.NextNode;
            }
            jsonWriter.WriteEndObject();
            return sw.ToString();
        }

        // image mouseover
        private void pictureBox_MouseOver(object sender, MouseEventArgs e)
        {
            statusCoordsLabel.Text = "X: " + (e.X / zoomSlider.Value) + " Y: " + (e.Y / zoomSlider.Value);
        }

        // helper for zoom functionality
        public Image pictureBoxZoom(Image img, Size size)
        {
            Bitmap bm = new Bitmap(img, Convert.ToInt32(img.Width * size.Width), Convert.ToInt32(img.Height * size.Height));
            Graphics grap = Graphics.FromImage(bm);
            grap.InterpolationMode = InterpolationMode.HighQualityBicubic;
            return bm;
        }

        // view > zoom in selected
        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (zoomSlider.Value == zoomSlider.Maximum)
                return;
            zoomSlider.Value++;
            imageDisplay.Image = null;
            imageDisplay.Image = pictureBoxZoom(_imageOriginal, new Size(zoomSlider.Value, zoomSlider.Value));
            imageDisplay.Width = imageDisplay.Image.Width;
            imageDisplay.Height = imageDisplay.Image.Height;
        }

        // veiw > zoom out selected
        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (zoomSlider.Value == zoomSlider.Minimum)
                return;
            zoomSlider.Value--;
            imageDisplay.Image = null;
            imageDisplay.Image = pictureBoxZoom(_imageOriginal, new Size(zoomSlider.Value, zoomSlider.Value));
            imageDisplay.Width = imageDisplay.Image.Width;
            imageDisplay.Height = imageDisplay.Image.Height;
        }

        // view > actual size selected
        private void actualSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageDisplay.Image = _imageOriginal;
            imageDisplay.Width = imageDisplay.Image.Width;
            imageDisplay.Height = imageDisplay.Image.Height;
            zoomSlider.Value = 1;
        }

        // event handler for recently loaded images
        private void imageDropDownMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            if (mi != null)
                imageDisplay.Image = new Bitmap(mi.Tag.ToString());
            _imageOriginal = imageDisplay.Image;
            zoomSlider.Value = 1;
            imageDisplay.Width = imageDisplay.Image.Width;
            imageDisplay.Height = imageDisplay.Image.Height;
            statusLabel.Text = Text + " loaded OK.";
        }

        // helper for adding recently used files
        private static void addMRU(string fileLoc)
        {
            List<string> g = new List<string>(Properties.Settings.Default.MRU.Split('?'));
            
            if (g.Any(i => i.Equals(fileLoc)))
                return;
            if (g.Count > 3)
                g.RemoveAt(0);

            Properties.Settings.Default.MRU = g[0].Equals("") ? fileLoc : string.Concat(string.Join("?", g), "?", fileLoc);
            Properties.Settings.Default.Save();
        }

        // helper for populating recently used files
        private void populateRecentFiles()
        {
            recentSpritesToolStripMenuItem.DropDownItems.Clear();
            string[] split = Properties.Settings.Default.MRU.Split('?');

            foreach (string rs in split)
            {
                if (rs.Equals(""))
                    continue;
                ToolStripMenuItem dynamicMenuItem = new ToolStripMenuItem(getFileNameFromPath(rs));
                dynamicMenuItem.Click += dynamicMenuItem_Click;
                dynamicMenuItem.Tag = rs;
                recentSpritesToolStripMenuItem.DropDownItems.Add(dynamicMenuItem);
            }
        }

        // tools > sprite preview selected
        private void spritePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new frmPreview(_images, _imageLocations, ref treeView))
            {
                form.ShowDialog();
            }
        }

        // view > hide tree selected
        private void hideTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                hideTreeToolStripMenuItem.Text = "Hide Tree";
                splitContainer1.Panel2Collapsed = false;
            }
            else
            {
                hideTreeToolStripMenuItem.Text = "Show Tree";
                splitContainer1.Panel2Collapsed = true;
            }
        }

        // tools > test sprite selected
        private void testSpriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count <= 0)
                return;
            TextWriter tw = new StreamWriter("test.spr");
            tw.WriteLine(writeJsonFromTree(treeView));
            tw.Close();

            int i = 0;
            foreach(Bitmap b in _images)
                b.Save(getFileNameFromPath(_imageLocations[i++]));

            Process.Start("test.spr");
        }

        // event handler for context menu EDIT item
        private void manualEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
                treeView.SelectedNode.BeginEdit();
        }

        // edit > grab X value selected
        private void grabXValueToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && (treeView.SelectedNode.Tag.Equals("Parameter") || treeView.SelectedNode.Tag.Equals("Value")))
            {
                _request = "grabX";
                statusLabel.Text = "Click the image to set a X value.";
            }
            else
            {
                MessageBox.Show("Please select a Parameter in the tree to store the X value in", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // edit > grab Y value selected
        private void grabYValueToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && (treeView.SelectedNode.Tag.Equals("Parameter") || treeView.SelectedNode.Tag.Equals("Value")))
            {
                _request = "grabY";
                statusLabel.Text = "Click the image to set a Y value.";
            }
            else
            {
                MessageBox.Show("Please select a Parameter in the tree to store the X value in", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // edit > grab color selected
        private void grabColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && (treeView.SelectedNode.Tag.Equals("Parameter") || treeView.SelectedNode.Tag.Equals("Value")))
            {
                _request = "grabHTML";
                statusLabel.Text = "Click the image to grab color value.";
            }
            else
            {
                MessageBox.Show("Please select a Parameter in the tree to store the X value in", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // view > open image selected
        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0)
            {
                MessageBox.Show("Cannot load picture. Go to File > New then try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                OpenFileDialog openFile = new OpenFileDialog
                                              {
                                                  InitialDirectory = "c:\\",
                                                  Filter = "Image Files(*.png;*.jpg;*.gif;*.bmp)|*.png;*.jpg;*.gif;*.bmp|All files (*.*)|*.*",
                                                  RestoreDirectory = true,
                                                  Multiselect = false
                                              };
                DialogResult result = openFile.ShowDialog();
                if (result == DialogResult.OK)
                {
                    imageDisplay.Image = new Bitmap(openFile.FileName);
                    imageDisplay.Tag = openFile.SafeFileName;
                    imageDisplay.Width = imageDisplay.Image.Width;
                    imageDisplay.Height = imageDisplay.Image.Height;

                    statusLabel.Text = openFile.SafeFileName + " loaded " + result + ".";

                    zoomSlider.Value = 1;
                    _imageOriginal = imageDisplay.Image;
                    _images.Add(new Bitmap(openFile.FileName));
                    _imageLocations.Add(openFile.FileName);
                }
                else
                {
                    statusLabel.Text = "File was not be loaded.";
                }
                openFile.Dispose();
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Error: Picture could not be loaded.";
                MessageBox.Show("Error: File not found.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // event handler to populate Recent Images
        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            loadedImagesToolStripMenuItem.DropDownItems.Clear();
            foreach (string i in _imageLocations)
            {
                ToolStripMenuItem addThis = new ToolStripMenuItem(getFileNameFromPath(i), null, imageDropDownMenu_Click) { Tag = i };
                loadedImagesToolStripMenuItem.DropDownItems.Add(addThis);
            }
            loadedImagesToolStripMenuItem.Enabled = loadedImagesToolStripMenuItem.DropDownItems.Count != 0;
            dependenciesToolStripMenuItem.Enabled = dependenciesToolStripMenuItem.DropDownItems.Count != 0;
        }

        // 
        private void useImageURIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && treeView.SelectedNode.Tag.Equals("Parameter"))
            {
                TreeNode addHere = treeView.SelectedNode;
                if (addHere.Nodes.Count == 0)
                    addHere.Nodes.Add(getNewNode(imageDisplay.Tag.ToString(), "Value"));
                else
                    addHere.Nodes[0].Text = imageDisplay.Tag.ToString();
                statusLabel.Text = "Operation successful.";
            }
            else
            {
                MessageBox.Show("Please select a Parameter in the tree to store the URI.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // event handler for edit > delete state menu item
        private void deleteStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && treeView.SelectedNode.Tag.Equals("State"))
                treeView.SelectedNode.Remove();
        }

        // event handler for edit > delete parameter menu item
        private void deleteParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            if (treeView.SelectedNode.Tag.Equals("Parameter") || treeView.SelectedNode.Tag.Equals("Value"))
                treeView.SelectedNode.Remove();
        }

        // event handler for add state
        private void addState_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem paramToAdd = sender as ToolStripMenuItem;
            if (treeView.Nodes.Count != 0 && paramToAdd != null)
                treeView.Nodes[0].Nodes.Add(getNewNode(paramToAdd.Text, "State"));
            treeView.TreeViewNodeSorter = new NodeSorter();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && treeView.SelectedNode.Tag.Equals("Index"))
                treeView.SelectedNode.Remove();
        }

        private void addNewGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && treeView.SelectedNode.Tag.Equals("Index"))
            {
                TreeNode groupNode = treeView.SelectedNode;
                if (treeView.SelectedNode.Text.Contains("["))
                    groupNode = treeView.SelectedNode.Parent;
                int count = groupNode.Nodes.Count;
                groupNode.Nodes.Add(getNewNode(String.Concat("[", ++count, "]"), "Index"));

                if (groupNode.Text.Equals("fixtures"))
                {
                    groupNode.LastNode.Nodes.Add(getNewNode("x", "Parameter"));
                    groupNode.LastNode.Nodes.Add(getNewNode("y", "Parameter"));
                    groupNode.LastNode.Nodes.Add(getNewNode("w", "Parameter"));
                    groupNode.LastNode.Nodes.Add(getNewNode("h", "Parameter"));
                }
                else if (groupNode.Text.Equals("credits"))
                {
                    groupNode.LastNode.Nodes.Add(getNewNode("author", "Parameter"));
                    groupNode.LastNode.Nodes.Add(getNewNode("description", "Parameter"));
                    groupNode.LastNode.Nodes.Add(getNewNode("url", "Parameter"));
                }
                else if (groupNode.Text.Equals("spawn"))
                {
                    groupNode.LastNode.Nodes.Add(getNewNode("uri", "Parameter"));
                    groupNode.LastNode.Nodes.Add(getNewNode("spawnX", "Parameter"));
                    groupNode.LastNode.Nodes.Add(getNewNode("spawnY", "Parameter"));
                    groupNode.LastNode.Nodes.Add(getNewNode("spawnExplode", "Parameter"));
                }
            }
        }

        private void fileMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            recentSpritesToolStripMenuItem.Enabled = recentSpritesToolStripMenuItem.DropDownItems.Count != 0;
        }

        // custom TreeNode sorter implementation
        public class NodeSorter : IComparer
        {
            public int Compare(object thisObj, object otherObj)
            {
                TreeNode thisNode = thisObj as TreeNode;
                TreeNode otherNode = otherObj as TreeNode;

                if (thisNode.Tag.Equals("SPRITE_META_DATA") || thisNode.Tag.Equals("SPRITE_STATE_DEFAULT"))
                    return 1;

                return thisNode.Text.CompareTo(otherNode.Text);
            }
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0 || !treeView.Nodes.Find("SPRITE_META_DATA", true).Any() || !treeView.Nodes.Find("SPRITE_STATE_DEFAULT", true).Any())
            {
                spritePreviewToolStripMenuItem.Enabled = false;
                testSpriteToolStripMenuItem.Enabled = false;
            }
            else
            {
                spritePreviewToolStripMenuItem.Enabled = true;
                testSpriteToolStripMenuItem.Enabled = true;
            }
        }
    }
}