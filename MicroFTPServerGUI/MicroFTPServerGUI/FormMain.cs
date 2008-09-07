/*
 * MicroFTPServer
 * 
 * A little FTP server in .Net technologie
 * 
 * CopyRight MARTINEAU Emeric (C) 2008
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.See the GNU GENERAL PUBLIC LICENSE for more
 * details.
 *
 * You should have received a copy of the GNU GENERAL PUBLIC LICENSE along
 * with this program; if not, write to the Free Software Foundation, Inc., 59
 * Temple Place, Suite 330, Boston, MA 02111-1307 USA.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace MicroFTPServerGUI
{
    public partial class FormMain : Form
    {
        private String CONFIG_FILE;
        private String ConfigPath;
        private String ServerPath ;
        private bool HideSystray ;
        private bool LaunchAtStartup ;
        private Process ServerProcessus = new Process();
        private bool ServerRunning = false;

        /*
         * Add Directory separator at end if not set
         */
        private String AddDirSeparatorAtEnd(String PathDir)
        {
            String Value = PathDir;

            if (PathDir.EndsWith("" + Path.DirectorySeparatorChar) == false)
            {
                Value = Value + Path.DirectorySeparatorChar;
            }

            return Value;
        }

        private void ReadLogFromServer()
        {
            String Line;

            while (ServerProcessus.StandardOutput.EndOfStream == false)
            {
                Line = ServerProcessus.StandardOutput.ReadLine();

                if ((Line == null) || (Line == ""))
                {
                    break;
                }

                try
                {
                    textBoxLog.AppendText(Line + Environment.NewLine);
                }
                catch
                {
                    break;
                }

                Application.DoEvents();
            } 
        }

        /*
         * Call when server shutdown
         */
        void Server_Exited(object sender, EventArgs e)
        {
            ReadLogFromServer();
            ServerRunning = false;
            ActiveMenu();
        }

        /*
         * Start server
         */
        private void StartServer()
        {
            String FileName = AddDirSeparatorAtEnd(ServerPath) + "MicroFTPServer.exe" ;

            try
            {
                if (File.Exists(FileName) == true)
                {
                    ServerProcessus.StartInfo.UseShellExecute = false;
                    ServerProcessus.StartInfo.RedirectStandardOutput = true;
                    ServerProcessus.StartInfo.FileName = FileName;
                    
                    if (ConfigPath != "")
                    {
                        ServerProcessus.StartInfo.Arguments = "-root \"" + ConfigPath + "\"";
                    }

                    ServerProcessus.StartInfo.CreateNoWindow = true;
                    ServerProcessus.Exited += new EventHandler(Server_Exited);
                    ServerProcessus.Start();
                    ServerRunning = true;
                    backgroundWorker1.RunWorkerAsync(ServerProcessus);
                }
                else
                {
                    StatusLabelStatus.Text = "Server not found" ;
                }
            }
            catch
            {
                ServerRunning = false;
                StatusLabelStatus.Text = "Server error";
            }
        }

        /*
         * Stop server
         */
        private void StopServer()
        {
            try
            {
                //InvalidOperationException The process has not exited.
                if (ServerProcessus.ExitCode == 0)
                {
                }                
            }
            catch
            {
                ServerProcessus.Kill();
            }

            ServerRunning = false;
            StatusLabelStatus.Text = "Server stopped";
        }

        /*
         * Convert image to icon
         */
        private Icon ImageToIcon(Image img)
        {
            Bitmap MyIcon = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(MyIcon);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, 0, 0, img.Width, img.Height);
            g.Flush();

            return Icon.FromHandle(MyIcon.GetHicon());
        }
        /*
         * Show in notify icon
         */
        private void ShowInNotifyIcon()
        {
            if (ServerRunning == true)
            {
                notifyIcon1.Icon = ImageToIcon(imageList1.Images[0]);
            }
            else
            {
                notifyIcon1.Icon = ImageToIcon(imageList1.Images[1]);
            }

            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
            this.Hide();
        }

        /*
         * Hide in notify icon
         */
        private void HideInNotifyIcon()
        {
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            this.Show();
        }

        /*
         * Enabled disabled menu
         */
        private void ActiveMenu()
        {
            if (ServerRunning == true)
            {
                MenuStopServer.Enabled = true;
                MenuStartServer.Enabled = false;
                MenuRestartServer.Enabled = true;

                ContextMenuStop.Enabled = true;
                ContextMenuStart.Enabled = false;
                ContextMenuRestart.Enabled = true;
            }
            else
            {
                MenuStopServer.Enabled = false;
                MenuStartServer.Enabled = true;
                MenuRestartServer.Enabled = false;

                ContextMenuStop.Enabled = false;
                ContextMenuStart.Enabled = true;
                ContextMenuRestart.Enabled = false;
            }
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void gUIConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormGUISetting fgs = new FormGUISetting();

            fgs.CONFIG_FILE = CONFIG_FILE;

            if (fgs.ShowDialog() == DialogResult.OK)
            {
                ConfigPath = fgs.ConfigPath;
                ServerPath = fgs.ServerPath;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Process[] ListOfProcess = Process.GetProcessesByName("MicroFTPServer") ;
            Process[] ListOfProcessGUI = Process.GetProcessesByName("MicroFTPServerGUI");
            Process MyProcess = Process.GetCurrentProcess() ;
            FormAlreadyRunning far;
            int i;

            CONFIG_FILE = AddDirSeparatorAtEnd(Path.GetDirectoryName(Application.ExecutablePath)) + "GUIConfig.ini";

            ClassIniReader Ini = new ClassIniReader(CONFIG_FILE);

            ConfigPath = Ini.GetValue("main", "config");
            ServerPath = Ini.GetValue("main", "server");
            HideSystray = Ini.GetValue("main", "HideSystray").ToLower() == "yes";
            LaunchAtStartup = Ini.GetValue("main", "RunAutomatically").ToLower() == "yes";


            if (ListOfProcessGUI.Length > 1)
            {
                if (MessageBox.Show("MicroFTPServerGUI already launch. Exit it ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    /* Kill all server */
                    for (i = 0; i < ListOfProcess.Length; i++)
                    {
                        ListOfProcess[i].Kill();
                        ListOfProcess[i].WaitForExit();
                    }

                    /* Kill all GUI */
                    for (i = 0; i < ListOfProcessGUI.Length; i++)
                    {
                        if (MyProcess != ListOfProcessGUI[i])
                        {
                            ListOfProcessGUI[i].Kill();
                            ListOfProcessGUI[i].WaitForExit();
                        }
                    }

                    if (LaunchAtStartup == true)
                    {
                        StartServer();
                    }
                }
            }
            else if ((ListOfProcess.Length > 0) && (ListOfProcessGUI.Length == 1))
            {
                far = new FormAlreadyRunning();

                if (far.ShowDialog() == DialogResult.Cancel)
                {
                    for (i = 0; i < ListOfProcess.Length; i++)
                    {
                        ListOfProcess[i].Kill();
                        ListOfProcess[i].WaitForExit();
                    }

                    if (LaunchAtStartup == true)
                    {
                        StartServer();
                    }
                }
                else
                {
                    ServerProcessus = ListOfProcess[0];
                    ServerRunning = true;
                }
            }
            else
            {
                if (LaunchAtStartup == true)
                {
                    StartServer();                
                }
            }

            ActiveMenu();

            if (HideSystray == true)
            {
                ShowInNotifyIcon();
            }

            if (Ini.FileExists == false)
            {
                MessageBox.Show("It's you first run. Go to menu Config > GUI Config first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void MenuStartServer_Click(object sender, EventArgs e)
        {
            StartServer();

            if (ServerRunning == true)
            {
                StatusLabelStatus.Text = "Server running";
            }

            ActiveMenu();
        }

        private void MenuStopServer_Click(object sender, EventArgs e)
        {
            StopServer();
            StatusLabelStatus.Text = "Server stopped";
            ActiveMenu();
        }

        private void MenuRestartServer_Click(object sender, EventArgs e)
        {
            StopServer();
            StatusLabelStatus.Text = "Server stopped";
            ActiveMenu();
            StartServer();
            StatusLabelStatus.Text = "Server running";
            ActiveMenu();
        }

        private void MenuGeneralConfig_Click(object sender, EventArgs e)
        {
            FormGeneralConfig fgc = new FormGeneralConfig();

            fgc.CONFIG_FILE = AddDirSeparatorAtEnd(ConfigPath) + "general.ini"; 

            if (fgc.ShowDialog() == DialogResult.OK)
            {
                if (ServerRunning == true)
                {
                    if (MessageBox.Show("Server must be restarted to apply change. Would-you like restart server ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        MenuRestartServer_Click(sender, e);
                    }
                }
            }
        }

        private void usersConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUser fu = new FormUser();

            fu.USER_PATH = AddDirSeparatorAtEnd(ConfigPath);

            fu.ShowDialog();
        }

        private void ContextMenuShow_Click(object sender, EventArgs e)
        {
            HideInNotifyIcon();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            HideInNotifyIcon();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ServerRunning == true)
            {
                if (MessageBox.Show("Serveur is running, would-you like shutdown it ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MenuStopServer_Click(sender, new EventArgs());
                    ServerProcessus.WaitForExit();
                }
            }            
        }

        private void saveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i;
            StreamWriter sw ;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sw = new StreamWriter(saveFileDialog1.FileName);

                    for (i = 0; i < textBoxLog.Lines.Length; i++)
                    {
                        sw.WriteLine(textBoxLog.Lines[i]);
                    }

                    sw.Close();
                }
                catch
                {
                    MessageBox.Show("Can't save log file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Process MyServerProcessus = (Process)e.Argument;

            while (MyServerProcessus.StandardOutput.EndOfStream == false)
            {
                try
                {
                    (sender as BackgroundWorker).ReportProgress(0, MyServerProcessus.StandardOutput.ReadLine());
                }
                catch
                {
                    break;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            String Line = (String)e.UserState;

            textBoxLog.AppendText(Line + Environment.NewLine);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {                
                this.WindowState = FormWindowState.Normal;

                ShowInNotifyIcon();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout fa = new FormAbout();

            fa.ShowDialog();
        }
    }
}
