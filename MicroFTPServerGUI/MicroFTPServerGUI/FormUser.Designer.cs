namespace MicroFTPServerGUI
{
    partial class FormUser
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxEncrypte = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRoot = new System.Windows.Forms.TextBox();
            this.buttonRoot = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxModify = new System.Windows.Forms.CheckBox();
            this.checkBoxDeleteDirectory = new System.Windows.Forms.CheckBox();
            this.checkBoxMakeDirectory = new System.Windows.Forms.CheckBox();
            this.checkBoxDelete = new System.Windows.Forms.CheckBox();
            this.checkBoxRename = new System.Windows.Forms.CheckBox();
            this.checkBoxUpload = new System.Windows.Forms.CheckBox();
            this.checkBoxDownload = new System.Windows.Forms.CheckBox();
            this.checkBoxDisable = new System.Windows.Forms.CheckBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.checkBoxSubDir = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(144, 325);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "User name";
            this.columnHeader1.Width = 140;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(181, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name :";
            // 
            // textBoxName
            // 
            this.textBoxName.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.textBoxName.Location = new System.Drawing.Point(266, 9);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(100, 20);
            this.textBoxName.TabIndex = 2;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(266, 38);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(100, 20);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxPassword_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(181, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password :";
            // 
            // checkBoxEncrypte
            // 
            this.checkBoxEncrypte.AutoSize = true;
            this.checkBoxEncrypte.Location = new System.Drawing.Point(266, 66);
            this.checkBoxEncrypte.Name = "checkBoxEncrypte";
            this.checkBoxEncrypte.Size = new System.Drawing.Size(116, 17);
            this.checkBoxEncrypte.TabIndex = 5;
            this.checkBoxEncrypte.Text = "Encrypte password";
            this.checkBoxEncrypte.UseVisualStyleBackColor = true;
            this.checkBoxEncrypte.Click += new System.EventHandler(this.checkBoxEncrypte_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(181, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Root directory :";
            // 
            // textBoxRoot
            // 
            this.textBoxRoot.Location = new System.Drawing.Point(266, 89);
            this.textBoxRoot.Name = "textBoxRoot";
            this.textBoxRoot.Size = new System.Drawing.Size(100, 20);
            this.textBoxRoot.TabIndex = 7;
            // 
            // buttonRoot
            // 
            this.buttonRoot.Location = new System.Drawing.Point(372, 87);
            this.buttonRoot.Name = "buttonRoot";
            this.buttonRoot.Size = new System.Drawing.Size(25, 23);
            this.buttonRoot.TabIndex = 8;
            this.buttonRoot.Text = "...";
            this.buttonRoot.UseVisualStyleBackColor = true;
            this.buttonRoot.Click += new System.EventHandler(this.buttonRoot_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxSubDir);
            this.groupBox1.Controls.Add(this.checkBoxModify);
            this.groupBox1.Controls.Add(this.checkBoxDeleteDirectory);
            this.groupBox1.Controls.Add(this.checkBoxMakeDirectory);
            this.groupBox1.Controls.Add(this.checkBoxDelete);
            this.groupBox1.Controls.Add(this.checkBoxRename);
            this.groupBox1.Controls.Add(this.checkBoxUpload);
            this.groupBox1.Controls.Add(this.checkBoxDownload);
            this.groupBox1.Location = new System.Drawing.Point(184, 116);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(213, 198);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rights";
            // 
            // checkBoxModify
            // 
            this.checkBoxModify.AutoSize = true;
            this.checkBoxModify.Location = new System.Drawing.Point(10, 157);
            this.checkBoxModify.Name = "checkBoxModify";
            this.checkBoxModify.Size = new System.Drawing.Size(107, 17);
            this.checkBoxModify.TabIndex = 6;
            this.checkBoxModify.Text = "Modify time of file";
            this.checkBoxModify.UseVisualStyleBackColor = true;
            // 
            // checkBoxDeleteDirectory
            // 
            this.checkBoxDeleteDirectory.AutoSize = true;
            this.checkBoxDeleteDirectory.Location = new System.Drawing.Point(10, 134);
            this.checkBoxDeleteDirectory.Name = "checkBoxDeleteDirectory";
            this.checkBoxDeleteDirectory.Size = new System.Drawing.Size(100, 17);
            this.checkBoxDeleteDirectory.TabIndex = 5;
            this.checkBoxDeleteDirectory.Text = "Delete directory";
            this.checkBoxDeleteDirectory.UseVisualStyleBackColor = true;
            // 
            // checkBoxMakeDirectory
            // 
            this.checkBoxMakeDirectory.AutoSize = true;
            this.checkBoxMakeDirectory.Location = new System.Drawing.Point(10, 111);
            this.checkBoxMakeDirectory.Name = "checkBoxMakeDirectory";
            this.checkBoxMakeDirectory.Size = new System.Drawing.Size(96, 17);
            this.checkBoxMakeDirectory.TabIndex = 4;
            this.checkBoxMakeDirectory.Text = "Make directory";
            this.checkBoxMakeDirectory.UseVisualStyleBackColor = true;
            // 
            // checkBoxDelete
            // 
            this.checkBoxDelete.AutoSize = true;
            this.checkBoxDelete.Location = new System.Drawing.Point(10, 88);
            this.checkBoxDelete.Name = "checkBoxDelete";
            this.checkBoxDelete.Size = new System.Drawing.Size(57, 17);
            this.checkBoxDelete.TabIndex = 3;
            this.checkBoxDelete.Text = "Delete";
            this.checkBoxDelete.UseVisualStyleBackColor = true;
            // 
            // checkBoxRename
            // 
            this.checkBoxRename.AutoSize = true;
            this.checkBoxRename.Location = new System.Drawing.Point(11, 65);
            this.checkBoxRename.Name = "checkBoxRename";
            this.checkBoxRename.Size = new System.Drawing.Size(66, 17);
            this.checkBoxRename.TabIndex = 2;
            this.checkBoxRename.Text = "Rename";
            this.checkBoxRename.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpload
            // 
            this.checkBoxUpload.AutoSize = true;
            this.checkBoxUpload.Location = new System.Drawing.Point(10, 42);
            this.checkBoxUpload.Name = "checkBoxUpload";
            this.checkBoxUpload.Size = new System.Drawing.Size(60, 17);
            this.checkBoxUpload.TabIndex = 1;
            this.checkBoxUpload.Text = "Upload";
            this.checkBoxUpload.UseVisualStyleBackColor = true;
            // 
            // checkBoxDownload
            // 
            this.checkBoxDownload.AutoSize = true;
            this.checkBoxDownload.Location = new System.Drawing.Point(10, 19);
            this.checkBoxDownload.Name = "checkBoxDownload";
            this.checkBoxDownload.Size = new System.Drawing.Size(74, 17);
            this.checkBoxDownload.TabIndex = 0;
            this.checkBoxDownload.Text = "Download";
            this.checkBoxDownload.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisable
            // 
            this.checkBoxDisable.AutoSize = true;
            this.checkBoxDisable.Location = new System.Drawing.Point(184, 320);
            this.checkBoxDisable.Name = "checkBoxDisable";
            this.checkBoxDisable.Size = new System.Drawing.Size(84, 17);
            this.checkBoxDisable.TabIndex = 10;
            this.checkBoxDisable.Text = "Disable user";
            this.checkBoxDisable.UseVisualStyleBackColor = true;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(12, 343);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(70, 23);
            this.buttonAdd.TabIndex = 11;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(86, 343);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(70, 23);
            this.buttonDelete.TabIndex = 12;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(12, 389);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(322, 343);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 14;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Location = new System.Drawing.Point(183, 343);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(75, 23);
            this.buttonCopy.TabIndex = 15;
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // checkBoxSubDir
            // 
            this.checkBoxSubDir.AutoSize = true;
            this.checkBoxSubDir.Location = new System.Drawing.Point(10, 175);
            this.checkBoxSubDir.Name = "checkBoxSubDir";
            this.checkBoxSubDir.Size = new System.Drawing.Size(156, 17);
            this.checkBoxSubDir.TabIndex = 7;
            this.checkBoxSubDir.Text = "Show/Access sub directory";
            this.checkBoxSubDir.UseVisualStyleBackColor = true;
            // 
            // FormUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 420);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.checkBoxDisable);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonRoot);
            this.Controls.Add(this.textBoxRoot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxEncrypte);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Users config";
            this.Load += new System.EventHandler(this.FormUser_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxEncrypte;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxRoot;
        private System.Windows.Forms.Button buttonRoot;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxDeleteDirectory;
        private System.Windows.Forms.CheckBox checkBoxMakeDirectory;
        private System.Windows.Forms.CheckBox checkBoxDelete;
        private System.Windows.Forms.CheckBox checkBoxRename;
        private System.Windows.Forms.CheckBox checkBoxUpload;
        private System.Windows.Forms.CheckBox checkBoxDownload;
        private System.Windows.Forms.CheckBox checkBoxDisable;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.CheckBox checkBoxModify;
        private System.Windows.Forms.CheckBox checkBoxSubDir;

    }
}