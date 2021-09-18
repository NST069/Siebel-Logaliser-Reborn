using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SiebelLogaliserReborn.Model
{
    class LogLine : INotifyPropertyChanged
	{
		private DateTime _prevTimestamp;
		public DateTime PrevTimestamp
		{
			get { return _prevTimestamp; }
			set
			{
				_prevTimestamp = value;
				OnPropertyChanged(nameof(PrevTimestamp));
			}
		}
		private DateTime _curTimestamp;
		public DateTime CurTimestamp
		{
			get { return _curTimestamp; }
			set
			{
				_curTimestamp = value;
				OnPropertyChanged(nameof(CurTimestamp));
			}
		}
		private double _timeDiff;
		public double TimeDiff
		{
			get { return _timeDiff; }
			set
			{
				_timeDiff = value;
				OnPropertyChanged(nameof(TimeDiff));
			}
		}
		private long _prevLine;
		public long PrevLine
		{
			get { return _prevLine; }
			set
			{
				_prevLine = value;
				OnPropertyChanged(nameof(PrevLine));
			}
		}
		public LogLine(DateTime prev_ts, DateTime cur_ts, double time_diff, long Line)
		{
			this.PrevTimestamp = prev_ts;
			this.CurTimestamp = cur_ts;
			this.TimeDiff = time_diff;
			this.PrevLine = Line;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] String info = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
	}
}
