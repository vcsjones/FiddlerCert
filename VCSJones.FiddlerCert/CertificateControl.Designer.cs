namespace VCSJones.FiddlerCert
{
    partial class CertificateControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.exportCertButton = new System.Windows.Forms.Button();
            this.installCertButton = new System.Windows.Forms.Button();
            this.certStatusToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.itemsTabControl = new System.Windows.Forms.TabControl();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.certStatusImage = new System.Windows.Forms.PictureBox();
            this.hashAlgorithmLabel = new System.Windows.Forms.Label();
            this.hashAlgorithmTextLabel = new System.Windows.Forms.Label();
            this.validDatesLabel = new System.Windows.Forms.Label();
            this.validFromTextLabel = new System.Windows.Forms.Label();
            this.keySizeLabel = new System.Windows.Forms.Label();
            this.keySizeTextLabel = new System.Windows.Forms.Label();
            this.algorithmLabel = new System.Windows.Forms.Label();
            this.algorithmTextLabel = new System.Windows.Forms.Label();
            this.thumbprintLabel = new System.Windows.Forms.Label();
            this.thumbprintTextLabel = new System.Windows.Forms.Label();
            this.subjectAltNameLabel = new System.Windows.Forms.Label();
            this.subjectAltNameTextLabel = new System.Windows.Forms.Label();
            this.commonNameLabel = new System.Windows.Forms.Label();
            this.certIcon = new System.Windows.Forms.PictureBox();
            this.pkpHashesTab = new System.Windows.Forms.TabPage();
            this.shaThumbprintLabel = new System.Windows.Forms.Label();
            this.shaFingerpintTextLabel = new System.Windows.Forms.Label();
            this.itemsTabControl.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.certStatusImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.certIcon)).BeginInit();
            this.pkpHashesTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // exportCertButton
            // 
            this.exportCertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exportCertButton.Location = new System.Drawing.Point(455, 172);
            this.exportCertButton.Name = "exportCertButton";
            this.exportCertButton.Size = new System.Drawing.Size(75, 23);
            this.exportCertButton.TabIndex = 2;
            this.exportCertButton.Text = "View";
            this.exportCertButton.UseVisualStyleBackColor = true;
            this.exportCertButton.Click += new System.EventHandler(this.viewCertButton_Click);
            // 
            // installCertButton
            // 
            this.installCertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.installCertButton.Location = new System.Drawing.Point(374, 172);
            this.installCertButton.Name = "installCertButton";
            this.installCertButton.Size = new System.Drawing.Size(75, 23);
            this.installCertButton.TabIndex = 13;
            this.installCertButton.Text = "Install";
            this.installCertButton.UseVisualStyleBackColor = true;
            this.installCertButton.Click += new System.EventHandler(this.installCertButton_Click);
            // 
            // certStatusToolTip
            // 
            this.certStatusToolTip.AutoPopDelay = 5000;
            this.certStatusToolTip.InitialDelay = 250;
            this.certStatusToolTip.ReshowDelay = 100;
            // 
            // itemsTabControl
            // 
            this.itemsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsTabControl.Controls.Add(this.generalTabPage);
            this.itemsTabControl.Controls.Add(this.pkpHashesTab);
            this.itemsTabControl.Location = new System.Drawing.Point(3, 3);
            this.itemsTabControl.Name = "itemsTabControl";
            this.itemsTabControl.SelectedIndex = 0;
            this.itemsTabControl.Size = new System.Drawing.Size(527, 163);
            this.itemsTabControl.TabIndex = 19;
            // 
            // generalTabPage
            // 
            this.generalTabPage.AutoScroll = true;
            this.generalTabPage.Controls.Add(this.certStatusImage);
            this.generalTabPage.Controls.Add(this.hashAlgorithmLabel);
            this.generalTabPage.Controls.Add(this.hashAlgorithmTextLabel);
            this.generalTabPage.Controls.Add(this.validDatesLabel);
            this.generalTabPage.Controls.Add(this.validFromTextLabel);
            this.generalTabPage.Controls.Add(this.keySizeLabel);
            this.generalTabPage.Controls.Add(this.keySizeTextLabel);
            this.generalTabPage.Controls.Add(this.algorithmLabel);
            this.generalTabPage.Controls.Add(this.algorithmTextLabel);
            this.generalTabPage.Controls.Add(this.thumbprintLabel);
            this.generalTabPage.Controls.Add(this.thumbprintTextLabel);
            this.generalTabPage.Controls.Add(this.subjectAltNameLabel);
            this.generalTabPage.Controls.Add(this.subjectAltNameTextLabel);
            this.generalTabPage.Controls.Add(this.commonNameLabel);
            this.generalTabPage.Controls.Add(this.certIcon);
            this.generalTabPage.Location = new System.Drawing.Point(4, 22);
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalTabPage.Size = new System.Drawing.Size(519, 137);
            this.generalTabPage.TabIndex = 0;
            this.generalTabPage.Text = "Overview";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // certStatusImage
            // 
            this.certStatusImage.BackColor = System.Drawing.SystemColors.Control;
            this.certStatusImage.Image = global::VCSJones.FiddlerCert.Properties.Resources.security_Shields_Blank_16xLG;
            this.certStatusImage.Location = new System.Drawing.Point(44, 10);
            this.certStatusImage.Name = "certStatusImage";
            this.certStatusImage.Size = new System.Drawing.Size(16, 16);
            this.certStatusImage.TabIndex = 33;
            this.certStatusImage.TabStop = false;
            // 
            // hashAlgorithmLabel
            // 
            this.hashAlgorithmLabel.AutoSize = true;
            this.hashAlgorithmLabel.Location = new System.Drawing.Point(139, 105);
            this.hashAlgorithmLabel.Name = "hashAlgorithmLabel";
            this.hashAlgorithmLabel.Size = new System.Drawing.Size(104, 13);
            this.hashAlgorithmLabel.TabIndex = 32;
            this.hashAlgorithmLabel.Text = "[Signature Algorithm]";
            // 
            // hashAlgorithmTextLabel
            // 
            this.hashAlgorithmTextLabel.AutoSize = true;
            this.hashAlgorithmTextLabel.Location = new System.Drawing.Point(3, 105);
            this.hashAlgorithmTextLabel.Name = "hashAlgorithmTextLabel";
            this.hashAlgorithmTextLabel.Size = new System.Drawing.Size(101, 13);
            this.hashAlgorithmTextLabel.TabIndex = 31;
            this.hashAlgorithmTextLabel.Text = "Signature Algorithm:";
            // 
            // validDatesLabel
            // 
            this.validDatesLabel.AutoSize = true;
            this.validDatesLabel.Location = new System.Drawing.Point(139, 121);
            this.validDatesLabel.Name = "validDatesLabel";
            this.validDatesLabel.Size = new System.Drawing.Size(67, 13);
            this.validDatesLabel.TabIndex = 30;
            this.validDatesLabel.Text = "[Valid Dates]";
            // 
            // validFromTextLabel
            // 
            this.validFromTextLabel.AutoSize = true;
            this.validFromTextLabel.Location = new System.Drawing.Point(3, 121);
            this.validFromTextLabel.Name = "validFromTextLabel";
            this.validFromTextLabel.Size = new System.Drawing.Size(59, 13);
            this.validFromTextLabel.TabIndex = 29;
            this.validFromTextLabel.Text = "Valid From:";
            // 
            // keySizeLabel
            // 
            this.keySizeLabel.AutoSize = true;
            this.keySizeLabel.Location = new System.Drawing.Point(139, 89);
            this.keySizeLabel.Name = "keySizeLabel";
            this.keySizeLabel.Size = new System.Drawing.Size(54, 13);
            this.keySizeLabel.TabIndex = 28;
            this.keySizeLabel.Text = "[Key Size]";
            // 
            // keySizeTextLabel
            // 
            this.keySizeTextLabel.AutoSize = true;
            this.keySizeTextLabel.Location = new System.Drawing.Point(3, 89);
            this.keySizeTextLabel.Name = "keySizeTextLabel";
            this.keySizeTextLabel.Size = new System.Drawing.Size(83, 13);
            this.keySizeTextLabel.TabIndex = 27;
            this.keySizeTextLabel.Text = "Public Key Size:";
            // 
            // algorithmLabel
            // 
            this.algorithmLabel.AutoSize = true;
            this.algorithmLabel.Location = new System.Drawing.Point(139, 73);
            this.algorithmLabel.Name = "algorithmLabel";
            this.algorithmLabel.Size = new System.Drawing.Size(56, 13);
            this.algorithmLabel.TabIndex = 26;
            this.algorithmLabel.Text = "[Algorithm]";
            // 
            // algorithmTextLabel
            // 
            this.algorithmTextLabel.AutoSize = true;
            this.algorithmTextLabel.Location = new System.Drawing.Point(3, 73);
            this.algorithmTextLabel.Name = "algorithmTextLabel";
            this.algorithmTextLabel.Size = new System.Drawing.Size(106, 13);
            this.algorithmTextLabel.TabIndex = 25;
            this.algorithmTextLabel.Text = "Public Key Algorithm:";
            // 
            // thumbprintLabel
            // 
            this.thumbprintLabel.AutoSize = true;
            this.thumbprintLabel.Location = new System.Drawing.Point(139, 57);
            this.thumbprintLabel.Name = "thumbprintLabel";
            this.thumbprintLabel.Size = new System.Drawing.Size(66, 13);
            this.thumbprintLabel.TabIndex = 24;
            this.thumbprintLabel.Text = "[Thumbprint]";
            // 
            // thumbprintTextLabel
            // 
            this.thumbprintTextLabel.AutoSize = true;
            this.thumbprintTextLabel.Location = new System.Drawing.Point(3, 57);
            this.thumbprintTextLabel.Name = "thumbprintTextLabel";
            this.thumbprintTextLabel.Size = new System.Drawing.Size(63, 13);
            this.thumbprintTextLabel.TabIndex = 23;
            this.thumbprintTextLabel.Text = "Thumbprint:";
            // 
            // subjectAltNameLabel
            // 
            this.subjectAltNameLabel.AutoSize = true;
            this.subjectAltNameLabel.Location = new System.Drawing.Point(139, 41);
            this.subjectAltNameLabel.Name = "subjectAltNameLabel";
            this.subjectAltNameLabel.Size = new System.Drawing.Size(30, 13);
            this.subjectAltNameLabel.TabIndex = 22;
            this.subjectAltNameLabel.Text = "[san]";
            // 
            // subjectAltNameTextLabel
            // 
            this.subjectAltNameTextLabel.AutoSize = true;
            this.subjectAltNameTextLabel.Location = new System.Drawing.Point(3, 41);
            this.subjectAltNameTextLabel.Name = "subjectAltNameTextLabel";
            this.subjectAltNameTextLabel.Size = new System.Drawing.Size(130, 13);
            this.subjectAltNameTextLabel.TabIndex = 21;
            this.subjectAltNameTextLabel.Text = "Subject Alternative Name:";
            // 
            // commonNameLabel
            // 
            this.commonNameLabel.AutoSize = true;
            this.commonNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commonNameLabel.Location = new System.Drawing.Point(66, 6);
            this.commonNameLabel.Name = "commonNameLabel";
            this.commonNameLabel.Size = new System.Drawing.Size(195, 20);
            this.commonNameLabel.TabIndex = 20;
            this.commonNameLabel.Text = "[certificate common name]";
            // 
            // certIcon
            // 
            this.certIcon.Image = global::VCSJones.FiddlerCert.Properties.Resources.certificate_32xLG;
            this.certIcon.Location = new System.Drawing.Point(6, 6);
            this.certIcon.Name = "certIcon";
            this.certIcon.Size = new System.Drawing.Size(32, 32);
            this.certIcon.TabIndex = 19;
            this.certIcon.TabStop = false;
            // 
            // pkpHashesTab
            // 
            this.pkpHashesTab.Controls.Add(this.shaThumbprintLabel);
            this.pkpHashesTab.Controls.Add(this.shaFingerpintTextLabel);
            this.pkpHashesTab.Location = new System.Drawing.Point(4, 22);
            this.pkpHashesTab.Name = "pkpHashesTab";
            this.pkpHashesTab.Padding = new System.Windows.Forms.Padding(3);
            this.pkpHashesTab.Size = new System.Drawing.Size(519, 137);
            this.pkpHashesTab.TabIndex = 1;
            this.pkpHashesTab.Text = "HPKP";
            this.pkpHashesTab.UseVisualStyleBackColor = true;
            // 
            // shaThumbprintLabel
            // 
            this.shaThumbprintLabel.AutoSize = true;
            this.shaThumbprintLabel.Location = new System.Drawing.Point(139, 3);
            this.shaThumbprintLabel.Name = "shaThumbprintLabel";
            this.shaThumbprintLabel.Size = new System.Drawing.Size(68, 13);
            this.shaThumbprintLabel.TabIndex = 23;
            this.shaThumbprintLabel.Text = "Calculating...";
            // 
            // shaFingerpintTextLabel
            // 
            this.shaFingerpintTextLabel.AutoSize = true;
            this.shaFingerpintTextLabel.Location = new System.Drawing.Point(6, 3);
            this.shaFingerpintTextLabel.Name = "shaFingerpintTextLabel";
            this.shaFingerpintTextLabel.Size = new System.Drawing.Size(129, 13);
            this.shaFingerpintTextLabel.TabIndex = 0;
            this.shaFingerpintTextLabel.Text = "SHA256 SPKI Fingerprint:";
            // 
            // CertificateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.itemsTabControl);
            this.Controls.Add(this.installCertButton);
            this.Controls.Add(this.exportCertButton);
            this.Name = "CertificateControl";
            this.Size = new System.Drawing.Size(533, 198);
            this.itemsTabControl.ResumeLayout(false);
            this.generalTabPage.ResumeLayout(false);
            this.generalTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.certStatusImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.certIcon)).EndInit();
            this.pkpHashesTab.ResumeLayout(false);
            this.pkpHashesTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button exportCertButton;
        private System.Windows.Forms.Button installCertButton;
        private System.Windows.Forms.ToolTip certStatusToolTip;
        private System.Windows.Forms.TabControl itemsTabControl;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.PictureBox certStatusImage;
        private System.Windows.Forms.Label hashAlgorithmLabel;
        private System.Windows.Forms.Label hashAlgorithmTextLabel;
        private System.Windows.Forms.Label validDatesLabel;
        private System.Windows.Forms.Label validFromTextLabel;
        private System.Windows.Forms.Label keySizeLabel;
        private System.Windows.Forms.Label keySizeTextLabel;
        private System.Windows.Forms.Label algorithmLabel;
        private System.Windows.Forms.Label algorithmTextLabel;
        private System.Windows.Forms.Label thumbprintLabel;
        private System.Windows.Forms.Label thumbprintTextLabel;
        private System.Windows.Forms.Label subjectAltNameLabel;
        private System.Windows.Forms.Label subjectAltNameTextLabel;
        private System.Windows.Forms.Label commonNameLabel;
        private System.Windows.Forms.PictureBox certIcon;
        private System.Windows.Forms.TabPage pkpHashesTab;
        private System.Windows.Forms.Label shaThumbprintLabel;
        private System.Windows.Forms.Label shaFingerpintTextLabel;
    }
}
