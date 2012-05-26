namespace SpriteEditor
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
            this.tabSpriteOptions = new System.Windows.Forms.TabPage();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.actionWalk = new System.Windows.Forms.CheckBox();
            this.actionDeath = new System.Windows.Forms.CheckBox();
            this.actionRun = new System.Windows.Forms.CheckBox();
            this.actionJump = new System.Windows.Forms.CheckBox();
            this.actionDestroy = new System.Windows.Forms.CheckBox();
            this.actionFly = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flagIsCollector = new System.Windows.Forms.CheckBox();
            this.flagDisablePhysics = new System.Windows.Forms.CheckBox();
            this.flagDisableWindowCollide = new System.Windows.Forms.CheckBox();
            this.flagDisableSpriteCollide = new System.Windows.Forms.CheckBox();
            this.flagIsItem = new System.Windows.Forms.CheckBox();
            this.flagDoFadeOut = new System.Windows.Forms.CheckBox();
            this.flagDisableJump = new System.Windows.Forms.CheckBox();
            this.stateStand = new System.Windows.Forms.CheckBox();
            this.tabSpriteInfo = new System.Windows.Forms.TabPage();
            this.grpInfo = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.infoName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.infoVersion = new System.Windows.Forms.NumericUpDown();
            this.grpCredits = new System.Windows.Forms.GroupBox();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.infoAuthor = new System.Windows.Forms.TextBox();
            this.infoDescription = new System.Windows.Forms.TextBox();
            this.infoURL = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblURL = new System.Windows.Forms.Label();
            this.tabContainer = new System.Windows.Forms.TabControl();
            this.tabSpriteOptions.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabSpriteInfo.SuspendLayout();
            this.grpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoVersion)).BeginInit();
            this.grpCredits.SuspendLayout();
            this.tabContainer.SuspendLayout();
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
            // tabSpriteOptions
            // 
            this.tabSpriteOptions.Controls.Add(this.stateStand);
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
            // stateStand
            // 
            this.stateStand.AutoSize = true;
            this.stateStand.Location = new System.Drawing.Point(8, 126);
            this.stateStand.Name = "stateStand";
            this.stateStand.Size = new System.Drawing.Size(291, 17);
            this.stateStand.TabIndex = 8;
            this.stateStand.Text = "Add SPRITE_STATE_STAND_LEFT and RIGHT states";
            this.stateStand.UseVisualStyleBackColor = true;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sprite Name:";
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Version:";
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
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(7, 20);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(41, 13);
            this.lblAuthor.TabIndex = 0;
            this.lblAuthor.Text = "Author:";
            // 
            // infoAuthor
            // 
            this.infoAuthor.BackColor = System.Drawing.Color.White;
            this.infoAuthor.Location = new System.Drawing.Point(80, 12);
            this.infoAuthor.Name = "infoAuthor";
            this.infoAuthor.Size = new System.Drawing.Size(333, 20);
            this.infoAuthor.TabIndex = 1;
            // 
            // infoDescription
            // 
            this.infoDescription.BackColor = System.Drawing.Color.White;
            this.infoDescription.Location = new System.Drawing.Point(80, 38);
            this.infoDescription.Name = "infoDescription";
            this.infoDescription.Size = new System.Drawing.Size(333, 20);
            this.infoDescription.TabIndex = 2;
            // 
            // infoURL
            // 
            this.infoURL.BackColor = System.Drawing.Color.White;
            this.infoURL.Location = new System.Drawing.Point(80, 64);
            this.infoURL.Name = "infoURL";
            this.infoURL.Size = new System.Drawing.Size(333, 20);
            this.infoURL.TabIndex = 3;
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
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(6, 64);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(32, 13);
            this.lblURL.TabIndex = 5;
            this.lblURL.Text = "URL:";
            // 
            // tabContainer
            // 
            this.tabContainer.Controls.Add(this.tabSpriteInfo);
            this.tabContainer.Controls.Add(this.tabSpriteOptions);
            this.tabContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabContainer.Location = new System.Drawing.Point(0, 0);
            this.tabContainer.Name = "tabContainer";
            this.tabContainer.SelectedIndex = 0;
            this.tabContainer.Size = new System.Drawing.Size(433, 210);
            this.tabContainer.TabIndex = 5;
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
            this.tabSpriteOptions.ResumeLayout(false);
            this.tabSpriteOptions.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabSpriteInfo.ResumeLayout(false);
            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoVersion)).EndInit();
            this.grpCredits.ResumeLayout(false);
            this.grpCredits.PerformLayout();
            this.tabContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tabSpriteOptions;
        private System.Windows.Forms.CheckBox stateStand;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox flagDisableJump;
        private System.Windows.Forms.CheckBox flagDoFadeOut;
        private System.Windows.Forms.CheckBox flagIsItem;
        private System.Windows.Forms.CheckBox flagDisableSpriteCollide;
        private System.Windows.Forms.CheckBox flagDisableWindowCollide;
        private System.Windows.Forms.CheckBox flagDisablePhysics;
        private System.Windows.Forms.CheckBox flagIsCollector;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.CheckBox actionFly;
        private System.Windows.Forms.CheckBox actionDestroy;
        private System.Windows.Forms.CheckBox actionJump;
        private System.Windows.Forms.CheckBox actionRun;
        private System.Windows.Forms.CheckBox actionDeath;
        private System.Windows.Forms.CheckBox actionWalk;
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
        private System.Windows.Forms.TabControl tabContainer;
    }
}