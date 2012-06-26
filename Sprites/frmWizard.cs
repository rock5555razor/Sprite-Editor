using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpriteEditor
{
    public partial class frmWizard : Form
    {
        public Dictionary<string, string> _spriteInfo;
        public List<string> _actions;
        public List<string> _flags;
        public List<string> _states;
        public List<CheckBox> _checkBoxes;

        public frmWizard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Main Form > File > New selected. Event handler that initializes fields to send back.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void frmWizard_Load(object sender, EventArgs e)
        {
            _spriteInfo = new Dictionary<string,string>();
            _actions = new List<string>();
            _flags = new List<string>();
            _states = new List<string>();
            _checkBoxes = new List<CheckBox>();

            foreach (Control c in this.Controls)
            {
                if (c is CheckBox)
                {
                    _checkBoxes.Add((CheckBox)c);
                }
                Debug.WriteLine(_checkBoxes.Count);
            } 
        }

        /// <summary>
        /// OK button event handler. Bundles checkbox results to send back to main form.
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event arguments.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // validate and process INFO
            if (!infoName.Text.Equals(string.Empty))
                _spriteInfo.Add("name", infoName.Text);
            else
            {
                MessageBox.Show("You must enter a sprite name!");
                return;
            }
            if (!infoVersion.Text.Equals(string.Empty))
                _spriteInfo.Add("version", infoVersion.Value.ToString());
            if (!infoAuthor.Text.Equals(string.Empty))
                _spriteInfo.Add("author", infoAuthor.Text);
            if (!infoDescription.Text.Equals(string.Empty))
                _spriteInfo.Add("description", infoDescription.Text);
            if (!infoURL.Text.Equals(string.Empty))
                _spriteInfo.Add("url", infoURL.Text);

            // validate and process ACTIONS
            if (actionWalk.Checked)
            {
                _actions.Add("walk");
                _states.Add("SPRITE_STATE_WALK_LEFT");
                _states.Add("SPRITE_STATE_WALK_RIGHT");
            }
            if (actionFly.Checked)
            {
                _actions.Add("fly");
                _states.Add("SPRITE_STATE_FLY_LEFT");
                _states.Add("SPRITE_STATE_FLY_RIGHT");
            }
            if (actionRun.Checked)
            {
                _actions.Add("run");
                _states.Add("SPRITE_STATE_RUN_LEFT");
                _states.Add("SPRITE_STATE_RUN_RIGHT");
            }
            if (actionJump.Checked)
            {
                _actions.Add("jump");
                _states.Add("SPRITE_STATE_JUMP_LEFT");
                _states.Add("SPRITE_STATE_JUMP_RIGHT");
            }
            if (actionDestroy.Checked)
            {
                _actions.Add("destroy");
                _states.Add("SPRITE_STATE_DESTROY_LEFT");
                _states.Add("SPRITE_STATE_DESTROY_RIGHT");
            }
            if (actionDeath.Checked)
            {
                _actions.Add("death");
            }

            // validate and parse FLAGS
            if (flagIsCollector.Checked)
                _flags.Add("isCollector");
            if (flagIsItem.Checked)
                _flags.Add("isItem");
            if (flagDisablePhysics.Checked)
                _flags.Add("disablePhysics");
            if (flagDisableWindowCollide.Checked)
                _flags.Add("disableWindowCollide");
            if (flagDisableSpriteCollide.Checked)
                _flags.Add("disableSpriteCollide");
            if (flagDisableJump.Checked)
                _flags.Add("disableJump");
            if (flagDoFadeOut.Checked)
                _flags.Add("doFadeOut");

            // validate and parse STATES
            if (stateStand.Checked)
            {
                _states.Add("SPRITE_STATE_STAND_LEFT");
                _states.Add("SPRITE_STATE_STAND_RIGHT");
            }

            DialogResult = DialogResult.OK;
        }

        private void buttonWalker_Click(object sender, EventArgs e)
        {

        }
    }
}
