using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace SpriteEditor
{
    public partial class frmWizard : Form
    {
        public Dictionary<string, string> spriteInfo;
        public List<string> actions;
        public List<string> flags;
        public List<string> states;


        public frmWizard(ref TreeView tree)
        {
            InitializeComponent();

            // handle non-new spr files
            if (tree.Nodes.Count == 0)
            {
                return;
            }
            else // handle new spr files
            {
                Debug.WriteLine("Load Tree Settings for change");
            }
        }

        public frmWizard()
        {
            InitializeComponent();
        }

        private void frmProperties_Load(object sender, EventArgs e)
        {
            spriteInfo = new Dictionary<string,string>();
            actions = new List<string>();
            flags = new List<string>();
            states = new List<string>();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // validate and process INFO
            if (!infoName.Text.Equals(""))
                spriteInfo.Add("name", infoName.Text);
            else { MessageBox.Show("You must enter a sprite name!"); return; }
            if (!infoVersion.Text.Equals(""))
                spriteInfo.Add("version", infoVersion.Value.ToString());
            if (!infoAuthor.Text.Equals(""))
                spriteInfo.Add("author", infoAuthor.Text);
            if (!infoDescription.Text.Equals(""))
                spriteInfo.Add("description", infoDescription.Text);
            if (!infoURL.Text.Equals(""))
                spriteInfo.Add("url", infoURL.Text);

            // validate and process ACTIONS
            if (actionWalk.Checked)
                actions.Add("walk");
            if (actionFly.Checked)
                actions.Add("fly");
            if (actionRun.Checked)
                actions.Add("run");
            if (actionJump.Checked)
                actions.Add("jump");
            if (actionDestroy.Checked)
                actions.Add("destroy");
            if (actionDeath.Checked)
                actions.Add("death");

            // validate and parse FLAGS
            if (flagIsCollector.Checked)
                flags.Add("isCollector");
            if (flagIsItem.Checked)
                flags.Add("isItem");
            if (flagDisablePhysics.Checked)
                flags.Add("disablePhysics");
            if (flagDisableWindowCollide.Checked)
                flags.Add("disableWindowCollide");
            if (flagDisableSpriteCollide.Checked)
                flags.Add("disableSpriteCollide");
            if (flagDisableJump.Checked)
                flags.Add("disableJump");
            if (flagDoFadeOut.Checked)
                flags.Add("doFadeOut");

            // validate and parse STATES
            if (stateWalk.Checked) { states.Add("SPRITE_STATE_WALK_LEFT"); states.Add("SPRITE_STATE_WALK_RIGHT"); }
            if (stateRun.Checked) { states.Add("SPRITE_STATE_RUN_LEFT"); states.Add("SPRITE_STATE_RUN_LEFT"); }
            if (stateJump.Checked) { states.Add("SPRITE_STATE_JUMP_LEFT"); states.Add("SPRITE_STATE_JUMP_LEFT"); }
            if (stateFly.Checked) { states.Add("SPRITE_STATE_FLY_LEFT"); states.Add("SPRITE_STATE_FLY_LEFT"); }

            this.DialogResult = DialogResult.OK;
        }
    }
}
