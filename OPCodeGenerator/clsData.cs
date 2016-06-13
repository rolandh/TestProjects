using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace OPCodeGenerator
{
	public class clsData
	{
		/// <summary>
		/// connection string to the database of your choice
		/// </summary>
		private const string sConnection = "Data Source=localhost;Initial Catalog=AdventureWorks;Integrated Security=True;Pooling=True;Min Pool Size=1;Max Pool Size=10";

		/// <summary>
		/// Get the data from the database or the XML string
		/// </summary>
		/// <returns></returns>
		public DataTable GetData()
		{
			DataTable oTable = new DataTable();
			FileInfo oFI = new FileInfo("SampleData.xml");

			//test for the existing sample data xml
			if (!oFI.Exists)
			{
				//get the data from the database
				string sSQL = "SELECT SalesPersonID,FirstName,LastName,SalesQuota FROM Sales.vSalesPerson";
				oTable = GetTableSQL(sSQL);

				//you need to write out the schema with the data to read it into the table.
				oTable.WriteXml(oFI.FullName, XmlWriteMode.WriteSchema);
			}

			//read in the data from the xml file
			oTable.ReadXml(oFI.FullName);

			return oTable;

		}


		/// <summary>
		/// Get the data from the SQL database the first time or of the XML is missing
		/// THIS IS NOT THE RECOMMENDED METHOD FOR GETTING PRODUCTION DATA FROM THE DB - USE STORED PROCEDURES
		/// </summary>
		/// <param name="sSQL">Select statement</param>
		/// <returns>datatable from Adventureworks</returns>
		private DataTable GetTableSQL(string sSQL)
		{

			SqlCommand oCmd = new SqlCommand();
			SqlDataAdapter oDA = new SqlDataAdapter();
			DataSet oDS = new DataSet();

			oCmd.Connection = new SqlConnection(sConnection);
			oCmd.CommandType = CommandType.Text;
			oCmd.CommandText = sSQL;

			oDA.SelectCommand = oCmd;
			oDA.Fill(oDS);
			return oDS.Tables[0];
		}
	}
}
