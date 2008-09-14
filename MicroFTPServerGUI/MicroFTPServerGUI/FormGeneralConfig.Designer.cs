namespace MicroFTPServerGUI
{
    partial class FormGeneralConfig
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
            this.Port = new System.Windows.Forms.Label();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIPAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxWelcomeMessage = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownUser = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownClient = new System.Windows.Forms.NumericUpDown();
            this.checkBoxFullLog = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownTimeOut = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPassivePort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxAllowIP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxDenyIP = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxDenyPriority = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxGoodbyeMessage = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeOut)).BeginInit();
            this.SuspendLayout();
            // 
            // Port
            // 
            this.Port.AutoSize = true;
            this.Port.Location = new System.Drawing.Point(12, 9);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(32, 13);
            this.Port.TabIndex = 0;
            this.Port.Text = "Port :";
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(15, 25);
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            this.numericUpDownPort.Size = new System.Drawing.Size(100, 20);
            this.numericUpDownPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP address or hostname :";
            // 
            // textBoxIPAddress
            // 
            this.textBoxIPAddress.Location = new System.Drawing.Point(15, 64);
            this.textBoxIPAddress.Name = "textBoxIPAddress";
            this.textBoxIPAddress.Size = new System.Drawing.Size(100, 20);
            this.textBoxIPAddress.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Welcome message :";
            // 
            // textBoxWelcomeMessage
            // 
            this.textBoxWelcomeMessage.Location = new System.Drawing.Point(15, 103);
            this.textBoxWelcomeMessage.Multiline = true;
            this.textBoxWelcomeMessage.Name = "textBoxWelcomeMessage";
            this.textBoxWelcomeMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxWelcomeMessage.Size = new System.Drawing.Size(265, 77);
            this.textBoxWelcomeMessage.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(294, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Maximum session per user (0 disabled):";
            // 
            // numericUpDownUser
            // 
            this.numericUpDownUser.Location = new System.Drawing.Point(298, 25);
            this.numericUpDownUser.Maximum = new decimal(new int[] {
            -402653185,
            -1613725636,
            54210108,
            0});
            this.numericUpDownUser.Name = "numericUpDownUser";
            this.numericUpDownUser.Size = new System.Drawing.Size(99, 20);
            this.numericUpDownUser.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(295, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Maximum client (0 disabled):";
            // 
            // numericUpDownClient
            // 
            this.numericUpDownClient.Location = new System.Drawing.Point(298, 64);
            this.numericUpDownClient.Maximum = new decimal(new int[] {
            -402653185,
            -1613725636,
            54210108,
            0});
            this.numericUpDownClient.Name = "numericUpDownClient";
            this.numericUpDownClient.Size = new System.Drawing.Size(99, 20);
            this.numericUpDownClient.TabIndex = 9;
            // 
            // checkBoxFullLog
            // 
            this.checkBoxFullLog.AutoSize = true;
            this.checkBoxFullLog.Location = new System.Drawing.Point(298, 90);
            this.checkBoxFullLog.Name = "checkBoxFullLog";
            this.checkBoxFullLog.Size = new System.Drawing.Size(59, 17);
            this.checkBoxFullLog.TabIndex = 10;
            this.checkBoxFullLog.Text = "Full log";
            this.checkBoxFullLog.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(295, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Time out (0 to disabled) :";
            // 
            // numericUpDownTimeOut
            // 
            this.numericUpDownTimeOut.Location = new System.Drawing.Point(297, 126);
            this.numericUpDownTimeOut.Name = "numericUpDownTimeOut";
            this.numericUpDownTimeOut.Size = new System.Drawing.Size(100, 20);
            this.numericUpDownTimeOut.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(295, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Passive port (start-stop)  :";
            // 
            // textBoxPassivePort
            // 
            this.textBoxPassivePort.Location = new System.Drawing.Point(297, 165);
            this.textBoxPassivePort.Name = "textBoxPassivePort";
            this.textBoxPassivePort.Size = new System.Drawing.Size(100, 20);
            this.textBoxPassivePort.TabIndex = 14;
            this.textBoxPassivePort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPassivePort_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(295, 188);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(236, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Allow IP (empty = none, comma to serparate IP) :";
            // 
            // textBoxAllowIP
            // 
            this.textBoxAllowIP.Location = new System.Drawing.Point(297, 204);
            this.textBoxAllowIP.Name = "textBoxAllowIP";
            this.textBoxAllowIP.Size = new System.Drawing.Size(100, 20);
            this.textBoxAllowIP.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(295, 227);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(236, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Deny IP (empty = none, comma to serparate IP) :";
            // 
            // textBoxDenyIP
            // 
            this.textBoxDenyIP.Location = new System.Drawing.Point(297, 243);
            this.textBoxDenyIP.Name = "textBoxDenyIP";
            this.textBoxDenyIP.Size = new System.Drawing.Size(100, 20);
            this.textBoxDenyIP.TabIndex = 18;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(15, 353);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 19;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(461, 353);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 20;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkBoxDenyPriority
            // 
            this.checkBoxDenyPriority.AutoSize = true;
            this.checkBoxDenyPriority.Location = new System.Drawing.Point(298, 274);
            this.checkBoxDenyPriority.Name = "checkBoxDenyPriority";
            this.checkBoxDenyPriority.Size = new System.Drawing.Size(115, 17);
            this.checkBoxDenyPriority.TabIndex = 21;
            this.checkBoxDenyPriority.Text = "Deny IP are priotity";
            this.checkBoxDenyPriority.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(286, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(2, 330);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(314, 294);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(217, 45);
            this.label9.TabIndex = 23;
            this.label9.Text = "Unckeck it, for exemple, if you want deny all IP address (*.*.*.*) except allow I" +
                "P (192.168.1.*)";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(15, 345);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(521, 2);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            // 
            // textBoxGoodbyeMessage
            // 
            this.textBoxGoodbyeMessage.Location = new System.Drawing.Point(15, 204);
            this.textBoxGoodbyeMessage.Multiline = true;
            this.textBoxGoodbyeMessage.Name = "textBoxGoodbyeMessage";
            this.textBoxGoodbyeMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxGoodbyeMessage.Size = new System.Drawing.Size(265, 77);
            this.textBoxGoodbyeMessage.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 188);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Goodbye message :";
            // 
            // FormGeneralConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 386);
            this.Controls.Add(this.textBoxGoodbyeMessage);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxDenyPriority);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxDenyIP);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxAllowIP);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxPassivePort);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDownTimeOut);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBoxFullLog);
            this.Controls.Add(this.numericUpDownClient);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownUser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxWelcomeMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIPAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownPort);
            this.Controls.Add(this.Port);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGeneralConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "General config";
            this.Load += new System.EventHandler(this.FormGeneralConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownClient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeOut)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Port;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIPAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxWelcomeMessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownClient;
        private System.Windows.Forms.CheckBox checkBoxFullLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownTimeOut;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPassivePort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxAllowIP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxDenyIP;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxDenyPriority;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxGoodbyeMessage;
        private System.Windows.Forms.Label label10;
    }
}