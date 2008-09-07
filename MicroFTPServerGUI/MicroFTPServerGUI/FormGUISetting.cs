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

namespace MicroFTPServerGUI
{
    public partial class FormGUISetting : Form
    {
        public String CONFIG_FILE = "";
        public String ConfigPath = "";
        public String ServerPath = "";

        public FormGUISetting()
        {
            InitializeComponent();
        }

        private void buttonServerPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.SelectedPath = textBoxServerPath.Text;

            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                textBoxServerPath.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBoxConfig.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxConfig.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void FormGUISetting_Load(object sender, EventArgs e)
        {
            ClassIniReader Ini = new ClassIniReader(CONFIG_FILE);

            textBoxConfig.Text = Ini.GetValue("main", "config");
            textBoxServerPath.Text = Ini.GetValue("main", "server");
            checkBoxHideSystray.Checked = Ini.GetValue("main", "HideSystray").ToLower() == "yes" ;
            checkBoxLaunchAtStartup.Checked = Ini.GetValue("main", "RunAutomatically").ToLower() == "yes";
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter(CONFIG_FILE);

            sw.WriteLine("[main]");
            sw.WriteLine("config=\"" + textBoxConfig.Text + "\"");
            sw.WriteLine("server=\"" + textBoxServerPath.Text + "\"");
            sw.WriteLine("HideSystray=\"" + (checkBoxHideSystray.Checked == true ? "yes" : "no") + "\"");
            sw.WriteLine("RunAutomatically=\"" + (checkBoxLaunchAtStartup.Checked == true ? "yes" : "no") + "\"");

            sw.Close() ;

            ServerPath = textBoxServerPath.Text;
            ConfigPath = textBoxConfig.Text;
        }
    }
}
