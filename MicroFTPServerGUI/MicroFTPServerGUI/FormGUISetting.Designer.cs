namespace MicroFTPServerGUI
{
    partial class FormGUISetting
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxServerPath = new System.Windows.Forms.TextBox();
            this.buttonServerPath = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxConfig = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxLaunchAtStartup = new System.Windows.Forms.CheckBox();
            this.checkBoxHideSystray = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButtonButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FTP server executable path :";
            // 
            // textBoxServerPath
            // 
            this.textBoxServerPath.Location = new System.Drawing.Point(15, 25);
            this.textBoxServerPath.Name = "textBoxServerPath";
            this.textBoxServerPath.Size = new System.Drawing.Size(388, 20);
            this.textBoxServerPath.TabIndex = 1;
            // 
            // buttonServerPath
            // 
            this.buttonServerPath.Location = new System.Drawing.Point(409, 23);
            this.buttonServerPath.Name = "buttonServerPath";
            this.buttonServerPath.Size = new System.Drawing.Size(25, 23);
            this.buttonServerPath.TabIndex = 2;
            this.buttonServerPath.Text = "...";
            this.buttonServerPath.UseVisualStyleBackColor = true;
            this.buttonServerPath.Click += new System.EventHandler(this.buttonServerPath_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(409, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxConfig
            // 
            this.textBoxConfig.Location = new System.Drawing.Point(15, 64);
            this.textBoxConfig.Name = "textBoxConfig";
            this.textBoxConfig.Size = new System.Drawing.Size(388, 20);
            this.textBoxConfig.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "FTP config path :";
            // 
            // checkBoxLaunchAtStartup
            // 
            this.checkBoxLaunchAtStartup.AutoSize = true;
            this.checkBoxLaunchAtStartup.Location = new System.Drawing.Point(15, 90);
            this.checkBoxLaunchAtStartup.Name = "checkBoxLaunchAtStartup";
            this.checkBoxLaunchAtStartup.Size = new System.Drawing.Size(158, 17);
            this.checkBoxLaunchAtStartup.TabIndex = 6;
            this.checkBoxLaunchAtStartup.Text = "Launch server automatically";
            this.checkBoxLaunchAtStartup.UseVisualStyleBackColor = true;
            // 
            // checkBoxHideSystray
            // 
            this.checkBoxHideSystray.AutoSize = true;
            this.checkBoxHideSystray.Location = new System.Drawing.Point(15, 113);
            this.checkBoxHideSystray.Name = "checkBoxHideSystray";
            this.checkBoxHideSystray.Size = new System.Drawing.Size(141, 17);
            this.checkBoxHideSystray.TabIndex = 7;
            this.checkBoxHideSystray.Text = "Hide in systray at startup";
            this.checkBoxHideSystray.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(15, 139);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 8;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButtonButton
            // 
            this.CancelButtonButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButtonButton.Location = new System.Drawing.Point(359, 139);
            this.CancelButtonButton.Name = "CancelButtonButton";
            this.CancelButtonButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButtonButton.TabIndex = 9;
            this.CancelButtonButton.Text = "Cancel";
            this.CancelButtonButton.UseVisualStyleBackColor = true;
            // 
            // FormGUISetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 174);
            this.Controls.Add(this.CancelButtonButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.checkBoxHideSystray);
            this.Controls.Add(this.checkBoxLaunchAtStartup);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxConfig);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonServerPath);
            this.Controls.Add(this.textBoxServerPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGUISetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormGUISetting";
            this.Load += new System.EventHandler(this.FormGUISetting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxServerPath;
        private System.Windows.Forms.Button buttonServerPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxConfig;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxLaunchAtStartup;
        private System.Windows.Forms.CheckBox checkBoxHideSystray;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelButtonButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
    }
}