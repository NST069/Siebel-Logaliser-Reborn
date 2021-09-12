using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

        private DataSet _dsData;
        public DataSet dsData {
            /*get { return _dsData; }
            set {
                _dsData = value;
                OnPropertyChanged(nameof(dsData));
               }*/
            get { return fileProcessor?.dsSQLSet; }
            set {
                fileProcessor.dsSQLSet = value;
                OnPropertyChanged(nameof(dsData));
            }
        }

        private Model.FileProcessor fileProcessor;
        //public Model.FileProcessor fileProcessor {
        //    get { return _fileProcessor; }
        //    set {
        //        _fileProcessor = value;
        //        OnPropertyChanged(nameof(fileProcessor));
        //    }
        //}
        
        private Model.LogInfo _logInfo;
        public Model.LogInfo LogInfo {
            get { return _logInfo; }
            set {
                _logInfo = value;
                OnPropertyChanged(nameof(LogInfo));
            }
        }

        private DateTime _elapsedTime;
        public DateTime ElapsedTime {
            get { return _elapsedTime; }
            set {
                _elapsedTime = value;
                OnPropertyChanged(nameof(ElapsedTime));
                OnPropertyChanged(nameof(ElapsedTimeDisp));
            }
        }
        public string ElapsedTimeDisp {
            get { return ElapsedTime.ToString("mm:ss"); }
        }

        private long _fileSizeStr;
        public long FileSizeStr {
            get { return _fileSizeStr; }
            set {
                _fileSizeStr = value;
                OnPropertyChanged(nameof(FileSizeStr));
            }
        }

        private long _curPosition;
        public long CurPosition
        {/*
            get { return _curPosition; }
            set {
                _curPosition = value;
                OnPropertyChanged(nameof(CurPosition));
            }*/
            get { return (fileProcessor!=null)?fileProcessor.lLineNo:0; }
            set {
                fileProcessor.lLineNo = value;
                OnPropertyChanged(nameof(CurPosition));
            }
        }
        

        public ICommand OpenFile
        {
            get
            {
                return new Model.DelegateCommand((obj) =>
                {
                    String fName = Model.FileHandler.OpenFile();
                    if (fName != "")
                    {
                        FileName = fName;
                        fileProcessor = new Model.FileProcessor();
                        LogInfo = fileProcessor.GetLogInfo(fName);
                        FileSizeStr = 999;
                        CurPosition = 0;
                        ThreadPool.QueueUserWorkItem(j =>
                        {
                            FileSizeStr = Model.FileHandler.TotalLines(fName);
                        });
                    }
                });
            }
        }

        public ICommand StartAnalyzing
        {
            get
            {
                return new Model.DelegateCommand((obj) =>
                {
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        dsData = null;
                        DateTime start = DateTime.Now;
                        //FileSizeStr = 100;
                        //CurPosition = 50;
                        bool isExecuting = true;
                        ThreadPool.QueueUserWorkItem(j =>
                        {
                            while (isExecuting)
                            {
                                DateTime now = DateTime.Now;
                                TimeSpan nts = TimeSpan.FromSeconds((now - start).TotalSeconds);
                                ElapsedTime = DateTime.Today.Add(nts);
                                CurPosition = fileProcessor.lLineNo;
                                Thread.Sleep(100);
                            }
                        }, isExecuting);
                        dsData = fileProcessor.ProcessFile(FileName);
                        //CurPosition = 100;
                        isExecuting = false;
                        DateTime end = DateTime.Now;
                        TimeSpan ts = TimeSpan.FromSeconds((end - start).TotalSeconds);
                        ElapsedTime = DateTime.Today.Add(ts);
                    });


                }, (obj) => _fileName != null);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
