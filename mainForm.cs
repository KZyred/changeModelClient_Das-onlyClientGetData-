using changeModelClient_Das.modul;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace changeModelClient_Das
{
    public partial class mainForm : Form
    {
        public static System.Windows.Forms.Label staConnectSIO_;
        //
        private IniFile _iniFile;
        //
        private SocketIO socketIO;
        private Thread _thread_ReadInit;
        private Logger logger;
        public static bool lockApp;

        private static Socket.IO.NET35.Socket _mainSocket { get; set; }
        public mainForm()
        {
            InitializeComponent();
            // thay đổi thư mục hiện tại về thư mục có file .exe thực thi (quan trọng khi startup with win)
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(exeDir);
            //
            socketIO = new SocketIO();
            // khai báo đèn trạng thái các kết nối
            staConnectSIO_ = staConnectSIO;
            logger = new Logger("main");

            _iniFile = new IniFile(@"Data.ini");        // ghi log thời gian cho chương trình

            // update time run chương trình
            _iniFile.Write("DATETIME", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", "AUTORUN");
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            logger.WriteLog($"{DateTime.Now} : Bật phần mềm");
            AutoStartWithComputer();

            socketIO.ConnectToSocket();
            // chuyển dữ liệu lên Server lần đầu (dành cho tiện khi khởi động + startup)
            ModifyLineAndMachine(Color.Yellow);

            lockApp = false;

            RunProgram.startProgram("AutoRunModelChange_DAS.exe");
        }
        private async void FlashStateLabel(System.Windows.Forms.Label label, System.Drawing.Color color1, System.Drawing.Color color2)
        {
            await Task.Delay(500);
            label.BackColor = label.BackColor == color1 ? color2 : color1;
        }

        private void Thread_ReadInit()  // đa luồng vòng lặp đọc log file
        {
            Invoke((ThreadStart)delegate   //tránh xung đột thread
            {
                textBoxLog.Text += $"Đang khởi tạo dữ liệu\r\n";
            });
            try
            {
                // duyệt qua một lượt các folder nhằm tìm line name
                DirectoryInfo[] dateFolders = new DirectoryInfo(@"C:\SmartSEED\BACKUP").GetDirectories()
                                                    .Where(d => d.Name.Length == 8)
                                                    .OrderByDescending(d => d.Name)
                                                    .ToArray();  // sắp xếp theo thứ tự giảm dần của ngày
                if (dateFolders.Length <= 0)
                {
                    return;
                }
                for (int i = 0; i < dateFolders.Length; i++)
                {
                    // bắt đầu duyệt qua các ngày C:\SmartSEED\BACKUP \ 20230821
                    string dateFolder;
                    dateFolder = dateFolders[i].FullName;
                    if (!Directory.Exists(dateFolder))
                    {
                        continue;
                    }
                    Invoke((ThreadStart)delegate   //tránh xung đột thread
                    {
                        textBoxLog.Text += $"{dateFolder}\r\n";
                    });
                    GetLineName(dateFolder);
                }
                //////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                Invoke((ThreadStart)delegate   //tránh xung đột thread
                {
                    textBoxLog.Text += $"{DateTime.Now} : {ex.Message}\r\n";
                    logger.WriteLog($"{DateTime.Now} : {ex.Message}");
                });
            }
        }
        private void textBoxLog_TextChanged(object sender, EventArgs e)
        {
            if (textBoxLog.Visible)
            {
                textBoxLog.SelectionStart = textBoxLog.TextLength;
                textBoxLog.ScrollToCaret();
            }
        }
        /// <summary>
        /// 1. Lấy dữ liệu line hiện tại
        /// 2. Tạo bảng máy maoi, paoi, mouter  trong csdl
        /// </summary>
        /// <param name="dateFolder">Dữ liệu ngày hiện tại</param>
        private void GetLineName(string dateFolder)
        {
            if (General.line == "" || General.line == null)
            {
                string machineGroupFolder;
                machineGroupFolder = dateFolder + @"\D007"; // Mouter
                if (!Directory.Exists(machineGroupFolder))
                {
                    return;
                }
                DirectoryInfo[] machineFolders = new DirectoryInfo(machineGroupFolder).GetDirectories()
                                    .Where(mF => mF.Name.Count(x => x == '.') == 3)
                                    .ToArray();
                if (machineFolders.Length <= 0)
                {
                    return;
                }
                // lấy tên line
                string[] arrMachineFolder = machineFolders[0].Name.Split('.');
                General.line = arrMachineFolder[1];   // 202.{24}.4.1 

                // lấy modul lớn nhất
                int maxModul = 1;
                for (int i = 0; i < machineFolders.Length; i++)
                {
                    arrMachineFolder = machineFolders[i].Name.Split('.');
                    int modul = Int32.Parse(arrMachineFolder[3]);   // 202.24.4.{1} 
                    if (modul > maxModul)
                    {
                        maxModul = modul;
                    }
                }
                General.indexLastModul = maxModul.ToString();
                // hiển thị linename lên form
                Invoke((ThreadStart)delegate   //tránh xung đột thread
                {
                    lb_lineName.Text = General.line;
                });
            }
        }
        private void tmReadNewLog_Tick(object sender, EventArgs e)
        {
            try
            {
                // update time run chương trình -> cho phần mềm autorun detect -> từ đó khởi động lại ứng dụng
                _iniFile.Write("DATETIME", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", "AUTORUN");
            }
            catch (Exception ex)
            {
                textBoxLog.Text += $"{DateTime.Now} : {ex.Message}\r\n";
                logger.WriteLog($"{DateTime.Now} : {ex.Message}");
            }
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var frm_pw = new passwordForm();
            frm_pw.ShowDialog();
            if (lockApp)
            {
                tmReadNewLog.Stop();
                logger.WriteLog($"{DateTime.Now} : Tắt phần mềm");
            }
            else
            {
                e.Cancel = true;
            }

            //string message = "Hệ thống Model Change yêu cầu không tắt ứng dụng!!! \nBạn có muốn tắt ứng dụng không?";
            //string title = "Close Windows";
            //DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //if (result == DialogResult.No)
            //{
            //    e.Cancel = true;
            //}
            //else
            //{
            //    tmReadNewLog.Stop();
            //    logger.WriteLog($"{DateTime.Now} : Tắt phần mềm");
            //}
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            notifyIcon.ShowBalloonTip(10, "Model Change Project (DAS)", "Show", ToolTipIcon.Info);
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void mainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(10);
            }
        }
        private void AutoStartWithComputer()
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            reg.SetValue("Model Change Client", "\"" + Application.ExecutablePath.ToString() + "\"");
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            ModifyLineAndMachine(Color.Green);
        }



        private void ModifyLineAndMachine(Color color)
        {
            //3. Dừng timer tick, reset lại dữ liệu các máy
            tmReadNewLog.Stop();
            //4. tạo thread và chạy
            _thread_ReadInit = new Thread(() =>
            {
                try
                {
                    // 
                    Thread_ReadInit();
                }
                finally
                {
                    Invoke((ThreadStart)delegate   //tránh xung đột thread
                    {
                        textBoxLog.Text += $"{DateTime.Now} : Kết thúc thread\r\n";
                        textBoxLog.Text += $"Khởi tạo dữ liệu thành công\r\n";
                        btn_reset.BackColor = color;
                        // tick
                        tmReadNewLog.Start();
                    });
                }
            });
            _thread_ReadInit.IsBackground = true;
            _thread_ReadInit.Start();
        }
    }
}
