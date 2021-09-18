using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SiebelLogaliserReborn.Model
{
	public class SQLLine : INotifyPropertyChanged
	{
		private string _sqlId;
		public string SQLId
		{
			get { return _sqlId; }
			set
			{
				_sqlId = value;
				OnPropertyChanged(nameof(SQLId));
			}
		}
		private string _execTime;
		public string ExecTime {
			get { return _execTime; }
			set {
				_execTime = value;
				OnPropertyChanged(nameof(ExecTime));
			}
		}
		private string _execNo;
		public string ExecNo {
			get { return _execNo; }
			set {
				_execNo = value;
				OnPropertyChanged(nameof(ExecNo));
			}
		}
		private string _totalExecTime;
		public string TotalExecTime {
			get { return _totalExecTime; }
			set {
				_totalExecTime = value;
				OnPropertyChanged(nameof(TotalExecTime));
			}
		}
		private string _sql;
		public string SQL {
			get { return _sql; }
			set {
				_sql = value;
				OnPropertyChanged(nameof(SQL));
			}
		}
		private string _bindVar;
		public string BindVar {
			get { return _bindVar; }
            set {
				_bindVar = value;
				OnPropertyChanged(nameof(BindVar));
			}
		}
		private long _line;
		public long Line {
			get { return _line; }
			set {
				_line = value;
				OnPropertyChanged(nameof(Line));
			}
		}
		public SQLLine(string SQLId, string ExecTime, string ExecNo, string TotalExecTime, string SQL, string BindVar, long Line)
		{
			this.SQLId = SQLId;
			this.ExecTime = ExecTime;
			this.ExecNo = ExecNo;
			this.TotalExecTime = TotalExecTime;
			this.SQL = SQL;
			this.BindVar = BindVar;
			this.Line = Line;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] String info = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
	}
}
