namespace CleanStorageFiles
{
    partial class Main
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
            this.Go = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.RichTextBox();
            this.directory = new System.Windows.Forms.TextBox();
            this.ErrorLogBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DeletedFileCountBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.Recurse = new System.Windows.Forms.CheckBox();
            this.DontDelete = new System.Windows.Forms.CheckBox();
            this.AllFolders = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Go
            // 
            this.Go.Location = new System.Drawing.Point(12, 12);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(75, 23);
            this.Go.TabIndex = 0;
            this.Go.Text = "Go!";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.button1_Click);
            // 
            // LogBox
            // 
            this.LogBox.Location = new System.Drawing.Point(13, 245);
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(643, 281);
            this.LogBox.TabIndex = 1;
            this.LogBox.Text = "";
            // 
            // directory
            // 
            this.directory.Location = new System.Drawing.Point(12, 66);
            this.directory.Name = "directory";
            this.directory.Size = new System.Drawing.Size(563, 20);
            this.directory.TabIndex = 2;
            // 
            // ErrorLogBox
            // 
            this.ErrorLogBox.Location = new System.Drawing.Point(13, 141);
            this.ErrorLogBox.Name = "ErrorLogBox";
            this.ErrorLogBox.Size = new System.Drawing.Size(643, 75);
            this.ErrorLogBox.TabIndex = 3;
            this.ErrorLogBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Error Log:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Log:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Files Deleted:";
            // 
            // DeletedFileCountBox
            // 
            this.DeletedFileCountBox.Location = new System.Drawing.Point(86, 92);
            this.DeletedFileCountBox.Name = "DeletedFileCountBox";
            this.DeletedFileCountBox.Size = new System.Drawing.Size(61, 20);
            this.DeletedFileCountBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Directory To Clean:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(123, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(581, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Find";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Recurse
            // 
            this.Recurse.AutoSize = true;
            this.Recurse.Checked = true;
            this.Recurse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Recurse.Location = new System.Drawing.Point(116, 49);
            this.Recurse.Name = "Recurse";
            this.Recurse.Size = new System.Drawing.Size(180, 17);
            this.Recurse.TabIndex = 11;
            this.Recurse.Text = "Clean all Sub directories as well?";
            this.Recurse.UseVisualStyleBackColor = true;
            // 
            // DontDelete
            // 
            this.DontDelete.AutoSize = true;
            this.DontDelete.Location = new System.Drawing.Point(302, 49);
            this.DontDelete.Name = "DontDelete";
            this.DontDelete.Size = new System.Drawing.Size(87, 17);
            this.DontDelete.TabIndex = 12;
            this.DontDelete.Text = "Just list files?";
            this.DontDelete.UseVisualStyleBackColor = true;
            // 
            // AllFolders
            // 
            this.AllFolders.AutoSize = true;
            this.AllFolders.Checked = true;
            this.AllFolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AllFolders.Location = new System.Drawing.Point(116, 16);
            this.AllFolders.Name = "AllFolders";
            this.AllFolders.Size = new System.Drawing.Size(178, 17);
            this.AllFolders.TabIndex = 14;
            this.AllFolders.Text = "Check my libraries (C,D,E,F,G,K)";
            this.AllFolders.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 538);
            this.Controls.Add(this.AllFolders);
            this.Controls.Add(this.DontDelete);
            this.Controls.Add(this.Recurse);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DeletedFileCountBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ErrorLogBox);
            this.Controls.Add(this.directory);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.Go);
            this.Name = "Main";
            this.Text = "Clean";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.RichTextBox LogBox;
        private System.Windows.Forms.TextBox directory;
        private System.Windows.Forms.RichTextBox ErrorLogBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox DeletedFileCountBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox Recurse;
        private System.Windows.Forms.CheckBox DontDelete;
        private System.Windows.Forms.CheckBox AllFolders;
    }
}

