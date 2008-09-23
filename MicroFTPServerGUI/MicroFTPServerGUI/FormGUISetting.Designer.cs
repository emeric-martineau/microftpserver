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
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxLogFile = new System.Windows.Forms.CheckBox();
            this.textBoxLogFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLogSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxEraseLogFile = new System.Windows.Forms.CheckBox();
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
            this.checkBoxLaunchAtStartup.Location = new System.Drawing.Point(15, 406);
            this.checkBoxLaunchAtStartup.Name = "checkBoxLaunchAtStartup";
            this.checkBoxLaunchAtStartup.Size = new System.Drawing.Size(158, 17);
            this.checkBoxLaunchAtStartup.TabIndex = 6;
            this.checkBoxLaunchAtStartup.Text = "Launch server automatically";
            this.checkBoxLaunchAtStartup.UseVisualStyleBackColor = true;
            // 
            // checkBoxHideSystray
            // 
            this.checkBoxHideSystray.AutoSize = true;
            this.checkBoxHideSystray.Location = new System.Drawing.Point(15, 429);
            this.checkBoxHideSystray.Name = "checkBoxHideSystray";
            this.checkBoxHideSystray.Size = new System.Drawing.Size(141, 17);
            this.checkBoxHideSystray.TabIndex = 7;
            this.checkBoxHideSystray.Text = "Hide in systray at startup";
            this.checkBoxHideSystray.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(15, 455);
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
            this.CancelButtonButton.Location = new System.Drawing.Point(359, 455);
            this.CancelButtonButton.Name = "CancelButtonButton";
            this.CancelButtonButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButtonButton.TabIndex = 9;
            this.CancelButtonButton.Text = "Cancel";
            this.CancelButtonButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Log file :";
            // 
            // checkBoxLogFile
            // 
            this.checkBoxLogFile.AutoSize = true;
            this.checkBoxLogFile.Location = new System.Drawing.Point(15, 90);
            this.checkBoxLogFile.Name = "checkBoxLogFile";
            this.checkBoxLogFile.Size = new System.Drawing.Size(78, 17);
            this.checkBoxLogFile.TabIndex = 11;
            this.checkBoxLogFile.Text = "Use log file";
            this.checkBoxLogFile.UseVisualStyleBackColor = true;
            // 
            // textBoxLogFile
            // 
            this.textBoxLogFile.Location = new System.Drawing.Point(16, 132);
            this.textBoxLogFile.Name = "textBoxLogFile";
            this.textBoxLogFile.Size = new System.Drawing.Size(387, 20);
            this.textBoxLogFile.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(390, 81);
            this.label4.TabIndex = 13;
            this.label4.Text = "%Y : year\r\n%M : month\r\n%d : day\r\n%H : hour\r\n%m : minute\r\n%s : seconde\r\n";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 247);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Log size :";
            // 
            // textBoxLogSize
            // 
            this.textBoxLogSize.Location = new System.Drawing.Point(15, 263);
            this.textBoxLogSize.Name = "textBoxLogSize";
            this.textBoxLogSize.Size = new System.Drawing.Size(388, 20);
            this.textBoxLogSize.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(13, 286);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(390, 84);
            this.label6.TabIndex = 16;
            this.label6.Text = "0 : no size\r\n1024 : 1024 byte\r\n1024k : 1024 kilo-byte\r\n1024M : 1024 mega-byte\r\n10" +
                "24G : 1024 giga-byte\r\n1024T : 1024 tetra-byte";
            // 
            // checkBoxEraseLogFile
            // 
            this.checkBoxEraseLogFile.AutoSize = true;
            this.checkBoxEraseLogFile.Location = new System.Drawing.Point(15, 373);
            this.checkBoxEraseLogFile.Name = "checkBoxEraseLogFile";
            this.checkBoxEraseLogFile.Size = new System.Drawing.Size(226, 17);
            this.checkBoxEraseLogFile.TabIndex = 17;
            this.checkBoxEraseLogFile.Text = "Erase log when log file have maximum size";
            this.checkBoxEraseLogFile.UseVisualStyleBackColor = true;
            // 
            // FormGUISetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 496);
            this.Controls.Add(this.checkBoxEraseLogFile);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxLogSize);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxLogFile);
            this.Controls.Add(this.checkBoxLogFile);
            this.Controls.Add(this.label3);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxLogFile;
        private System.Windows.Forms.TextBox textBoxLogFile;
        private System.Windows.Forms.TextBox textBoxLogSize;
        private System.Windows.Forms.CheckBox checkBoxEraseLogFile;
    }
}