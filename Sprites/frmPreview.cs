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
        private readonly List<Bitmap> _imageBitmaps;
        private readonly List<String> _imageLocations;
        private readonly List<Image> _previewable;
        private static readonly Regex _stateNumber = new Regex(@"\d$");
        private static readonly Regex _stateName = new Regex(@"\d+$");

        public frmPreview()
        {
            InitializeComponent();
        }

        public frmPreview(List<Bitmap> images, List<String> imageLocations, ref TreeView tree)
        {
            InitializeComponent();
            Dictionary<string, string> defaultInfo = new Dictionary<string, string>();
            _imageBitmaps = images;
            _imageLocations = imageLocations;
            _previewable = new List<Image>();

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
                    if(param.Nodes.Count != 0)
                        defaultInfo[param.Text] = param.Nodes[0].Text;
            }

            string trimmedPrevStateName = "";
            Image imageToProcess = _imageBitmaps[0];
            //int x, y, w, h;
            
            // loop through each state Node in the tree and process
            foreach (TreeNode n in tree.Nodes[0].Nodes)
            {
                // ignore META_DATA and populate combobox
                if (n.Text.EndsWith("META_DATA")) continue;
                if (cmbStateToPreview.ComboBox != null) cmbStateToPreview.ComboBox.Items.Add(n.Text);

                // should this state inherit from the previous state?
                string trimmedCurrentStateName = _stateName.Replace(n.Text, "").TrimEnd('_');
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
                        imageToProcess = _imageBitmaps[_imageLocations.IndexOf(s)];
                }

                // determine cropArea using supplied params
                if (cropInfo["cropX"] != null && cropInfo["cropY"] != null && cropInfo["cropW"] != null && cropInfo["cropH"] != null)
                {
                    Rectangle cropArea = new Rectangle(int.Parse(cropInfo["cropX"]), int.Parse(cropInfo["cropY"]),
                                                       int.Parse(cropInfo["cropW"]), int.Parse(cropInfo["cropH"]));
                    imageToProcess = cropImage(imageToProcess, cropArea);
                }

                // flipX if param is found
                if(cropInfo["flipX"] != null && cropInfo["flipX"].Equals("1"))
                    imageToProcess.RotateFlip(RotateFlipType.RotateNoneFlipX);

                // finalize by storing state name for inheritence and saving processed/cropped image
                trimmedPrevStateName = trimmedCurrentStateName;
                _previewable.Add(imageToProcess);
            }
            // all done, set zoom level to supplied sizeMultiplier
            if (defaultInfo["sizeMultiplier"] != null)
                _zoomLevel = int.Parse(defaultInfo["sizeMultiplier"]);
            cmbStateToPreview.SelectedItem = "SPRITE_STATE_DEFAULT";
        }

        private void cmbStateToPreview_SelectedIndexChanged(object sender, EventArgs e)
        {
            previewBox.Image = _previewable[cmbStateToPreview.SelectedIndex];
            previewBox.Width = previewBox.Image.Width * _zoomLevel;
            previewBox.Height = previewBox.Image.Height * _zoomLevel;
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
    }
}
