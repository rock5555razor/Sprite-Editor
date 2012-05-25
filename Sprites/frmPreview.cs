using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpriteEditor
{
    public partial class frmPreview : Form
    {
        private List<string> imageFileLocations;
        private List<Bitmap> imageBitmaps;

        public frmPreview()
        {
            InitializeComponent();
        }

        public frmPreview(List<Bitmap> images, List<string> imageLocations)
        {
            InitializeComponent();
            foreach (string i in imageLocations)
            {
                cmbStateToPreview.ComboBox.Items.Add(i.Substring(i.LastIndexOf("\\") + 1));
            }
            imageBitmaps = images;
            imageFileLocations = imageLocations;
        }

        private void frmPreview_Load(object sender, EventArgs e)
        {

        }

        private void cmbStateToPreview_SelectedIndexChanged(object sender, EventArgs e)
        {
            previewBox.Image = imageBitmaps[cmbStateToPreview.SelectedIndex];
        }
    }
}
