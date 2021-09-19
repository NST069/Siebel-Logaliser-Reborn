using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Windows;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace SiebelLogaliserReborn.Model
{
	class FileProcessor : INotifyPropertyChanged
	{
		private ObservableCollection<SQLLine> _sqlLines;
		public ObservableCollection<SQLLine> SqlLines {
			get { return _sqlLines; }
			set {
				_sqlLines = value;
				OnPropertyChanged(nameof(SqlLines));
			}
		}

		private ObservableCollection<ErrLine> _errLines;
		public ObservableCollection<ErrLine> ErrLines
		{
			get { return _errLines; }
			set
			{
				_errLines = value;
				OnPropertyChanged(nameof(ErrLines));
			}
		}

		private ObservableCollection<LogLine> _logLines;
		public ObservableCollection<LogLine> LogLines
		{
			get { return _logLines; }
			set
			{
				_logLines = value;
				OnPropertyChanged(nameof(LogLines));
			}
		}

		private TextReader trReader;

		public LogInfo logInfo;

		private long _lLineNo;
		public long lLineNo {
            get { return _lLineNo; }
			set {
				_lLineNo = value;
				OnPropertyChanged(nameof(lLineNo));
			}
		}

		private object slStopThread = new object();

		public bool bStopThread = false;

		public FileProcessor() {
			SqlLines = new ObservableCollection<SQLLine>();
			ErrLines = new ObservableCollection<ErrLine>();
			LogLines = new ObservableCollection<LogLine>();
		}
		
		public void ProcessFile(string fileName, int iSQLExecDiff = 0, int iLogExecDiff = 0)
		{
			//https://docs.oracle.com/cd/E14004_01/books/SysDiag/SysDiagEvntLogAdmin14.html
			bool bLog = true;
			slStopThread = new object();
			App.Current.Dispatcher.Invoke(()=>{
				SqlLines.Clear();
				ErrLines.Clear();
				LogLines.Clear();
			});

			logInfo = new LogInfo();
			try
			{
				DateTime dtPrevDateTime = DateTime.Now;
				DateTime dtCurDateTime = DateTime.Now;
				string sLine = null;
				string sSQLId = "";
				string sSQLTime = "";
				string sSQL = "";
				string sBindVars = "";
				lLineNo = 0;
				long lSQLStartLineNo = 0;
				long lPrevLineNo = 0;
				int iTempPos = 0;
				int iErrCodePos = 0;
				double dTimeDiff = 0;
				//double dSQLTime = 0;
				bool bRecording = false;
				bool bLineProcessed = false;

				string sStartLine = Properties.Settings.Default.SQLStartLine;
				//Select SQL start
				string sStartLineUpd = Properties.Settings.Default.SQLStartLineUpd;
				//Update SQL start
				string sExecLine = Properties.Settings.Default.SQLExecLine;
				//SQL Exec line, signifies statement end
				string sInsExecLine = Properties.Settings.Default.SQLInsertExecLine;
				string sPrepLine = Properties.Settings.Default.SQLPrepareExecLine;
				string sBindLine = Properties.Settings.Default.SQLBindLine;
				//Line with bind variable

				// Read line by line
				// to-do: find out if reading in blocks gets better performance
				using (StreamReader srRead = new StreamReader(fileName))
				{
					sLine = srRead.ReadLine();

					//Getting log information from the first line
					logInfo.ParseLogInfo(sLine);
					lLineNo++;

					while (!(sLine == null || bStopThread))
					{
						
						if (sLine.Length > 0 && lLineNo > 1)
						{
							string[] l = TryParseLine(sLine);
							
							bLineProcessed = true;
							//SQL
							if (l != null && l[5].IndexOf(sStartLine) >= 0)
							{
								// This line is start of Select SQL, start SQL recording
								bRecording = true;
								//dtPrevDateTime = null;
								sSQLId = l[5].Substring(l[5].LastIndexOf(":") + 1).Trim();
								lSQLStartLineNo = lLineNo;

							}
							else if (l != null && l[5].IndexOf(sStartLineUpd) >= 0)
							{
								// This line is start of Update SQL, start SQL recording
								bRecording = true;
								//dtPrevDateTime = null;
								sSQLId = l[5].Substring(l[5].LastIndexOf(":") + 1).Trim();
								lSQLStartLineNo = lLineNo;

							}
							else if (l != null && l[5].IndexOf(sBindLine) >= 0)
							{
								// Collect bind variables if recording is already on (b/w SQL start & end statements)
								if (bRecording)
								{
									//if ((bLog && l[0].Equals("SQLParseAndExecute")) || bLog == false)
									//{
									sBindVars += l[5] + Environment.NewLine;
									//}
								}

							}
							else if (sLine.IndexOf(sExecLine) >= 0 || sLine.IndexOf(sInsExecLine) >= 0 || sLine.IndexOf(sPrepLine) >= 0)
							{
								// End of SQL - select or update. Stop recording
								bRecording = false;
								iTempPos = sLine.LastIndexOf(":");
								sSQLTime = sLine.Substring(iTempPos + 1, sLine.IndexOf(".") + 4 - iTempPos);
								// Collect data elements only if exec time >= user specified time
								//try
								//{
								//	dSQLTime = Convert.ToDouble(sSQLTime);
								//}
								//catch
								//{
								//	dSQLTime = -1;
								//}

								//if ((dSQLTime > iSQLExecDiff))
								//{
								iTempPos = sSQL.IndexOf(Environment.NewLine + Environment.NewLine);
								//first blank line in this string represents the position where SQL has ended
								
								App.Current.Dispatcher.Invoke(() =>
								{
									SqlLines.Add(new SQLLine(sSQLId, sSQLTime, "1", sSQLTime, sSQL/*.Substring(0, iTempPos)*/, sBindVars.Substring(0), lSQLStartLineNo));
								});
								//}
								sBindVars = "";
								sSQL = "";

							}
							else
							{
								// if recording is on, SQL is being recorded. Else, there is no use of the line
								if (bRecording && sLine!="" && l == null) //bruh
								{
									//l.Value.AddStringToQuery(sLine);
									sSQL = sSQL + sLine.TrimEnd() + Environment.NewLine;
								}
								else
								{
									bLineProcessed = false;
								}
							}
							//

							//LOGS
							if (l != null && !bLineProcessed && bLog)
							{
								if (sLine.Length > 0)//(iTempPos > 0)
								{
									try
									{
										if (DateTime.TryParseExact(l[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtCurDateTime))
										{
											if (!(dtPrevDateTime == null))
											{

												dTimeDiff = (dtCurDateTime - dtPrevDateTime).TotalSeconds;
												// Collect info only if transaction time >= user specified time
												if ((dTimeDiff > iLogExecDiff))
												{
													App.Current.Dispatcher.Invoke(() =>
													{
														LogLines.Add(new LogLine(dtPrevDateTime, dtCurDateTime, dTimeDiff, lPrevLineNo));
													});
												}
											}
											lPrevLineNo = lLineNo;
											dtPrevDateTime = dtCurDateTime;
										}
									}
									catch (Exception e) { MessageBox.Show(e.Message); }
								}
							}
							//

							//ERRORS
							if (l != null && l[1].Equals("Error"))
							{
								iErrCodePos = l[5].IndexOf("SBL");
								DateTime ts;
								DateTime.TryParseExact(l[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ts);
								App.Current.Dispatcher.Invoke(() =>
								{
									ErrLines.Add(new ErrLine(ts, (iErrCodePos > 0) ? l[5].Substring(iErrCodePos, 13) : "", l[5], lLineNo));
								});
							}
						}
						sLine = srRead.ReadLine();
						lLineNo+=1;
                        // check whether process needs to be stopped. This happens when user hits 'Cancel' button
                        lock (slStopThread)
                        {
                            if (bStopThread)
                                bStopThread = true;
                        }
                    }
				}


                // Remove duplicates and aggregate
                //if (dsSQLSet.Tables["SQL"].Rows.Count > 0)
                //    ScrubDataTable(ref _dsSQLSet, "SQL", "SQL");

            }
            catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				slStopThread = null;

			}
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

		public LogInfo GetLogInfo(string fileName)
		{
			try
			{
				using (StreamReader srRead = new StreamReader(fileName))
				{
					string sLine = srRead.ReadLine();
					LogInfo li = new LogInfo();
					bool isParsed = li.ParseLogInfo(sLine);
					return li;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			return null;
		}
		
		public static string[] TryParseLine(string Line)
		{
			try
			{
				string[] logLn = Line.Split('\t');
				int logLevel = int.Parse(logLn[2]);
				DateTime ts;
				DateTime.TryParseExact(logLn[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ts);
				return logLn;
			}
			catch (Exception e)
			{
				return null;
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] String info = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}

	}
}