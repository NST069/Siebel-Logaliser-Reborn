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

        private Model.FileProcessor fileProcessor;
        
        private Model.LogInfo _logInfo;
        public Model.LogInfo LogInfo {
            get { return _logInfo; }
            set {
                _logInfo = value;
                OnPropertyChanged(nameof(LogInfo));
            }
        } 
        public ICommand OpenFile {
            get {
                return new Model.DelegateCommand((obj) => {
                    String fName = Model.FileHandler.OpenFile();
                    if (fName != "")
                    {
                        FileName = fName;
                        fileProcessor = new Model.FileProcessor();
                        LogInfo = fileProcessor.GetLogInfo(fName);
                    }
                });
            }
        }

        public ICommand StartAnalyzing {
            get
            {
                return new Model.DelegateCommand((obj) =>
                {
                    dsData = fileProcessor.ProcessFile(FileName);
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
