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
    public partial class FormGeneralConfig : Form
    {
        public String CONFIG_FILE = "";

        public FormGeneralConfig()
        {
            InitializeComponent();
        }

        private void FormGeneralConfig_Load(object sender, EventArgs e)
        {
            ClassIniReader Ini = new ClassIniReader(CONFIG_FILE);
            int value;
            String tmp;

            tmp = Ini.GetValue("main", "Port");

            if (int.TryParse(tmp, out value) == false)
            {
                value = 21 ;
            }

            numericUpDownPort.Value = value;

            textBoxIPAddress.Text = Ini.GetValue("main", "IpAddress");

            textBoxWelcomeMessage.Text = Ini.GetValue("main", "WelcomeMessage").Replace("\\n", Environment.NewLine);

            textBoxGoodbyeMessage.Text = Ini.GetValue("main", "GoodbyeMessage").Replace("\\n", Environment.NewLine);

            tmp = Ini.GetValue("main", "MaxSessionPerUser");

            if (int.TryParse(tmp, out value) == false)
            {
                value = 2;
            }

            numericUpDownUser.Value = value;

            tmp = Ini.GetValue("main", "MaxClient");

            if (int.TryParse(tmp, out value) == false)
            {
                value = 2;
            }

            numericUpDownClient.Value = value;

            checkBoxFullLog.Checked = Ini.GetValue("main", "FullLog").Equals("yes", StringComparison.OrdinalIgnoreCase) ;

            tmp = Ini.GetValue("main", "TimeOut");

            if (int.TryParse(tmp, out value) == false)
            {
                value = 2;
            }

            numericUpDownTimeOut.Value = value;

            textBoxPassivePort.Text = Ini.GetValue("main", "PassivePort");

            checkBoxDenyPriority.Checked = Ini.GetValue("main", "DenyPriority").Equals("yes", StringComparison.OrdinalIgnoreCase);

            textBoxAllowIP.Text = Ini.GetValue("main", "AllowIPAddress");
            textBoxDenyIP.Text = Ini.GetValue("main", "DenyIPAddress");
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                StreamWriter sw = new StreamWriter(CONFIG_FILE);

                sw.WriteLine("[main]");
                sw.WriteLine("Port=" + numericUpDownPort.Value);
                sw.WriteLine("; IP Address or host name");
                sw.WriteLine("");
                sw.WriteLine("IpAddress=" + textBoxIPAddress.Text);
                sw.WriteLine("");
                sw.WriteLine("WelcomeMessage=\"" + textBoxWelcomeMessage.Text.Replace(Environment.NewLine, "\\n") + "\"");
                sw.WriteLine("GoodbyeMessage=\"" + textBoxGoodbyeMessage.Text.Replace(Environment.NewLine, "\\n") + "\"");
                sw.WriteLine("");
                sw.WriteLine("; 0 for unlimited");
                sw.WriteLine("MaxSessionPerUser=" + numericUpDownUser.Value);
                sw.WriteLine("");
                sw.WriteLine("; 0 for unlimited");
                sw.WriteLine("MaxClient=" + numericUpDownClient.Value);
                sw.WriteLine("");
                sw.WriteLine("; Yes = enabled full log, No = disabled");
                sw.WriteLine("FullLog=" + (checkBoxFullLog.Checked == true ? "yes" : "no"));
                sw.WriteLine("");
                sw.WriteLine("; time-out in seconds. 0 = disabled");
                sw.WriteLine("TimeOut=" + numericUpDownTimeOut.Value);
                sw.WriteLine("");
                sw.WriteLine("; Passive port range xxx-yyy");
                sw.WriteLine("PassivePort=" + textBoxPassivePort.Text);
                sw.WriteLine("");
                sw.WriteLine("; yes = deny address are priority on allowed address.") ;
                sw.WriteLine("; if you want deny allow address except an IP set no") ;
                sw.WriteLine("DenyPriority=" + (checkBoxDenyPriority.Checked == true ? "yes" : "no"));
                sw.WriteLine("") ;
                sw.WriteLine("; Allow IP. ? -> replace one caractere, * -> replace a number.");
                sw.WriteLine("; Separe IP by comma (,)");
                sw.WriteLine("; Empty to disable");
                sw.WriteLine("AllowIPAddress=" + textBoxAllowIP.Text);
                sw.WriteLine("DenyIPAddress=" + textBoxDenyIP.Text);

                sw.Close();
            }
            catch
            {
                MessageBox.Show("Can't save configuration. Check if you have right to write file in '" + CONFIG_FILE + "'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxPassivePort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0') || (e.KeyChar > '9'))
            {
                if (e.KeyChar == '-')
                {
                    if ((sender as TextBox).Text.IndexOf('-') != -1)
                    {
                        e.KeyChar = (char)0x00;
                    }
                }
                else if (e.KeyChar != '\b')
                {
                    e.KeyChar = (char)0x00;
                }
            }
        }
    }
}
