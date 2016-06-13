using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace OPCodeGenerator
{
    public partial class FindAndReplaceForm : Form
    {
        // make the regex and match class level variables
        // to make happen find next
        private Regex regex;
        private Match match;

        private int currentCellX = 0;
        private int currentCellY = 0;

        public DataGridView dataSource;
        public bool modbusGrid = false;

        private ROCDB roc;


        private Form1 GUIReference;

        public FindAndReplaceForm(DataGridView _dataSource, ROCDB _roc, bool _modbusGrid, Form1 _GUIReference)
        {
            InitializeComponent();
            dataSource = _dataSource;
            roc = _roc;
            GUIReference = _GUIReference;
            modbusGrid = _modbusGrid;

            //For some reason you can't set these past 100 in the designer
            LogicalIncrement.Maximum = 100000;
            PointIncrement.Maximum = 100000;
            registerIncrement.Maximum = 100000;
            LogicalIncrement.Minimum = -100000;
            PointIncrement.Minimum = -100000;
            registerIncrement.Minimum = -100000;

            checkReplaceStatus();
        }


        private void findButton_Click(object sender, EventArgs e)
        {
            FindAndReplaceText(false, false);
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            FindAndReplaceText(true, false);
        }

        private void replaceAllButton_Click(object sender, EventArgs e)
        {
            FindAndReplaceText(true, true);
        }
        private void FindAndReplaceText(bool replace, bool replaceAll)
        {
            //Don't let the user update the increment values during an update, this can cause issues
            PointIncrement.Enabled = false;
            registerIncrement.Enabled = false;
            LogicalIncrement.Enabled = false;

            if (replace || replaceAll)
            {
                if (modbusGrid)
                {
                    GUIReference.UpdateModbusUndoLevel();
                }
                else
                {
                    GUIReference.UpdateOPCUndoLevel();
                }
                
            }

            //if we are searching a selection or doing a replace all start at the start
            if (replaceAll || searchReplaceSelection.Checked)
            {
                //Start at the beginning if replacing all
                currentCellX = 0;
                currentCellY = 0;
            }

            if (!searchReplaceSelection.Checked)
            {
                dataSource.ClearSelection();
            }

            for (int y = currentCellY; y < dataSource.Rows.Count; y++)
            {
                for (int x = currentCellX; x < dataSource.Rows[y].Cells.Count; x++)
                {
                    //if we are searching the selected area only skip this cell if it isn't selected
                    if (searchReplaceSelection.Checked) if (!dataSource.Rows[y].Cells[x].Selected) continue;

                    if (dataSource.Rows[y].Cells[x] == null) continue;
                    if (dataSource.Rows[y].Cells[x].Value == null) continue;

                    if (FindAndReplaceText(dataSource.Rows[y].Cells[x], replace))
                    {
                        //If we are at the end of the row start at the beginning on the next row
                        if (x == dataSource.Rows[y].Cells.Count - 1)
                        {
                            currentCellX = 0;
                            currentCellY = y + 1;
                        }
                        else
                        {
                            //Add one so we iterate the cells on this row
                            currentCellX = x + 1;
                            currentCellY = y;
                        }

                        if (searchReplaceSelection.Checked && !replace) dataSource.ClearSelection();
                        dataSource.Rows[y].Cells[x].Selected = true;

                        //If this cell is off the screen scroll the window so we can see this cell
                        if (!dataSource.Rows[y].Cells[x].Displayed)
                        {
                            dataSource.FirstDisplayedScrollingColumnIndex = currentCellX;
                            dataSource.FirstDisplayedScrollingRowIndex = currentCellY;
                        }

                        //If we have pressed replace all keep going until we get to the end.
                        if (!replaceAll)
                        {
                            //Reenable increment boxes when we are done
                            checkReplaceStatus();
                            return;                            
                        }
                    }
                }
                currentCellX = 0;
            }
            //We didn't find anything, select the previous find
            if (currentCellY != 0 && currentCellX != 0)
            {
                dataSource.Rows[currentCellY].Cells[currentCellX].Selected = true;
            }
            else
            {
                //if the previous find was also nothing warn the user
                if (!replaceAll) MessageBox.Show(String.Format("Cannot find '{0}'.   ", searchTextBox.Text), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            currentCellX = 0;
            currentCellY = 0;

            //Reenable increment boxes when we are done
            checkReplaceStatus();
        }

        /// <summary>
        /// finds the text in searchTextBox in contentTextBox
        /// </summary>
        private bool FindAndReplaceText(DataGridViewCell cell, bool replace)
        {
            regex = GetRegExpression();
            match = regex.Match(cell.Value.ToString());

            if(!match.Success) return false;

            if (replace && !incrementReg.Checked && !incrementP.Checked && !incrementL.Checked)
            {
                cell.Value = replaceTextBox.Text;
            }
            else if (replace && (incrementP.Checked || incrementL.Checked))
            {
                String TLP;
                int T;
                int L;
                int P;
                //Is this a TLP? If so increment the appropriate value and convert it back to a TLP string
                if (roc.abbrevToTLP(cell.Value.ToString(), out TLP, out T, out L, out P))
                {
                    if (incrementL.Checked) L += Convert.ToInt32(LogicalIncrement.Value.ToString());
                    if (incrementP.Checked) P += Convert.ToInt32(PointIncrement.Value.ToString());

                    //Increment the values and update the TLP
                    cell.Value = roc.TLPToAbbrev(T, L, P);
                }
            } else if (replace && incrementReg.Checked && modbusGrid){
                //Check if we are in a register column
                 if (cell.ColumnIndex <= 1 ){
                    int register;
                    if (Int32.TryParse(cell.Value.ToString(), out register)){
                        //If we are an integer increment it
                        register += Convert.ToInt32(registerIncrement.Value.ToString());
                        cell.Value = register.ToString();
                    }
                }
            }

            return match.Success;
        }

        /// <summary>
        /// This function makes and returns a RegEx object
        /// depending on user input
        /// </summary>
        /// <returns></returns>
        private Regex GetRegExpression()
        {
            Regex result;
            String regExString;
            
            // Get what the user entered
            regExString = searchTextBox.Text;

            if (useRegulatExpressionCheckBox.Checked)
            {
                // If regular expressions checkbox is selected,
                // our job is easy. Just do nothing
            }
            // wild cards checkbox checked
            else if (useWildcardsCheckBox.Checked)
            {
                regExString = regExString.Replace("*", @"\w*");     // multiple characters wildcard (*)
                regExString = regExString.Replace("?", @"\w");      // single character wildcard (?)

                // if wild cards selected, find whole words only
                regExString = String.Format("{0}{1}{0}",  @"\b", regExString);
            }
            else
            {
                // replace escape characters
                regExString = Regex.Escape(regExString);
            }

            // Is whole word check box checked?
            if (matchWholeWordCheckBox.Checked)
            {
                regExString = String.Format("{0}{1}{0}",  @"\b", regExString);
            }

            // Is match case checkbox checked?
            if (matchCaseCheckBox.Checked)
            {
                result = new Regex(regExString);
            }
            else
            {
                try
                {
                    result = new Regex(regExString, RegexOptions.IgnoreCase);
                } catch (Exception) {
                    //invalid regex
                    result = new Regex("");
                }
            }

            return result;
        }

                
        private void FindAndReplaceForm_Load(object sender, EventArgs e)
        {
            
        }

        private void FindAndReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void checkReplaceStatus()
        {
            if (incrementL.Checked || incrementP.Checked || incrementReg.Checked)
            {
                searchTextBox.Enabled = false;
                replaceTextBox.Enabled = false;
                useRegulatExpressionCheckBox.Enabled = false;
                useRegulatExpressionCheckBox.Checked = false;
                useWildcardsCheckBox.Enabled = false;
                useWildcardsCheckBox.Checked = true;
                searchTextBox.Text = "*";
                replaceTextBox.Text = "";

            }
            else
            {
                searchTextBox.Enabled = true;
                replaceTextBox.Enabled = true;
                useRegulatExpressionCheckBox.Enabled = true;
                useWildcardsCheckBox.Enabled = true;
            }
            registerIncrement.Enabled = incrementReg.Checked;
            LogicalIncrement.Enabled = incrementL.Checked;
            PointIncrement.Enabled = incrementP.Checked;
        }

        private void incrementReg_CheckedChanged(object sender, EventArgs e)
        {
            checkReplaceStatus();
        }

        private void IncrementL_CheckedChanged(object sender, EventArgs e)
        {
            checkReplaceStatus();
        }

        private void incrementP_CheckedChanged(object sender, EventArgs e)
        {
            checkReplaceStatus();
        }

        private void LogicalIncrement_ValueChanged(object sender, EventArgs e)
        {

        }

        private void PointIncrement_ValueChanged(object sender, EventArgs e)
        {

        }

        private void registerIncrement_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}