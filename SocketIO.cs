using changeModelClient_Das.modul;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace changeModelClient_Das
{
    class SocketIO
    {
        private static Socket.IO.NET35.Socket _mainSocket { get; set; }
        private Logger logger;
        private IniFile _iniFile;
        private bool _firstInitial;
        private string _server;
        private string _port;

        public SocketIO()
        {
            logger = new Logger("SocketIO");
            _iniFile = new IniFile(@"Data\Config\Data.ini");
            _firstInitial = true;
        }
        private Socket.IO.NET35.IO.Options CreateOptions()
        {
            Socket.IO.NET35.IO.Options op = new Socket.IO.NET35.IO.Options();
            op.AutoConnect = true;
            op.Reconnection = true;
            op.ReconnectionAttempts = int.MaxValue;
            op.ReconnectionDelayMax = 5000;
            op.ReconnectionDelay = 500;
            return op;
        }

        /// <summary>
        /// kết nối với webSocket
        /// </summary>
        public void ConnectToSocket()
        {
            FirstInitial();
            Socket.IO.NET35.IO.Options options = CreateOptions();
            // kết nối
            _mainSocket = Socket.IO.NET35.IO.Socket($"ws://{_server}:{_port}/", options);
            RegisterByTopic();

            //// nhận dữ liệu từ topic "hi"
            _mainSocket.On("check_periods", (data) =>
            {
                //Console.WriteLine(data.ToString());
                JObject joResponse = JObject.Parse(data.ToString());
                JArray array = (JArray)joResponse["data"];
                // 1. check line, ngày, khoảng thời gian
                for (int i = 0; i < array.Count; i++)
                {
                    Console.WriteLine(array[i].ToString());
                    // kiểm tra xem dữ liệu line nhận từ server có trùng với line hiện tại của máy không?
                    if (array[i]["line"].ToString().ToString().Trim() == General.line.ToString().Trim())
                    {
                        string[] timeSub = array[i]["chung_time"].ToString().Replace('h', ' ').Split('-');
                        DateTime startTime = DateTime.ParseExact($"{DateTime.Parse(array[i]["date_time"].ToString()).ToString("yyyy-MM-dd")} {timeSub[0].Trim()}:00", "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture);
                        DateTime endTime = DateTime.ParseExact($"{DateTime.Parse(array[i]["date_time"].ToString()).ToString("yyyy-MM-dd")} {timeSub[1].Trim()}:00", "yyyy-MM-dd H:mm", CultureInfo.InvariantCulture);
                        string dateTime = DateTime.Parse(array[i]["date_time"].ToString()).ToString("yyyyMMdd");
                        string id = array[i]["id"].ToString();

                        // 2. list ngày + khoảng thời gian -> mang đi đếm
                        string pathProduct = $@"C:\SmartSEED\BACKUP\{dateTime}\D007\202.{General.line}.4.{General.indexLastModul}";

                        var sl = new List<SanLuongModel>();
                        sl.Add(ModulMouter.SanLuongBotTopPeriod(pathProduct, startTime, endTime, "_1_", id));
                        sl.Add(ModulMouter.SanLuongBotTopPeriod(pathProduct, startTime, endTime, "_2_", id));
                        string strJson = JsonConvert.SerializeObject(sl);
                        _mainSocket.Emit("sanluong", strJson);        // chuyển dữ liệu đi\                   
                    }
                }
            });
        }
        /// <summary>
        /// kết nối ban đầu -> máy chủ gửi dữ liệu kết nối lần đầu tới socket.io server
        /// </summary>
        public void RegisterByTopic()
        {
            // kết nối tới máy chủ thành công
            // đăng ký machine bằng địa chỉ IP máy
            _mainSocket.On(Socket.IO.NET35.Socket.EVENT_CONNECT, () =>
            {
                mainForm.staConnectSIO_.BackColor = System.Drawing.Color.Green;
                logger.WriteLog($"{DateTime.Now} : Kết nối thành công : {GetIPMachine()}!");
                _mainSocket.Emit("connected", GetIPMachine());        // chuyển dữ liệu đi
            });

            // mất kết nối tới máy chủ 
            _mainSocket.On(Socket.IO.NET35.Socket.EVENT_DISCONNECT, () =>
            {
                mainForm.staConnectSIO_.BackColor = System.Drawing.Color.Red;
                logger.WriteLog($"{DateTime.Now} : Đóng kết nối : {GetIPMachine()}");
            });
        }
        /// <summary>
        /// Nhận ip và line
        /// </summary>
        /// <returns></returns>
        public string GetIPMachine()
        {
            // get IP
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST     
            return Dns.GetHostByName(hostName).AddressList[0].ToString() + ";" + General.line.ToString().Trim();
        }
        /// <summary>
        /// khởi tạo lần đầu -> lấy dữ liệu từ socket.io
        /// </summary>
        private void FirstInitial()
        {
            if (_firstInitial)
            {
                _firstInitial = false;
                if (!_iniFile.KeyExists("Server", "SOCKETIO"))
                {
                    _iniFile.Write("Server", "127.0.0.1", "MYSQL");
                }
                if (!_iniFile.KeyExists("Port", "SOCKETIO"))
                {
                    _iniFile.Write("Port", "3006", "MYSQL");
                }

                _server = _iniFile.Read("Server", "SOCKETIO"); ;
                _port = _iniFile.Read("Port", "SOCKETIO"); ;
            }
        }
    }
}
