using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Initializes and loads new instance of frmWizard
        /// </summary>
        /// <param name="possibleActions">List of valid actions</param>
        /// <param name="possibleFlags">List of valid Flags</param>
        /// <param name="possibleStates">List of valid States</param>
        public frmWizard(IEnumerable<string> possibleActions, IEnumerable<string> possibleFlags, IEnumerable<string> possibleStates)
        {
            InitializeComponent();
            foreach (string a in possibleActions)
                actionsCheckedListBox.Items.Add(a);
            foreach (string f in possibleFlags)
                flagsCheckedListBox.Items.Add(f);
            foreach (string stateless in possibleStates.Where(s => !s.Equals("SPRITE_META_DATA") && !s.Equals("SPRITE_STATE_DEFAULT")).Select(s => s.Replace("SPRITE_STATE_", "")))
                statesCheckedListBox.Items.Add(stateless);
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
            foreach (string a in actionsCheckedListBox.CheckedItems)
                _actions.Add(a);

            // validate and parse FLAGS
            foreach (string f in flagsCheckedListBox.CheckedItems)
                _flags.Add(f);

            // validate and parse FLAGS
            foreach (string s in statesCheckedListBox.CheckedItems)
                _states.Add("SPRITE_STATE_" + s);

            DialogResult = DialogResult.OK;
        }
    }
}
