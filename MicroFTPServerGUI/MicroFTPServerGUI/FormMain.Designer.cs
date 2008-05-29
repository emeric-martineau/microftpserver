namespace MicroFTPServeurGUI
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuServer = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStartServer = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuRestartServer = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStopServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gUIConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuGeneralConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.usersConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.ContextMenuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextMenuStart = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.ContextMenuShow = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.ContextMenuNotify.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 244);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(292, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabelStatus
            // 
            this.StatusLabelStatus.Name = "StatusLabelStatus";
            this.StatusLabelStatus.Size = new System.Drawing.Size(81, 17);
            this.StatusLabelStatus.Text = "Server stopped";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuServer,
            this.configToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(292, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MenuServer
            // 
            this.MenuServer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStartServer,
            this.MenuRestartServer,
            this.MenuStopServer,
            this.toolStripMenuItem2,
            this.saveLogToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.MenuServer.Name = "MenuServer";
            this.MenuServer.Size = new System.Drawing.Size(51, 20);
            this.MenuServer.Text = "Server";
            // 
            // MenuStartServer
            // 
            this.MenuStartServer.Name = "MenuStartServer";
            this.MenuStartServer.Size = new System.Drawing.Size(126, 22);
            this.MenuStartServer.Text = "Start";
            this.MenuStartServer.Click += new System.EventHandler(this.MenuStartServer_Click);
            // 
            // MenuRestartServer
            // 
            this.MenuRestartServer.Name = "MenuRestartServer";
            this.MenuRestartServer.Size = new System.Drawing.Size(126, 22);
            this.MenuRestartServer.Text = "Restart";
            this.MenuRestartServer.Click += new System.EventHandler(this.MenuRestartServer_Click);
            // 
            // MenuStopServer
            // 
            this.MenuStopServer.Name = "MenuStopServer";
            this.MenuStopServer.Size = new System.Drawing.Size(126, 22);
            this.MenuStopServer.Text = "Stop";
            this.MenuStopServer.Click += new System.EventHandler(this.MenuStopServer_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(123, 6);
            // 
            // saveLogToolStripMenuItem
            // 
            this.saveLogToolStripMenuItem.Name = "saveLogToolStripMenuItem";
            this.saveLogToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.saveLogToolStripMenuItem.Text = "Save log";
            this.saveLogToolStripMenuItem.Click += new System.EventHandler(this.saveLogToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(123, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gUIConfigToolStripMenuItem,
            this.MenuGeneralConfig,
            this.usersConfigToolStripMenuItem});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.configToolStripMenuItem.Text = "Config";
            // 
            // gUIConfigToolStripMenuItem
            // 
            this.gUIConfigToolStripMenuItem.Name = "gUIConfigToolStripMenuItem";
            this.gUIConfigToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.gUIConfigToolStripMenuItem.Text = "GUI Config";
            this.gUIConfigToolStripMenuItem.Click += new System.EventHandler(this.gUIConfigToolStripMenuItem_Click);
            // 
            // MenuGeneralConfig
            // 
            this.MenuGeneralConfig.Name = "MenuGeneralConfig";
            this.MenuGeneralConfig.Size = new System.Drawing.Size(156, 22);
            this.MenuGeneralConfig.Text = "General Config";
            this.MenuGeneralConfig.Click += new System.EventHandler(this.MenuGeneralConfig_Click);
            // 
            // usersConfigToolStripMenuItem
            // 
            this.usersConfigToolStripMenuItem.Name = "usersConfigToolStripMenuItem";
            this.usersConfigToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.usersConfigToolStripMenuItem.Text = "Users Config";
            this.usersConfigToolStripMenuItem.Click += new System.EventHandler(this.usersConfigToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "main.ico");
            this.imageList1.Images.SetKeyName(1, "mainstop.ico");
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Location = new System.Drawing.Point(0, 24);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(292, 220);
            this.textBoxLog.TabIndex = 2;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.ContextMenuNotify;
            this.notifyIcon1.Text = "MicroFTPServer";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // ContextMenuNotify
            // 
            this.ContextMenuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextMenuStart,
            this.ContextMenuRestart,
            this.ContextMenuStop,
            this.toolStripMenuItem1,
            this.ContextMenuShow});
            this.ContextMenuNotify.Name = "ContextMenuStart";
            this.ContextMenuNotify.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ContextMenuNotify.Size = new System.Drawing.Size(122, 98);
            // 
            // ContextMenuStart
            // 
            this.ContextMenuStart.Name = "ContextMenuStart";
            this.ContextMenuStart.Size = new System.Drawing.Size(121, 22);
            this.ContextMenuStart.Text = "Start";
            // 
            // ContextMenuRestart
            // 
            this.ContextMenuRestart.Name = "ContextMenuRestart";
            this.ContextMenuRestart.Size = new System.Drawing.Size(121, 22);
            this.ContextMenuRestart.Text = "Restart";
            // 
            // ContextMenuStop
            // 
            this.ContextMenuStop.Name = "ContextMenuStop";
            this.ContextMenuStop.Size = new System.Drawing.Size(121, 22);
            this.ContextMenuStop.Text = "Stop";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(118, 6);
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ContextMenuShow_Click);
            // 
            // ContextMenuShow
            // 
            this.ContextMenuShow.Name = "ContextMenuShow";
            this.ContextMenuShow.Size = new System.Drawing.Size(121, 22);
            this.ContextMenuShow.Text = "Show";
            this.ContextMenuShow.Click += new System.EventHandler(this.ContextMenuShow_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "log.txt";
            this.saveFileDialog1.Filter = "Log|*.txt|All|*.*";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "MicroFTPServer GUI";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ContextMenuNotify.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuServer;
        private System.Windows.Forms.ToolStripMenuItem MenuStartServer;
        private System.Windows.Forms.ToolStripMenuItem MenuRestartServer;
        private System.Windows.Forms.ToolStripMenuItem MenuStopServer;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gUIConfigToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelStatus;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.ToolStripMenuItem MenuGeneralConfig;
        private System.Windows.Forms.ToolStripMenuItem usersConfigToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip ContextMenuNotify;
        private System.Windows.Forms.ToolStripMenuItem ContextMenuStart;
        private System.Windows.Forms.ToolStripMenuItem ContextMenuRestart;
        private System.Windows.Forms.ToolStripMenuItem ContextMenuStop;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ContextMenuShow;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem saveLogToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

