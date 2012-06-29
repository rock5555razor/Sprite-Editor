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
            this.label8 = new System.Windows.Forms.Label();
            this.statesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.flagsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.actionsCheckedListBox = new System.Windows.Forms.CheckedListBox();
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
            this.tabContainer = new System.Windows.Forms.TabControl();
            this.tabSpriteOptions.SuspendLayout();
            this.tabSpriteInfo.SuspendLayout();
            this.grpCredits.SuspendLayout();
            this.grpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoVersion)).BeginInit();
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
            this.tabSpriteOptions.Controls.Add(this.label8);
            this.tabSpriteOptions.Controls.Add(this.statesCheckedListBox);
            this.tabSpriteOptions.Controls.Add(this.label7);
            this.tabSpriteOptions.Controls.Add(this.label6);
            this.tabSpriteOptions.Controls.Add(this.flagsCheckedListBox);
            this.tabSpriteOptions.Controls.Add(this.actionsCheckedListBox);
            this.tabSpriteOptions.Location = new System.Drawing.Point(4, 22);
            this.tabSpriteOptions.Name = "tabSpriteOptions";
            this.tabSpriteOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpriteOptions.Size = new System.Drawing.Size(425, 184);
            this.tabSpriteOptions.TabIndex = 1;
            this.tabSpriteOptions.Text = "Sprite Options";
            this.tabSpriteOptions.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(270, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "States:";
            // 
            // statesCheckedListBox
            // 
            this.statesCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statesCheckedListBox.CheckOnClick = true;
            this.statesCheckedListBox.FormattingEnabled = true;
            this.statesCheckedListBox.Location = new System.Drawing.Point(273, 23);
            this.statesCheckedListBox.Name = "statesCheckedListBox";
            this.statesCheckedListBox.Size = new System.Drawing.Size(149, 152);
            this.statesCheckedListBox.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(133, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Flags:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(9, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Actions:";
            // 
            // flagsCheckedListBox
            // 
            this.flagsCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flagsCheckedListBox.CheckOnClick = true;
            this.flagsCheckedListBox.FormattingEnabled = true;
            this.flagsCheckedListBox.Location = new System.Drawing.Point(133, 23);
            this.flagsCheckedListBox.Name = "flagsCheckedListBox";
            this.flagsCheckedListBox.Size = new System.Drawing.Size(134, 152);
            this.flagsCheckedListBox.TabIndex = 10;
            // 
            // actionsCheckedListBox
            // 
            this.actionsCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.actionsCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.actionsCheckedListBox.CheckOnClick = true;
            this.actionsCheckedListBox.FormattingEnabled = true;
            this.actionsCheckedListBox.Location = new System.Drawing.Point(3, 23);
            this.actionsCheckedListBox.Name = "actionsCheckedListBox";
            this.actionsCheckedListBox.Size = new System.Drawing.Size(124, 152);
            this.actionsCheckedListBox.TabIndex = 9;
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
            this.infoURL.Location = new System.Drawing.Point(75, 64);
            this.infoURL.Name = "infoURL";
            this.infoURL.Size = new System.Drawing.Size(338, 20);
            this.infoURL.TabIndex = 3;
            this.infoURL.Text = "http://";
            // 
            // infoDescription
            // 
            this.infoDescription.BackColor = System.Drawing.Color.White;
            this.infoDescription.Location = new System.Drawing.Point(75, 38);
            this.infoDescription.Name = "infoDescription";
            this.infoDescription.Size = new System.Drawing.Size(338, 20);
            this.infoDescription.TabIndex = 2;
            this.infoDescription.Text = "Scripted by ";
            // 
            // infoAuthor
            // 
            this.infoAuthor.BackColor = System.Drawing.Color.White;
            this.infoAuthor.Location = new System.Drawing.Point(75, 12);
            this.infoAuthor.Name = "infoAuthor";
            this.infoAuthor.Size = new System.Drawing.Size(338, 20);
            this.infoAuthor.TabIndex = 1;
            this.infoAuthor.Text = "name";
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
            this.infoVersion.Location = new System.Drawing.Point(75, 40);
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
            this.infoVersion.Size = new System.Drawing.Size(338, 20);
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
            this.infoName.Location = new System.Drawing.Point(75, 12);
            this.infoName.Name = "infoName";
            this.infoName.Size = new System.Drawing.Size(338, 20);
            this.infoName.TabIndex = 1;
            this.infoName.Text = "new.spr";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Name:";
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
            this.Load += new System.EventHandler(this.frmWizard_Load);
            this.tabSpriteOptions.ResumeLayout(false);
            this.tabSpriteOptions.PerformLayout();
            this.tabSpriteInfo.ResumeLayout(false);
            this.grpCredits.ResumeLayout(false);
            this.grpCredits.PerformLayout();
            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoVersion)).EndInit();
            this.tabContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tabSpriteOptions;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckedListBox statesCheckedListBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckedListBox flagsCheckedListBox;
        private System.Windows.Forms.CheckedListBox actionsCheckedListBox;
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