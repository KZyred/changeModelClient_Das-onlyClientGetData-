using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace changeModelClient_Das
{
    class Logger
    {
        private DirectoryInfo _dir;
        private string _addressFileLog;
        private string _machine;
        public Logger(string machine)
        {
            _dir = new DirectoryInfo(@".");
            _addressFileLog = "";
            _machine = machine;
        }

        public void WriteLog(string message)
        {
            lock (this)
            {
                _addressFileLog = _dir.FullName + $@"\Data\LogFile\Log_{_machine}_{DateTime.Now.ToString("yyyyMMdd")}.txt";
                FileInfo LogFile = new FileInfo(_addressFileLog);
                if (!LogFile.Exists)
                {
                    FileStream fs = LogFile.Create();
                    fs.Close();
                }
                if (LogFile.Exists)
                {
                    using (StreamWriter writer = new StreamWriter(_addressFileLog, true))
                    {
                        writer.WriteLine($"{DateTime.Now} : {message}");
                    }
                }
            }
        }
    }
}
