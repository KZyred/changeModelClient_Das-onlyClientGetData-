using changeModelClient_Das.modul;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace changeModelClient_Das
{
    public class SanLuongModel
    {
        public string face { get; set; }
        public int sanLuong { get; set; }
        public string id { get; set; }
    }
    class ModulMouter
    {
        public ModulMouter()
        {
        }
        public static SanLuongModel SanLuongBotTopPeriod(string pathProduct, DateTime startTime, DateTime endTime, string subFace, string id)
        {
            if (!Directory.Exists(pathProduct))
            {
                return null;
            }
            // đếm số file
            FileInfo[] txtFiles = new DirectoryInfo(pathProduct).GetFiles("*.txt", SearchOption.TopDirectoryOnly)
            .Where(d => (d.LastWriteTime < endTime) && (d.LastWriteTime > startTime) && (d.Name.Contains(subFace))).ToArray();
            if (txtFiles.Length < 1)
            {
                return null;
            }
            if (!File.Exists(txtFiles[0].FullName))
            {
                return null;
            }
            // txt file
            SanLuongModel sanluong = new SanLuongModel();
            IniFile iniFile = new IniFile(txtFiles[0].FullName);
            string programName = iniFile.Read("Program Name", "General");
            string[] arrprogramName = programName.Split('_');
            sanluong.face = arrprogramName[arrprogramName.Length - 1].Substring(0, 1);
            sanluong.sanLuong = txtFiles.Length;
            sanluong.id = id;
            return sanluong;
        }
    }
}
