using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace SpriteEditor
{
    public partial class frmPreview : Form
    {
        private int _zoomLevel = 1;
        private readonly List<Image> _imageBitmaps;
        private readonly List<String> _imageLocations;
        private readonly List<Image> _previewable;
        private readonly Dictionary<string,string> _offs;
        private static readonly Regex _stateName = new Regex(@"\d+$");

        public frmPreview()
        {
            InitializeComponent();
        }

        public frmPreview(List<Image> images, List<String> imageLocations, ref TreeView tree)
        {
            InitializeComponent();
            Dictionary<string, string> defaultInfo = new Dictionary<string, string>();
            _imageBitmaps = new List<Image>(images);
            _imageLocations = new List<string>(imageLocations);
            _previewable = new List<Image>();
            _offs = new Dictionary<string, string>();
            string curImageLoc = imageLocations[0];

            // list of possible state parameters
            string[] keys = {
                                "uri", "flipX", "sizeMultiplier", "sizeDivider", "frameDelay",
                                "cropX", "cropY", "cropW", "cropH", "offsX", "offsY", "isChain",
                                "usePrevious", "transparent"
                            };

            // add each key to dictionary - one for default and one for comparison
            foreach (string s in keys)
                defaultInfo.Add(s, null);

            Dictionary<string, string> cropInfo = new Dictionary<string, string>(defaultInfo);

            // populate SPRITE_STATE_DEFAULT param settings into defaultInfo dictionary
            TreeNode[] defaultNode = tree.Nodes.Find("SPRITE_STATE_DEFAULT", true);
            if (defaultNode.Length != 0 && defaultNode[0].Nodes.Count != 0)
            {
                foreach (TreeNode param in defaultNode[0].Nodes)
                    if (param.Nodes.Count != 0)
                        defaultInfo[param.Text] = param.Nodes[0].Text;
            }

            string trimmedPrevStateName = string.Empty;
            
            // loop through each state Node in the tree and process
            foreach (TreeNode n in tree.Nodes[0].Nodes)
            {
                if (images.Count == 0)
                {
                    MessageBox.Show("No images loaded! Cannot preview any sprite states.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Abort;
                    return;
                }
                Image imageToProcess = images[0];
                // ignore META_DATA and populate combobox
                if (n.Text.EndsWith("META_DATA"))
                    continue;
                if (cmbStateToPreview.ComboBox != null)
                    cmbStateToPreview.ComboBox.Items.Add(n.Text);

                // should this state inherit from the previous state?
                string trimmedCurrentStateName = _stateName.Replace(n.Text, string.Empty).TrimEnd('_');
                if (!trimmedCurrentStateName.Equals(trimmedPrevStateName))
                    cropInfo = new Dictionary<string, string>(defaultInfo);

                // populate current STATE's parameters from tree and keys[]
                foreach (string k in keys)
                {
                    TreeNode[] results = n.Nodes.Find(k, true);
                    if (results.Length != 0 && results[0].Nodes.Count != 0)
                        cropInfo[k] = results[0].Nodes[0].Text;
                }

                // determine image to use from URI param
                foreach (string s in _imageLocations)
                {
                    if (s.Contains(cropInfo["uri"]))
                    {
                        imageToProcess = images[_imageLocations.IndexOf(s)];
                        curImageLoc = imageLocations[_imageLocations.IndexOf(s)];
                    }
                }

                //calculate offset X and Y values
                int left = previewBox.Left + Convert.ToInt32(cropInfo["offsX"]);
                int top = previewBox.Top + Convert.ToInt32(cropInfo["offsY"]);
                _offs.Add(n.Text,String.Concat(left, ",", top));

                if (!imageToProcess.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                {
                    // determine cropArea using supplied params
                    if (!string.IsNullOrEmpty(cropInfo["cropX"]) && !string.IsNullOrEmpty(cropInfo["cropY"]) &&
                        !string.IsNullOrEmpty(cropInfo["cropW"]) && !string.IsNullOrEmpty(cropInfo["cropH"]))
                    {
                        // prevent OutOfMemoryException by making sure coords are in bounds
                        int cropx = int.Parse(cropInfo["cropX"]);
                        int cropy = int.Parse(cropInfo["cropY"]);
                        int cropw = int.Parse(cropInfo["cropW"]);
                        int croph = int.Parse(cropInfo["cropH"]);
                        if (cropy + croph > imageToProcess.Height) croph = imageToProcess.Height - cropy;
                        if (cropx + cropw > imageToProcess.Width) cropw = imageToProcess.Width - cropx;
                        Rectangle cropArea = new Rectangle(cropx, cropy, cropw, croph);
                        imageToProcess = cropImage(imageToProcess, cropArea);
                    }

                    // flipX if param is found
                    if (!string.IsNullOrEmpty(cropInfo["flipX"]) && cropInfo["flipX"].Equals("1"))
                        imageToProcess.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                //}
                //else
                //{
                //    // special case - flipX on animated gif without losing data
                //    if (!string.IsNullOrEmpty(cropInfo["flipX"]) && cropInfo["flipX"].Equals("1"))
                //    {
                //        // decode GIF and store frames
                //        List<Image> frameList = new List<Image>();
                //        //gifDecoder.Read(curImageLoc);
                //        //for (int i = 0, count = gifDecoder.GetFrameCount(); i < count; i++)
                //        //{
                //        //    Image frame = gifDecoder.GetFrame(i); // frame i
                //        //    frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
                //        //    frameList.Add(frame);
                //        //}
                //        //delay = gifDecoder.GetDelay(0);
                //
                //        //// encode GIF from frames
                //        //AnimatedGifEncoder enc = new AnimatedGifEncoder();
                //        //enc.SetQuality(0);
                //        //enc.Start(tempfile);
                //        //enc.SetDelay(delay);
                //        ////enc.SetTransparent(Color.Black);
                //        //enc.SetRepeat(0); // -1: no repeat, 0: always repeat
                //        //for (int i = 0, count = frameList.Count; i < count; i++)
                //        //{
                //        //    enc.AddFrame(frameList[i]);
                //        //}
                //        //enc.Finish();
                //        //imageToProcess = Image.FromFile(tempfile);
                //    }
                //}

                // finalize by storing state name for inheritence and saving processed/cropped image
                trimmedPrevStateName = trimmedCurrentStateName;
                _previewable.Add(imageToProcess);
            }

            // all done, set zoom level to supplied sizeMultiplier
            if (!string.IsNullOrEmpty(defaultInfo["sizeMultiplier"]))
                _zoomLevel = int.Parse(defaultInfo["sizeMultiplier"]);

            // select STATE according to TREE selection
            if (tree.SelectedNode != null)
            {
                if (tree.SelectedNode.Tag.Equals("State"))
                    cmbStateToPreview.SelectedItem = tree.SelectedNode.Text;
                else if (tree.SelectedNode.Tag.Equals("Parameter"))
                    cmbStateToPreview.SelectedItem = tree.SelectedNode.Parent.Text;
                else if (tree.SelectedNode.Tag.Equals("Value"))
                    cmbStateToPreview.SelectedItem = tree.SelectedNode.Parent.Parent.Text;
                else
                    cmbStateToPreview.SelectedItem = "SPRITE_STATE_DEFAULT";
            }
            else
                cmbStateToPreview.SelectedItem = "SPRITE_STATE_DEFAULT";
        }

        private void cmbStateToPreview_SelectedIndexChanged(object sender, EventArgs e)
        {
            previewBox.Image = _previewable[cmbStateToPreview.SelectedIndex];
            previewBox.Width = previewBox.Image.Width * _zoomLevel;
            previewBox.Height = previewBox.Image.Height * _zoomLevel;
            string[] xy = _offs[cmbStateToPreview.Text].Split(',');
            previewBox.Left = Convert.ToInt32(xy[0]);
            previewBox.Top = Convert.ToInt32(xy[1]);
        }

        private static Image cropImage(Image img, Rectangle rect)
        {
            return ((Bitmap)img).Clone(rect, img.PixelFormat);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            if (cmbStateToPreview.SelectedIndex > 0)
                cmbStateToPreview.SelectedIndex = --cmbStateToPreview.SelectedIndex;
        }

        private void nextbutton_Click(object sender, EventArgs e)
        {
            if (cmbStateToPreview.SelectedIndex < cmbStateToPreview.Items.Count-1)
                cmbStateToPreview.SelectedIndex = ++cmbStateToPreview.SelectedIndex;
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            if (_zoomLevel < 10)
                _zoomLevel++;
            previewBox.Width = previewBox.Image.Width * _zoomLevel;
            previewBox.Height = previewBox.Image.Height * _zoomLevel;
        }

        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            if (_zoomLevel > 1)
                _zoomLevel--;
            previewBox.Width = previewBox.Image.Width * _zoomLevel;
            previewBox.Height = previewBox.Image.Height * _zoomLevel;
        }

        private void frmPreview_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
