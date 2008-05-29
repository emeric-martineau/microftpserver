/*
 * µLeechFTPServer
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
using System.Security.Cryptography;

namespace MicroFTPServeurGUI
{
    public partial class FormUser : Form
    {
        public String USER_PATH = "";

        private String CurrentUserPassword = "";

        /*
         * Encrypte string
         */
        public static string EncodePassword(string password)
        {
            byte[] original_bytes = System.Text.Encoding.ASCII.GetBytes(password);
            byte[] encoded_bytes = new MD5CryptoServiceProvider().ComputeHash(original_bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < encoded_bytes.Length; i++)
            {
                result.Append(encoded_bytes[i].ToString("x2"));
            }
            return result.ToString();
        }

        public FormUser()
        {
            InitializeComponent();
        }

        private ListViewItem AddUSerInList(String UserName, int start, int end)
        {
            int mid ;
            int comp;
            ListViewItem lvi = new ListViewItem();

            lvi.Text = UserName;

            mid = start + (int)((end - start) / 2) ;

            if (mid < listView1.Items.Count)
            {
                comp = UserName.CompareTo(listView1.Items[mid].Text);

                if (comp < 0)
                {
                    if (mid == start)
                    {
                        listView1.Items.Insert(start, lvi);
                    }
                    else
                    {
                        lvi = AddUSerInList(UserName, start, mid - 1);
                    }
                }
                else if (comp > 0)
                {
                    if (mid == end)
                    {
                        listView1.Items.Insert(mid + 1, lvi);
                    }
                    else
                    {
                        lvi = AddUSerInList(UserName, mid + 1, end);
                    }
                }
                else
                {
                    listView1.Items.Insert(mid, lvi);
                }
            }
            else
            {
                listView1.Items.Add(lvi);
            }

            return lvi;
        }

        private void FormUser_Load(object sender, EventArgs e)
        {
            try
            {
                String[] ListOfUser = Directory.GetFiles(USER_PATH + "users");
                int i;

                for (i = 0; i < ListOfUser.Length; i++)
                {
                    AddUSerInList(Path.GetFileNameWithoutExtension(ListOfUser[i]), 0, listView1.Items.Count - 1);
                }
            }
            catch
            {
                Directory.CreateDirectory(USER_PATH + "users");
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ClassIniReader Ini = new ClassIniReader(USER_PATH + "users\\" + listView1.SelectedItems[0].Text + ".ini");

                textBoxName.Text = listView1.SelectedItems[0].Text;

                textBoxPassword.Text = "****";

                CurrentUserPassword = Ini.GetValue("user", "password");

                textBoxRoot.Text = Ini.GetValue("user", "Root");

                checkBoxDelete.Checked = Ini.GetValue("user", "delete").ToLower() == "yes";
                checkBoxDeleteDirectory.Checked = Ini.GetValue("User", "deletedirectory").ToLower() == "yes";
                checkBoxDisable.Checked = Ini.GetValue("user", "Disabled").ToLower() == "yes";
                checkBoxDownload.Checked = Ini.GetValue("user", "Download").ToLower() == "yes";
                checkBoxEncrypte.Checked = Ini.GetValue("user", "PasswordProtected").ToLower() == "yes";
                checkBoxMakeDirectory.Checked = Ini.GetValue("user", "MakeDirectory").ToLower() == "yes";
                checkBoxRename.Checked = Ini.GetValue("user", "Rename").ToLower() == "yes";
                checkBoxUpload.Checked = Ini.GetValue("user", "Upload").ToLower() == "yes";
                checkBoxModify.Checked = Ini.GetValue("user", "ModifyTime").ToLower() == "yes";

                if (listView1.SelectedItems[0].Text == "anonymous")
                {
                    textBoxPassword.Visible = false;
                    checkBoxEncrypte.Visible = false;
                    label2.Visible = false;
                }
                else
                {
                    textBoxPassword.Visible = true;
                    checkBoxEncrypte.Visible = true;
                    label2.Visible = true;
                }
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0].Text != textBoxName.Text)
                {
                    File.Move(USER_PATH + "users\\" + listView1.SelectedItems[0].Text + ".ini", USER_PATH + "users\\" + textBoxName.Text + ".ini");
                    listView1.SelectedItems[0].Text = textBoxName.Text;
                }

                try
                {
                    StreamWriter sw = new StreamWriter(USER_PATH + "users\\" + listView1.SelectedItems[0].Text + ".ini");

                    sw.WriteLine("[user]");

                    sw.WriteLine("password=" + CurrentUserPassword);
                    sw.WriteLine("; if yes, password is MD5 crypted") ;
                    sw.WriteLine("passwordProtected=" + (checkBoxEncrypte.Checked == true ? "yes" : "no")) ;
                    sw.WriteLine("root=" + textBoxRoot.Text) ;
                    sw.WriteLine("Download=" + (checkBoxDownload.Checked == true ? "yes" : "no")) ;
                    sw.WriteLine("Upload=" + (checkBoxUpload.Checked == true ? "yes" : "no")) ;
                    sw.WriteLine("Rename=" + (checkBoxRename.Checked == true ? "yes" : "no")) ;
                    sw.WriteLine("Delete=" + (checkBoxDelete.Checked == true ? "yes" : "no")) ;
                    sw.WriteLine("MakeDirectory=" + (checkBoxMakeDirectory.Checked == true ? "yes" : "no")) ;
                    sw.WriteLine("DeleteDirectory=" + (checkBoxDeleteDirectory.Checked == true ? "yes" : "no")) ;
                    sw.WriteLine("DeleteDirectory=" + (checkBoxDeleteDirectory.Checked == true ? "yes" : "no"));
                    sw.WriteLine("ModifyTime=" + (checkBoxModify.Checked == true ? "yes" : "no"));
                    sw.Close();
                }
                catch
                {
                    MessageBox.Show("You have no right to write '" + USER_PATH + "users\\'" + listView1.SelectedItems[0].Text + ".ini !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Can't rename user. Check if you have right to rename file in '" + USER_PATH + "users\\' !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxEncrypte_Click(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == false)
            {
                MessageBox.Show("You must re-enter password, cause password cannot decrypte !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                CurrentUserPassword = "";

                textBoxPassword.Text = "";
            }
            else
            {
                CurrentUserPassword = EncodePassword(textBoxPassword.Text);
            }            
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            CurrentUserPassword = (sender as TextBox).Text;
        }

        private void buttonRoot_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBoxRoot.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxRoot.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            FormCopy fc = new FormCopy();

            if (listView1.SelectedItems.Count > 0)
            {
                fc.textBoxName.Text = listView1.SelectedItems[0].Text;

                if (fc.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(USER_PATH + "users\\" + listView1.SelectedItems[0].Text + ".ini", USER_PATH + "users\\" + fc.textBoxName.Text + ".ini");

                        AddUSerInList(fc.textBoxName.Text, 0, listView1.Items.Count - 1);

                    }
                    catch
                    {
                        MessageBox.Show("Can't copy user. Check if you have right to copy file in '" + USER_PATH + "users\\' and if username is valide filename (can't have /,\\,*,?) !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].Text.ToLower() != "anonymous")
                {
                    if (MessageBox.Show("Are you sur you want delete user '" + listView1.SelectedItems[0].Text + "' ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            File.Delete(USER_PATH + "users\\" + listView1.SelectedItems[0].Text + ".ini");

                            listView1.Items.Remove(listView1.SelectedItems[0]);
                        }
                        catch
                        {
                            MessageBox.Show("Can't copy user. Check if you have right to copy file in '" + USER_PATH + "users\\' and if username is valide filename (can't have /,\\,*,?) !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You cannot delete 'anonymous' user. If you want not anonymous login, disabled it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormCopy fc = new FormCopy();
            ListViewItem lvi ;

            if (fc.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(USER_PATH + "users\\" + fc.textBoxName.Text + ".ini");
                    sw.WriteLine("[user]");
                    sw.WriteLine("Password=");
                    sw.WriteLine("; if yes, password is MD5 crypted");
                    sw.WriteLine("PasswordProtected=yes");
                    sw.WriteLine("Root=");
                    sw.WriteLine("Download=no");
                    sw.WriteLine("Upload=no");
                    sw.WriteLine("Rename=no");
                    sw.WriteLine("Delete=no");
                    sw.WriteLine("MakeDirectory=no");
                    sw.WriteLine("DeleteDirectory=no");
                    sw.WriteLine("ModifyTime=no");
                    sw.WriteLine("disabled=yes");
                    sw.Close();

                    lvi = AddUSerInList(fc.textBoxName.Text, 0, listView1.Items.Count - 1);

                    listView1.SelectedItems.Clear();

                    lvi.Selected = true;
                }
                catch
                {
                    MessageBox.Show("Can't create user. Check if you have right to create file in '" + USER_PATH + "users\\' and if username is valide filename (can't have /,\\,*,?) !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
