using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SiebelLogaliserReborn.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private String _fileName;
        public String FileName {
            get { return _fileName; }
            set {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        private System.Data.DataSet _dsData;
        public System.Data.DataSet dsData {
            get { return _dsData; }
            set {
                _dsData = value;
                OnPropertyChanged(nameof(dsData));
               }
        }

        public ICommand OpenFile {
            get {
                return new Model.DelegateCommand((obj) => {
                    String fName = Model.FileHandler.OpenFile();
                    if (fName != "")
                    {
                        FileName = fName;
                    }
                });
            }
        }

        public ICommand StartAnalyzing {
            get
            {
                return new Model.DelegateCommand((obj) =>
                {
                    dsData = new Model.FileProcessor().ProcessFile(FileName);
                    //MessageBox.Show(dsData.ToString());
                }, (obj)=>_fileName!=null);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
