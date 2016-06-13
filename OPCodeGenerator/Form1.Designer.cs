namespace OPCodeGenerator
{
	partial class Form1
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.saveModbusTablesCheckBox = new System.Windows.Forms.CheckBox();
            this.saveOPCTablesCheckBox = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ModbusGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.OPCodeGrid = new System.Windows.Forms.DataGridView();
            this.OPCode1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OPCode16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.btnGetData = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModbusGrid)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OPCodeGrid)).BeginInit();
            this.TabControl.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.saveModbusTablesCheckBox);
            this.panel2.Controls.Add(this.saveOPCTablesCheckBox);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 655);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1227, 37);
            this.panel2.TabIndex = 1;
            // 
            // saveModbusTablesCheckBox
            // 
            this.saveModbusTablesCheckBox.AutoSize = true;
            this.saveModbusTablesCheckBox.Checked = true;
            this.saveModbusTablesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveModbusTablesCheckBox.Location = new System.Drawing.Point(201, 11);
            this.saveModbusTablesCheckBox.Name = "saveModbusTablesCheckBox";
            this.saveModbusTablesCheckBox.Size = new System.Drawing.Size(127, 17);
            this.saveModbusTablesCheckBox.TabIndex = 3;
            this.saveModbusTablesCheckBox.Text = "Save Modbus Tables";
            this.saveModbusTablesCheckBox.UseVisualStyleBackColor = true;
            this.saveModbusTablesCheckBox.CheckedChanged += new System.EventHandler(this.saveModbusTablesCheckBox_CheckedChanged);
            // 
            // saveOPCTablesCheckBox
            // 
            this.saveOPCTablesCheckBox.AutoSize = true;
            this.saveOPCTablesCheckBox.Checked = true;
            this.saveOPCTablesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveOPCTablesCheckBox.Location = new System.Drawing.Point(88, 11);
            this.saveOPCTablesCheckBox.Name = "saveOPCTablesCheckBox";
            this.saveOPCTablesCheckBox.Size = new System.Drawing.Size(111, 17);
            this.saveOPCTablesCheckBox.TabIndex = 2;
            this.saveOPCTablesCheckBox.Text = "Save OPC Tables";
            this.saveOPCTablesCheckBox.UseVisualStyleBackColor = true;
            this.saveOPCTablesCheckBox.CheckedChanged += new System.EventHandler(this.saveOPCTablesCheckBox_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(7, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1145, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ModbusGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1205, 568);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Modbus Registers";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ModbusGrid
            // 
            this.ModbusGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ModbusGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.ModbusGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModbusGrid.Location = new System.Drawing.Point(3, 3);
            this.ModbusGrid.Name = "ModbusGrid";
            this.ModbusGrid.Size = new System.Drawing.Size(1199, 562);
            this.ModbusGrid.TabIndex = 4;
            this.ModbusGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ModbusGridView_CellValueChanged);
            this.ModbusGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModbusGridView_KeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Start Register";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "End Register";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "TLP";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Indexing";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Conversion";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Comm Port";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.OPCodeGrid);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1205, 568);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "OPCode Tables";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // OPCodeGrid
            // 
            this.OPCodeGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.OPCodeGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OPCodeGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OPCode1,
            this.OPCode2,
            this.OPCode3,
            this.OPCode4,
            this.OPCode5,
            this.OPCode6,
            this.OPCode7,
            this.OPCode8,
            this.OPCode9,
            this.OPCode10,
            this.OPCode11,
            this.OPCode12,
            this.OPCode13,
            this.OPCode14,
            this.OPCode15,
            this.OPCode16});
            this.OPCodeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OPCodeGrid.Location = new System.Drawing.Point(3, 3);
            this.OPCodeGrid.Name = "OPCodeGrid";
            this.OPCodeGrid.Size = new System.Drawing.Size(1199, 562);
            this.OPCodeGrid.TabIndex = 3;
            this.OPCodeGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.OPCodeGrid_CellValueChanged);
            this.OPCodeGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OPCodeGrid_KeyDown);
            // 
            // OPCode1
            // 
            this.OPCode1.HeaderText = "OPCode1";
            this.OPCode1.Name = "OPCode1";
            // 
            // OPCode2
            // 
            this.OPCode2.HeaderText = "OPCode2";
            this.OPCode2.Name = "OPCode2";
            // 
            // OPCode3
            // 
            this.OPCode3.HeaderText = "OPCode3";
            this.OPCode3.Name = "OPCode3";
            // 
            // OPCode4
            // 
            this.OPCode4.HeaderText = "OPCode4";
            this.OPCode4.Name = "OPCode4";
            // 
            // OPCode5
            // 
            this.OPCode5.HeaderText = "OPCode5";
            this.OPCode5.Name = "OPCode5";
            // 
            // OPCode6
            // 
            this.OPCode6.HeaderText = "OPCode6";
            this.OPCode6.Name = "OPCode6";
            // 
            // OPCode7
            // 
            this.OPCode7.HeaderText = "OPCode7";
            this.OPCode7.Name = "OPCode7";
            // 
            // OPCode8
            // 
            this.OPCode8.HeaderText = "OPCode8";
            this.OPCode8.Name = "OPCode8";
            // 
            // OPCode9
            // 
            this.OPCode9.HeaderText = "OPCode9";
            this.OPCode9.Name = "OPCode9";
            // 
            // OPCode10
            // 
            this.OPCode10.HeaderText = "OPCode10";
            this.OPCode10.Name = "OPCode10";
            // 
            // OPCode11
            // 
            this.OPCode11.HeaderText = "OPCode11";
            this.OPCode11.Name = "OPCode11";
            // 
            // OPCode12
            // 
            this.OPCode12.HeaderText = "OPCode12";
            this.OPCode12.Name = "OPCode12";
            // 
            // OPCode13
            // 
            this.OPCode13.HeaderText = "OPCode13";
            this.OPCode13.Name = "OPCode13";
            // 
            // OPCode14
            // 
            this.OPCode14.HeaderText = "OPCode14";
            this.OPCode14.Name = "OPCode14";
            // 
            // OPCode15
            // 
            this.OPCode15.HeaderText = "OPCode15";
            this.OPCode15.Name = "OPCode15";
            // 
            // OPCode16
            // 
            this.OPCode16.HeaderText = "OPCode16";
            this.OPCode16.Name = "OPCode16";
            // 
            // TabControl
            // 
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Controls.Add(this.tabPage2);
            this.TabControl.Location = new System.Drawing.Point(7, 55);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(1213, 594);
            this.TabControl.TabIndex = 3;
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(21, 13);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(75, 23);
            this.btnGetData.TabIndex = 0;
            this.btnGetData.Text = "Load Data";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.label1.Location = new System.Drawing.Point(103, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(354, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Load a ROCLink .800 File";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnGetData);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1227, 49);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.label2.Location = new System.Drawing.Point(463, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(354, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Press Enter to Re-Validate a red cell";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(823, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Search/Replace/Increment";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 692);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ROCLink Opcode Table Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ModbusGrid)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OPCodeGrid)).EndInit();
            this.TabControl.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView ModbusGrid;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView OPCodeGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode1;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode2;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode3;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode4;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode5;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode6;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode7;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode8;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode9;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode10;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode11;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode12;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode13;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode14;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode15;
        private System.Windows.Forms.DataGridViewTextBoxColumn OPCode16;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.CheckBox saveModbusTablesCheckBox;
        private System.Windows.Forms.CheckBox saveOPCTablesCheckBox;
        private System.Windows.Forms.Button button1;
	}
}

