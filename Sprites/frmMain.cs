using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpriteEditor
{
    public partial class frmMain : Form
    {
        private static readonly Regex _stateNumberRegex = new Regex(@"\d+$");

        private static readonly Regex _indexNodeRegex = new Regex(@"\[\d+\]");

        private static readonly Regex _htmlHexRegex = new Regex(@"[A-Fa-f0-9]{6}$");

        private readonly string[] _possibleActions = {
                                                         "walk", "fly", "run", "jump", "destroy", "death"
                                                     };

        private readonly string[] _possibleFlags = {
                                                       "isCollector", "isItem", "disablePhysics",
                                                       "disableWindowCollide", "disableSpriteCollide",
                                                       "disableJump", "doFadeOut"
                                                   };

        private readonly string[] _possibleParams = {
                                                        "uri", "flipX", "sizeMultiplier", "sizeDivider", "frameDelay",
                                                        "cropX", "cropY", "cropW", "cropH",
                                                        "offsX", "offsY", "isChain", "usePrevious", "autoClose",
                                                        "transparent", "walkMultiplier",
                                                        "actions", "flags"
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

        /// <summary>
        /// Enum that lists valid TreeNode.Tag types.
        /// </summary>
        public enum TreeNodeTag
        {
            None,
            Value,
            Parameter,
            Index,
            State,
            File
        };

        private List<ToolStripMenuItem> _dependencies; // container for View > Sprite Dependencies items (spi files, images, etc)
        private List<string> _imageLocations; // container that holds filelocations of current spr file associated images
        private List<Image> _images; // container that holds all Images for current spr file -> indices consistent with _imageLocations
        private Image _imageOriginal; // store original image for usage with Zoom functionality
        private string _request = string.Empty; // used for user input wih pictureBox
        private string _workingFile = string.Empty; // store current working file location for quick saving via File > Save
        private bool draw = false; // user is using Quick Crop functionality
        private int quickX = 1; // X val for Quick Crop functionality
        private int quickY = 1; // Y val for Quick Crop functionality
        private int quickW = 1; // Width val for Quick Crop functionality
        private int quickH = 1; // Height val for Quick Crop functionality

        /// <summary>
        /// Initializes a new instance of the FormMain class.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Application launch event.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            Debug.WriteLine(Path.GetDirectoryName(Application.ExecutablePath));
            Debug.WriteLine(Path.GetDirectoryName(Application.ExecutablePath).ToString() + "\\temp.gif");


            // instantiate form vars
            _images = new List<Image>();
            _imageLocations = new List<string>();
            _dependencies = new List<ToolStripMenuItem>();

            // set up picturebox
            pictureBox.Width = pictureBox.Image.Width;
            pictureBox.Height = pictureBox.Image.Height;
            _imageOriginal = pictureBox.Image;
            populateRecentFiles();

            // use remembered window locations
            Width = Properties.Settings.Default.FormWidth > 0 ? Properties.Settings.Default.FormWidth : 675;
            Height = Properties.Settings.Default.FormHeight > 0 ? Properties.Settings.Default.FormHeight : 375;
            splitContainer1.SplitterDistance = Properties.Settings.Default.SplitterDistance > 0 ? Properties.Settings.Default.SplitterDistance : 460;
            Top = Properties.Settings.Default.SplitterDistance > -5 ? Properties.Settings.Default.FormTop : 20;
            Left = Properties.Settings.Default.SplitterDistance > -5 ? Properties.Settings.Default.FormLeft : 20;

            // populate menus
            foreach (ToolStripMenuItem addThis in _possibleParams.Select(s => new ToolStripMenuItem(s, null, treeAddParameter_Click)))
                stateContextMenuAddParameter.DropDownItems.Add(addThis);
            foreach (ToolStripMenuItem addThis in _possibleStates.Select(s => new ToolStripMenuItem(s, null, addState_Click)))
                fileContextMenuAddState.DropDownItems.Add(addThis);
            foreach (ToolStripMenuItem addThis in _possibleStates.Select(s => new ToolStripMenuItem(s, null, addState_Click)))
                stateContextMenuAddState.DropDownItems.Add(addThis);
            foreach (ToolStripMenuItem addThis in _possibleFlags.Select(s => new ToolStripMenuItem(s, null, treeAddFlagOrAction_Click)))
                flagsContextMenuAddFlag.DropDownItems.Add(addThis);
            foreach (ToolStripMenuItem addThis in _possibleActions.Select(s => new ToolStripMenuItem(s, null, treeAddFlagOrAction_Click)))
                actionsContextMenuAddAction.DropDownItems.Add(addThis);

            // handle command line args
            string fileToOpen = string.Empty;
            foreach (string arg in Environment.GetCommandLineArgs().Where(arg => arg.EndsWith("spr") || arg.EndsWith("spi")))
                fileToOpen = arg;
            if (!string.IsNullOrEmpty(fileToOpen) && File.Exists(fileToOpen))
                loadSprite(fileToOpen);
        }

        /// <summary>
        /// Add Flag/Action treeview context menu event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void treeAddFlagOrAction_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem paramToAdd = sender as ToolStripMenuItem;
            if (treeView.SelectedNode == null || treeView.SelectedNode.Tag.Equals(TreeNodeTag.Parameter.ToString()) ||
                paramToAdd == null || treeView.SelectedNode.Nodes.ContainsKey(paramToAdd.Text))
            {
                MessageBox.Show("Node already exists! Aborted.", "Unable to add node.",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            treeView.SelectedNode.Nodes.Add(getNewNode(paramToAdd.Text, TreeNodeTag.Value));
            treeView.SelectedNode.Expand();
        }

        /// <summary>
        /// Add Parameter treeView context menu event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void treeAddParameter_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem paramToAdd = sender as ToolStripMenuItem;
            if (treeView.SelectedNode == null || !treeView.SelectedNode.Tag.Equals(TreeNodeTag.State) ||
                paramToAdd == null || treeView.SelectedNode.Nodes.ContainsKey(paramToAdd.Text))
            {
                MessageBox.Show("Node already exists or there was an unexpected error.", "Unable to add node.",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            treeView.SelectedNode.Nodes.Add(getNewNode(paramToAdd.Text, TreeNodeTag.Parameter));
            treeView.SelectedNode.Expand();
        }

        /// <summary>
        /// File > Load event handler that imports JSON-formatted file for usage.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void fileLoadMenu_Click(object sender, EventArgs e)
        {
            // confirmUnsavedChanges with user that unsaved changes will be lsot
            if (confirmUnsavedChanges() == DialogResult.Cancel) return;
            try
            {
                string defaultDir = Properties.Settings.Default.lastOpenedSpriteDir;
                if (string.IsNullOrEmpty(defaultDir) || !Directory.Exists(defaultDir))
                    defaultDir = string.IsNullOrEmpty(getSpriteCollectionLocation()) ?
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) : getSpriteCollectionLocation();
                OpenFileDialog openFile = new OpenFileDialog
                                              {
                                                  InitialDirectory = defaultDir,
                                                  Filter = "Sprite Files(*.spr;*.spi)|*.spr;*.spi|All files (*.*)|*.*",
                                                  RestoreDirectory = true,
                                                  Multiselect = false
                                              };
                DialogResult result = openFile.ShowDialog();
                if (result == DialogResult.OK)
                {
                    loadSprite(openFile.FileName);
                    Properties.Settings.Default.lastOpenedSpriteDir = Path.GetDirectoryName(openFile.FileName);
                    Properties.Settings.Default.Save();
                }
                sortTree();
            }
            catch (FileNotFoundException ex)
            {
                statusLabel.Text = "Error: File not found.";
                MessageBox.Show("Error: File not found.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Loads a JSON formatted *.spr into the program for usage.
        /// </summary>
        /// <param name="fileLocation">Location of *.spr file to load.</param>
        private void loadSprite(string fileLocation)
        {
            try
            {
                string jsonString = string.Empty;

                //read file into string and close. if null or empty, abort.
                using (StreamReader myFile = new StreamReader(fileLocation))
                {
                    jsonString = myFile.ReadToEnd();
                }

                // preformat *.spr file to avoid errors and clear tree
                jsonString = prettifyJSON(jsonString);
                treeView.Nodes.Clear();

                // deserialize jsonString into Sprite instance and add tree root
                Sprite sprizite = JsonConvert.DeserializeObject<Sprite>(jsonString);
                sprizite.fileName = fileLocation;
                sprizite.safeFileName = Path.GetFileName(fileLocation);
                treeView.Nodes.Add(getNewNode(sprizite.safeFileName, TreeNodeTag.File));

                // get list of states and iterate over states
                List<string> validatedStateList = getStatesFromJSONString(jsonString);
                int stateNodeIndex = 0;
                foreach (string state in validatedStateList)
                {
                    // add SPRITE_* nodes
                    treeView.Nodes[0].Nodes.Add(getNewNode(state, TreeNodeTag.State));

                    // Special Case: Preloaded STATES ( META and DEFAULT )
                    if (state.Equals("SPRITE_META_DATA") || state.Equals("SPRITE_STATE_DEFAULT"))
                    {
                        // list the properties to populate
                        List<string> propList = sprizite.getNonNullProperties(state.Equals("SPRITE_META_DATA") ? "meta" : "default", sprizite);

                        // add parameterName nodes for each state
                        foreach (string vs in propList)
                        {
                            if (vs.Equals("fixtures") || vs.Equals("credits") || vs.Equals("spawn"))
                                treeView.Nodes[0].Nodes[stateNodeIndex].Nodes.Add(getNewNode(vs, TreeNodeTag.Index));
                            else
                                treeView.Nodes[0].Nodes[stateNodeIndex].Nodes.Add(getNewNode(vs, TreeNodeTag.Parameter));
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
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode("[" + (z + 1) + "]", TreeNodeTag.Index));

                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_META_DATA.credits[z].author))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("author", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[0].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.credits[z].author, TreeNodeTag.Value));
                                        }
                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_META_DATA.credits[z].description))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("description", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[1].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.credits[z].description, TreeNodeTag.Value));
                                        }
                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_META_DATA.credits[z].url))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("url", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[2].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.credits[z].url, TreeNodeTag.Value));
                                        }
                                    }
                                    k++;
                                    continue;
                                }
                                if (prop.Equals("fixtures"))
                                {
                                    for (int z = 0; z < sprizite.SPRITE_STATE_DEFAULT.fixtures.Count; z++)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode("[" + (z + 1) + "]", TreeNodeTag.Index));

                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].x))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("x", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[0].Nodes.Add(getNewNode(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].x, TreeNodeTag.Value));
                                        }
                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].y))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("y", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[1].Nodes.Add(getNewNode(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].y, TreeNodeTag.Value));
                                        }
                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].w))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("w", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[2].Nodes.Add(getNewNode(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].w, TreeNodeTag.Value));
                                        }
                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].h))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("h", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[3].Nodes.Add(getNewNode(sprizite.SPRITE_STATE_DEFAULT.fixtures[z].h, TreeNodeTag.Value));
                                        }
                                    }
                                    k++;
                                    continue;
                                }
                                if (prop.Equals("spawn"))
                                {
                                    for (int z = 0; z < sprizite.SPRITE_META_DATA.spawn.Count; z++)
                                    {
                                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode("[" + (z + 1) + "]", TreeNodeTag.Index));

                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_META_DATA.spawn[z].uri))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("uri", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[0].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.spawn[z].uri, TreeNodeTag.Value));
                                        }
                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_META_DATA.spawn[z].spawnX))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("spawnX", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[1].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.spawn[z].spawnX, TreeNodeTag.Value));
                                        }
                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_META_DATA.spawn[z].spawnY))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("spawnY", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[2].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.spawn[z].spawnY, TreeNodeTag.Value));
                                        }
                                        if (!string.IsNullOrEmpty(sprizite.SPRITE_META_DATA.spawn[z].spawnExplode))
                                        {
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes.Add(getNewNode("spawnExplode", TreeNodeTag.Parameter));
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes[z].Nodes[3].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.spawn[z].spawnExplode, TreeNodeTag.Value));
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
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.flags[z], TreeNodeTag.Parameter));
                                        else if (prop.Equals("actions"))
                                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode(sprizite.SPRITE_META_DATA.actions[z], TreeNodeTag.Value));
                                    }
                                    k++;
                                    continue;
                                }
                            }
                            treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[k].Nodes.Add(getNewNode(val, TreeNodeTag.Value));
                            k++;
                        }
                        stateNodeIndex++;
                        continue;
                    } // end processing of META and DEFAULT

                    // SPRITE_STATE_* detected - start processing using JTokens
                    SpriteState addThis = new SpriteState(state);

                    // Special Case: check if state is empty
                    if (!JObject.Parse(jsonString)[state].Any())
                    {
                        stateNodeIndex++;
                        continue;
                    }

                    JToken iteratorToken = JObject.Parse(jsonString)[state].First();

                    int j = 0;
                    // check each token and parse
                    while (iteratorToken != null)
                    {
                        string propName = ((JProperty)iteratorToken).Name;
                        string propValue = ((JProperty)iteratorToken).Value.ToString();
                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes.Add(getNewNode(propName, TreeNodeTag.Parameter));
                        treeView.Nodes[0].Nodes[stateNodeIndex].Nodes[j].Nodes.Add(getNewNode(propValue, TreeNodeTag.Value));
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
                _dependencies.Clear();

                // extract dependencies from uri params and load them
                TreeNode[] imagesToLoad = treeView.Nodes.Find("uri", true);
                foreach (TreeNode u in imagesToLoad)
                {
                    if (u.FirstNode == null) continue;
                    string imageLoc = u.FirstNode.Text;
                    string fullImagePath = String.Concat(sprizite.fileName.Substring(0, sprizite.fileName.LastIndexOf("\\", StringComparison.Ordinal)), "\\", imageLoc);
                    if (_imageLocations.Contains(fullImagePath))
                        continue;

                    // add *.spi files to associated files list
                    if (fullImagePath.EndsWith("spi") || fullImagePath.EndsWith("spr") && !Path.GetFileName(fullImagePath).Equals(sprizite.safeFileName))
                    {
                        ToolStripMenuItem dynamicMenuItem = new ToolStripMenuItem(Path.GetFileName(fullImagePath));
                        dynamicMenuItem.Image = Properties.Resources.favicon;
                        dynamicMenuItem.Tag = fullImagePath;
                        dynamicMenuItem.Click += dependencyMenuItem_Click;
                        _dependencies.Add(dynamicMenuItem);
                        continue;
                    }
                    if (File.Exists(fullImagePath))
                        _images.Add(new Bitmap(fullImagePath));
                    _imageLocations.Add(fullImagePath);
                }

                // display DEFAULT image in picturebox
                if (_imageLocations.Any() && File.Exists(_imageLocations[0]))
                {
                    pictureBox.Image = new Bitmap(_imageLocations[0]);
                    pictureBox.Tag = sprizite.SPRITE_STATE_DEFAULT.uri;
                    pictureBox.Width = pictureBox.Image.Width;
                    pictureBox.Height = pictureBox.Image.Height;
                    _imageOriginal = pictureBox.Image;
                }

                // all done populating controls - finalize
                statusLabel.Text = sprizite.safeFileName + " was imported successfully.";
                treeView.Nodes[0].Expand();
                addMRU(sprizite.fileName);
                populateRecentFiles();
                _workingFile = sprizite.fileName;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Unable to load sprite. File not found or error reading input file.\n\nDetails:\n" + ex.Message,
                    "File Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to load sprite. File not found?";
                return;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Unable to load sprite. Is the input file blank, corrupt, or malformatted?\n\nDetails:\n" + ex.Message,
                    "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to load sprite. Is the file blank?";
                return;
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Unable to load sprite. Is the input file blank, corrupt, or malformatted?\n\nDetails:\n" + ex.Message,
                    "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to load sprite. File is malformatted.";
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load sprite. Unknown error. Please send avian the *.spr file so he can fix this.\n\nDetails:\n" + ex.Message,
                    "Generic Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to load sprite. Unknown error.";
                return;
            }
        }

        /// <summary>
        /// Fetch property value from Sprite object for processing META_DATA and STATE_DEFAULT.
        /// </summary>
        /// <param name="sprizite">Sprite object to fetch params from.</param>
        /// <param name="prop">Sprite property to fetch.</param>
        /// <returns>Specified property value.</returns>
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

        /// <summary>
        /// Application exit event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void fileExitMenu_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// File > New menu event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void fileNewMenu_Click(object sender, EventArgs e)
        {
            // confirmUnsavedChanges with user that unsaved changed will be lost
            if (confirmUnsavedChanges() == DialogResult.Cancel) return;

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
                    pictureBox.Image = Properties.Resources.favicon;
                    pictureBox.Height = pictureBox.Image.Height;
                    pictureBox.Width = pictureBox.Image.Width;

                    // store values from Sprite Wizard
                    Dictionary<string, string> sprInfo = form._spriteInfo;
                    List<string> actions = form._actions;
                    List<string> flags = form._flags;
                    List<string> states = form._states;

                    // add required nodes
                    treeView.Nodes.Add(getNewNode(sprInfo["name"], TreeNodeTag.File));
                    treeView.Nodes[0].Nodes.Add(getNewNode("SPRITE_META_DATA", TreeNodeTag.State));
                    treeView.Nodes[0].Nodes.Add(getNewNode("SPRITE_STATE_DEFAULT", TreeNodeTag.State));

                    // process frmWizard results and populate tree accordingly
                    foreach (KeyValuePair<string, string> i in sprInfo)
                    {
                        switch (i.Key)
                        {
                            case "name":
                                break;
                            case "version":
                                treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes.Add(getNewNode("version", TreeNodeTag.Parameter));
                                treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["version"].Nodes.Add(getNewNode(i.Value, TreeNodeTag.Value));
                                break;
                            case "author":
                            case "description":
                            case "url":
                                if (treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["credits"] == null)
                                {
                                    treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes.Add(getNewNode("credits", TreeNodeTag.Index));
                                    treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["credits"].Nodes.Add(getNewNode("[1]", TreeNodeTag.Index));
                                }
                                treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["credits"].Nodes["[1]"].Nodes.Add(getNewNode(i.Key, TreeNodeTag.Value));
                                treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["credits"].Nodes["[1]"].Nodes[i.Key].Nodes.Add(getNewNode(i.Value, TreeNodeTag.Value));
                                break;
                        }
                    }

                    if (actions.Count != 0)
                    {
                        treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes.Add(getNewNode("actions", TreeNodeTag.Parameter));
                        foreach (string a in actions)
                            treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["actions"].Nodes.Add(getNewNode(a, TreeNodeTag.Value));
                    }

                    if (flags.Count != 0)
                    {
                        treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes.Add(getNewNode("flags", TreeNodeTag.Parameter));
                        foreach (string f in flags)
                            treeView.Nodes[0].Nodes["SPRITE_META_DATA"].Nodes["flags"].Nodes.Add(getNewNode(f, TreeNodeTag.Value));
                    }

                    if (states.Count != 0)
                    {
                        foreach (string s in states)
                            treeView.Nodes[0].Nodes.Add(getNewNode(s, TreeNodeTag.State));
                    }
                    sortTree();
                    treeView.Nodes[0].Expand();
                }
            }
        }

        /// <summary>
        /// Loaded Image clicked event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void imageDisplay_Click(object sender, EventArgs e)
        {
            if (_request.Equals("grabHTML"))
            {
                if (treeView.SelectedNode == null)
                {
                    MessageBox.Show("Please select a valid Tree Node to place the value in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string[] coords = statusCoordsLabel.Text.Split(new[] { 'X', 'Y', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Bitmap getThisColor = (Bitmap)pictureBox.Image;
                Color chosenColor = getThisColor.GetPixel(int.Parse(coords[0]), int.Parse(coords[1]));
                
                string chosenColorHTML = hexConverter(chosenColor);
                TreeNode addHere = treeView.SelectedNode;
                if (addHere.Tag.Equals(TreeNodeTag.Value))
                    addHere.Text = chosenColorHTML;
                else
                {
                    if (addHere.Tag.Equals(TreeNodeTag.Parameter) && addHere.Nodes.Count == 0)
                        addHere.Nodes.Add(getNewNode(chosenColorHTML, TreeNodeTag.Value));
                    else
                    {
                        addHere.Nodes[0].Remove();
                        addHere.Nodes.Add(getNewNode(chosenColorHTML, TreeNodeTag.Value));
                    }
                }
                _request = string.Empty;
                statusLabel.Text = chosenColorHTML + " was selected.";
            }
            else if (_request.Equals("grabX"))
            {
                if (treeView.SelectedNode == null)
                {
                    MessageBox.Show("Please select a valid Tree Node to place the value in.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string[] coords = statusCoordsLabel.Text.Split(new[] { 'X', 'Y', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                TreeNode addHere = treeView.SelectedNode;
                if (addHere.Tag.Equals(TreeNodeTag.Value))
                    addHere.Text = coords[0];
                else
                {
                    if (addHere.Tag.Equals(TreeNodeTag.Parameter) && addHere.Nodes.Count == 0)
                        addHere.Nodes.Add(getNewNode(coords[0], TreeNodeTag.Value));
                    else
                    {
                        addHere.Nodes[0].Remove();
                        addHere.Nodes.Add(getNewNode(coords[0], TreeNodeTag.Value));
                    }
                }
                statusLabel.Text = treeView.SelectedNode.FullPath + " set to " + coords[0];
                _request = string.Empty;
            }
            else if (_request.Equals("grabY"))
            {
                if (treeView.SelectedNode == null)
                {
                    MessageBox.Show("Please select a valid Tree Node to place the value in.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                    return;
                }

                string[] coords = statusCoordsLabel.Text.Split(new[] { 'X', 'Y', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                TreeNode addHere = treeView.SelectedNode;
                if (addHere.Tag.Equals(TreeNodeTag.Value))
                    addHere.Text = coords[1];
                else
                {
                    if (addHere.Tag.Equals(TreeNodeTag.Parameter) && addHere.Nodes.Count == 0)
                        addHere.Nodes.Add(getNewNode(coords[1], TreeNodeTag.Value));
                    else
                    {
                        addHere.Nodes[0].Remove();
                        addHere.Nodes.Add(getNewNode(coords[1], TreeNodeTag.Value));
                    }
                }
                statusLabel.Text = treeView.SelectedNode.FullPath + " set to " + coords[0];
                _request = string.Empty;
            }
        }

        /// <summary>
        /// TreeView event handler used to display context menus on right click.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void treeView_OnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //treeView.Font = new Font(treeView.Font, FontStyle.Bold);
            if (e.Button != MouseButtons.Right) return;
            Point p = new Point(e.X, e.Y);
            TreeNode node = treeView.GetNodeAt(p);
            Debug.WriteLine(node.Text + " is " + node.Tag);
            
            if (Convert.ToString(node.Tag).Equals(TreeNodeTag.Parameter) && node.Text.Equals("actions"))
            {
                treeActionsContextMenu.Show(treeView, p);
                treeView.SelectedNode = node;
                return;
            }

            if (Convert.ToString(node.Tag).Equals(TreeNodeTag.Parameter) && node.Text.Equals("flags"))
            {
                treeFlagsContextMenu.Show(treeView, p);
                treeView.SelectedNode = node;
                return;
            }

            switch ((TreeNodeTag)node.Tag)
            {
                case TreeNodeTag.File:
                    treeFileContextMenu.Show(treeView, p);
                    break;
                case TreeNodeTag.State:
                    treeStateContextMenu.Show(treeView, p);
                    break;
                case TreeNodeTag.Parameter:
                    treeParamContextMenu.Show(treeView, p);
                    break;
                case TreeNodeTag.Value:
                    treeValueContextMenu.Show(treeView, p);
                    break;
                case TreeNodeTag.Index:
                    treeIndexContextMenu.Show(treeView, p);
                    break;
            }
            treeView.SelectedNode = node;
        }

        /// <summary>
        /// View > Dependencies > Item event handler to open image or *spi file.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        void dependencyMenuItem_Click(object sender, EventArgs e)
        {
            // confirmUnsavedChanges with user that unsaved changed will be lost
            if (confirmUnsavedChanges() == DialogResult.Cancel) return;

            ToolStripMenuItem sent = sender as ToolStripMenuItem;
            if (sent != null)
                loadSprite(sent.Tag.ToString());
            else
                MessageBox.Show("Error: File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Help > Visit Editor Website event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void helpVisitWebsiteMenu_Click(object sender, EventArgs e)
        {
            Process.Start("http://avian.netne.net/index.php?p=programming&pid=7");
        }

        /// <summary>
        /// help > Submit Sprite event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void helpSubmitSpriteMenu_Click(object sender, EventArgs e)
        {
            Process.Start("http://sprites.caustik.com/forum/10-character-submissions/");
        }

        /// <summary>
        /// Tree Context Menu > Expand All nodes event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void expandAllContextMenuItem_Click(object sender, EventArgs e)
        {
            treeView.ExpandAll();
        }

        /// <summary>
        /// Tree Context Menu > Collapse All nodes event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void collapseAllContextMenuItem_Click(object sender, EventArgs e)
        {
            treeView.CollapseAll();
        }

        /// <summary>
        /// Tools > Prettify SPR status label event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelPrettifySPRMenuItem_OnMouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Give SPR file proper indentation, spacing, and line breaks.";
        }

        /// <summary>
        /// Set status label to default text on control mouse leave.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelSetDefault_OnMouseLeave(object sender, EventArgs e)
        {
            statusLabel.Text = "Sprite Editor By Avian";
        }
        
        /// <summary>
        /// Tools > Prettify SPR clicked event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void toolsPrettifySPRMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // set default directory to last opened or LocalAppData
                string defaultDir = Properties.Settings.Default.lastOpenedSpriteDir;
                if (string.IsNullOrEmpty(defaultDir) || !Directory.Exists(defaultDir))
                    defaultDir = string.IsNullOrEmpty(getSpriteCollectionLocation()) ?
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) : getSpriteCollectionLocation();

                    // display Open File dialog to select file to format
                    OpenFileDialog openFile = new OpenFileDialog
                                                  {
                                                      InitialDirectory = defaultDir,
                                                      Filter = "Sprite Files(*.spr;*.spi;*.json)|*.spr;*.spi;*.json|All files (*.*)|*.*",
                                                      RestoreDirectory = true,
                                                      Multiselect = false
                                                  };
                    DialogResult result = openFile.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        //read file to string then close
                        string jsonString = string.Empty;
                        using (StreamReader myFile = new StreamReader(openFile.FileName))
                        {
                            jsonString = myFile.ReadToEnd();
                        }

                        // parse into JSON object, serialize, save file
                        File.WriteAllText(openFile.FileName, prettifyJSON(jsonString));
                        Properties.Settings.Default.lastOpenedSpriteDir = Path.GetDirectoryName(openFile.FileName);
                        Properties.Settings.Default.Save();
                    }
                    statusLabel.Text = openFile.SafeFileName + " was cleaned and updated.";
                    openFile.Dispose();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Unable to load sprite. File not found or error reading input file.\n\nDetails:\n" + ex.Message,
                    "File Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to load sprite. File not found?";
                return;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Unable to load sprite. Is the input file blank, corrupt, or malformatted?\n\nDetails:\n" + ex.Message,
                    "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to load sprite. Is the file blank?";
                return;
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Unable to load sprite. Is the input file blank, corrupt, or malformatted?\n\nDetails:\n" + ex.Message,
                    "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to load sprite. File is malformatted.";
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load sprite. Unknown error. Please send avian the *.spr file so he can fix this.\n\nDetails:\n" + ex.Message,
                    "Generic Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to load sprite. Unknown error.";
                return;
            }
        }

        /// <summary>
        /// Helper for treeNode creation that sets proper Key/Name/Text/Tag.
        /// </summary>
        /// <param name="nameText">Name/Text of the node.</param>
        /// <param name="tag">Tag of node, must be File, Index, State, Parameter, Value</param>
        /// <returns>Properly initialized treeNode that is ready to add to tree.</returns>
        public TreeNode getNewNode(string nameText, TreeNodeTag tag)
        {
            TreeNode node = new TreeNode(nameText)
                           {
                               Name = nameText,
                               Tag = tag,
                               NodeFont = treeView.Font
                           };
            return node;
        }

        /// <summary>
        /// Helper that properly formats JSON files
        /// </summary>
        /// <param name="formatThis">Original JSON in string format</param>
        /// <returns>Properly formatted JSON string</returns>
        public static string prettifyJSON(string formatThis)
        {
            try
            {
                return JsonConvert.SerializeObject(JObject.Parse(formatThis), Formatting.Indented);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Convert System.Drawing.Color to HTML HEX string
        /// </summary>
        /// <param name="c">Color to convert</param>
        /// <returns>Hex value of input.</returns>
        public static String hexConverter(Color c)
        {
            return c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        /// <summary>
        /// Retrieve list of states appearing in a JSON formatted string.
        /// </summary>
        /// <param name="jsonString">Original JSON in string format</param>
        /// <returns>List of state names</returns>
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

        /// <summary>
        /// File > Save As event handler
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void fileSaveAsMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0)
            {
                statusLabel.Text = "Cannot save. Empty file!";
                return;
            }
            string defaultDir = Properties.Settings.Default.lastSavedDir;
            if (string.IsNullOrEmpty(defaultDir) || !Directory.Exists(defaultDir))
                defaultDir = string.IsNullOrEmpty(getSpriteCollectionLocation()) ?
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) : getSpriteCollectionLocation();

            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog
                                                     {
                                                         InitialDirectory = defaultDir,
                                                         Filter = "Sprite Files(*.spr;*.spi)|*.spr;*.spi|All files (*.*)|*.*",
                                                         FilterIndex = 1,
                                                         RestoreDirectory = true,
                                                         FileName = treeView.Nodes[0].Text
                                                     };

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter file = new StreamWriter(saveFileDialog1.FileName))
                    {
                        file.WriteLine(writeJsonFromTree(treeView));
                    }

                    _workingFile = saveFileDialog1.FileName;
                    addMRU(saveFileDialog1.FileName);
                    populateRecentFiles();
                    Properties.Settings.Default.lastSavedDir = Path.GetDirectoryName(saveFileDialog1.FileName);
                    Properties.Settings.Default.Save();
                }
                Debug.WriteLine(writeJsonFromTree(treeView));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save sprite. Unknown error. Please send avian the *.spr file so he can fix this.\n\nDetails:\n" + ex.Message,
                    "Generic Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Unable to save sprite. Unknown error.";
                return;
            }
        }

        /// <summary>
        /// Serialize TreeView into JSOn format.
        /// </summary>
        /// <param name="tree">TreeView to write JSON from.</param>
        /// <returns>Valid, formatted JSON string of tree contents or string.Empty if error occurs.</returns>
        private static string writeJsonFromTree(TreeView tree)
        {
            try
            {
                StringWriter sw = new StringWriter(new StringBuilder());
                JsonWriter jsonWriter = new JsonTextWriter(sw);
                jsonWriter.Formatting = Formatting.Indented;

                // START WRITING JSON
                jsonWriter.WriteStartObject();

                TreeNode stateNode = tree.Nodes[0].Nodes[0];

                // loop through each SPRITE_* node and process
                while (stateNode != null)
                {
                    jsonWriter.WritePropertyName(stateNode.Text);
                    jsonWriter.WriteStartObject();

                    // Check if state node has no subnodes
                    if (stateNode.Nodes.Count == 0)
                    {
                        jsonWriter.WriteEndObject();
                        stateNode = stateNode.NextNode;
                        continue;
                    }

                    // check properties and write arrays / subarrays
                    TreeNode parameterNode = stateNode.Nodes[0];
                    while (parameterNode != null)
                    {
                        // handle properties with sub arrays
                        if (parameterNode.Text.Equals("credits") || parameterNode.Text.Equals("fixtures") || parameterNode.Text.Equals("spawn"))
                        {
                            if (parameterNode.Nodes.Count == 0)
                            {
                                stateNode = stateNode.NextNode;
                                continue;
                            }
                            TreeNode indexNode = parameterNode.Nodes[0];

                            jsonWriter.WritePropertyName(parameterNode.Text);
                            jsonWriter.WriteStartArray();
                            while (indexNode != null)
                            {
                                if (indexNode.Nodes.Count == 0)
                                {
                                    indexNode = indexNode.NextNode;
                                }
                                TreeNode credProps = indexNode.Nodes[0];
                                jsonWriter.WriteStartObject();
                                int i = 0;
                                while (credProps != null)
                                {
                                    if (indexNode.Nodes[i].Nodes.Count == 0)
                                    {
                                        i++;
                                        credProps = credProps.NextNode;
                                        continue;
                                    }
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
                            if (parameterNode.Nodes.Count == 0)
                            {
                                parameterNode = parameterNode.NextNode;
                                continue;
                            }
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
                        TreeNode stateValue = parameterNode.Nodes.Count != 0 ? parameterNode.Nodes[0] : new TreeNode(string.Empty);
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
            catch (JsonException ex)
            {
                MessageBox.Show("Unable to serialize sprite. A write error has occurred.\n\nDetails:\n" + ex.Message,
                    "JSON Write Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to serialize sprite. Unknown error - please report problem to avian if issue persists.\n\nDetails:\n" + ex.Message,
                    "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        /// <summary>
        /// PictureBox event handler to set statusLabel and handle Quick Crop.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            // capture X/Y coords and display in statusLabel
            statusCoordsLabel.Text = "X: " + (e.X / zoomSlider.Value) + " Y: " + (e.Y / zoomSlider.Value);

            // draw rect for Quick Crop
            if (draw == true)
            {
                pictureBox.Refresh();
                quickW =  e.X - quickX;
                quickH = e.Y - quickY;
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    Rectangle rect = new Rectangle(quickX, quickY, quickW, quickH);
                    g.DrawRectangle(Pens.Black, rect);
                }
            }
        }

        /// <summary>
        /// View > Zoom In event handler. Zooms pictureBox.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void viewZoomInMenuItem_Click(object sender, EventArgs e)
        {
            if (zoomSlider.Value == zoomSlider.Maximum)
                return;
            zoomSlider.Value++;
            pictureBox.Width = pictureBox.Image.Width * zoomSlider.Value;
            pictureBox.Height = pictureBox.Image.Height * zoomSlider.Value;
        }

        /// <summary>
        /// View > Zoom Out event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void viewZoomOutMenuItem_Click(object sender, EventArgs e)
        {
            if (zoomSlider.Value == zoomSlider.Minimum)
                return;
            zoomSlider.Value--;
            pictureBox.Width = pictureBox.Image.Width * zoomSlider.Value;
            pictureBox.Height = pictureBox.Image.Height * zoomSlider.Value;
        }

        /// <summary>
        /// View > Actual Size event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void viewActualSizeMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox.Image = _imageOriginal;
            pictureBox.Width = pictureBox.Image.Width;
            pictureBox.Height = pictureBox.Image.Height;
            zoomSlider.Value = 1;
        }

        /// <summary>
        /// Event handler for image dependencies.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void viewDependencyImageMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            if (mi != null)
            {
                pictureBox.Image = new Bitmap(mi.Tag.ToString());
                _imageOriginal = pictureBox.Image;
                zoomSlider.Value = 1;
                pictureBox.Width = pictureBox.Image.Width;
                pictureBox.Height = pictureBox.Image.Height;
                pictureBox.Tag = mi.Text;
                statusLabel.Text = Text + " loaded OK.";
            }
        }

        /// <summary>
        /// Add new file to Properties.Settings.Default.MRU. Method enforces a max of 4 saved items.
        /// </summary>
        /// <param name="fileLoc">Location of the file that was opened.</param>
        private static void addMRU(string fileLoc)
        {
            List<string> g = new List<string>(Properties.Settings.Default.MRU.Split('?'));
            
            if (g.Any(i => i.Equals(fileLoc)))
                return;
            if (g.Count > 3)
                g.RemoveAt(0);

            Properties.Settings.Default.MRU = string.IsNullOrEmpty(g[0]) ? fileLoc : string.Concat(string.Join("?", g), "?", fileLoc);
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Helper for populating MRU list in File Menu
        /// </summary>
        private void populateRecentFiles()
        {
            fileMRUMenu.DropDownItems.Clear();
            string[] split = Properties.Settings.Default.MRU.Split('?');

            foreach (string rs in split)
            {
                if (string.IsNullOrEmpty(rs) || !File.Exists(rs))
                    continue;
                ToolStripMenuItem dynamicMenuItem = new ToolStripMenuItem(Path.GetFileName(rs));
                dynamicMenuItem.Click += dependencyMenuItem_Click;
                dynamicMenuItem.Tag = rs;
                fileMRUMenu.DropDownItems.Add(dynamicMenuItem);
            }
        }

        /// <summary>
        /// Tools > Sprite Preview clicked event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void toolsSpritePreviewMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new frmPreview(_images, _imageLocations, ref treeView))
            {
                if(form.DialogResult != DialogResult.Abort)
                    form.ShowDialog();
            }
        }

        /// <summary>
        /// View > Hide/Show Tree event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void viewHideTreeMenuItem_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                viewHideTreeMenuItem.Text = "Hide Tree";
                splitContainer1.Panel2Collapsed = false;
            }
            else
            {
                viewHideTreeMenuItem.Text = "Show Tree";
                splitContainer1.Panel2Collapsed = true;
            }
        }

        /// <summary>
        /// Tools > Test Sprite event handler.
        /// Saves working file and launches it in sprites.exe
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void toolsTestSpriteMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(getSpriteCollectionLocation()))
                {
                    MessageBox.Show("Unable to find sprites.exe. Do you have sprites installed?", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (treeView.Nodes.Count <= 0)
                    return;
                if (string.IsNullOrEmpty(_workingFile))
                {
                    MessageBox.Show("Please use File > Save As to set the working file location.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabel.Text = "Error: sprites.exe could not be found.";
                    return;
                }
                string folder = Path.GetDirectoryName(_workingFile);
                using (TextWriter tw = new StreamWriter(_workingFile))
                {
                    tw.WriteLine(writeJsonFromTree(treeView));
                }

                int i = 0;
                foreach (Bitmap b in _images)
                {
                    string loc = folder + "\\" + Path.GetFileName(_imageLocations[i++]);
                    if (!File.Exists(loc))
                        b.Save(loc);
                }

                Process.Start(_workingFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error preparing or testing sprite.\n\nDetails:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                statusLabel.Text = "Error: Could not Test Sprite - unexpected error.";
            }
        }

        /// <summary>
        /// Context Menu > Edit event handler. Lets you edit selected treeNode.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void editContextMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
                treeView.SelectedNode.BeginEdit();
        }

        /// <summary>
        /// Context Menu > Grab X Value event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void grabXContextMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && (treeView.SelectedNode.Tag.Equals(TreeNodeTag.Parameter) || treeView.SelectedNode.Tag.Equals(TreeNodeTag.Value)))
            {
                _request = "grabX";
                statusLabel.Text = "Click the image to set a X value.";
            }
            else
            {
                MessageBox.Show("Please select a Parameter in the tree to store the X value in", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Context Menu > Grab Y Value event handler.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void grabYMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && (treeView.SelectedNode.Tag.Equals(TreeNodeTag.Parameter) || treeView.SelectedNode.Tag.Equals(TreeNodeTag.Value)))
            {
                _request = "grabY";
                statusLabel.Text = "Click the image to set a Y value.";
            }
            else
            {
                MessageBox.Show("Please select a Parameter in the tree to store the X value in", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Context Menu > Grab Color event handler. Enables user to retrieve HTML Hex color from image.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void grabColorMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && (treeView.SelectedNode.Tag.Equals(TreeNodeTag.Parameter) || treeView.SelectedNode.Tag.Equals(TreeNodeTag.Value)))
            {
                _request = "grabHTML";
                statusLabel.Text = "Click the image to grab color value.";
            }
            else
            {
                MessageBox.Show("Please select a Parameter in the tree to store the X value in", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// View > Open image event handler. Loads an image for usage.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void viewOpenImageMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0)
            {
                MessageBox.Show("Cannot load picture. create a new file and try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string defaultDir = Properties.Settings.Default.lastOpenedImageDir;
                if (string.IsNullOrEmpty(defaultDir) || !Directory.Exists(defaultDir))
                    defaultDir = string.IsNullOrEmpty(getSpriteCollectionLocation()) ? 
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) : getSpriteCollectionLocation();
                OpenFileDialog openFile = new OpenFileDialog
                                              {
                                                  InitialDirectory = defaultDir,
                                                  Filter = "Image Files(*.png;*.jpg;*.gif;*.bmp)|*.png;*.jpg;*.gif;*.bmp|All files (*.*)|*.*",
                                                  RestoreDirectory = true,
                                                  Multiselect = false
                                              };
                DialogResult result = openFile.ShowDialog();
                if (result == DialogResult.OK)
                {
                    pictureBox.Image = new Bitmap(openFile.FileName);
                    pictureBox.Tag = openFile.SafeFileName;
                    pictureBox.Width = pictureBox.Image.Width;
                    pictureBox.Height = pictureBox.Image.Height;

                    statusLabel.Text = openFile.SafeFileName + " loaded " + result + ".";

                    zoomSlider.Value = 1;
                    _imageOriginal = pictureBox.Image;
                    _images.Add(new Bitmap(openFile.FileName));
                    _imageLocations.Add(openFile.FileName);
                    Properties.Settings.Default.lastOpenedImageDir = Path.GetDirectoryName(openFile.FileName);
                    Properties.Settings.Default.Save();
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

        /// <summary>
        /// Event handler to populate View > Dependencies menu.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void viewMenu_DropDownOpening(object sender, EventArgs e)
        {
            viewDependenciesMenu.DropDownItems.Clear();
            foreach (string i in _imageLocations)
            {
                ToolStripMenuItem addThis = new ToolStripMenuItem(Path.GetFileName(i), null, viewDependencyImageMenuItem_Click)
                {
                    Tag = i,
                    Image = Properties.Resources.generic_picture
                };
                viewDependenciesMenu.DropDownItems.Add(addThis);
            }
            foreach (ToolStripMenuItem d in _dependencies)
                viewDependenciesMenu.DropDownItems.Add(d);

            viewDependenciesMenu.Enabled = viewDependenciesMenu.DropDownItems.Count != 0;
            viewOpenImageMenuItem.Enabled = treeView.Nodes.Count > 0;
            viewShowErrorListMenuItem.Enabled = treeView.Nodes.Count > 0;
            if(errorList.Visible == false)
                viewShowErrorListMenuItem.Text = "Show Error List";
            else
                viewShowErrorListMenuItem.Text = "Hide Error List";
        }

        /// <summary>
        /// Event handler that sets selected parameter in tree to the URI of the currently displayed image.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void useImageMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                TreeNode addHere = treeView.SelectedNode;
                if (treeView.SelectedNode.Tag.Equals(TreeNodeTag.Parameter))
                {
                    if (addHere.Nodes.Count != 0)
                        addHere.Nodes[0].Text = pictureBox.Tag.ToString();
                    else
                        addHere.Nodes.Add(getNewNode(pictureBox.Tag.ToString(), TreeNodeTag.Value));
                }
                else if (treeView.SelectedNode.Tag.Equals(TreeNodeTag.Value))
                {
                    addHere.Text = pictureBox.Tag.ToString();
                }
                statusLabel.Text = "Operation successful.";
            }
            else
            {
                MessageBox.Show("Please select a Parameter in the tree to store the URI.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Event handler for Context Menu > Delete that deletes the selected treenode.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void deleteSelectedNodeMenuItem_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode.Remove();
        }

        /// <summary>
        /// Context Menu > Add State event handler. Prevents duplicate states and alerts user.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void addState_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem paramToAdd = sender as ToolStripMenuItem;
            if (treeView.Nodes.Count != 0 && paramToAdd != null && treeView.Nodes.Find(paramToAdd.Text, true).Length == 0)
                treeView.Nodes[0].Nodes.Add(getNewNode(paramToAdd.Text, TreeNodeTag.State));
            else
                MessageBox.Show("Node already exists, aborting!", "Unable to add Node.",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            sortTree();
        }

        /// <summary>
        /// Index Context Menu > Add New Group event handler. Creates new Index node.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void addNewGroupContextMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && treeView.SelectedNode.Tag.Equals(TreeNodeTag.Index))
            {
                TreeNode groupNode = treeView.SelectedNode;
                if (treeView.SelectedNode.Text.Contains("["))
                    groupNode = treeView.SelectedNode.Parent;
                int count = groupNode.Nodes.Count;
                groupNode.Nodes.Add(getNewNode(String.Concat("[", ++count, "]"), TreeNodeTag.Index));

                if (groupNode.Text.Equals("fixtures"))
                {
                    groupNode.LastNode.Nodes.Add(getNewNode("x", TreeNodeTag.Parameter));
                    groupNode.LastNode.Nodes.Add(getNewNode("y", TreeNodeTag.Parameter));
                    groupNode.LastNode.Nodes.Add(getNewNode("w", TreeNodeTag.Parameter));
                    groupNode.LastNode.Nodes.Add(getNewNode("h", TreeNodeTag.Parameter));
                }
                else if (groupNode.Text.Equals("credits"))
                {
                    groupNode.LastNode.Nodes.Add(getNewNode("author", TreeNodeTag.Parameter));
                    groupNode.LastNode.Nodes.Add(getNewNode("description", TreeNodeTag.Parameter));
                    groupNode.LastNode.Nodes.Add(getNewNode("url", TreeNodeTag.Parameter));
                }
                else if (groupNode.Text.Equals("spawn"))
                {
                    groupNode.LastNode.Nodes.Add(getNewNode("uri", TreeNodeTag.Parameter));
                    groupNode.LastNode.Nodes.Add(getNewNode("spawnX", TreeNodeTag.Parameter));
                    groupNode.LastNode.Nodes.Add(getNewNode("spawnY", TreeNodeTag.Parameter));
                    groupNode.LastNode.Nodes.Add(getNewNode("spawnExplode", TreeNodeTag.Parameter));
                }
            }
        }

        /// <summary>
        /// Event handler for File Menu to Enable/Disable MRU List
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void fileMenu_DropDownOpening(object sender, EventArgs e)
        {
            fileMRUMenu.Enabled = fileMRUMenu.DropDownItems.Count != 0;
        }

        /// <summary>
        /// Event handler for Tools Menu to Enable/Disable Sprite Preview and Test Sprite menu items.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void toolsMenu_DropDownOpening(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0 || !treeView.Nodes.Find("SPRITE_META_DATA", true).Any() ||
                !treeView.Nodes.Find("SPRITE_STATE_DEFAULT", true).Any())
            {
                toolsSpritePreviewMenuItem.Enabled = false;
                toolsTestSpriteMenuItem.Enabled = false;
            }
            else
            {
                toolsSpritePreviewMenuItem.Enabled = true;
                toolsTestSpriteMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Event Handler to set status label.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelSpritePreviewMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Show preview of each individual SPRITE_STATE";
        }

        /// <summary>
        /// Event Handler to set status label.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statsLabelTestSpriteMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Test sprite in sprites.exe";
        }

        /// <summary>
        /// Event Handler to set status label.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelVisitWebstieMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Visit Sprite Editor homepage.";
        }

        /// <summary>
        /// Event Handler to set status label.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelSubmitSpriteMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Visit Character Submissions Forum on sprites.caustik.com";
        }

        /// <summary>
        /// Event Handler to set status label.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelDependenciesMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Open associated images, *.spi, and *.spr files";
        }

        /// <summary>
        /// Event Handler to set status label.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelOpenImageMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Add new image to current sprite";
        }

        /// <summary>
        /// Event Handler to view online SPR file documentation in Web Browser.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelSPRFileDocumentationMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://sprites.caustik.com/topic/356-how-to-create-your-own-spr-files/");
        }

        /// <summary>
        /// Event Handler to set status label.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void statusLabelSPRFileDocumentationMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "View *.spr file format specs in web browser";
        }

        /// <summary>
        /// File > Save menu event handler. Saves if working file has been set.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void fileSaveMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_workingFile))
            {
                MessageBox.Show("Please use File > Save As to set the working file.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (treeView.Nodes.Count == 0)
            {
                statusLabel.Text = "Cannot save. Empty file!";
                return;
            }

            try
            {
                using (StreamWriter file = new StreamWriter(_workingFile))
                {
                    file.WriteLine(writeJsonFromTree(treeView));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to serialize sprite.\n\nDetails:\n" + ex.Message,
                    "File Write Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error: Cannot write file.";
                return;
            }

            statusLabel.Text = "File successfully saved!";
        }

        /// <summary>
        /// Adds next chain of selected state to tree. Prevents duplicates.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void incrementStateMenuItem_Click(object sender, EventArgs e)
        {
            // use regex to determine state number and create new state name
            string stateNo = _stateNumberRegex.Match(treeView.SelectedNode.Text).Value;
            string newState = string.IsNullOrEmpty(stateNo) ? string.Concat(treeView.SelectedNode.Text, "_0") : 
                string.Concat(_stateNumberRegex.Replace(treeView.SelectedNode.Text, string.Empty), int.Parse(stateNo) + 1);

            // add the new state node unless it already exists
            if (treeView.Nodes.Find(newState, true).Length == 0)
            {
                treeView.Nodes[0].Nodes.Add(getNewNode(newState, TreeNodeTag.State));

                // add isChain: 1 if first call to Increment State
                if (string.IsNullOrEmpty(stateNo))
                {
                    if (!treeView.SelectedNode.Nodes.ContainsKey("isChain"))
                    {
                        treeView.SelectedNode.Nodes.Add(getNewNode("isChain", TreeNodeTag.Parameter));
                        treeView.SelectedNode.Nodes[treeView.SelectedNode.Nodes.IndexOfKey("isChain")].Nodes.Add( getNewNode("1", TreeNodeTag.Value) );
                    }
                }
            }
            else
                MessageBox.Show("State " + newState + " already exists! Aborting.", "Unable to increment state.",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            sortTree();
        }

        /// <summary>
        /// Helper that will sort the tree using custom NodeSorter from Extensions.cs
        /// </summary>
        private void sortTree()
        {
            treeView.TreeViewNodeSorter = new NodeSorter();
        }

        /// <summary>
        /// Context Menu > Manual Entry selected. Lets user enter custom item into tree.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void manualEntryMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem s = sender as ToolStripMenuItem;
            if (s == null) return;
            treeView.SelectedNode.Nodes.Add(getNewNode(" ", treeNodeTagEnumFromString(s.Tag.ToString())));
            treeView.SelectedNode.Expand();
            treeView.SelectedNode.LastNode.BeginEdit();
        }

        /// <summary>
        /// Returns associated TreeNodetag enum from a string.
        /// </summary>
        /// <param name="s">String to parse into TreeNodeTag</param>
        /// <returns>Corresponding TreeNodeTag or TreeNodeTag.None if error.</returns>
        private static TreeNodeTag treeNodeTagEnumFromString(string s)
        {
            TreeNodeTag t;
            switch (s)
            {
                case "File":
                    t = TreeNodeTag.File;
                    break;
                case "Index":
                    t = TreeNodeTag.Index;
                    break;
                case "State":
                    t = TreeNodeTag.State;
                    break;
                case "Value":
                    t = TreeNodeTag.Value;
                    break;
                case "Parameter":
                    t = TreeNodeTag.Parameter;
                    break;
                default:
                    t = TreeNodeTag.None;
                    break;
            }
            return t;
        }

        /// <summary>
        /// Context Menu > Move Up event handler. Uses extension method to move selected node upwards.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void moveUpMenuItem_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode.MoveUp();
        }

        /// <summary>
        /// Context Menu > Move Down event handler. Uses extension method to move selected node downwards.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void moveDownMenuItem_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode.MoveDown();
        }

        /// <summary>
        /// Application Exit. Saves user preferences.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.FormHeight = Height;
            Properties.Settings.Default.FormWidth = Width;
            Properties.Settings.Default.FormTop = Top;
            Properties.Settings.Default.FormLeft = Left;
            Properties.Settings.Default.SplitterDistance = splitContainer1.SplitterDistance;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Context Menu > Add Param > Spawn event handler. Prevents duplicates.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void spawnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null || !treeView.SelectedNode.Tag.Equals(TreeNodeTag.State) ||
                treeView.SelectedNode.Nodes.Find("spawn", true).Any())
            {
                MessageBox.Show("Node \"spawn\" already exists or a valid state is not selected! Aborting.",
                    "Unable to add Node.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            treeView.SelectedNode.Expand();
            TreeNode spawnNode = getNewNode("spawn", TreeNodeTag.Index);
            TreeNode groupNode = getNewNode("[1]", TreeNodeTag.Index);
            treeView.SelectedNode.Nodes.Add(spawnNode);
            spawnNode.Nodes.Add(groupNode);
            groupNode.Nodes.Add(getNewNode("uri", TreeNodeTag.Parameter));
            groupNode.Nodes.Add(getNewNode("spawnX", TreeNodeTag.Parameter));
            groupNode.Nodes.Add(getNewNode("spawnY", TreeNodeTag.Parameter));
            groupNode.Nodes.Add(getNewNode("spawnExplode", TreeNodeTag.Parameter));
            spawnNode.Expand();
            groupNode.Expand();
        }

        // add parameter > fixtures clicked
        /// <summary>
        /// Context Menu > Add Param > Fixtures event handler. Prevents duplicates.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void fixturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null || !treeView.SelectedNode.Tag.Equals(TreeNodeTag.State) ||
                treeView.SelectedNode.Nodes.Find("fixtures", true).Any())
            {
                MessageBox.Show("Node \"fixtures\" already exists or a valid state is not selected! Aborting.",
                    "Unable to add Node.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            treeView.SelectedNode.Expand();
            TreeNode spawnNode = getNewNode("fixtures", TreeNodeTag.Index);
            TreeNode groupNode = getNewNode("[1]", TreeNodeTag.Index);
            treeView.SelectedNode.Nodes.Add(spawnNode);
            spawnNode.Nodes.Add(groupNode);
            groupNode.Nodes.Add(getNewNode("x", TreeNodeTag.Parameter));
            groupNode.Nodes.Add(getNewNode("y", TreeNodeTag.Parameter));
            groupNode.Nodes.Add(getNewNode("w", TreeNodeTag.Parameter));
            groupNode.Nodes.Add(getNewNode("h", TreeNodeTag.Parameter));
            spawnNode.Expand();
            groupNode.Expand();
        }

        /// <summary>
        /// Help > About event handler. Displays frmAbout.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void helpAboutMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new frmAbout())
            {
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Helper function to fetch location of sprite collection folder.
        /// </summary>
        /// <returns>Sprite Collection directory location or string.Empty if not found.</returns>
        private string getSpriteCollectionLocation()
        {
            object k = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Sprites", string.Empty, string.Empty);
            string val = k != null ? string.Concat(k, "\\collection") : string.Empty;
            return Directory.Exists(val) ? val : string.Empty;
        }

        /// <summary>
        /// Tools > Sprite Collection event handler. Opens collection in explorer.exe
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void toolsOpenSpriteCollectionMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(getSpriteCollectionLocation()))
                Process.Start(getSpriteCollectionLocation());
            else
                statusLabel.Text = "Unable to locate sprites collection. Is sprites installed?";
        }

        /// <summary>
        /// Confirm unsaved changes will be lost with user before opening new file
        /// </summary>
        /// <returns>DialogResult enum indicating user response (OK, Cancel, None)</returns>
        private DialogResult confirmUnsavedChanges()
        {
            return treeView.Nodes.Count != 0 ? MessageBox.Show("You will lose any unsaved changes to " + treeView.Nodes[0].Text + ". Continue?",
                "New File", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) : DialogResult.None;
        }

        /// <summary>
        /// Event handler for Quick Crop on PictureBox.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (treeView.Nodes.Count == 0)
                return;
            draw = true;
            quickX = e.X;
            quickY = e.Y;
        }

        /// <summary>
        /// Event handler for Quick Crop on PictureBox.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;

            Debug.WriteLine("Actual: cropX: " + quickX + ", cropY: " + quickY + ", cropW: " + quickW + ", cropH: " + quickH);
            Debug.WriteLine("Scaled: cropX: " + quickX / zoomSlider.Value + ", cropY: " + quickY / zoomSlider.Value
                + ", cropW: " + quickW / zoomSlider.Value + ", cropH: " + quickH / zoomSlider.Value);


            // get final cropX/Y/W/H values and set them in tree accordingly
            if (treeView.SelectedNode != null && treeView.SelectedNode.Tag.Equals(TreeNodeTag.State))
            {
                if (quickW > 0 && quickH > 0)
                {
                    // check to see if crop_ nodes already exist
                    TreeNode[] cropXnodes = treeView.SelectedNode.Nodes.Find("cropX", true);
                    TreeNode[] cropYnodes = treeView.SelectedNode.Nodes.Find("cropY", true);
                    TreeNode[] cropWnodes = treeView.SelectedNode.Nodes.Find("cropW", true);
                    TreeNode[] cropHnodes = treeView.SelectedNode.Nodes.Find("cropH", true);

                    // if node doesnt exist, create new one
                    // else check and see if node already has child and set accordingly
                    if (cropXnodes.Length == 0) // node doesnt exist, create it
                    {
                        treeView.SelectedNode.Nodes.Add(getNewNode("cropX", TreeNodeTag.Parameter));
                        treeView.SelectedNode.Nodes[treeView.SelectedNode.Nodes.IndexOfKey("cropX")].Nodes.Add(getNewNode((quickX / zoomSlider.Value).ToString(), TreeNodeTag.Value));
                    }
                    else // node does exist -> set text to new value
                    {
                        if (cropXnodes[0].Nodes.Count == 0) // node has no child, create new Value node and add
                            cropXnodes[0].Nodes.Add(getNewNode((quickX / zoomSlider.Value).ToString(), TreeNodeTag.Value));
                        else // node already exists, change value
                            cropXnodes[0].Nodes[0].Text = (quickX / zoomSlider.Value).ToString();
                    }

                    // repeat the above logic for cropY / cropW / cropH
                    if (cropYnodes.Length == 0)
                    {
                        treeView.SelectedNode.Nodes.Add(getNewNode("cropY", TreeNodeTag.Parameter));
                        treeView.SelectedNode.Nodes[treeView.SelectedNode.Nodes.IndexOfKey("cropY")].Nodes.Add(getNewNode((quickY / zoomSlider.Value).ToString(), TreeNodeTag.Value));
                    }
                    else
                    {
                        if (cropYnodes[0].Nodes.Count == 0)
                            cropYnodes[0].Nodes.Add(getNewNode((quickY / zoomSlider.Value).ToString(), TreeNodeTag.Value));
                        else
                            cropYnodes[0].Nodes[0].Text = (quickY / zoomSlider.Value).ToString();
                    }

                    if (cropWnodes.Length == 0)
                    {
                        treeView.SelectedNode.Nodes.Add(getNewNode("cropW", TreeNodeTag.Parameter));
                        treeView.SelectedNode.Nodes[treeView.SelectedNode.Nodes.IndexOfKey("cropW")].Nodes.Add(getNewNode((quickW / zoomSlider.Value).ToString(), TreeNodeTag.Value));
                    }
                    else
                    {
                        if (cropWnodes[0].Nodes.Count == 0)
                            cropWnodes[0].Nodes.Add(getNewNode((quickW / zoomSlider.Value).ToString(), TreeNodeTag.Value));
                        else
                            cropWnodes[0].Nodes[0].Text = (quickW / zoomSlider.Value).ToString();
                    }

                    if (cropHnodes.Length == 0)
                    {
                        treeView.SelectedNode.Nodes.Add(getNewNode("cropH", TreeNodeTag.Parameter));
                        treeView.SelectedNode.Nodes[treeView.SelectedNode.Nodes.IndexOfKey("cropH")].Nodes.Add(getNewNode((quickH / zoomSlider.Value).ToString(), TreeNodeTag.Value));
                    }
                    else
                    {
                        if (cropHnodes[0].Nodes.Count == 0)
                            cropHnodes[0].Nodes.Add(getNewNode((quickH / zoomSlider.Value).ToString(), TreeNodeTag.Value));
                        else
                            cropHnodes[0].Nodes[0].Text = (quickH / zoomSlider.Value).ToString();
                    }

                    // all done! set status label to let the user know it worked
                    statusLabel.Text = "Quick Crop successful. cropX: " + quickX / zoomSlider.Value + " cropY: " + quickY / zoomSlider.Value
                        + " cropW: " + quickW / zoomSlider.Value + " cropH: " + quickH / zoomSlider.Value;
                    treeView.SelectedNode.Expand();
                }
            }
            quickX = quickY = quickW = quickH = 1;
        }

        /// <summary>
        /// Tools > Validate Sprite selected.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void toolsValidateSpriteMenuItem_Click(object sender, EventArgs e)
        {
            List<string> errors = validateSprite();
            StringBuilder sb = new StringBuilder();

            // scan for errors and append to stringbuilder
            int i = 1;
            foreach (string s in errors)
            {
                sb.Append("\r\n[" + i + "] " + s);
                i++;
            }

            // post results to errorPanel controls
            if (errors.Count == 0)
            {
                errorList.Text = "No errors found.";
                errorIndicator.Image = Properties.Resources.check;
            }
            else
            {
                errorList.Text = sb.ToString();
                errorIndicator.Image = Properties.Resources.errorList;
            }
            errorIndicator.Text = "[" + errors.Count + "] errors found.";
            errorPanel.Visible = true;
        }


        /// <summary>
        /// Traverse treeView and check for errors in sprite.
        /// </summary>
        /// <returns>List of errors in string form.</returns>
        private List<string> validateSprite()
        {
            List<string> errorList = new List<string>();
            string chainNodeName = string.Empty;
            string prevStateName = string.Empty;

            if (treeView.Nodes.Count == 0 || treeView.Nodes[0].Nodes.Count == 0)
            {
                errorList.Add("Tree is empty or has no STATES to validate!");
                return errorList;
            }

            // check for required states
            if (!treeView.Nodes[0].Nodes.Find("SPRITE_META_DATA", true).Any())
                errorList.Add("State SPRITE_META_DATA is required and not found.");
            if (!treeView.Nodes[0].Nodes.Find("SPRITE_STATE_DEFAULT", true).Any())
                errorList.Add("State SPRITE_STATE_DEFAULT is required and not found.");

            //validate states
            TreeNode stateNode = treeView.Nodes[0].Nodes[0];
            while (stateNode != null)
            {
                Debug.WriteLine("Parsing: " + stateNode.Text);

                // get state name and number without chain
                string stateNo = _stateNumberRegex.Match(stateNode.Text).Value;
                string stateName = _stateNumberRegex.Replace(stateNode.Text, string.Empty);
                if (stateName.EndsWith("_"))
                    stateName = stateName.Remove(stateName.Length - 1);

                // detect invalid and missing isChains
                if (!stateNo.Equals(string.Empty) && prevStateName.StartsWith(chainNodeName) && !chainNodeName.Equals(stateName))
                    errorList.Add(stateNode.Text + " is unreachable. Add isChain:1 to " + stateName + ".");
                if (stateNode.Nodes.Find("isChain", true).Length != 0)
                    chainNodeName = stateNode.Text;

                // check sprite state is UPPER_CASE
                if (!stateNode.Text.Equals(stateNode.Text.ToUpper()) && _possibleStates.Contains<string>(stateName.ToUpper()))
                    errorList.Add("State [" + stateNode.Text + "] must be all UPPER CASE.");

                // check if STATE is listed in _possibleStates
                if (!_possibleStates.Contains<string>(stateName.ToUpper()))
                    errorList.Add("State [" + stateNode.Text + "] is not a supported state.");

                // parse STATE nodes
                if(stateNode.Nodes.Count != 0)
                {
                    TreeNode paramOrIndexNode = stateNode.Nodes[0];
                    while(paramOrIndexNode != null)
                    {
                        // validate parameters
                        if (paramOrIndexNode.Tag.Equals(TreeNodeTag.Parameter))
                        {
                            //Debug.WriteLine("In: " + stateNode.Text + "\\" + paramOrIndexNode.Text);
                            // is it in the list of supported params? if not, error
                            if (!_possibleParams.Contains<string>(paramOrIndexNode.Text) &&
                                !_possibleActions.Contains<string>(paramOrIndexNode.Text) &&
                                !_possibleFlags.Contains<string>(paramOrIndexNode.Text))
                            {
                                errorList.Add("Parameter [" + paramOrIndexNode.Text + "] in [" + stateNode.Text + "] is the wrong case or is invalid.");
                            }

                            // if param has no value subnode, error
                            if (paramOrIndexNode.Nodes.Count == 0 || string.IsNullOrWhiteSpace(paramOrIndexNode.Nodes[0].Text))
                                errorList.Add("Parameter [" + paramOrIndexNode.Text + "] in [" + stateNode.Text + "] does not have an attatched value.");
                            else
                            {
                                // check flag param types for 0 or 1 values
                                if (paramOrIndexNode.Text.Equals("flipX") || paramOrIndexNode.Text.Equals("isChain") ||
                                    paramOrIndexNode.Text.Equals("usePrevious") || paramOrIndexNode.Text.Equals("autoClose")
                                    || paramOrIndexNode.Text.Equals("spawnExplode"))
                                {
                                    if (!isValidFlag(paramOrIndexNode.Nodes[0].Text))
                                        errorList.Add("Param [" + paramOrIndexNode.Text + "] in [" + stateNode.Text + "] must have a value of 0 or 1.");
                                }

                                // check html hex value is valid
                                if (paramOrIndexNode.Text.Equals("transparent") && !isValidHTMLHexColor(paramOrIndexNode.Nodes[0].Text))
                                    errorList.Add("Param [" + paramOrIndexNode.Text + "] in [" + stateNode.Text + "] must have a valid HTML Hex color code such as \"F0F0F0\".");

                                // check uri filename is valid
                                if (paramOrIndexNode.Text.Equals("uri") && !isValidFileName(paramOrIndexNode.Nodes[0].Text))
                                    errorList.Add("Param [" + paramOrIndexNode.Text + "] in [" + stateNode.Text + "] does not have a valid filename.");

                                // check int params are valid
                                if ((paramOrIndexNode.Text.Equals("sizeMultiplier") || paramOrIndexNode.Text.Equals("sizeDivider") ||
                                    paramOrIndexNode.Text.Equals("frameDelay") || paramOrIndexNode.Text.Equals("cropX") ||
                                    paramOrIndexNode.Text.Equals("cropY") || paramOrIndexNode.Text.Equals("cropW") ||
                                    paramOrIndexNode.Text.Equals("cropH") || paramOrIndexNode.Text.Equals("offsX") ||
                                    paramOrIndexNode.Text.Equals("offsY")) && !isValidInteger((paramOrIndexNode.Nodes[0].Text)))
                                {
                                    errorList.Add("Param [" + paramOrIndexNode.Text + "] in [" + stateNode.Text + "] must be an Integer.");
                                }

                                // check double params valid
                                if (paramOrIndexNode.Text.Equals("walkMultiplier") && !isValidDouble(paramOrIndexNode.Nodes[0].Text))
                                    errorList.Add("Param [" + paramOrIndexNode.Text + "] in [" + stateNode.Text + "] must be a Double value.");
                            }
                        }

                        // validate indexes
                        if (paramOrIndexNode.Tag.Equals(TreeNodeTag.Index))
                        {
                            // check if valid array index node
                            if (!isValidIndexNode(paramOrIndexNode))
                                errorList.Add("Array index " + paramOrIndexNode.Text + " in [" + stateNode.Text + "] is invalid.");

                            // if index has no value subnode, error
                            if (paramOrIndexNode.Nodes.Count == 0 || string.IsNullOrWhiteSpace(paramOrIndexNode.Nodes[0].Text))
                                errorList.Add("Array Index " + paramOrIndexNode.Text + " in [" + stateNode.Text + "] does not have any attatched values.");
                            else
                            {
                                // iterate index subnodes and validate
                                TreeNode indexSubnode = paramOrIndexNode.Nodes[0];
                                while (indexSubnode != null)
                                {
                                    // check for empty indices
                                    if (indexSubnode.Nodes.Count == 0 || string.IsNullOrWhiteSpace(indexSubnode.Nodes[0].Text))
                                        errorList.Add("Node [" + indexSubnode.Text + "] in [" + stateNode.Text + "\\" + paramOrIndexNode.Text + "] does not have any attatched values.");
                                    else
                                    {
                                        // iterate index params and validate
                                        TreeNode indexParam = indexSubnode.Nodes[0];
                                        while (indexParam != null)
                                        {
                                            //Debug.WriteLine("In " + stateNode.Text + "\\" + paramOrIndexNode.Text + "\\" + indexSubnode.Text + "\\" + indexParam.Text);

                                            // check for valueless indexParams
                                            if (indexParam.Nodes.Count == 0 || string.IsNullOrWhiteSpace(indexParam.Nodes[0].Text))
                                                errorList.Add("Node [" + indexParam.Text + "] in [" + stateNode.Text + "\\" + paramOrIndexNode.Text + "\\" + indexSubnode.Text + "] does not have any attatched values.");
                                            else
                                            {
                                                // check for invalid indexParams in credits/spawn/fixtures
                                                if (paramOrIndexNode.Text.Equals("credits") && !indexParam.Text.Equals("author") && !indexParam.Text.Equals("url") && !indexParam.Text.Equals("description"))
                                                    errorList.Add("Node [" + indexParam.Text + "] in [" + stateNode.Text + "\\" + paramOrIndexNode.Text + "\\" + indexSubnode.Text + "] is invalid or the wrong case.");
                                                if (paramOrIndexNode.Text.Equals("spawn") && !indexParam.Text.Equals("uri") && !indexParam.Text.Equals("spawnX") && !indexParam.Text.Equals("spawnY") && !indexParam.Text.Equals("spawnExplode"))
                                                    errorList.Add("Node [" + indexParam.Text + "] in [" + stateNode.Text + "\\" + paramOrIndexNode.Text + "\\" + indexSubnode.Text + "] is invalid or the wrong case.");
                                                if (paramOrIndexNode.Text.Equals("fixtures") && !indexParam.Text.Equals("x") && !indexParam.Text.Equals("y") && !indexParam.Text.Equals("w") && !indexParam.Text.Equals("h"))
                                                    errorList.Add("Node [" + indexParam.Text + "] in [" + stateNode.Text + "\\" + paramOrIndexNode.Text + "\\" + indexSubnode.Text + "] is invalid or the wrong case.");
                                            }
                                            indexParam = indexParam.NextNode;
                                        }
                                    }
                                    indexSubnode = indexSubnode.NextNode;
                                } // end indexSubNode loop
                            }
                        }
                        paramOrIndexNode = paramOrIndexNode.NextNode;
                    } // end paramOrIndexNode loop
                }
                prevStateName = stateNode.Text;
                stateNode = stateNode.NextNode;
            } // end stateNode loop
            return errorList;
        }

        /// <summary>
        /// Checks if string has value of 0 or 1.
        /// </summary>
        /// <param name="p">String to validate.</param>
        /// <returns>True if p is 0 or 1, else false.</returns>
        private bool isValidFlag(string p)
        {
            if (!p.Equals("0") && !p.Equals("1"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Check if sprite is valid or not.
        /// </summary>
        /// <returns>True if no erros found. else false.</returns>
        private bool isValidSprite()
        {
            if (validateSprite().Count == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks to see if fileName or filePath contain invalid characters.
        /// </summary>
        /// <param name="fileName">File Name or Path to validate.</param>
        /// <returns>True if it does not contain invalid chars.</returns>
        private static bool isValidFileName(String fileName)
        {
            if (fileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Checks for valid HTML hex color code.
        /// </summary>
        /// <param name="colorHex">6 digit HTML hex color (excluding preceding sign).</param>
        /// <returns>True if valid color code, else false.</returns>
        private static bool isValidHTMLHexColor(String colorHex)
        {
            if (colorHex.Length == 6 && _htmlHexRegex.Match(colorHex).Success)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Validated an Index TreeNode to be in the format [n].
        /// </summary>
        /// <param name="indexNode">TreeNode with IndexNode tag to validate</param>
        /// <returns>True if valid, else false.</returns>
        private static bool isValidIndexNode(TreeNode indexNode)
        {
            if (!_indexNodeRegex.Match(indexNode.Text).Success && !indexNode.Text.Contains("credits")
                && !indexNode.Text.Contains("spawn") && !indexNode.Text.Contains("fixtures"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if string can be parsed as Integer.
        /// </summary>
        /// <param name="numberString">String to parse.</param>
        /// <returns>True if string is int, else false.</returns>
        private static bool isValidInteger(String numberString)
        {
            int conversionResult;
            if (Int32.TryParse(numberString, out conversionResult))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if string can be parsed as Double.
        /// </summary>
        /// <param name="numberString">String to parse.</param>
        /// <returns>True if string is double, else false.</returns>
        private static bool isValidDouble(String numberString)
        {
            double conversionResult;
            if (Double.TryParse(numberString, out conversionResult))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if string is a valid URL.
        /// </summary>
        /// <param name="url">URL to check.</param>
        /// <returns>True if valid URL is provided, else false.</returns>
        private static bool isValidURL(String url)
        {
            Regex urlRegEx = new Regex(@"(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?");
            if (urlRegEx.Match(url).Success)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Event handler to delete TreeNode on keys.Delete press.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && !treeView.SelectedNode.Tag.Equals(TreeNodeTag.File))
                treeView.SelectedNode.Remove();
        }


        /// <summary>
        /// Toggle ErrorListPanel's visibility.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void viewShowErrorListMenuItem_Click(object sender, EventArgs e)
        {
            if (errorPanel.Visible)
            {
                viewShowErrorListMenuItem.Text = "Show Error List";
                errorPanel.Visible = false;
            }
            else
            {
                viewShowErrorListMenuItem.Text = "Hide Error List";
                errorPanel.Visible = true;
            }
        }
    }
}