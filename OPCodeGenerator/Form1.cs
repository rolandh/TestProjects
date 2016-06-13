using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.IO;

namespace OPCodeGenerator
{
    public partial class Form1 : Form
    {

        ROCDB ROC;
        //UndoOPC history for OPCodeGrid
        Stack<String[,]> OPCodeGridOld = new Stack<String[,]>();
        Stack<String[,]> ModbusGridGridOld = new Stack<String[,]>();

        FindAndReplaceForm SearchAndReplace;

        public bool OPCodeGridActive = true;

        bool blank = true;

        public Form1()
        {
            InitializeComponent();
            oData = new clsData();

            OPCodeGrid.AllowUserToAddRows = false;
            OPCodeGrid.AllowUserToDeleteRows = false;

            ModbusGrid.AllowUserToAddRows = false;
            ModbusGrid.AllowUserToDeleteRows = false;

            Initialize();

        }

        private void InitializeOpcodeTable()
        {
            OPCodeGrid.Columns.Clear();

            for (int i = 1; i <= ROC.numberOfOPCTables; i++) OPCodeGrid.Columns.Add(i.ToString(), "OPCode" + i.ToString());

            OPCodeGrid.Rows.Clear();

            for (int i = 1; i < 45; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = i.ToString();
                OPCodeGrid.Rows.Add(row);
            }
            OPCodeGrid.RowHeadersWidth = 55;
            ModbusGrid.RowHeadersWidth = 55;
        }
        private void InitializeModbusTable()
        {
            ModbusGrid.Columns.Clear();

            for (int table = 1; table <= ROC.numberOfModbusRegTables; table++)
            {
                ModbusGrid.Rows.Add("Register Table " + table.ToString());
                for (int i = 1; i < 16; i++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.HeaderCell.Value = i.ToString();
                    ModbusGrid.Rows.Add(row);
                }
            }
        }
        private void Initialize()
        {
            ROC = new ROCDB();
        }

        public void UpdateOPCUndoLevel()
        {
            String[,] undoLevel = new String[16, 44];

            for (int i = 0; i < OPCodeGrid.Columns.Count; i++)
            {
                for (int j = 0; j < OPCodeGrid.Rows.Count; j++)
                {
                    if (OPCodeGrid[i, j].Value == null) OPCodeGrid[i, j].Value = "";

                    if (!string.IsNullOrEmpty(OPCodeGrid[i, j].Value.ToString())){

                        undoLevel[i, j] = OPCodeGrid[i, j].Value.ToString();
                    } else {
                        undoLevel[i, j] = "";
                    }
                }
            }

            OPCodeGridOld.Push(undoLevel);

        }

        public void UpdateModbusUndoLevel()
        {
            String[,] undoLevel = new String[6, 16*24];

            for (int i = 0; i < ModbusGrid.Columns.Count; i++)
            {
                for (int j = 0; j < ModbusGrid.Rows.Count; j++)
                {
                    if (ModbusGrid[i, j].Value == null)
                    {
                        ModbusGrid[i, j].Value = "";
                        continue;
                    }

                    if (!string.IsNullOrEmpty(ModbusGrid[i, j].Value.ToString())){


                        undoLevel[i, j] = ModbusGrid[i, j].Value.ToString();
                    } else {
                        undoLevel[i, j] = "";
                    }
                }
            }

            ModbusGridGridOld.Push(undoLevel);
        }

        private void UndoModbus()
        {
            if (ModbusGridGridOld.Count == 0) return;

            String[,] undoLevel = ModbusGridGridOld.Pop();

            for (int i = 0; i < ModbusGrid.Columns.Count; i++)
            {
                for (int j = 0; j < ModbusGrid.Rows.Count; j++)
                {
                    if (!ModbusGrid[i, j].Value.Equals(undoLevel[i, j]))
                    ModbusGrid[i, j].Value = undoLevel[i, j];
                }
            }

        }

