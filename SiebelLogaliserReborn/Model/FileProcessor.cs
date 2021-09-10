﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Windows;

namespace SiebelLogaliserReborn.Model
{
	class FileProcessor
	{
		public DataSet dsSQLSet;
		public DataTable dtSQL;
		public DataTable dtLog;

		public DataTable dtLogErr;
		private TextReader trReader;

		private object slStopThread = new object();

		public bool bStopThread = false;

		public DataSet ProcessFile(String fileName, int iSQLExecDiff = 0, int iLogExecDiff = 0)
		{
			//https://docs.oracle.com/cd/E14004_01/books/SysDiag/SysDiagEvntLogAdmin14.html
			// Main function that loops through all lines, collects required information in three datatables of a single dataset
			bool bLog = false;
            try
            {
                DateTime dtPrevDateTime = DateTime.Now;
			DateTime dtCurDateTime = DateTime.Now;
			string sLine = null;
			string sSQLId = "";
			string sSQLTime = "";
			string sSQL = "";
			string sBindVars = "";
			long lLineNo = 0;
			long lSQLStartLineNo = 0;
			long lPrevLineNo = 0;
			int iTempPos = 0;
			int iErrCodePos = 0;
			double dTimeDiff = 0;
			double dSQLTime = 0;
			bool bRecording = false;
			bool bLineProcessed = false;

			string sStartLine = Properties.Settings.Default.SQLStartLine;
			//Select SQL start
			string sStartLineUpd = Properties.Settings.Default.SQLStartLineUpd;
			//Update SQL start
			string sExecLine = Properties.Settings.Default.SQLExecLine;
			//SQL Exec line, signifies statement end
			string sInsExecLine = Properties.Settings.Default.SQLInsertExecLine;
			string sBindLine = Properties.Settings.Default.SQLBindLine;
			//Line with bind variable

			// Read line by line
			// to-do: find out if reading in blocks gets better performance
			using (StreamReader srRead = new StreamReader(fileName))
			{
				sLine = srRead.ReadLine();

				// the following line signifies log. This is useful to fill in log performance and log error tables
				if (sLine.IndexOf(".log") > 0)
					bLog = true;
				//First line with with a ".log" signifies log file

				// initialize data tables
				CreateDataTable(bLog);

				while (!((sLine == null || bStopThread))) //TODO: Fix Query recording
				{
					bLineProcessed = true;
					lLineNo += 1;

					if (sLine.IndexOf(sStartLine) >= 0)
					{
						// This line is start of Select SQL, start SQL recording
						bRecording = true;
						//dtPrevDateTime = null;
						sSQLId = sLine.Substring(sLine.LastIndexOf(":") + 1).Trim();
						lSQLStartLineNo = lLineNo;

					}
					else if (sLine.IndexOf(sStartLineUpd) >= 0)
					{
						// This line is start of Update SQL, start SQL recording
						bRecording = true;
						//dtPrevDateTime = null;
						sSQLId = sLine.Substring(sLine.LastIndexOf(":") + 1).Trim();
						lSQLStartLineNo = lLineNo;

					}
					else if (sLine.IndexOf(sBindLine) >= 0)
					{
						// Collect bind variables if recording is already on (b/w SQL start & end statements)
						if (bRecording)
						{
							if ((bLog & sLine.IndexOf("SQLParseAndExecute") >= 0) | bLog == false)
							{
								sBindVars += sLine + Environment.NewLine;
							}
						}

					}
					else if (sLine.IndexOf(sExecLine) >= 0 || sLine.IndexOf(sInsExecLine) >= 0)
					{
						// End of SQL - select or update. Stop recording
						bRecording = false;
						iTempPos = sLine.LastIndexOf(":");
						sSQLTime = sLine.Substring(iTempPos + 1, sLine.IndexOf(".") + 4 - iTempPos);
						// Collect data elements only if exec time >= user specified time
						try
						{
							dSQLTime = Convert.ToDouble(sSQLTime);
						}
						catch
						{
							dSQLTime = -1;
						}

						if ((dSQLTime > iSQLExecDiff))
						{
							iTempPos = sSQL.IndexOf(Environment.NewLine + Environment.NewLine);
							//first blank line in this string represents the position where SQL has ended
							dtSQL.Rows.Add(sSQLId, sSQLTime, "1", sSQLTime, sSQL.Substring(0, iTempPos), sBindVars.Substring(0), lSQLStartLineNo);
						}
						sBindVars = "";
						sSQL = "";

					}
					else
					{
						// if recording is on, SQL is being recorded. Else, there is no use of the line
						if (bRecording)
						{
							sSQL = sSQL + sLine.TrimEnd() + Environment.NewLine;
						}
						else
						{
							bLineProcessed = false;
						}
					}


					if (bLog && !bLineProcessed)
					{
						// Collect log information as well (for Siebel logs - just stating the obvious)
						// Date in log appears after the fourth tab
						//iTempPos = mdlGeneric.IndexOfN(sLine, 'v', 4) + 1;


						if (sLine.Length > 0)//(iTempPos > 0)
						{
							try
							{
								if (DateTime.TryParseExact(sLine.Split('\t')[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtCurDateTime))
								{
									//if (!(dtPrevDateTime == null))
									//{

									dTimeDiff = (dtCurDateTime - dtPrevDateTime).TotalSeconds;
									// Collect info only if transaction time >= user specified time
									if ((dTimeDiff > iLogExecDiff))
									{
										dtLog.Rows.Add(dtPrevDateTime.ToString("dd-MMM-yy HH:mm:ss"), dtCurDateTime.ToString("dd-MMM-yy HH:mm:ss"), dTimeDiff, lPrevLineNo);
									}
									//}
									lPrevLineNo = lLineNo;
									dtPrevDateTime = dtCurDateTime;
								}
							}
							catch (Exception e) { }
						}

                    }

                    // Record errors if "Error" appears after the first tab
                    //iTempPos = sLine.IndexOf("vbTab");
                    if (sLine.Length>0)//(iTempPos > 0)
                    {
                        if (sLine.Contains("Error\t1"))//Substring(iTempPos + 1).StartsWith("Error"))
						{
							iErrCodePos = sLine.IndexOf("SBL");
							if(iErrCodePos>-1)
								dtLogErr.Rows.Add(dtCurDateTime.ToString("dd-MMM-yy HH:mm:ss"), sLine.Substring(iErrCodePos, 13), sLine.Substring(iErrCodePos + 13 + 1).Trim(), lLineNo);
						}
                    }

                    // read next line
                    sLine = srRead.ReadLine();

					// check whether process needs to be stopped. This happens when user hits 'Cancel' button
					lock (slStopThread)
					{
						if (bStopThread)
							bStopThread = true;
					}

				}
			}


			// Remove duplicates and aggregate
			if (dsSQLSet.Tables["SQL"].Rows.Count > 0)
				ScrubDataTable(ref dsSQLSet, "SQL", "SQL");

                //AddDummyRow(ref dsSQLSet);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                slStopThread = null;

            }
            return dsSQLSet;
		}

		public string ProcessFile(string sFileName, bool MultiThread)
		{
			string functionReturnValue = null;
			// this is supposed to be a multi-threaded way of achieving things, not in use
			// to-do: complete this section
			FileStream fs = null;
            try
            {
                if (MultiThread == false)
			{
				this.ProcessFile(sFileName);
			}
			else
			{
				fs = new FileStream(sFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				trReader = TextReader.Synchronized(new StreamReader(fs));
				dsSQLSet = new DataSet();
			}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                functionReturnValue = "";
            }
            return functionReturnValue;
		}

		private void CreateDataTable(bool CreateLogSpecificInfo)
		{
            // initialize all datatables. The name specified here should be the same as name specified in individual columns in datagridview
            try
            {
                DataColumn dCol = null;

			this.dsSQLSet = null;
			this.dsSQLSet = new DataSet();
			this.dtSQL = new DataTable("SQL");

			this.dsSQLSet.Tables.Add(this.dtSQL);
			dCol = this.dtSQL.Columns.Add("SQLId");
			dCol.Caption = "SQL Id";
			dCol = this.dtSQL.Columns.Add("ExecTime");
			dCol.Caption = "Exec Id";
			dCol = this.dtSQL.Columns.Add("ExecNo");
			dCol.Caption = "#";
			dCol = this.dtSQL.Columns.Add("TotalExecTime");
			dCol.Caption = "Total";
			dCol = this.dtSQL.Columns.Add("SQL");
			dCol.Caption = "SQL";
			dCol = this.dtSQL.Columns.Add("BindVar");
			dCol.Caption = "Bind Var";
			dCol = this.dtSQL.Columns.Add("Line");
			dCol.Caption = "Line";

			// Following lines dont play a role in case of spool. However they are executed so that a table is available for validation
			this.dtLog = new DataTable("Log");
			this.dsSQLSet.Tables.Add(this.dtLog);
			dCol = this.dtLog.Columns.Add("LogStart");
			dCol.Caption = "Start";
			dCol = this.dtLog.Columns.Add("LogEnd");
			dCol.Caption = "End";
			dCol = this.dtLog.Columns.Add("LogTimeTaken");
			dCol.Caption = "Exec Time";
			dCol = this.dtLog.Columns.Add("LogLine");
			dCol.Caption = "Line";

			this.dtLogErr = new DataTable("Error");
			this.dsSQLSet.Tables.Add(this.dtLogErr);
			dCol = this.dtLogErr.Columns.Add("LogErrStart");
			dCol.Caption = "Start";
			dCol = this.dtLogErr.Columns.Add("LogErrCode");
			dCol.Caption = "Error Code";
			dCol = this.dtLogErr.Columns.Add("LogErrError");
			dCol.Caption = "Error";
			dCol = this.dtLogErr.Columns.Add("LogErrLine");
			dCol.Caption = "Line";

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

		private void ScrubDataTable(ref DataSet dsData, string TableName, string GroupColumn)
		{
			// Remove duplicates and do aggregation
			// to-do: Evaluate using linq for this, or find a better way
			DataTable dtTemp = null;
			DataTable dtData = null;
			DataRow drCur = null;
			DataRow drPrev = null;
			int lCtr = 0;

            try
            {
                dtData = dsData.Tables[TableName];
			dtData.DefaultView.Sort = GroupColumn;
			dtTemp = dtData.Clone();
			drPrev = dtData.Rows[0];

			// to-do: This is specific to processing SQL, generalise
			for (lCtr = 1; lCtr <= dtData.Rows.Count - 1; lCtr++)
			{
				drCur = dtData.Rows[lCtr];
				if (drCur[GroupColumn] == drPrev[GroupColumn])
				{
					if (Convert.ToDouble(drCur["ExecTime"]) > Convert.ToDouble(drPrev["ExecTime"]))
					{
						drPrev["SQLId"] = drCur["SQLId"];
						drPrev["ExecTime"] = drCur["ExecTime"];
						drPrev["BindVar"] = drCur["BindVar"];
						drPrev["Line"] = drCur["Line"];
					}
					drPrev["ExecNo"] = Convert.ToInt32(drPrev["ExecNo"]) + 1;
					drPrev["TotalExecTime"] = Convert.ToDouble(drPrev["TotalExecTime"]) + Convert.ToDouble(drCur["ExecTime"]);
				}
				else
				{
					dtTemp.ImportRow(drPrev);
					drPrev = drCur;
				}

			}
			dtTemp.ImportRow(drPrev);
			dtTemp.AcceptChanges();

			dsData.Tables.Remove(TableName);
			dsData.Tables.Add(dtTemp);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                drCur = null;
                drPrev = null;
                dtTemp = null;

            }
        }

		private void AddDummyRow(ref DataSet dsData)
		{
			// create dummy rows to display in UI when there is no data
			if (dsData.Tables["SQL"].Rows.Count <= 0)
				dsData.Tables["SQL"].Rows.Add("", "", "", "", "No data to display. Hurray!", "", "");
			if (dsData.Tables["Log"].Rows.Count <= 0)
				dsData.Tables["Log"].Rows.Add("", "No data to display. Hurray!", "", "");
			if (dsData.Tables["Error"].Rows.Count <= 0)
				dsData.Tables["Error"].Rows.Add("", "No errors. Something's wrong!", "", "");
		}
	}
}