using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SiebelLogaliserReborn.Model
{
	class LogInfo : INotifyPropertyChanged
	{
		private string _logEolXlate;//
		public string LogEolXlate {
			get { return _logEolXlate; }
			set {
				_logEolXlate = value;
				OnPropertyChanged(nameof(LogEolXlate));
			}
		}
		private DateTime _startDate;
		public DateTime StartDate {
			get { return _startDate; }
			set {
				_startDate = value;
				OnPropertyChanged(nameof(StartDate));
			}
		}
		private DateTime _endDate;
		public DateTime EndDate {
			get { return _endDate; }
			set {
				_endDate = value;
				OnPropertyChanged(nameof(EndDate));
			}
		}
		private string _timeZone;//
		public string TimeZone
		{
			get { return _timeZone; }
			set
			{
				_timeZone = value;
				OnPropertyChanged(nameof(TimeZone));
			}
		}
		private int _stringsHex;
		public int StringsHex
		{
			get { return _stringsHex; }
			set
			{
				_stringsHex = value;
				OnPropertyChanged(nameof(StringsHex));
			}
		}
		private int _segmentHex;
		public int SegmentHex
		{
			get { return _segmentHex; }
			set
			{
				_segmentHex = value;
				OnPropertyChanged(nameof(SegmentHex));
			}
		}
		private int _logEntryFlgs;//
		public int LogEntryFlgs
		{
			get { return _logEntryFlgs; }
			set
			{
				_logEntryFlgs = value;
				OnPropertyChanged(nameof(LogEntryFlgs));
			}
		}
		private int _logFileDelimCnt;
		public int LogFileDelimCnt
		{
			get { return _logFileDelimCnt; }
			set
			{
				_logFileDelimCnt = value;
				OnPropertyChanged(nameof(LogFileDelimCnt));
			}
		}
		private int _logFileDelim;
		public int LogFileDelim
		{
			get { return _logFileDelim; }
			set
			{
				_logFileDelim = value;
				OnPropertyChanged(nameof(LogFileDelim));
			}
		}
		private string _componentName;
		public string ComponentName
		{
			get { return _componentName; }
			set
			{
				_componentName = value;
				OnPropertyChanged(nameof(ComponentName));
			}
		}
		private int _sessionId;
		public int SessionId
		{
			get { return _sessionId; }
			set
			{
				_sessionId = value;
				OnPropertyChanged(nameof(SessionId));
			}
		}
		private int _processId;
		public int ProcessId
		{
			get { return _processId; }
			set
			{
				_processId = value;
				OnPropertyChanged(nameof(ProcessId));
			}
		}
		private int _threadId;
		public int ThreadId
		{
			get { return _threadId; }
			set
			{
				_threadId = value;
				OnPropertyChanged(nameof(ThreadId));
			}
		}
		private string _fileName;
		public string FileName
		{
			get { return _fileName; }
			set
			{
				_fileName = value;
				OnPropertyChanged(nameof(FileName));
			}
		}
		private string _versionInfo;
		public string VersionInfo
		{
			get { return _versionInfo; }
			set
			{
				_versionInfo = value;
				OnPropertyChanged(nameof(VersionInfo));
			}
		}
		private string _locale;
		public string Locale
		{
			get { return _locale; }
			set
			{
				_locale = value;
				OnPropertyChanged(nameof(Locale));
			}
		}

		public LogInfo() { }

		public bool ParseLogInfo(string input)
		{
			try
			{
				string[] props = input.Split(' ');
				LogEolXlate = props[0];
				DateTime sDate;
				DateTime.TryParseExact(string.Join(" ", props[1], props[2]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out sDate);
				StartDate = sDate;
				DateTime eDate;
				DateTime.TryParseExact(string.Join(" ", props[3], props[4]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out eDate);
				EndDate = eDate;
				TimeZone = props[5];
				StringsHex = int.Parse(props[6], System.Globalization.NumberStyles.HexNumber);
				SegmentHex = int.Parse(props[7], System.Globalization.NumberStyles.HexNumber);
				LogEntryFlgs = int.Parse(props[8], System.Globalization.NumberStyles.HexNumber);//
				LogFileDelimCnt = int.Parse(props[9]);
				LogFileDelim = int.Parse(props[10], System.Globalization.NumberStyles.HexNumber);
				ComponentName = props[11];
				SessionId = int.Parse(props[12]);
				ProcessId = int.Parse(props[13]);
				ThreadId = int.Parse(props[14]);
				FileName = props[15];
				VersionInfo = input.Substring(input.IndexOf(props[16]));// string.Join(" ", props[16], props[17]);
				Locale = props[props.Length-1];

				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}



		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] String info = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
	}
}
