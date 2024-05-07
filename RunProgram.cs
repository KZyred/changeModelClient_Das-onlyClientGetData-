using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace changeModelClient_Das
{
    class RunProgram
    {
        public static void startProgram(string pgm)
        {
            try
            {
                if (pgm.Contains(".exe"))
                {
                    if (File.Exists(pgm))
                        Process.Start(pgm);
                }
            }
            catch { }
        }
        public static bool stopProgram(string pgm)
        {
            try
            {
                if (pgm.Contains(".exe"))
                {
                    pgm = pgm.Split('.')[0];
                }
                System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(pgm);
                if (process.Length > 0)
                {
                    foreach (Process proc in process)
                    {
                        proc.Kill();
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }
        public static bool checkProgram(string pgm)
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(pgm.Split('.')[0]);
            if (process.Length <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
