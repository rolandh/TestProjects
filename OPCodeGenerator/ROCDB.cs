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
    public class ROCDB
    {
        string[,] tParametersDictionary = new string[256, 256];
        string[] tPointTypesDictionary = new string[256];
        string[] commPortTags = new string[256];

        public int numberOfOPCTables = 0;
        public int numberOfModbusRegTables = 24;
        public int OPCPointType = 0;
        public int pointsPerModule = 8;
        public int ROCType = 123;


        DataTable currentOPCRecordsFromFile;

        public String filename = "";

        String userDatabase = @"C:\Program Files (x86)\ROCLINK800\ROCLINK.mdw";
        String defaultDatabase = @"C:\Program Files (x86)\ROCLINK800\ROC.mdb";

        public ROCDB()
        {
            initParameters();
            readROCTLPStrings();
            
            //TestConversionOfTypes();
        }
        private void initParameters()
        {
            for (int i = 0; i < 256; i++) tPointTypesDictionary[i] = "UDP" + i.ToString();

            //Add the extended user type
            for (int i = 1; i <= 50; i++)
            {
                tParametersDictionary[232, i] = "FT" + (i).ToString();
                tParametersDictionary[232, i + 50] = "SINT" + (i).ToString();
                tParametersDictionary[232, i+100] = "BYTE" + (i).ToString();
            }
        }

        public string getCommPort(int i)
        {
            if (i < 0) return null;
            if (string.IsNullOrEmpty(commPortTags[i])) return i.ToString();
            else return commPortTags[i];
        }
        public bool getCommPortIndex(string commPort, out int index)
        {
            if (String.IsNullOrEmpty(commPort))
            {
                index = -1;
                return false;
            }

            for (int i = 0; i < commPortTags.Length; i++)
            {
                if (commPortTags[i] == null) continue;
                if (commPortTags[i].Trim().ToUpper().Equals(commPort.Trim().ToUpper()))
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        //This tests all possible TLP -> string -> TLP conversions to ensure they are reversible
        public void TestConversionOfTypes()
        {
            ROCType = 107;

            //Test all ROC800 tyoes
            ROCType = 800;
            for (int t = 0; t < 1000; t++)
            {
                if (IsRoc800Point(t))
                {
                    TestConversionOfType(t);
                    for (int l = 0; l < 256; l++) for(int p = 0; p < 256; p++) TestConversionOfTLP(t.ToString() + ", " + l.ToString() + ", " + p.ToString());
                }
            }

            //Test all ROC107 tyoes
            ROCType = 107;
            for (int t = 0; t < 1000; t++)
            {
                if (IsRoc107Point(t))
                {
                    TestConversionOfType(t);
                    int logicalCount = 0;
                    if (IsFB107IOPoint(t))
                    {
                        logicalCount = 6;
                    }
                    else
                    {
                        logicalCount = 256;
                    }
                        for (int l = 0; l < logicalCount; l++) for (int p = 0; p < logicalCount; p++) TestConversionOfTLP(t.ToString() + ", " + l.ToString() + ", " + p.ToString());
                }
            }


            //Test all ROC non specific tyoes
            ROCType = 1;
            for (int t = 0; t < 1000; t++)
            {
                if (!IsRoc107Point(t) && !IsRoc800Point(t) && !IsUnknownPoint(t))
                {
                    TestConversionOfType(t);
                    for (int l = 0; l < 256; l++) for (int p = 0; p < 256; p++) TestConversionOfTLP(t.ToString() + ", " + l.ToString() + ", " + p.ToString());
                }
            }

        }

        public bool TestConversionOfType(int t)
        {
            String type = pointTypeNumberToAbbrev(t);
            int _t = abbrevToPointTypeNumber(type);

            if (_t == t) return true;

            MessageBox.Show("Failed to convert type: " + t + " abbrev: " + type + " result: " + _t);

            return false;

        }

        //This routine will convert a TLP to a string and back again to make sure it is not irreversible
        //We do this every time so not to corrupt data
        public bool TestConversionOfTLP(string TLP)
        {
            String abbrev = TLPToAbbrev(TLP);
            string _TLP;

            if (abbrevToTLP(abbrev, out _TLP))
            {
                if (_TLP.Equals(TLP)) return true;
            }
            else
            {
                return false;
            }

            MessageBox.Show("Failed to convert TLP: " + TLP + " abbrev: " + abbrev + " result: " + _TLP);

            return false;
        }

        private static bool IsRoc800Point(int pointType)
        {
            return ((pointType >= 84 && pointType <= 85) ||
                    (pointType >= 101 && pointType <= 109) ||
                    (pointType >= 140 && pointType <= 141) ||
                    (pointType >= 90 && pointType <= 91) ||
                    (pointType >= 95 && pointType <= 96) ||
                    (pointType >= 98 && pointType <= 100) ||
                    (pointType >= 111 && pointType <= 111) ||
                    (pointType >= 136 && pointType <= 136));
        }
        private bool IsRoc107Point(int pointType)
        {
            return (IsFB107IOPoint(pointType) || 
                (pointType == 0) ||
                (pointType == 12) ||
                (pointType >= 14 && pointType <= 17) || 
                (pointType == 40) || 
                (pointType >= 44 && pointType <=45) || 
                (pointType == 48) || 
                (pointType == 59) ||
                (pointType >= 53 && pointType <= 53));
        }




        public List<List<ModbusRecord>> readModbusRegisterTables()
        {
            if (!File.Exists(filename)) return null;

            //if (ROCType != 800) return null;

            DataTable modbusRegisterTables = new DataTable();
            DataTable commPorts = new DataTable();

            string connectionString = string.Format(@"Provider=Microsoft.JET.OLEDB.4.0;User ID=FullAccess;Password=Error: Failed.;data source={0};Persist Security Info=True;Jet OLEDB:System database={1}", filename, userDatabase);
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    OleDbCommand cmd = new OleDbCommand("SELECT PointType, PointNumber, Parameter, Value FROM tConfigData WHERE PointType=118", conn);
                    using (var tableAdapter = new OleDbDataAdapter(cmd)) tableAdapter.Fill(modbusRegisterTables);

                    cmd = new OleDbCommand("SELECT PointType, PointNumber, Parameter, Value FROM tConfigData WHERE PointType=95 AND Parameter=0", conn);
                    using (var tableAdapter = new OleDbDataAdapter(cmd)) tableAdapter.Fill(commPorts);

                    conn.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            //Extract the comm ports
            foreach (DataRow row in commPorts.Rows)
            {
                //This should never happen if OLEDB returns correct data, check anyway though
                if (!row["PointType"].ToString().Equals("95") || !row["Parameter"].ToString().Equals("0")) break;

                int logical = Convert.ToInt32(row["PointNumber"].ToString());
                commPortTags[logical - 1] = row["Value"].ToString();
            }
            commPortTags[255] = "All Comm Ports";


            List<List<ModbusRecord>> modbusTables = new List<List<ModbusRecord>>();

            int index = 0;

            for (int table = 0; table < numberOfModbusRegTables; table++)
            {
                index++; //Ignore the tag
                List<ModbusRecord> modbusTable = new List<ModbusRecord>();

                //15 records per table
                for (int entry = 1; entry <= 15; entry++)
                {
                    ModbusRecord modbusRecord = new ModbusRecord(modbusRegisterTables.Rows[index++]["Value"].ToString(), modbusRegisterTables.Rows[index++]["Value"].ToString(),
                                                                 modbusRegisterTables.Rows[index++]["Value"].ToString(), modbusRegisterTables.Rows[index++]["Value"].ToString(),
                                                                 modbusRegisterTables.Rows[index++]["Value"].ToString(), modbusRegisterTables.Rows[index++]["Value"].ToString(), this);
                    modbusTable.Add(modbusRecord);
                }
                modbusTables.Add(modbusTable);
            }
            return modbusTables;
        }

        public bool writeModbusTables(string _filename, DataGridView ModbusGrid)
        {
            bool ignoreTLPerror = false;
            string connectionString = string.Format(@"Provider=Microsoft.JET.OLEDB.4.0;Mode=ReadWrite;User ID=FullAccess;Password=Error: Failed.;data source={0};Persist Security Info=True;Jet OLEDB:System database={1}", filename, userDatabase);
            DataTable tConfigData = new DataTable();
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    //Read the whole table and programatically update entrys, we then autoamtically generate an UPDATE sql query to update the file
                    var tableAdapter = new OleDbDataAdapter();
                    tableAdapter.SelectCommand = new OleDbCommand("SELECT * FROM tConfigData WHERE PointType=118", conn);
                    tableAdapter.Fill(tConfigData);

                    //Get index of the first modbus record, this should always be 0
                    int indexOfFirstEntry = -1;
                    for (int i = 0; i < tConfigData.Rows.Count; i++)
                    {
                        if (tConfigData.Rows[i]["PointType"].ToString().Equals("118"))
                        {
                            indexOfFirstEntry = i;
                            break;
                        }
                    }

                    if (indexOfFirstEntry == -1)
                    {
                        MessageBox.Show("Error: ROC 800 File not in expected format, save failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return false;
                    }

                    //Update the table
                    bool success = true;

                    //Update
                    foreach(DataRow row in tConfigData.Rows)
                    {
                        int parameterNumber = Convert.ToInt16(row["Parameter"].ToString());

                        //Skip if this is the tag as we don't update this value
                        if (parameterNumber ==  0) continue;

                        int ModbusTableNumber = Convert.ToInt16(row["PointNumber"].ToString());
                        String originalValue = row["Value"].ToString();

                        //Calculate the row index for element 1 of this OPC table
                        int rowNumber = ((ModbusTableNumber - 1) * 15) + ModbusTableNumber + (parameterNumber / 6);
                            
                        if(parameterNumber%6 == 0) rowNumber-=1;

                        //Calulate the offset for this value
                        int offset = (parameterNumber-1) % 6;

                        string newValue = ModbusGrid[offset, rowNumber].Value.ToString();

                        //Decode the variable based on its offset
                        int reg;
                        switch (offset)
                        {
                            case 0:
                                //Start register, check it is a number, positive and <= 65535
                                if (!Int32.TryParse(newValue, out reg))
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else if (reg < 0)
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else if (reg > 65535)
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    row["Value"] = newValue;
                                }
                                break;
                            case 1:
                                //End register, check it is a number, positive and <= 65535
                                if (!Int32.TryParse(newValue, out reg))
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else if (reg < 0)
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else if (reg > 65535)
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    row["Value"] = newValue;
                                }
                                break;
                            case 2:
                                //TLP
                                //Warn the user if we can't parse this value
                                if (abbrevToTLP(newValue, out newValue))
                                {
                                    row["Value"] = newValue;
                                }
                                else
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                break;
                            case 3:
                                //Point or Parameter
                                //Should be point or parameter, but accept 0 or 1 as well
                                if (newValue.Equals("0") || newValue.Equals("1"))
                                {
                                    row["Value"] = newValue;
                                    break;
                                } 
                                if (newValue.Trim().ToUpper().Equals("POINT"))
                                {
                                    row["Value"] = "0";
                                }
                                else if  (newValue.Trim().ToUpper().Equals("PARAMETER"))
                                {
                                    row["Value"] = "1";
                                }
                                else
                                {
                                    success = false;
                                }
                                break;
                            case 4:
                                //Must be between 0 and 81
                                if (!Int32.TryParse(newValue, out reg))
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else if (reg < 0)
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else if (reg > 81)
                                {
                                    success = false;
                                    ModbusGrid[offset, rowNumber].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    row["Value"] = newValue;
                                }
                                break;
                            case 5:
                                //Comm port, should be the correct text but accept a postive integer also
                                int index;
                                if (getCommPortIndex(newValue, out index)) row["Value"] = index.ToString();
                                else
                                {
                                    success = false;
                                }
                                break;
                            default:
                                MessageBox.Show("Error: ROC 800 File not in expected format, save failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                conn.Close();
                                return false;
                        }
                        string WriteValue = row["Value"].ToString();

                        if (!success && !ignoreTLPerror)
                        {
                            DialogResult result = MessageBox.Show("Error parsing TLPs, do you want to just update the successful ones?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (result != DialogResult.Yes)
                            {
                                conn.Close();
                                return false;
                            }
                            else
                            {
                                ignoreTLPerror = true;
                            }
                        }
                    }



                    //Build the update command query
                    OleDbCommandBuilder cb = new OleDbCommandBuilder(tableAdapter);
                    cb.QuotePrefix = "[";
                    cb.QuoteSuffix = "]";
                    tableAdapter.UpdateCommand = cb.GetUpdateCommand();

                    //Write the data back to the .800 file
                    tableAdapter.Update(tConfigData);

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error saving .800 file! Error: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        //This reads OPCTables to a matrix of OPC Strings
        public string[,] readOPCTables()
        {

            if (!File.Exists(filename)) return null;

            currentOPCRecordsFromFile = new DataTable();
            string connectionString = string.Format(@"Provider=Microsoft.JET.OLEDB.4.0;User ID=FullAccess;Password=Error: Failed.;data source={0};Persist Security Info=True;Jet OLEDB:System database={1}", filename, userDatabase);
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    OleDbCommand cmd = new OleDbCommand("SELECT PointType, PointNumber, Parameter, Value FROM tConfigData WHERE PointType=99 OR PointType=0 AND Value<>'0.0'", conn);

                    using (var tableAdapter = new OleDbDataAdapter(cmd))
                    {
                        tableAdapter.Fill(currentOPCRecordsFromFile);
                    }
                    conn.Close();

                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (currentOPCRecordsFromFile == null)
            {
                throw new Exception("ROC 800 file not in supported format!");
            }
            if (currentOPCRecordsFromFile.Rows.Count < 352)
            {
                throw new Exception("ROC 800 file not in supported format!");
            }

            //Extract the OPC records from the datatable 
            //FIXME we should read this from the 800 DB instead as this is a bit dodgy
            if (currentOPCRecordsFromFile.Rows[0]["PointType"].ToString().Equals("99"))
            {
                numberOfOPCTables = 16;
                OPCPointType = 99;
                numberOfModbusRegTables = 24;
                ROCType = 800;
            }
            else
            {
                numberOfModbusRegTables = 12;
                numberOfOPCTables = 8;
                OPCPointType = 0;
                ROCType = 107;
            }

            //Create a new OPCTable with 16 OPCs and 44 entries
            string[,] OPCTable = new string[numberOfOPCTables, 44];

            try
            {
                foreach (DataRow row in currentOPCRecordsFromFile.Rows)
                {
                    int ParameterNumber = Convert.ToInt16(row["Parameter"].ToString());
                    int OPCTableNumber = Convert.ToInt16(row["PointNumber"].ToString());
                    String value = row["Value"].ToString();

                    if (!value.Equals("0.0") && ParameterNumber != 0)
                    {
                        OPCTable[OPCTableNumber - 1, ParameterNumber - 1] = TLPToAbbrev(value);
                    }
                }

                //We set the ROC Type here

            }
            catch (Exception e)
            {
                //We catch any exceptions here due to the rows having incorrect columns
                throw new Exception("ROC 800 file not in supported format! Error: " + e.Message);
            }

            return OPCTable;

        }
        //FIXME move this logic to the GUI and only pass in OPC tables to update here. Error checking (setting cells red and messages should not be done in this class)
        public bool writeOPCTables(string _filename, DataGridView OPCodeGrid)
        {

            string connectionString = string.Format(@"Provider=Microsoft.JET.OLEDB.4.0;Mode=ReadWrite;User ID=FullAccess;Password=Error: Failed.;data source={0};Persist Security Info=True;Jet OLEDB:System database={1}", filename, userDatabase);
            DataTable tConfigData = new DataTable();
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    //Read the whole table and programatically update entrys, we then autoamtically generate an UPDATE sql query to update the file
                    var tableAdapter = new OleDbDataAdapter();

                    tableAdapter.SelectCommand = new OleDbCommand("SELECT * FROM tConfigData WHERE PointType=99 OR PointType=0 AND Value<>'0.0'", conn);
                    tableAdapter.Fill(tConfigData);

                    //Get index of the first OPC record, this should always be 0
                    int indexOfFirstEntry = -1;
                    for(int i = 0; i < tConfigData.Rows.Count; i++)
                    {
                        if (tConfigData.Rows[i]["PointType"].ToString().Equals("99") || tConfigData.Rows[i]["PointType"].ToString().Equals("0"))
                        {
                            indexOfFirstEntry = i;
                            break;
                        }
                    }

                    if (indexOfFirstEntry == -1)
                    {
                        MessageBox.Show("Error: ROC 800 File not in expected format, save failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return false;
                    }

                    //Update the table
                    bool success = true;

                    for (int i = indexOfFirstEntry; i < indexOfFirstEntry+(numberOfOPCTables * 44); i++)
                    {
                        int ParameterNumber = Convert.ToInt16(tConfigData.Rows[i]["Parameter"].ToString());
                        int OPCTableNumber = Convert.ToInt16(tConfigData.Rows[i]["PointNumber"].ToString());
                        String originalValue = tConfigData.Rows[i]["Value"].ToString();

                        if (!originalValue.Equals("0.0") && ParameterNumber != 0)
                        {
                            String value = OPCodeGrid.Rows[ParameterNumber - 1].Cells[OPCTableNumber - 1].Value.ToString();

                            String TLP;
                            if (abbrevToTLP(value, out TLP))
                            {
                                tConfigData.Rows[i]["Value"] = TLP;
                            }
                            else
                            {
                                OPCodeGrid[OPCTableNumber-1, ParameterNumber-1].Style.BackColor = Color.Red;
                                indexOfFirstEntry++; //Skip this item as we couldn't convert it back.
                                success = false;
                            }
                        }
                    }


                    if (!success)
                    {
                        DialogResult result = MessageBox.Show("Error parsing TLPs, do you want to just update the successful ones?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (result != DialogResult.Yes)
                        {
                            conn.Close();
                            return false;
                        }
                    }

                    //Build the update command query
                    OleDbCommandBuilder cb = new OleDbCommandBuilder(tableAdapter);
                    cb.QuotePrefix = "[";
                    cb.QuoteSuffix = "]";
                    tableAdapter.UpdateCommand = cb.GetUpdateCommand();

                    //Write the data back to the .800 file
                    tableAdapter.Update(tConfigData);

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error saving .800 file! Error: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public bool validateCommPortText(string commPort, out string validatedCommPort)
        {
            validatedCommPort = commPort;

            //if the user entered a number convert it to text
            int commPortNumber;
            if (Int32.TryParse(commPort, out commPortNumber))
            {
                if (commPortNumber < 0) return false;
                validatedCommPort = getCommPort(commPortNumber);
                return true;
            }
            //Otherwise search for the commport to make sure it is valid
            foreach (String port in commPortTags)
            {
                if (string.IsNullOrEmpty(port)) continue;
                if (port.Trim().ToUpper().Equals(commPort.Trim().ToUpper())) return true;
            }

            return false;
        }

        public string getDefaultModbusCellValue(DataGridViewCell cell)
        {
            if (cell == null) return null;
            if (cell.Value == null) return null;
            return getDefaultModbusCellValue(cell.ColumnIndex);
        }

        public string getDefaultModbusCellValue(int offset)
        {
            switch (offset)
            {
                case 2:
                    //TLP
                    return "Undefined";
                case 3:
                    //Point or Parameter
                    return "Point";
                case 5:
                    //Comm port,
                    return getCommPort(0);
                default:
                    return "0";
            }
        }

        public bool validateModbusEntry(DataGridViewCell entry, out string validatedString)
        {
            return validateModbusEntry(entry.Value.ToString(), entry.ColumnIndex, out validatedString);
        }

        //Returns default value if it can't validate a cell
        public bool validateModbusEntry(String entry, int offset, out string validatedString)
        {
            validatedString = entry;
            if (entry == null) return false;
            int reg;
            switch (offset)
            {
                case 0:
                    //Start register, check it is a number, positive and <= 65535
                    if (!Int32.TryParse(entry, out reg))
                    {
                        validatedString = "0";
                        return false;
                    }
                    if (reg < 0 || reg > 65535)
                    {
                        validatedString = "0";
                        return false;
                    }
                    return true;
                case 1:
                    //End register, check it is a number, positive and <= 65535
                    if (!Int32.TryParse(entry, out reg))
                    {
                        validatedString = "0";
                        return false;
                    }
                    if (reg < 0 || reg > 65535)
                    {
                        validatedString = "0";
                        return false;
                    }
                    return true;
                case 2:
                    //TLP
                    return validateTLP(entry, out validatedString);
                case 3:
                    //Point or Parameter
                    //Should be point or parameter, but accept 0 or 1 as well
                    if (entry.Trim().ToUpper().Equals("POINT") || entry.Trim().ToUpper().Equals("PARAMETER")) return true;
                    if (!Int32.TryParse(entry, out reg))
                    {
                        validatedString = "POINT";
                        return false;
                    }
                    if (reg == 0 || reg == 1) return true;
                    validatedString = "POINT";
                    return false;
                case 4:
                    //Conversion code 0-81
                    if (!Int32.TryParse(entry, out reg))
                    {
                        validatedString = "0";
                        return false;
                    }
                    if (reg >= 0 && reg <= 81) return true;
                    validatedString = "0";
                    return false;
                case 5:
                    //Comm port, should be the correct text but accept a postive integer also
                    if (validateCommPortText(entry, out validatedString)) return true;
                    validatedString = getCommPort(0);
                    return false;
  
                default:
                    validatedString = "ERROR";
                    return false;
            }
        }

        //Validates a TLP string and will convert it to text if it isn't already
        public bool validateTLP(String TLP, out string textTLP)
        {
            if (String.IsNullOrEmpty(TLP))
            {
                textTLP = TLP;
                return false;
            }
            //Convert the TLP to a string if it is not already
            textTLP = TLPToAbbrev(TLP);

            //If it can't be converted to a T,L,P let the program know
            string temp;
            return abbrevToTLP(TLP, out temp);
        }


        public string TLPToAbbrev(string TLPString)
        {
            if (String.IsNullOrEmpty(TLPString)) return null;

            int T, L, P;
            //Remove any white space
            String TLPString2 = TLPString.Replace(" ", "");
            string[] TLP = TLPString2.Split(',');

            //If we are in SFP 2, DATA1 format already do nothihng
            if (TLP.Length < 3) return TLPString;

            if (Int32.TryParse(TLP[0], out T) && Int32.TryParse(TLP[1], out L) && Int32.TryParse(TLP[2], out P)) return TLPToAbbrev(T, L, P);

            return TLPString;

        }

        public string TLPToAbbrev(int type, int logical, int parameter)
        {
            if (type == 0 && logical == 0 && parameter == 0) return "Undefined";

            string _type = pointTypeNumberToAbbrev(type);
            string _logical = logicalToString(type, logical);
            string _parameter = pointTypeAndParameterToAbbrev(type, parameter);

            //If we couldn't turn the TLP into its text string subtract 1 from the logical number as in text form it is 1 base, in number form it is 0 base
            int logicalNumber;
            int temp;
            if (Int32.TryParse(_type, out temp) && Int32.TryParse(_logical, out logicalNumber) && Int32.TryParse(_parameter, out temp))
            {
                _logical = (logicalNumber - 1).ToString();
                return _type + ", " + _logical + ", " + _parameter;
            }
            else
            {
                return _type + " " + _logical + ", " + _parameter;
            }
        }


        public bool abbrevToTLP(String abbrev, out String TLPString)
        {
            int T;
            int L;
            int P;
            return abbrevToTLP(abbrev, out TLPString, out T, out L, out P);
        }

        public bool abbrevToTLP(String abbrev, out String TLPString, out int Tint, out int Lint, out int Pint)
        {
            Tint = 0;
            Lint = 0;
            Pint = 0;

            abbrev = abbrev.Trim();

            if (abbrev.Equals("Undefined") || abbrev.Equals("undefined"))
            {
                TLPString = "0, 0, 0";
                return true;
            }
            abbrev = abbrev.Trim();

            String[] Temp = abbrev.Split(',');
            String[] Temp2 = Temp[0].Split(' ');

            //Unknown format, return error
            //If we have only 2 commas
            if (Temp.Length < 2 || Temp2.Length < 2) {
                if (Temp.Length != 3)
                {
                    //We are in text mode and the format is incorrect
                    TLPString = null;
                    return false;
                }
                //we are in T,L,P mode
            }
            String T, L, P;

            int pointTypeNumber, logicalNumber, parameterNumber;
            if (Temp.Length != 3)
            {
                //We are formatted SFP 1, DATA1
                T = Temp2[0].Trim();
                L = Temp2[1].Trim();
                P = Temp[1].Trim();
            }
            else
            {
                //We are formatted 98, 1, 1
                T = Temp[0].Trim();
                L = Temp[1].Trim();
                P = Temp[2].Trim();
            }

            pointTypeNumber = abbrevToPointTypeNumber(T);
            logicalNumber = getLogicalNumber(L);
            parameterNumber = getParameterNumber(pointTypeNumber, P);

            Tint = pointTypeNumber;
            Lint = logicalNumber;
            Pint = parameterNumber;

            //Check if the user has entered a RAW TLP with no text, if this is the case the logical number will be off by 1
            int temp;
            if (Int32.TryParse(T, out temp) && Int32.TryParse(L, out temp) && Int32.TryParse(P, out temp))
            {
                logicalNumber++;
                Lint = logicalNumber;
            }

            //Invalid number
            if (pointTypeNumber == -1 || logicalNumber == -1 || parameterNumber == -1)
            {
                TLPString = null;
                return false;
            }

            TLPString = pointTypeNumber.ToString() + ", " + logicalNumber.ToString() + ", " + parameterNumber.ToString();
            return true;
        }

        public string pointTypeNumberToAbbrev(int type)
        {
            if (type >= tPointTypesDictionary.Length) return type.ToString();

            String abbrev = tPointTypesDictionary[type];
            if (!string.IsNullOrEmpty(abbrev)) return abbrev;

            return type.ToString();

        }


        public string pointTypeAndParameterToAbbrev(int type, int parameter)
        {
            if (type >= tPointTypesDictionary.Length) return parameter.ToString();
            String abbrev = tParametersDictionary[type, parameter];
            if (!string.IsNullOrEmpty(abbrev)) return abbrev;

            return parameter.ToString();

        }

        public int abbrevToPointTypeNumber(String abbrev)
        {
            if (String.IsNullOrEmpty(abbrev)) return -1;

            //Attempt to convert it straight back to a number, eg unknown types
            int point;
            if (int.TryParse(abbrev, out point)) return point;

            for (int i = 0; i < tPointTypesDictionary.Length; i++)
            {
                if (!String.IsNullOrEmpty(tPointTypesDictionary[i]))
                {
                    //We found a string that matches in the table
                    if (tPointTypesDictionary[i].Equals(abbrev))
                    {
                        //If this index that matches is for a specific ROC type only, make sure we return the correct one.
                        if (IsRoc800Point(i) || IsRoc107Point(i))
                        {
                            //Check to make sure we return the ROC800 point type number if we have a ROC800 file
                            if (ROCType == 800)
                            {
                                //If we have a file of type 800 then make sure we return it only if it is a ROC800 point type
                                if (IsRoc800Point(i)) return i;

                            }
                            else if (ROCType == 107)
                            {
                                //we have a ROC107 file, make sure we return it only if it is a ROC107 point type number
                                if (IsRoc107Point(i)) return i;
                            }
                        }
                        else if (!IsUnknownPoint(i))
                        {
                            //This is non ROC specific type, return it
                            return i;
                        }
                    }
                }
            }

            //Can't do anything with this type, user error
            return -1;
        }


        public int getParameterNumber(int pointTypeNumber, String abbrev)
        {
            if (String.IsNullOrEmpty(abbrev) || pointTypeNumber == -1) return -1;

            //Attempt to parse the abbreviation for unknown types
            int parameter;
            if (Int32.TryParse(abbrev, out parameter)) return parameter;

            for (int i = 0; i < tPointTypesDictionary.Length; i++)
            {
                if (!String.IsNullOrEmpty(tParametersDictionary[pointTypeNumber, i]))
                {
                    if (tParametersDictionary[pointTypeNumber, i].Equals(abbrev))
                    {
                        return i;
                    }
                }
            }

            //This will only occur on user error
            return -1;
        }

        public int getLogicalNumber(String abbrev)
        {
            if (String.IsNullOrEmpty(abbrev)) return -1;

            int logicalNumber;

            //SFP 6, DATA1 will return 6, AIN 5-2, EU however must be converted via a different routine
            if (int.TryParse(abbrev, out logicalNumber)) return logicalNumber - 1;

            //Match a ROC800 point
            Regex regex = new Regex(@"([0-9][0-9]?)-([0-9][0-9]?)");
            Match match = regex.Match(abbrev);
            if (match.Success && match.Groups.Count == 3)
            {
                int slotNumber = Convert.ToInt16(match.Groups[1].Value);
                int slotIndex = Convert.ToInt16(match.Groups[2].Value);
                return slotNumber * pointsPerModule + (slotIndex - 1);
            }

            //Match a ROC107 point
            regex = new Regex(@"([A-Z])([0-9][0-9]?)");
            match = regex.Match(abbrev);
            if (match.Success)
            {
                string slot = match.Groups[1].Value;
                int slotIndex = Convert.ToInt16(match.Groups[2].Value);
                int slotNumber = 0;
                switch (slot)
                {
                    case "A":
                        slotNumber = 0;
                        break;
                    case "B":
                        slotNumber = 1;
                        break;
                    case "C":
                        slotNumber = 2;
                        break;
                    case "D":
                        slotNumber = 3;
                        break;
                    case "E":
                        slotNumber = 4;
                        break;
                    default:
                        return -1;
                }

                return slotNumber * 16 + (slotIndex - 1);
            }

            return -1;
        }

        private string getTLPFromTable(DataTable tConfigData, int T, int L, int P)
        {

            foreach (DataRow row in tConfigData.Rows)
            {
                int pointType = Convert.ToInt16(row["PointType"].ToString());
                int pointNumber = Convert.ToInt16(row["PointNumber"].ToString());
                int parameter = Convert.ToInt16(row["Parameter"].ToString());

                if (pointType == T && pointNumber == L && parameter == P)
                {
                    return row["Value"].ToString();
                }
            }
            return null;

        }

        public string logicalToString(int pointType, int logical)
        {
            if (IsRoc800Point(pointType) && IsIOPoint(pointType))
            {
                var slotNumber = (logical / pointsPerModule);
                int slotIndex = (logical % pointsPerModule) + 1;
                return string.Format("{0}-{1}", slotNumber, slotIndex);
            }
            else if (IsFB107IOPoint(pointType))
            {
                int rackNumber = logical / 16;
                int rackIndex = (logical % 16) + 1;
                var slotNumber = logical < 5 ? 0 : logical - 4; ;

                if ((slotNumber == 0 && rackNumber != 0 && rackNumber != 4) ||                      // Slot 0 = Rack A 0-15 and Rack E (system points)
                   ((slotNumber == 1 || slotNumber == 7) && (rackNumber != 1 || rackIndex > 8)) ||  // Slot 1/7 = Rack B 0-8
                   (slotNumber == 2 && (rackNumber != 1 || rackIndex < 9)) ||                       // Slot 2 = Rack B 9-15
                   (slotNumber == 3 && (rackNumber != 2 || rackIndex > 8)) ||                       // Slot 3 = Rack C 0-8
                   (slotNumber == 4 && (rackNumber != 2 || rackIndex < 9)) ||                       // Slot 4 = Rack C 9-15
                   (slotNumber == 5 && (rackNumber != 3 || rackIndex > 8)) ||                       // Slot 5 = Rack D 0-8
                   (slotNumber == 6 && (rackNumber != 3 || rackIndex < 9)))                   // Slot 6 = Rack D 9-15
                {
                    return (logical + 1).ToString();
                }
                

                return string.Format("{0}{1}", char.ConvertFromUtf32(65 + rackNumber), rackIndex);
            }

            //All other points are sim
            return (logical + 1).ToString();

        }

        private bool IsFB107IO(int pointType)
        {
            return (pointType >= 1 && pointType <= 5);
        }

        private bool IsUnknownPoint(int pointType)
        {
            return (pointType == 6);
        }

        private bool IsFB107IOPoint(int pointType)
        {
            return (pointType >= 1 && pointType <= 5);
        }

        private bool IsIOPoint(int pointType)
        {
            return ((pointType >= 1 && pointType <= 5) ||
                    (pointType >= 84 && pointType <= 85) ||
                    (pointType >= 101 && pointType <= 109) ||
                    (pointType >= 140 && pointType <= 141));
        }


        //Reads in roc dataTypes from ROC database
        public void readROCTLPStrings()
        {
            try{

                string connectionString = string.Format(@"Provider=Microsoft.JET.OLEDB.4.0;User ID=FullAccess;Password=Error: Failed.;data source={0};Persist Security Info=True;Jet OLEDB:System database={1}", defaultDatabase, userDatabase);
                DataTable tParameters = new DataTable();
                DataTable tPointTypes = new DataTable();

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM tPointTypes", conn);

                    using (var tableAdapter = new OleDbDataAdapter(cmd)) tableAdapter.Fill(tPointTypes);

                    cmd = new OleDbCommand("SELECT * FROM tParameters", conn);
                    using (var tableAdapter = new OleDbDataAdapter(cmd)) tableAdapter.Fill(tParameters);

                    conn.Close();
                }

                //Add all point types to a dictionary, also create a new pointtype parameter dictionary and add it to our parameter dictionary
                foreach (DataRow row in tPointTypes.Rows)
                {
                    int pointType = Convert.ToInt16(row["PointType"]);

                    //Skip MBPAR type 53 as it is an unknown type, it should be 117
                    if (pointType == 53) continue;

                    tPointTypesDictionary[pointType] = row["Abbrev"].ToString();

                }

                HashSet<String> paramHashSet = new HashSet<String>();

                foreach (DataRow row in tParameters.Rows)
                {
                    int pointType, parameter;
                    if (Int32.TryParse(row["PointType"].ToString(), out pointType) && Int32.TryParse(row["Parameter"].ToString(), out parameter))
                    {
                        String tParam = pointType.ToString() + parameter.ToString();



                        if (paramHashSet.Contains(tParam))
                        {
                            //If this abbreviation already exists for this point type make it null, this is as there are sometimes multiple abbreviations
                            //as there is no other way to handle this
                            tParametersDictionary[pointType, parameter] = null;
                            //FIXME what should we do here?
                        } else {
                            tParametersDictionary[pointType, parameter] = row["Abbrev"].ToString();
                            paramHashSet.Add(pointType.ToString() + row["Abbrev"].ToString());
                        }
                    }

                }

            }
            catch (Exception e)
            {
                throw new Exception("ROCLink database could not be read due to: " + e.Message);
            }

        }

        public class ModbusRecord
        {
            public string startRegister;
            public string endRegister;
            public string TLP;
            public string indexing;
            public string conversionCode;
            public string commPort;
            ROCDB roc;

            public ModbusRecord(string _startRegister, string _endRegister, string _TLP, string _indexing, string _conversionCode, string _commPort, ROCDB _roc)
            {
                roc = _roc;

                indexing = _indexing;
                if (indexing.Equals("0")) indexing = "Point";
                if (indexing.Equals("1")) indexing = "Parameter";

                int commPortNumber;
                if (Int32.TryParse(_commPort, out commPortNumber)) commPort = _roc.getCommPort(commPortNumber);
                else commPort = _commPort;

                startRegister = _startRegister;
                endRegister = _endRegister;
                TLP = _roc.TLPToAbbrev(_TLP);
                conversionCode = _conversionCode;
            }
        }
    }

}


