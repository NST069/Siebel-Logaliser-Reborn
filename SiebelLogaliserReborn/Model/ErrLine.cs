using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SiebelLogaliserReborn.Model
{
	class ErrLine : INotifyPropertyChanged
	{
		private DateTime _timestamp;
		public DateTime Timestamp
		{
			get { return _timestamp; }
			set
			{
				_timestamp = value;
				OnPropertyChanged(nameof(Timestamp));
			}
		}
		private string _errCode;
		public string ErrCode
		{
			get { return _errCode; }
			set
			{
				_errCode = value;
				OnPropertyChanged(nameof(ErrCode));
			}
		}
		private string _errMsg;
		public string ErrMsg
		{
			get { return _errMsg; }
			set
			{
				_errMsg = value;
				OnPropertyChanged(nameof(ErrMsg));
			}
		}
		private long _line;
		public long Line
		{
			get { return _line; }
			set
			{
				_line = value;
				OnPropertyChanged(nameof(Line));
			}
		}
		public ErrLine(DateTime ts, string errCode, string errMsg, long Line)
		{
			this.Timestamp = ts;
			this.ErrCode = errCode;
			this.ErrMsg = errMsg;
			this.Line = Line;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] String info = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
	}
}