        private void UndoOPC()
        {
            if (OPCodeGridOld.Count == 0) return;

            String[,] undoLevel = OPCodeGridOld.Pop();

            for (int i = 0; i < OPCodeGrid.Columns.Count; i++)
            {
                for (int j = 0; j < OPCodeGrid.Rows.Count; j++)
                {
                    OPCodeGrid[i, j].Value = undoLevel[i, j];
                }
            }
        }

        private clsData oData { get; set; }
        private BindingSource oBS = new BindingSource();

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //Open a new file
        private void btnGetData_Click(object sender, EventArgs e)
        {

            OpenFileDialog _dialogResult = new OpenFileDialog();
            DialogResult _result = _dialogResult.ShowDialog();
            

            if (_result == DialogResult.OK)
            {
                ROC.filename = _dialogResult.FileName;
            }
            else
            {
                return;
            }

            if (!File.Exists(ROC.filename)) MessageBox.Show("File does not exist: " + ROC.filename, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            OpenFile();
          
        }

        private void OpenFile()
        {
            DataGridViewCell oCell;
            try
            {
                if (!blank) UpdateOPCUndoLevel();

                blank = false;

                OPCodeGrid.Rows.Clear();

                ModbusGrid.Rows.Clear();

                string[,] OPCTable = ROC.readOPCTables();

                InitializeOpcodeTable();
                //pass the OPC table from the ROC file into the GUI
                for (int OPCTableNo = 0; OPCTableNo < ROC.numberOfOPCTables; OPCTableNo++)
                {
                    //For each entry
                    for (int OPCEntry = 0; OPCEntry < 44; OPCEntry++)
                    {
                        oCell = OPCodeGrid[OPCTableNo, OPCEntry];
                        string value = OPCTable[OPCTableNo, OPCEntry];
                        oCell.Value = Convert.ChangeType(value, oCell.ValueType);
                    }

                }
            }catch(Exception e){
                MessageBox.Show(e.Message, "Error reading OPC Tables, Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try {
                //if (ROC.ROCType != 800) return;
                //InitializeModbusTable();

                ModbusGrid.Rows.Clear();

                List<List<ROCDB.ModbusRecord>> modbusTables = ROC.readModbusRegisterTables();

                int tableCount = 1;
                int rowCount = 0;
                foreach (List<ROCDB.ModbusRecord> modbusTable in modbusTables)
                {
                    int recordCount = 1;
                    ModbusGrid.Rows.Add("Register Table " + (tableCount++).ToString());
                    rowCount++;
                    foreach(ROCDB.ModbusRecord mr in modbusTable)
                    {
                        ModbusGrid.Rows.Add(mr.startRegister, mr.endRegister, mr.TLP, mr.indexing, mr.conversionCode, mr.commPort);
                        ModbusGrid.Rows[rowCount++].HeaderCell.Value = (recordCount++).ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error reading Modbus Tables, Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
     

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyClipboardOPCodeTable();
        }

        private void CopyClipboardOPCodeTable()
        {
            DataObject d = OPCodeGrid.GetClipboardContent();
            if (d == null) return;
            Clipboard.SetDataObject(d);
        }

        private void pasteCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteClipboardOPCodeTable();
        }

        private void DeleteOPCSelection()
        {
            UpdateOPCUndoLevel();
            foreach (DataGridViewCell item in OPCodeGrid.SelectedCells)
            {
                item.Value = "Undefined";
                item.Style.BackColor = Color.White;
            }
        }

        private void DeleteModbusSelection()
        {
            UpdateModbusUndoLevel();
            foreach (DataGridViewCell item in ModbusGrid.SelectedCells)
            {
                item.Value = ROC.getDefaultModbusCellValue(item);
                item.Style.BackColor = Color.White;
            }
        }

        /// <summary>
        /// This will be moved to the util class so it can service any paste into a DGV
        /// </summary>
        private void PasteClipboardOPCodeTable()
        {
            UpdateOPCUndoLevel();
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');

                int iRow;
                int iCol;

                //Are we pasting over a selection or to the entire grid?
                bool selectionPaste = (OPCodeGrid.SelectedCells.Count > 1);

                //Get the top left cell if we have highlighted cells otherwise use the currently selected cell
                if (selectionPaste)
                {
                    iRow = OPCodeGrid.SelectedCells[OPCodeGrid.SelectedCells.Count - 1].RowIndex;
                    iCol = OPCodeGrid.SelectedCells[OPCodeGrid.SelectedCells.Count - 1].ColumnIndex;
                }
                else
                {
                    iRow = OPCodeGrid.CurrentCell.RowIndex;
                    iCol = OPCodeGrid.CurrentCell.ColumnIndex;
                }

                foreach (string line in lines)
                {
                    if (iRow < OPCodeGrid.RowCount && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');
                        for (int i = 0; i < sCells.GetLength(0); ++i)
                        {
                            if (iCol + i < this.OPCodeGrid.ColumnCount)
                            {
                                //If we have currently selected cells only paste over the highlighted ones, otherwise paste the whole clipboard
                                if ((selectionPaste && OPCodeGrid[iCol + i, iRow].Selected) || !selectionPaste)
                                {
                                    OPCodeGrid[iCol + i, iRow].Value = sCells[i];
                                    ValidateOPCodeCell(OPCodeGrid[iCol + i, iRow]);
                                }

                            }
                            else
                            { break; }
                        }
                        iRow++;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void PasteClipboardModbusTable()
        {
            UpdateOPCUndoLevel();
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');

                int iRow;
                int iCol;

                //Are we pasting over a selection or to the entire grid?
                bool selectionPaste = (ModbusGrid.SelectedCells.Count > 1);

                //Get the top left cell if we have highlighted cells otherwise use the currently selected cell
                if (selectionPaste)
                {
                    iRow = ModbusGrid.SelectedCells[ModbusGrid.SelectedCells.Count - 1].RowIndex;
                    iCol = ModbusGrid.SelectedCells[ModbusGrid.SelectedCells.Count - 1].ColumnIndex;
                }
                else
                {
                    iRow = ModbusGrid.CurrentCell.RowIndex;
                    iCol = ModbusGrid.CurrentCell.ColumnIndex;
                }

                foreach (string line in lines)
                {
                    if (iRow < ModbusGrid.RowCount && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');
                        for (int i = 0; i < sCells.GetLength(0); ++i)
                        {
                            if (iCol + i < this.ModbusGrid.ColumnCount)
                            {
                                //If we have currently selected cells only paste over the highlighted ones, otherwise paste the whole clipboard
                                if ((selectionPaste && ModbusGrid[iCol + i, iRow].Selected) || !selectionPaste)
                                {
                                    ModbusGrid[iCol + i, iRow].Value = sCells[i];

                                    ValidateModbusCell(ModbusGrid[iCol + i, iRow]);
                                }

                            }
                            else
                            { break; }
                        }
                        iRow++;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void ValidateOPCSelection()
        {
            foreach (DataGridViewCell cell in OPCodeGrid.SelectedCells) ValidateOPCodeCell(cell);
        }


        private void ValidateOPCodeCell(DataGridViewCell cell)
        {
            if (cell.Value == null) cell.Value = "Undefined";

            if (String.IsNullOrEmpty(cell.Value.ToString())) cell.Value = "Undefined";

            string validatedTLP;

            if (ROC.validateTLP(cell.Value.ToString(), out validatedTLP))
            {
                cell.Style.BackColor = Color.White;
                cell.Value = validatedTLP;
            }
            else
            {
                cell.Style.BackColor = Color.Red;
            }
        }

        private void ValidateModbusSelection()
        {
            foreach (DataGridViewCell cell in ModbusGrid.SelectedCells) ValidateModbusCell(cell);
        }

        private void ValidateModbusCell(DataGridViewCell cell)
        {
            int test = cell.RowIndex % 16;
            if (cell.RowIndex % 16 == 0)
            {
                if (cell.ColumnIndex == 0)
                {
                    cell.Value = "Register Table " + ((cell.RowIndex / 16) + 1).ToString();
                }
                else
                {
                    cell.Value = "";
                }
                
                return;
            }

            String validatedEntry;
            if (ROC.validateModbusEntry(cell, out validatedEntry))
            {
                cell.Value = validatedEntry;
                cell.Style.BackColor = Color.White;
            }
            else
            {
                cell.Style.BackColor = Color.Red;
            }
        }





        private void btnSave_Click(object sender, EventArgs e)
        {
            OpenFileDialog _dialogResult = new OpenFileDialog();
            DialogResult _result = _dialogResult.ShowDialog();


            if (_result == DialogResult.OK)
            {
                ROC.filename = _dialogResult.FileName;
            }
            else
            {
                return;
            }

            if (!File.Exists(ROC.filename)) MessageBox.Show("File does not exist: " + ROC.filename, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            bool success = true;
            bool success2 = true;
            if (saveOPCTablesCheckBox.Checked)
            {
                success = ROC.writeOPCTables(ROC.filename, OPCodeGrid);
            }

            if (saveModbusTablesCheckBox.Checked)
            {
                success2 = ROC.writeModbusTables(ROC.filename, ModbusGrid);
            }

            if (success && success2) MessageBox.Show("Successfull saved ROC .800 File!", "#Winning", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //Update all entries if we only saved some of them due to user error
            OpenFile();
        }

        private void OPCodeGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ValidateOPCSelection();
        }

        private void ModbusGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ValidateModbusSelection();
        }

        private void ModbusGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                UndoModbus();
            }
            if (e.KeyCode == Keys.Delete)
            {
                DeleteModbusSelection();
            }

            if ((e.Control && e.KeyCode == Keys.Insert) || (e.Shift && e.KeyCode == Keys.Insert))
            {
                PasteClipboardModbusTable();
            }
            if ((e.Control && e.KeyCode == Keys.V) || (e.Shift && e.KeyCode == Keys.Insert))
            {
                PasteClipboardModbusTable();
            } 
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (SearchAndReplace != null) if (SearchAndReplace.Visible) return;

                SearchAndReplace = new FindAndReplaceForm(ModbusGrid, ROC, true, this);
                SearchAndReplace.Show();
            }
        }

        private void OPCodeGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                UndoOPC();
            }
            if (e.KeyCode == Keys.Delete)
            {
                DeleteOPCSelection();
            }

            if ((e.Control && e.KeyCode == Keys.Insert) || (e.Shift && e.KeyCode == Keys.Insert))
            {
                PasteClipboardOPCodeTable();
            }
            if ((e.Control && e.KeyCode == Keys.V) || (e.Shift && e.KeyCode == Keys.Insert))
            {
                PasteClipboardOPCodeTable();
            }
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (SearchAndReplace != null) if (SearchAndReplace.Visible) return;

                SearchAndReplace = new FindAndReplaceForm(OPCodeGrid, ROC, false, this);
                SearchAndReplace.Show();
            }

        }
        private void SetSaveButtonState()
        {
            if (!saveOPCTablesCheckBox.Checked && !saveModbusTablesCheckBox.Checked) btnSave.Enabled = false;
            else btnSave.Enabled = true;
        }

        private void saveOPCTablesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetSaveButtonState();
        }

        private void saveModbusTablesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetSaveButtonState();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Keep track of which grid we are working on so the search knows which to operate on
            if(SearchAndReplace == null) return;

            if(SearchAndReplace.Visible){
                if (TabControl.SelectedIndex == 0)
                {
                    SearchAndReplace.dataSource = OPCodeGrid;
                    SearchAndReplace.modbusGrid = false;
                }
                else
                {
                    SearchAndReplace.dataSource = ModbusGrid;
                    SearchAndReplace.modbusGrid = true;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (SearchAndReplace != null) if (SearchAndReplace.Visible) return;
            bool modbusGrid = false;
            if (TabControl.SelectedIndex == 0) SearchAndReplace = new FindAndReplaceForm(OPCodeGrid, ROC, false, this);
            else SearchAndReplace = new FindAndReplaceForm(ModbusGrid, ROC, true, this);
            SearchAndReplace.Show();
        }



    }
}