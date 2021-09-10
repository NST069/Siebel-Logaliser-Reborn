using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiebelLogaliserReborn.Model
{
    static class FileHandler
    {
        public static String OpenFile()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Log Files|*.log";
            if (ofd.ShowDialog() == true) return ofd.FileName;
            return "";
        }
    }
}
