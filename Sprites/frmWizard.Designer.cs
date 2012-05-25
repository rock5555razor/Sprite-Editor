namespace Sprites
{
    partial class frmWizard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWizard));
            this.btnOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabContainer = new System.Windows.Forms.TabControl();
            this.tabSpriteInfo = new System.Windows.Forms.TabPage();
            this.grpCredits = new System.Windows.Forms.GroupBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.infoURL = new System.Windows.Forms.TextBox();
            this.infoDescription = new System.Windows.Forms.TextBox();
            this.infoAuthor = new System.Windows.Forms.TextBox();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.grpInfo = new System.Windows.Forms.GroupBox();
            this.infoVersion = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.infoName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSpriteOptions = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flagDisableJump = new System.Windows.Forms.CheckBox();
            this.flagDoFadeOut = new System.Windows.Forms.CheckBox();
            this.flagIsItem = new System.Windows.Forms.CheckBox();
            this.flagDisableSpriteCollide = new System.Windows.Forms.CheckBox();
            this.flagDisableWindowCollide = new System.Windows.Forms.CheckBox();
            this.flagDisablePhysics = new System.Windows.Forms.CheckBox();
            this.flagIsCollector = new System.Windows.Forms.CheckBox();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.actionFly = new System.Windows.Forms.CheckBox();
            this.actionDestroy = new System.Windows.Forms.CheckBox();
            this.actionJump = new System.Windows.Forms.CheckBox();
            this.actionRun = new System.Windows.Forms.CheckBox();
            this.actionDeath = new System.Windows.Forms.CheckBox();
            this.actionWalk = new System.Windows.Forms.CheckBox();
            this.tabSpriteStates = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stateMetaData = new System.Windows.Forms.CheckBox();
            this.stateJump = new System.Windows.Forms.CheckBox();
            this.stateRun = new System.Windows.Forms.CheckBox();
            this.stateWalk = new System.Windows.Forms.CheckBox();
            this.stateDefault = new System.Windows.Forms.CheckBox();
            this.stateFly = new System.Windows.Forms.CheckBox();
            this.tabContainer.SuspendLayout();
            this.tabSpriteInfo.SuspendLayout();
            this.grpCredits.SuspendLayout();
            this.grpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoVersion)).BeginInit();
            this.tabSpriteOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.tabSpriteStates.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOK.Location = new System.Drawing.Point(277, 212);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(358, 212);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tabContainer
            // 
            this.tabContainer.Controls.Add(this.tabSpriteInfo);
            this.tabContainer.Controls.Add(this.tabSpriteOptions);
            this.tabContainer.Controls.Add(this.tabSpriteStates);
            this.tabContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabContainer.Location = new System.Drawing.Point(0, 0);
            this.tabContainer.Name = "tabContainer";
            this.tabContainer.SelectedIndex = 0;
            this.tabContainer.Size = new System.Drawing.Size(433, 210);
            this.tabContainer.TabIndex = 5;
            // 
            // tabSpriteInfo
            // 
            this.tabSpriteInfo.Controls.Add(this.grpCredits);
            this.tabSpriteInfo.Controls.Add(this.grpInfo);
            this.tabSpriteInfo.Location = new System.Drawing.Point(4, 22);
            this.tabSpriteInfo.Name = "tabSpriteInfo";
            this.tabSpriteInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpriteInfo.Size = new System.Drawing.Size(425, 184);
            this.tabSpriteInfo.TabIndex = 0;
            this.tabSpriteInfo.Text = "Sprite Info";
            this.tabSpriteInfo.UseVisualStyleBackColor = true;
            // 
            // grpCredits
            // 
            this.grpCredits.Controls.Add(this.lblURL);
            this.grpCredits.Controls.Add(this.lblDescription);
            this.grpCredits.Controls.Add(this.infoURL);
            this.grpCredits.Controls.Add(this.infoDescription);
            this.grpCredits.Controls.Add(this.infoAuthor);
            this.grpCredits.Controls.Add(this.lblAuthor);
            this.grpCredits.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCredits.Location = new System.Drawing.Point(3, 71);
            this.grpCredits.Name = "grpCredits";
            this.grpCredits.Size = new System.Drawing.Size(419, 95);
            this.grpCredits.TabIndex = 6;
            this.grpCredits.TabStop = false;
            this.grpCredits.Text = "Credits";
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(6, 64);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(32, 13);
            this.lblURL.TabIndex = 5;
            this.lblURL.Text = "URL:";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 41);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "Description:";
            // 
            // infoURL
            // 
            this.infoURL.BackColor = System.Drawing.Color.White;
            this.infoURL.Location = new System.Drawing.Point(80, 64);
            this.infoURL.Name = "infoURL";
            this.infoURL.Size = new System.Drawing.Size(333, 20);
            this.infoURL.TabIndex = 3;
            // 
            // infoDescription
            // 
            this.infoDescription.BackColor = System.Drawing.Color.White;
            this.infoDescription.Location = new System.Drawing.Point(80, 38);
            this.infoDescription.Name = "infoDescription";
            this.infoDescription.Size = new System.Drawing.Size(333, 20);
            this.infoDescription.TabIndex = 2;
            // 
            // infoAuthor
            // 
            this.infoAuthor.BackColor = System.Drawing.Color.White;
            this.infoAuthor.Location = new System.Drawing.Point(80, 12);
            this.infoAuthor.Name = "infoAuthor";
            this.infoAuthor.Size = new System.Drawing.Size(333, 20);
            this.infoAuthor.TabIndex = 1;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(7, 20);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(41, 13);
            this.lblAuthor.TabIndex = 0;
            this.lblAuthor.Text = "Author:";
            // 
            // grpInfo
            // 
            this.grpInfo.Controls.Add(this.infoVersion);
            this.grpInfo.Controls.Add(this.label2);
            this.grpInfo.Controls.Add(this.infoName);
            this.grpInfo.Controls.Add(this.label1);
            this.grpInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpInfo.Location = new System.Drawing.Point(3, 3);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(419, 68);
            this.grpInfo.TabIndex = 5;
            this.grpInfo.TabStop = false;
            this.grpInfo.Text = "Sprite Info";
            // 
            // infoVersion
            // 
            this.infoVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoVersion.BackColor = System.Drawing.Color.White;
            this.infoVersion.Location = new System.Drawing.Point(80, 40);
            this.infoVersion.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.infoVersion.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.infoVersion.Name = "infoVersion";
            this.infoVersion.Size = new System.Drawing.Size(333, 20);
            this.infoVersion.TabIndex = 4;
            this.infoVersion.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Version:";
            // 
            // infoName
            // 
            this.infoName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoName.BackColor = System.Drawing.Color.White;
            this.infoName.Location = new System.Drawing.Point(80, 12);
            this.infoName.Name = "infoName";
            this.infoName.Size = new System.Drawing.Size(333, 20);
            this.infoName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sprite Name:";
            // 
            // tabSpriteOptions
            // 
            this.tabSpriteOptions.Controls.Add(this.groupBox1);
            this.tabSpriteOptions.Controls.Add(this.grpActions);
            this.tabSpriteOptions.Location = new System.Drawing.Point(4, 22);
            this.tabSpriteOptions.Name = "tabSpriteOptions";
            this.tabSpriteOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpriteOptions.Size = new System.Drawing.Size(425, 184);
            this.tabSpriteOptions.TabIndex = 1;
            this.tabSpriteOptions.Text = "Sprite Options";
            this.tabSpriteOptions.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flagDisableJump);
            this.groupBox1.Controls.Add(this.flagDoFadeOut);
            this.groupBox1.Controls.Add(this.flagIsItem);
            this.groupBox1.Controls.Add(this.flagDisableSpriteCollide);
            this.groupBox1.Controls.Add(this.flagDisableWindowCollide);
            this.groupBox1.Controls.Add(this.flagDisablePhysics);
            this.groupBox1.Controls.Add(this.flagIsCollector);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(419, 66);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sprite Flags";
            // 
            // flagDisableJump
            // 
            this.flagDisableJump.AutoSize = true;
            this.flagDisableJump.Location = new System.Drawing.Point(168, 42);
            this.flagDisableJump.Name = "flagDisableJump";
            this.flagDisableJump.Size = new System.Drawing.Size(83, 17);
            this.flagDisableJump.TabIndex = 6;
            this.flagDisableJump.Text = "disableJump";
            this.flagDisableJump.UseVisualStyleBackColor = true;
            // 
            // flagDoFadeOut
            // 
            this.flagDoFadeOut.AutoSize = true;
            this.flagDoFadeOut.Location = new System.Drawing.Point(285, 42);
            this.flagDoFadeOut.Name = "flagDoFadeOut";
            this.flagDoFadeOut.Size = new System.Drawing.Size(78, 17);
            this.flagDoFadeOut.TabIndex = 5;
            this.flagDoFadeOut.Text = "doFadeOut";
            this.flagDoFadeOut.UseVisualStyleBackColor = true;
            // 
            // flagIsItem
            // 
            this.flagIsItem.AutoSize = true;
            this.flagIsItem.Location = new System.Drawing.Point(98, 19);
            this.flagIsItem.Name = "flagIsItem";
            this.flagIsItem.Size = new System.Drawing.Size(52, 17);
            this.flagIsItem.TabIndex = 4;
            this.flagIsItem.Text = "isItem";
            this.flagIsItem.UseVisualStyleBackColor = true;
            // 
            // flagDisableSpriteCollide
            // 
            this.flagDisableSpriteCollide.AutoSize = true;
            this.flagDisableSpriteCollide.Location = new System.Drawing.Point(7, 42);
            this.flagDisableSpriteCollide.Name = "flagDisableSpriteCollide";
            this.flagDisableSpriteCollide.Size = new System.Drawing.Size(116, 17);
            this.flagDisableSpriteCollide.TabIndex = 3;
            this.flagDisableSpriteCollide.Text = "disableSpriteCollide";
            this.flagDisableSpriteCollide.UseVisualStyleBackColor = true;
            // 
            // flagDisableWindowCollide
            // 
            this.flagDisableWindowCollide.AutoSize = true;
            this.flagDisableWindowCollide.Location = new System.Drawing.Point(285, 19);
            this.flagDisableWindowCollide.Name = "flagDisableWindowCollide";
            this.flagDisableWindowCollide.Size = new System.Drawing.Size(128, 17);
            this.flagDisableWindowCollide.TabIndex = 2;
            this.flagDisableWindowCollide.Text = "disableWindowCollide";
            this.flagDisableWindowCollide.UseVisualStyleBackColor = true;
            // 
            // flagDisablePhysics
            // 
            this.flagDisablePhysics.AutoSize = true;
            this.flagDisablePhysics.Location = new System.Drawing.Point(168, 19);
            this.flagDisablePhysics.Name = "flagDisablePhysics";
            this.flagDisablePhysics.Size = new System.Drawing.Size(94, 17);
            this.flagDisablePhysics.TabIndex = 1;
            this.flagDisablePhysics.Text = "disablePhysics";
            this.flagDisablePhysics.UseVisualStyleBackColor = true;
            // 
            // flagIsCollector
            // 
            this.flagIsCollector.AutoSize = true;
            this.flagIsCollector.Location = new System.Drawing.Point(7, 19);
            this.flagIsCollector.Name = "flagIsCollector";
            this.flagIsCollector.Size = new System.Drawing.Size(73, 17);
            this.flagIsCollector.TabIndex = 0;
            this.flagIsCollector.Text = "isCollector";
            this.flagIsCollector.UseVisualStyleBackColor = true;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.actionFly);
            this.grpActions.Controls.Add(this.actionDestroy);
            this.grpActions.Controls.Add(this.actionJump);
            this.grpActions.Controls.Add(this.actionRun);
            this.grpActions.Controls.Add(this.actionDeath);
            this.grpActions.Controls.Add(this.actionWalk);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpActions.Location = new System.Drawing.Point(3, 3);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(419, 51);
            this.grpActions.TabIndex = 2;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Sprite Actions";
            // 
            // actionFly
            // 
            this.actionFly.AutoSize = true;
            this.actionFly.Location = new System.Drawing.Point(76, 20);
            this.actionFly.Name = "actionFly";
            this.actionFly.Size = new System.Drawing.Size(38, 17);
            this.actionFly.TabIndex = 5;
            this.actionFly.Text = "Fly";
            this.actionFly.UseVisualStyleBackColor = true;
            // 
            // actionDestroy
            // 
            this.actionDestroy.AutoSize = true;
            this.actionDestroy.Location = new System.Drawing.Point(270, 19);
            this.actionDestroy.Name = "actionDestroy";
            this.actionDestroy.Size = new System.Drawing.Size(61, 17);
            this.actionDestroy.TabIndex = 4;
            this.actionDestroy.Text = "Destroy";
            this.actionDestroy.UseVisualStyleBackColor = true;
            // 
            // actionJump
            // 
            this.actionJump.AutoSize = true;
            this.actionJump.Location = new System.Drawing.Point(198, 19);
            this.actionJump.Name = "actionJump";
            this.actionJump.Size = new System.Drawing.Size(50, 17);
            this.actionJump.TabIndex = 3;
            this.actionJump.Text = "Jump";
            this.actionJump.UseVisualStyleBackColor = true;
            // 
            // actionRun
            // 
            this.actionRun.AutoSize = true;
            this.actionRun.Location = new System.Drawing.Point(129, 19);
            this.actionRun.Name = "actionRun";
            this.actionRun.Size = new System.Drawing.Size(45, 17);
            this.actionRun.TabIndex = 2;
            this.actionRun.Text = "Run";
            this.actionRun.UseVisualStyleBackColor = true;
            // 
            // actionDeath
            // 
            this.actionDeath.AutoSize = true;
            this.actionDeath.Location = new System.Drawing.Point(351, 19);
            this.actionDeath.Name = "actionDeath";
            this.actionDeath.Size = new System.Drawing.Size(54, 17);
            this.actionDeath.TabIndex = 1;
            this.actionDeath.Text = "Death";
            this.actionDeath.UseVisualStyleBackColor = true;
            // 
            // actionWalk
            // 
            this.actionWalk.AutoSize = true;
            this.actionWalk.Location = new System.Drawing.Point(7, 20);
            this.actionWalk.Name = "actionWalk";
            this.actionWalk.Size = new System.Drawing.Size(50, 17);
            this.actionWalk.TabIndex = 0;
            this.actionWalk.Text = "Walk";
            this.actionWalk.UseVisualStyleBackColor = true;
            // 
            // tabSpriteStates
            // 
            this.tabSpriteStates.Controls.Add(this.groupBox2);
            this.tabSpriteStates.Location = new System.Drawing.Point(4, 22);
            this.tabSpriteStates.Name = "tabSpriteStates";
            this.tabSpriteStates.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpriteStates.Size = new System.Drawing.Size(425, 184);
            this.tabSpriteStates.TabIndex = 2;
            this.tabSpriteStates.Text = "Sprite States";
            this.tabSpriteStates.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.stateFly);
            this.groupBox2.Controls.Add(this.stateMetaData);
            this.groupBox2.Controls.Add(this.stateJump);
            this.groupBox2.Controls.Add(this.stateRun);
            this.groupBox2.Controls.Add(this.stateWalk);
            this.groupBox2.Controls.Add(this.stateDefault);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(419, 121);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sprite States";
            // 
            // stateMetaData
            // 
            this.stateMetaData.AutoSize = true;
            this.stateMetaData.Checked = true;
            this.stateMetaData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stateMetaData.Enabled = false;
            this.stateMetaData.Location = new System.Drawing.Point(210, 19);
            this.stateMetaData.Name = "stateMetaData";
            this.stateMetaData.Size = new System.Drawing.Size(135, 17);
            this.stateMetaData.TabIndex = 5;
            this.stateMetaData.Text = "SPRITE_META_DATA";
            this.stateMetaData.UseVisualStyleBackColor = true;
            // 
            // stateJump
            // 
            this.stateJump.AutoSize = true;
            this.stateJump.Location = new System.Drawing.Point(23, 65);
            this.stateJump.Name = "stateJump";
            this.stateJump.Size = new System.Drawing.Size(140, 17);
            this.stateJump.TabIndex = 4;
            this.stateJump.Text = "SPRITE_STATE_JUMP";
            this.stateJump.UseVisualStyleBackColor = true;
            // 
            // stateRun
            // 
            this.stateRun.AutoSize = true;
            this.stateRun.Location = new System.Drawing.Point(23, 42);
            this.stateRun.Name = "stateRun";
            this.stateRun.Size = new System.Drawing.Size(135, 17);
            this.stateRun.TabIndex = 2;
            this.stateRun.Text = "SPRITE_STATE_RUN";
            this.stateRun.UseVisualStyleBackColor = true;
            // 
            // stateWalk
            // 
            this.stateWalk.AutoSize = true;
            this.stateWalk.Location = new System.Drawing.Point(23, 19);
            this.stateWalk.Name = "stateWalk";
            this.stateWalk.Size = new System.Drawing.Size(142, 17);
            this.stateWalk.TabIndex = 1;
            this.stateWalk.Text = "SPRITE_STATE_WALK";
            this.stateWalk.UseVisualStyleBackColor = true;
            // 
            // stateDefault
            // 
            this.stateDefault.AutoSize = true;
            this.stateDefault.Checked = true;
            this.stateDefault.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stateDefault.Enabled = false;
            this.stateDefault.Location = new System.Drawing.Point(210, 42);
            this.stateDefault.Name = "stateDefault";
            this.stateDefault.Size = new System.Drawing.Size(160, 17);
            this.stateDefault.TabIndex = 0;
            this.stateDefault.Text = "SPRITE_STATE_DEFAULT";
            this.stateDefault.UseVisualStyleBackColor = true;
            // 
            // stateFly
            // 
            this.stateFly.AutoSize = true;
            this.stateFly.Location = new System.Drawing.Point(23, 88);
            this.stateFly.Name = "stateFly";
            this.stateFly.Size = new System.Drawing.Size(130, 17);
            this.stateFly.TabIndex = 6;
            this.stateFly.Text = "SPRITE_STATE_FLY";
            this.stateFly.UseVisualStyleBackColor = true;
            // 
            // frmWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 238);
            this.Controls.Add(this.tabContainer);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmWizard";
            this.Text = "New Sprite Wizard";
            this.Load += new System.EventHandler(this.frmProperties_Load);
            this.tabContainer.ResumeLayout(false);
            this.tabSpriteInfo.ResumeLayout(false);
            this.grpCredits.ResumeLayout(false);
            this.grpCredits.PerformLayout();
            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoVersion)).EndInit();
            this.tabSpriteOptions.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.tabSpriteStates.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabContainer;
        private System.Windows.Forms.TabPage tabSpriteInfo;
        private System.Windows.Forms.GroupBox grpCredits;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox infoURL;
        private System.Windows.Forms.TextBox infoDescription;
        private System.Windows.Forms.TextBox infoAuthor;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.GroupBox grpInfo;
        private System.Windows.Forms.NumericUpDown infoVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox infoName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabSpriteOptions;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.CheckBox actionFly;
        private System.Windows.Forms.CheckBox actionDestroy;
        private System.Windows.Forms.CheckBox actionJump;
        private System.Windows.Forms.CheckBox actionRun;
        private System.Windows.Forms.CheckBox actionDeath;
        private System.Windows.Forms.CheckBox actionWalk;
        private System.Windows.Forms.TabPage tabSpriteStates;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox flagDisableJump;
        private System.Windows.Forms.CheckBox flagDoFadeOut;
        private System.Windows.Forms.CheckBox flagIsItem;
        private System.Windows.Forms.CheckBox flagDisableSpriteCollide;
        private System.Windows.Forms.CheckBox flagDisableWindowCollide;
        private System.Windows.Forms.CheckBox flagDisablePhysics;
        private System.Windows.Forms.CheckBox flagIsCollector;
        private System.Windows.Forms.CheckBox stateJump;
        private System.Windows.Forms.CheckBox stateRun;
        private System.Windows.Forms.CheckBox stateWalk;
        private System.Windows.Forms.CheckBox stateDefault;
        private System.Windows.Forms.CheckBox stateMetaData;
        private System.Windows.Forms.CheckBox stateFly;
    }
}