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
            this.commonNameLabel = new System.Windows.Forms.Label();
            this.exportCertButton = new System.Windows.Forms.Button();
            this.subjectAltNameTextLabel = new System.Windows.Forms.Label();
            this.subjectAltNameLabel = new System.Windows.Forms.Label();
            this.thumbprintTextLabel = new System.Windows.Forms.Label();
            this.thumbprintLabel = new System.Windows.Forms.Label();
            this.algorithmLabel = new System.Windows.Forms.Label();
            this.algorithmTextLabel = new System.Windows.Forms.Label();
            this.keySizeLabel = new System.Windows.Forms.Label();
            this.keySizeTextLabel = new System.Windows.Forms.Label();
            this.validDatesLabel = new System.Windows.Forms.Label();
            this.validFromTextLabel = new System.Windows.Forms.Label();
            this.installCertButton = new System.Windows.Forms.Button();
            this.hashAlgorithmLabel = new System.Windows.Forms.Label();
            this.hashAlgorithmTextLabel = new System.Windows.Forms.Label();
            this.certStatusImage = new System.Windows.Forms.PictureBox();
            this.certIcon = new System.Windows.Forms.PictureBox();
            this.certStatusToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.keyHashLabel = new System.Windows.Forms.Label();
            this.keyHashTextLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.certStatusImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.certIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // commonNameLabel
            // 
            this.commonNameLabel.AutoSize = true;
            this.commonNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commonNameLabel.Location = new System.Drawing.Point(70, 10);
            this.commonNameLabel.Name = "commonNameLabel";
            this.commonNameLabel.Size = new System.Drawing.Size(195, 20);
            this.commonNameLabel.TabIndex = 1;
            this.commonNameLabel.Text = "[certificate common name]";
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
            // subjectAltNameTextLabel
            // 
            this.subjectAltNameTextLabel.AutoSize = true;
            this.subjectAltNameTextLabel.Location = new System.Drawing.Point(7, 45);
            this.subjectAltNameTextLabel.Name = "subjectAltNameTextLabel";
            this.subjectAltNameTextLabel.Size = new System.Drawing.Size(130, 13);
            this.subjectAltNameTextLabel.TabIndex = 3;
            this.subjectAltNameTextLabel.Text = "Subject Alternative Name:";
            // 
            // subjectAltNameLabel
            // 
            this.subjectAltNameLabel.AutoSize = true;
            this.subjectAltNameLabel.Location = new System.Drawing.Point(143, 45);
            this.subjectAltNameLabel.Name = "subjectAltNameLabel";
            this.subjectAltNameLabel.Size = new System.Drawing.Size(30, 13);
            this.subjectAltNameLabel.TabIndex = 4;
            this.subjectAltNameLabel.Text = "[san]";
            // 
            // thumbprintTextLabel
            // 
            this.thumbprintTextLabel.AutoSize = true;
            this.thumbprintTextLabel.Location = new System.Drawing.Point(7, 61);
            this.thumbprintTextLabel.Name = "thumbprintTextLabel";
            this.thumbprintTextLabel.Size = new System.Drawing.Size(63, 13);
            this.thumbprintTextLabel.TabIndex = 5;
            this.thumbprintTextLabel.Text = "Thumbprint:";
            // 
            // thumbprintLabel
            // 
            this.thumbprintLabel.AutoSize = true;
            this.thumbprintLabel.Location = new System.Drawing.Point(143, 61);
            this.thumbprintLabel.Name = "thumbprintLabel";
            this.thumbprintLabel.Size = new System.Drawing.Size(66, 13);
            this.thumbprintLabel.TabIndex = 6;
            this.thumbprintLabel.Text = "[Thumbprint]";
            // 
            // algorithmLabel
            // 
            this.algorithmLabel.AutoSize = true;
            this.algorithmLabel.Location = new System.Drawing.Point(143, 77);
            this.algorithmLabel.Name = "algorithmLabel";
            this.algorithmLabel.Size = new System.Drawing.Size(56, 13);
            this.algorithmLabel.TabIndex = 8;
            this.algorithmLabel.Text = "[Algorithm]";
            // 
            // algorithmTextLabel
            // 
            this.algorithmTextLabel.AutoSize = true;
            this.algorithmTextLabel.Location = new System.Drawing.Point(7, 77);
            this.algorithmTextLabel.Name = "algorithmTextLabel";
            this.algorithmTextLabel.Size = new System.Drawing.Size(106, 13);
            this.algorithmTextLabel.TabIndex = 7;
            this.algorithmTextLabel.Text = "Public Key Algorithm:";
            // 
            // keySizeLabel
            // 
            this.keySizeLabel.AutoSize = true;
            this.keySizeLabel.Location = new System.Drawing.Point(143, 93);
            this.keySizeLabel.Name = "keySizeLabel";
            this.keySizeLabel.Size = new System.Drawing.Size(54, 13);
            this.keySizeLabel.TabIndex = 10;
            this.keySizeLabel.Text = "[Key Size]";
            // 
            // keySizeTextLabel
            // 
            this.keySizeTextLabel.AutoSize = true;
            this.keySizeTextLabel.Location = new System.Drawing.Point(7, 93);
            this.keySizeTextLabel.Name = "keySizeTextLabel";
            this.keySizeTextLabel.Size = new System.Drawing.Size(83, 13);
            this.keySizeTextLabel.TabIndex = 9;
            this.keySizeTextLabel.Text = "Public Key Size:";
            // 
            // validDatesLabel
            // 
            this.validDatesLabel.AutoSize = true;
            this.validDatesLabel.Location = new System.Drawing.Point(143, 125);
            this.validDatesLabel.Name = "validDatesLabel";
            this.validDatesLabel.Size = new System.Drawing.Size(67, 13);
            this.validDatesLabel.TabIndex = 12;
            this.validDatesLabel.Text = "[Valid Dates]";
            // 
            // validFromTextLabel
            // 
            this.validFromTextLabel.AutoSize = true;
            this.validFromTextLabel.Location = new System.Drawing.Point(7, 125);
            this.validFromTextLabel.Name = "validFromTextLabel";
            this.validFromTextLabel.Size = new System.Drawing.Size(59, 13);
            this.validFromTextLabel.TabIndex = 11;
            this.validFromTextLabel.Text = "Valid From:";
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
            // hashAlgorithmLabel
            // 
            this.hashAlgorithmLabel.AutoSize = true;
            this.hashAlgorithmLabel.Location = new System.Drawing.Point(143, 109);
            this.hashAlgorithmLabel.Name = "hashAlgorithmLabel";
            this.hashAlgorithmLabel.Size = new System.Drawing.Size(104, 13);
            this.hashAlgorithmLabel.TabIndex = 15;
            this.hashAlgorithmLabel.Text = "[Signature Algorithm]";
            // 
            // hashAlgorithmTextLabel
            // 
            this.hashAlgorithmTextLabel.AutoSize = true;
            this.hashAlgorithmTextLabel.Location = new System.Drawing.Point(7, 109);
            this.hashAlgorithmTextLabel.Name = "hashAlgorithmTextLabel";
            this.hashAlgorithmTextLabel.Size = new System.Drawing.Size(101, 13);
            this.hashAlgorithmTextLabel.TabIndex = 14;
            this.hashAlgorithmTextLabel.Text = "Signature Algorithm:";
            // 
            // certStatusImage
            // 
            this.certStatusImage.BackColor = System.Drawing.SystemColors.Control;
            this.certStatusImage.Image = global::VCSJones.FiddlerCert.Properties.Resources.security_Shields_Blank_16xLG;
            this.certStatusImage.Location = new System.Drawing.Point(48, 14);
            this.certStatusImage.Name = "certStatusImage";
            this.certStatusImage.Size = new System.Drawing.Size(16, 16);
            this.certStatusImage.TabIndex = 16;
            this.certStatusImage.TabStop = false;
            // 
            // certIcon
            // 
            this.certIcon.Image = global::VCSJones.FiddlerCert.Properties.Resources.certificate_32xLG;
            this.certIcon.Location = new System.Drawing.Point(10, 10);
            this.certIcon.Name = "certIcon";
            this.certIcon.Size = new System.Drawing.Size(32, 32);
            this.certIcon.TabIndex = 0;
            this.certIcon.TabStop = false;
            // 
            // certStatusToolTip
            // 
            this.certStatusToolTip.AutoPopDelay = 5000;
            this.certStatusToolTip.InitialDelay = 250;
            this.certStatusToolTip.ReshowDelay = 100;
            // 
            // keyHashLabel
            // 
            this.keyHashLabel.AutoSize = true;
            this.keyHashLabel.Location = new System.Drawing.Point(143, 141);
            this.keyHashLabel.Name = "keyHashLabel";
            this.keyHashLabel.Size = new System.Drawing.Size(59, 13);
            this.keyHashLabel.TabIndex = 18;
            this.keyHashLabel.Text = "[Key Hash]";
            // 
            // keyHashTextLabel
            // 
            this.keyHashTextLabel.AutoSize = true;
            this.keyHashTextLabel.Location = new System.Drawing.Point(7, 141);
            this.keyHashTextLabel.Name = "keyHashTextLabel";
            this.keyHashTextLabel.Size = new System.Drawing.Size(105, 13);
            this.keyHashTextLabel.TabIndex = 17;
            this.keyHashTextLabel.Text = "SHA-256 PKP Hash:";
            // 
            // CertificateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.keyHashLabel);
            this.Controls.Add(this.keyHashTextLabel);
            this.Controls.Add(this.certStatusImage);
            this.Controls.Add(this.hashAlgorithmLabel);
            this.Controls.Add(this.hashAlgorithmTextLabel);
            this.Controls.Add(this.installCertButton);
            this.Controls.Add(this.validDatesLabel);
            this.Controls.Add(this.validFromTextLabel);
            this.Controls.Add(this.keySizeLabel);
            this.Controls.Add(this.keySizeTextLabel);
            this.Controls.Add(this.algorithmLabel);
            this.Controls.Add(this.algorithmTextLabel);
            this.Controls.Add(this.thumbprintLabel);
            this.Controls.Add(this.thumbprintTextLabel);
            this.Controls.Add(this.subjectAltNameLabel);
            this.Controls.Add(this.subjectAltNameTextLabel);
            this.Controls.Add(this.exportCertButton);
            this.Controls.Add(this.commonNameLabel);
            this.Controls.Add(this.certIcon);
            this.Name = "CertificateControl";
            this.Size = new System.Drawing.Size(533, 198);
            ((System.ComponentModel.ISupportInitialize)(this.certStatusImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.certIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox certIcon;
        private System.Windows.Forms.Label commonNameLabel;
        private System.Windows.Forms.Button exportCertButton;
        private System.Windows.Forms.Label subjectAltNameTextLabel;
        private System.Windows.Forms.Label subjectAltNameLabel;
        private System.Windows.Forms.Label thumbprintTextLabel;
        private System.Windows.Forms.Label thumbprintLabel;
        private System.Windows.Forms.Label algorithmLabel;
        private System.Windows.Forms.Label algorithmTextLabel;
        private System.Windows.Forms.Label keySizeLabel;
        private System.Windows.Forms.Label keySizeTextLabel;
        private System.Windows.Forms.Label validDatesLabel;
        private System.Windows.Forms.Label validFromTextLabel;
        private System.Windows.Forms.Button installCertButton;
        private System.Windows.Forms.Label hashAlgorithmLabel;
        private System.Windows.Forms.Label hashAlgorithmTextLabel;
        private System.Windows.Forms.PictureBox certStatusImage;
        private System.Windows.Forms.ToolTip certStatusToolTip;
        private System.Windows.Forms.Label keyHashLabel;
        private System.Windows.Forms.Label keyHashTextLabel;
    }
}
