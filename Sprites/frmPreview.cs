using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SpriteEditor
{
    public partial class frmPreview : Form
    {
        private List<string> _imageFileLocations;
        private List<Bitmap> _imageBitmaps;

        public frmPreview()
        {
            InitializeComponent();
        }

        public frmPreview(List<Bitmap> images, List<string> imageLocations)
        {
            InitializeComponent();
            foreach (string i in imageLocations)
            {
                if (cmbStateToPreview.ComboBox != null)
                    cmbStateToPreview.ComboBox.Items.Add(i.Substring(i.LastIndexOf("\\", StringComparison.Ordinal) + 1));
            }
            _imageBitmaps = images;
            _imageFileLocations = imageLocations;
        }

        private void frmPreview_Load(object sender, EventArgs e)
        {

        }

        private void cmbStateToPreview_SelectedIndexChanged(object sender, EventArgs e)
        {
            previewBox.Image = _imageBitmaps[cmbStateToPreview.SelectedIndex];
        }
    }
}
