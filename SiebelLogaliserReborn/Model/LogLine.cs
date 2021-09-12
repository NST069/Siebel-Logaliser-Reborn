using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiebelLogaliserReborn.Model
{
    static class LogLine
    {


        public static string[] TryParseLine(string Line) {
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

        /*
        public static logLine? TryParseLine(string Line)
        {
            try
            {
                string[] logLn = Line.Split('\t');
                logLine l = new logLine();
                l._evtTypeAlias = logLn[0];
                l._evtSubtype = logLn[1];
                l._logLevel=int.Parse(logLn[2]);
                l._sarmId = logLn[3];
                DateTime.TryParseExact(logLn[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out l._timestamp);
                l._logMessage = logLn[5];

                return l;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public struct logLine {
            public string _evtTypeAlias;
            public string _evtSubtype;
            public int _logLevel;
            public string _sarmId;
            public DateTime _timestamp;
            public string _logMessage;
            public string _query;//if caught SQL tags

            public void AddStringToQuery(string str) {
                _query += str.TrimEnd() + "\n";
            }

            public void AddBindVars(string pos, string var) {
                
            }
        }
        */
    }
}
