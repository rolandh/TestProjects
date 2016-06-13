namespace OPCodeGenerator
{
    partial class FindAndReplaceForm
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
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.findButton = new System.Windows.Forms.Button();
            this.matchWholeWordCheckBox = new System.Windows.Forms.CheckBox();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.replaceButton = new System.Windows.Forms.Button();
            this.replaceTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.replaceAllButton = new System.Windows.Forms.Button();
            this.useWildcardsCheckBox = new System.Windows.Forms.CheckBox();
            this.useRegulatExpressionCheckBox = new System.Windows.Forms.CheckBox();
            this.searchReplaceSelection = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.incrementL = new System.Windows.Forms.CheckBox();
            this.incrementP = new System.Windows.Forms.CheckBox();
            this.incrementReg = new System.Windows.Forms.CheckBox();
            this.LogicalIncrement = new System.Windows.Forms.NumericUpDown();
            this.PointIncrement = new System.Windows.Forms.NumericUpDown();
            this.registerIncrement = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.LogicalIncrement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PointIncrement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.registerIncrement)).BeginInit();
            this.SuspendLayout();
            // 
            // searchTextBox
            // 
            this.searchTextBox.Location = new System.Drawing.Point(106, 8);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(213, 20);
            this.searchTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search";
            // 
            // findButton
            // 
            this.findButton.Location = new System.Drawing.Point(321, 154);
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(88, 29);
            this.findButton.TabIndex = 8;
            this.findButton.Text = "Find";
            this.findButton.UseVisualStyleBackColor = true;
            this.findButton.Click += new System.EventHandler(this.findButton_Click);
            // 
            // matchWholeWordCheckBox
            // 
            this.matchWholeWordCheckBox.AutoSize = true;
            this.matchWholeWordCheckBox.Location = new System.Drawing.Point(106, 70);
            this.matchWholeWordCheckBox.Name = "matchWholeWordCheckBox";
            this.matchWholeWordCheckBox.Size = new System.Drawing.Size(113, 17);
            this.matchWholeWordCheckBox.TabIndex = 5;
            this.matchWholeWordCheckBox.Text = "Match whole word";
            this.matchWholeWordCheckBox.UseVisualStyleBackColor = true;
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.AutoSize = true;
            this.matchCaseCheckBox.Location = new System.Drawing.Point(106, 93);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new System.Drawing.Size(82, 17);
            this.matchCaseCheckBox.TabIndex = 6;
            this.matchCaseCheckBox.Text = "Match case";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // replaceButton
            // 
            this.replaceButton.Location = new System.Drawing.Point(415, 154);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(88, 29);
            this.replaceButton.TabIndex = 9;
            this.replaceButton.Text = "Replace";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // replaceTextBox
            // 
            this.replaceTextBox.Location = new System.Drawing.Point(106, 41);
            this.replaceTextBox.Name = "replaceTextBox";
            this.replaceTextBox.Size = new System.Drawing.Size(213, 20);
            this.replaceTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Replace with:";
            // 
            // replaceAllButton
            // 
            this.replaceAllButton.Location = new System.Drawing.Point(509, 154);
            this.replaceAllButton.Name = "replaceAllButton";
            this.replaceAllButton.Size = new System.Drawing.Size(88, 29);
            this.replaceAllButton.TabIndex = 10;
            this.replaceAllButton.Text = "Replace All";
            this.replaceAllButton.UseVisualStyleBackColor = true;
            this.replaceAllButton.Click += new System.EventHandler(this.replaceAllButton_Click);
            // 
            // useWildcardsCheckBox
            // 
            this.useWildcardsCheckBox.AutoSize = true;
            this.useWildcardsCheckBox.Location = new System.Drawing.Point(106, 115);
            this.useWildcardsCheckBox.Name = "useWildcardsCheckBox";
            this.useWildcardsCheckBox.Size = new System.Drawing.Size(95, 17);
            this.useWildcardsCheckBox.TabIndex = 7;
            this.useWildcardsCheckBox.Text = "Use wildcards ";
            this.useWildcardsCheckBox.UseVisualStyleBackColor = true;
            // 
            // useRegulatExpressionCheckBox
            // 
            this.useRegulatExpressionCheckBox.AutoSize = true;
            this.useRegulatExpressionCheckBox.Location = new System.Drawing.Point(106, 138);
            this.useRegulatExpressionCheckBox.Name = "useRegulatExpressionCheckBox";
            this.useRegulatExpressionCheckBox.Size = new System.Drawing.Size(138, 17);
            this.useRegulatExpressionCheckBox.TabIndex = 11;
            this.useRegulatExpressionCheckBox.Text = "Use regular expressions";
            this.useRegulatExpressionCheckBox.UseVisualStyleBackColor = true;
            // 
            // searchReplaceSelection
            // 
            this.searchReplaceSelection.AutoSize = true;
            this.searchReplaceSelection.Location = new System.Drawing.Point(362, 27);
            this.searchReplaceSelection.Name = "searchReplaceSelection";
            this.searchReplaceSelection.Size = new System.Drawing.Size(185, 17);
            this.searchReplaceSelection.TabIndex = 12;
            this.searchReplaceSelection.Text = "Replace/Increment selection only";
            this.searchReplaceSelection.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(359, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Replace Options:";
            // 
            // incrementL
            // 
            this.incrementL.AutoSize = true;
            this.incrementL.Location = new System.Drawing.Point(362, 99);
            this.incrementL.Name = "incrementL";
            this.incrementL.Size = new System.Drawing.Size(116, 17);
            this.incrementL.TabIndex = 14;
            this.incrementL.Text = "Increment L in TLP";
            this.incrementL.UseVisualStyleBackColor = true;
            this.incrementL.CheckedChanged += new System.EventHandler(this.IncrementL_CheckedChanged);
            // 
            // incrementP
            // 
            this.incrementP.AutoSize = true;
            this.incrementP.Location = new System.Drawing.Point(481, 99);
            this.incrementP.Name = "incrementP";
            this.incrementP.Size = new System.Drawing.Size(117, 17);
            this.incrementP.TabIndex = 15;
            this.incrementP.Text = "Increment P in TLP";
            this.incrementP.UseVisualStyleBackColor = true;
            this.incrementP.CheckedChanged += new System.EventHandler(this.incrementP_CheckedChanged);
            // 
            // incrementReg
            // 
            this.incrementReg.AutoSize = true;
            this.incrementReg.Location = new System.Drawing.Point(362, 50);
            this.incrementReg.Name = "incrementReg";
            this.incrementReg.Size = new System.Drawing.Size(156, 17);
            this.incrementReg.TabIndex = 18;
            this.incrementReg.Text = "Increment Modbus Register";
            this.incrementReg.UseVisualStyleBackColor = true;
            this.incrementReg.CheckedChanged += new System.EventHandler(this.incrementReg_CheckedChanged);
            // 
            // LogicalIncrement
            // 
            this.LogicalIncrement.Location = new System.Drawing.Point(362, 123);
            this.LogicalIncrement.Name = "LogicalIncrement";
            this.LogicalIncrement.Size = new System.Drawing.Size(113, 20);
            this.LogicalIncrement.TabIndex = 21;
            this.LogicalIncrement.ValueChanged += new System.EventHandler(this.LogicalIncrement_ValueChanged);
            // 
            // PointIncrement
            // 
            this.PointIncrement.Location = new System.Drawing.Point(481, 123);
            this.PointIncrement.Name = "PointIncrement";
            this.PointIncrement.Size = new System.Drawing.Size(116, 20);
            this.PointIncrement.TabIndex = 22;
            this.PointIncrement.ValueChanged += new System.EventHandler(this.PointIncrement_ValueChanged);
            // 
            // registerIncrement
            // 
            this.registerIncrement.Location = new System.Drawing.Point(362, 73);
            this.registerIncrement.Name = "registerIncrement";
            this.registerIncrement.Size = new System.Drawing.Size(113, 20);
            this.registerIncrement.TabIndex = 23;
            this.registerIncrement.ValueChanged += new System.EventHandler(this.registerIncrement_ValueChanged);
            // 
            // FindAndReplaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 202);
            this.Controls.Add(this.registerIncrement);
            this.Controls.Add(this.PointIncrement);
            this.Controls.Add(this.LogicalIncrement);
            this.Controls.Add(this.incrementReg);
            this.Controls.Add(this.incrementP);
            this.Controls.Add(this.incrementL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.searchReplaceSelection);
            this.Controls.Add(this.useRegulatExpressionCheckBox);
            this.Controls.Add(this.useWildcardsCheckBox);
            this.Controls.Add(this.replaceAllButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.replaceTextBox);
            this.Controls.Add(this.replaceButton);
            this.Controls.Add(this.matchCaseCheckBox);
            this.Controls.Add(this.matchWholeWordCheckBox);
            this.Controls.Add(this.findButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.searchTextBox);
            this.Name = "FindAndReplaceForm";
            this.Text = "Search and Replace";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindAndReplaceForm_FormClosing);
            this.Load += new System.EventHandler(this.FindAndReplaceForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LogicalIncrement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PointIncrement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.registerIncrement)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button findButton;
        private System.Windows.Forms.CheckBox matchWholeWordCheckBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.TextBox replaceTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button replaceAllButton;
        private System.Windows.Forms.CheckBox useWildcardsCheckBox;
        private System.Windows.Forms.CheckBox useRegulatExpressionCheckBox;
        private System.Windows.Forms.CheckBox searchReplaceSelection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox incrementL;
        private System.Windows.Forms.CheckBox incrementP;
        private System.Windows.Forms.CheckBox incrementReg;
        private System.Windows.Forms.NumericUpDown LogicalIncrement;
        private System.Windows.Forms.NumericUpDown PointIncrement;
        private System.Windows.Forms.NumericUpDown registerIncrement;
    }
}

