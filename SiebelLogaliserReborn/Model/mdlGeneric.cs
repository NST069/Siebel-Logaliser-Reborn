using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiebelLogaliserReborn.Model
{
    class mdlGeneric
    {
		public static long GetLineCount(string sFileName)
		{
			byte[] arrByteBuffer = new byte[16785];
			//16k buffer read at one time
			long lLineCount = -1;
            try
            {
				using (FileStream fs = new FileStream(sFileName, FileMode.Open, FileAccess.Read, FileShare.Read, arrByteBuffer.Length))
				{
					lLineCount = 0;
                    int iBuffer = fs.Read(arrByteBuffer, 0, arrByteBuffer.Length);
                    while ((iBuffer > 0))
					{
						for (int iCount = 0; iCount <= (iBuffer - 1); iCount += 1)
						{
							if (arrByteBuffer[iCount] == 0xd)
								lLineCount += 1;
						}
						iBuffer = fs.Read(arrByteBuffer, 0, arrByteBuffer.Length);
					}
					fs.Close();
					lLineCount += 1;
				}

			}
			catch (Exception ex)
			{
				lLineCount = -1;
				//Err().Raise(Err().Number, "GetLineCount" + Err().Description, Err().Description);
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				arrByteBuffer = null;
			}

			return lLineCount;
		}
		public static void ExportFile(string sFileName, int iFileType, ref DataGridView dgData, string sDelim = ",")
		{
			try
			{
				switch (iFileType)
				{
					case 1:
					case 2:
						StringBuilder sb = new StringBuilder();
						//create columnNames:
						for (int i = 0; i <= dgData.Rows.Count - 1; i++)
						{
							string[] array = new string[dgData.Columns.Count];
							if (i.Equals(0))
							{
								//get column header text from all columns:
								for (int j = 0; j <= dgData.Columns.Count - 1; j++)
								{
									array[j] = dgData.Columns[j].HeaderText;
								}
								sb.AppendLine(String.Join(sDelim, array));
							}
							//get values from columns for specific row (row[i]):
							for (int j = 0; j <= dgData.Columns.Count - 1; j++)
							{
								if (!dgData.Rows[i].IsNewRow)
								{
									array[j] = "\"" + dgData[j, i].Value.ToString() + "\"";
								}
							}
							if (!dgData.Rows[i].IsNewRow)
							{
								sb.AppendLine(String.Join(sDelim, array));
							}
						}

						File.WriteAllText(sFileName, sb.ToString());
						break;
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		public static int IndexOfN(string sString, char cSearchChar, int iNthPosition = 0)
		{
			int functionReturnValue = 0;
			int iReturn = -1;
			try
			{
				int iOccur = 0;
				int iPos = sString.IndexOf(cSearchChar, 0);
				while ((iPos != -1))
				{
					iOccur += 1;
					if (iOccur == iNthPosition)
					{
						iReturn = iPos;
						iPos = -1;
					}
					else
					{
						iPos = sString.IndexOf(cSearchChar, iPos + 1);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				functionReturnValue = iReturn;
			}
			return functionReturnValue;
		}

		public static object ExportDataToFileExp1(ref DataTable dtTable, string sFile, string sSeparator = ",")
		{
			object functionReturnValue = null;
			// EXPERIMENTAL
			//Export data from data table to a file
			//dtTable.WriteXml("1.xml")
			System.IO.StreamWriter writer = null;
			try
			{
				string sTempSep = "";
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				writer = new System.IO.StreamWriter(sFile);
				foreach (DataColumn col in dtTable.Columns)
				{
					builder.Append(sTempSep).Append("\"").Append(col.Caption).Append("\"");
					sTempSep = sSeparator;
				}
				writer.WriteLine(builder.ToString());

				foreach (DataRow row in dtTable.Rows)
				{
					sTempSep = "";
					builder = new System.Text.StringBuilder();

					foreach (DataColumn col in dtTable.Columns)
					{
						builder.Append(sTempSep).Append("\"").Append(row[col.Caption]).Append("\"");

						sTempSep = sSeparator;
					}
					writer.WriteLine(builder.ToString());
				}
				writer.Close();

				functionReturnValue = "";

			}
			catch (Exception ex)
			{
				functionReturnValue = ex.Message;

			}
			finally
			{
			}
			return functionReturnValue;
		}
	}
}
