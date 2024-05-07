using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace changeModelClient_Das
{
    class MySQL
    {
        private IniFile _iniFile;
        private MySqlConnection _con;
        private DataTable _dt;
        private MySqlDataAdapter _da;
        private MySqlCommand _cmd;
        private string _connectStr;
        private bool _firstInitial;
        private string _queryCmd;

        // nạp vào câu truy vấn
        public string queryCmd
        {
            set
            {
                _queryCmd = value;
            }
        }

        public MySQL()
        {
            _con = new MySqlConnection();
            _dt = new DataTable();
            _da = new MySqlDataAdapter();
            //_cmd = new MySqlCommand();
            _firstInitial = true;
            _iniFile = new IniFile(@"Data\Config\Data.ini");
        }
        /**
         * Khởi tạo kết nối (dữ liệu kết nối lấy từ hàm FirstInitial();)
         */
        public void Initial()
        {
            try
            {
                FirstInitial();
                _con = new MySqlConnection(_connectStr);
                _con.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Initial MySql lỗi: " + ex.Message);
            }
        }
        /**
         * lấy dữ liệu để kết nối tới MySQL
         * khởi tạo lần đầu
         */
        private void FirstInitial()
        {
            if (_firstInitial)
            {
                _firstInitial = false;
                if (!_iniFile.KeyExists("Server", "MYSQL"))
                {
                    _iniFile.Write("Server", "127.0.0.1", "MYSQL");
                }
                if (!_iniFile.KeyExists("Database", "MYSQL"))
                {
                    _iniFile.Write("Database", "testing", "MYSQL");
                }
                if (!_iniFile.KeyExists("UserName", "MYSQL"))
                {
                    _iniFile.Write("UserName", "admin", "MYSQL");
                }
                if (!_iniFile.KeyExists("Password", "MYSQL"))
                {
                    _iniFile.Write("Password", "abc13579", "MYSQL");
                }
                if (!_iniFile.KeyExists("Port", "MYSQL"))
                {
                    _iniFile.Write("Port", "3306", "MYSQL");
                }

                string Server;
                string Database;
                string DBUser;
                string DBPass;
                string Port;
                // dành cho testing ở máy local
                Server = _iniFile.Read("Server", "MYSQL");
                Database = _iniFile.Read("Database", "MYSQL");
                DBUser = _iniFile.Read("UserName", "MYSQL");
                DBPass = _iniFile.Read("Password", "MYSQL");
                Port = _iniFile.Read("Port", "MYSQL");
                _connectStr = "Persist Security Info=False;Server=" + Server + ";Port=" + Port + ";Database=" + Database + ";UID=" + DBUser + ";password=" + DBPass + ";charset=utf8; Connection Timeout=30";
            }
        }
        /**
         * Đóng kết nối MySQL
         */
        public void Close()
        {
            try
            {
                _con.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Close MySql lỗi: " + ex.Message);
            }
        }
        /**
         * kiểm tra kết nối MySQL
         */
        private bool IsConnected()
        {
            bool check = false;
            try
            {
                Initial();
                if (_con.State.ToString() == "Open")
                {
                    check = true;
                }
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Check connect MySql lỗi: " + ex.Message);
            }
            return check;
        }
        /**
         * kiểm tra kết nối MySQL
         */
        public Boolean CheckConnected()
        {
            Boolean isConnected = IsConnected();
            if (isConnected)
            {
                mainForm.staConnectMySQL_.BackColor = System.Drawing.Color.Green;
            }
            else
            {
                mainForm.staConnectMySQL_.BackColor = System.Drawing.Color.Red;
            }
            return isConnected;
        }
        /**
         * kiểm tra kết nối MySQL
         */
        private void CheckConnectedToLabel(string status)
        {
            if (status == "Open")
            {
                mainForm.staConnectMySQL_.BackColor = System.Drawing.Color.Green;
            }
            else
            {
                mainForm.staConnectMySQL_.BackColor = System.Drawing.Color.Red;
            }
        }
        /**
         * chuyển dữ liệu lấy từ SELECT hoặc StoredProduced 
         * sang JSON
         */
        private String SqlDataToJson(MySqlDataReader dataReader)
        {
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(dataTable);
            return JSONString;
        }
        /**
         * Cách sử dụng: 
         *  mySql.queryCmd = "SELECT * FROM aaa where id = 5";
         *  hoặc mySql.queryCmd = inputTextBox.Text;
            resultTextBox.Text = mySql.Query();   
         * Trả ra: 
         *      "Query Done" - thành công
         *      "Query MySql lỗi:..." - lỗi
         */
        public String Query()
        {
            string result = "Query Done";
            try
            {
                Initial();
                _cmd = new MySqlCommand(_queryCmd, _con);
                _cmd.CommandTimeout = 60;
                CheckConnectedToLabel(_con.State.ToString());
                using (MySqlDataReader reader = _cmd.ExecuteReader())
                {
                    if (reader.HasRows)    // check có dữ liệu gửi về
                    {
                        result = SqlDataToJson(reader);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                result = "Error " + ex.Number + " has occurred: " + ex.Message;
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }
            finally
            {
                _con.Close();
                _con.Dispose();
                _con = null;
            }
            return result;
        }

        /**
         * Phụ vụ cho truy vấn có 
         * Cách sử dụng: 
         *  IDictionary<string, object> numberNames = new Dictionary<string, object>();
            numberNames.Add("@id", 5);
            numberNames.Add("@add", "BN");
            ...
            mySql.queryCmd = "SELECT * FROM aaa where address = @add";
            textBoxResultSql.Text = mySql.Query(numberNames);   
         * Trả ra: 
         *      "Query Done" - thành công
         *      "Query MySql lỗi:..." - lỗi
         */
        public String Query(IDictionary<string, object> queryParams)
        {
            string result = "Query Done";
            //System.Diagnostics.Debug.WriteLine((prams == null || prams.Count < 1));   // check empty Dictionary
            try
            {
                Initial();
                _cmd = new MySqlCommand(_queryCmd, _con);
                _cmd.CommandTimeout = 60;
                foreach (KeyValuePair<string, object> queryParam in queryParams)
                {
                    _cmd.Parameters.AddWithValue(queryParam.Key, queryParam.Value);
                }
                CheckConnectedToLabel(_con.State.ToString());
                using (MySqlDataReader reader = _cmd.ExecuteReader())
                {
                    if (reader.HasRows)    // check có dữ liệu gửi về
                    {
                        result = SqlDataToJson(reader);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                result = "Error " + ex.Number + " has occurred: " + ex.Message;
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }
            finally
            {
                _con.Close();
                _con.Dispose();
                _con = null;
            }
            return result;
        }


        /**
         * Phục vụ cho truy vấn có Stored Procedure 
         * Cách sử dụng: 
         *  mySql.queryCmd = "getUsers";
            IDictionary<string, object> numberNames = new Dictionary<string, object>();
            numberNames.Add("@id_in", 1);
            textBoxResultSql.Text = mySql.Query_StoredProcedure(numberNames);
         * Trả ra: 
         *      "Query Done" - thành công
         *      "Query MySql lỗi:..." - lỗi
         */

        public string Query_StoredProcedure(IDictionary<string, object> queryParams)
        {
            string result = "Query Done";
            try
            {
                Initial();
                _cmd = new MySqlCommand(_queryCmd, _con);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 60;

                foreach (KeyValuePair<string, object> queryParam in queryParams)
                {
                    _cmd.Parameters.AddWithValue(queryParam.Key, queryParam.Value);
                    _cmd.Parameters[queryParam.Key].Direction = ParameterDirection.Input;
                }
                CheckConnectedToLabel(_con.State.ToString());
                using (MySqlDataReader reader = _cmd.ExecuteReader())
                {
                    if (reader.HasRows)    // check có dữ liệu gửi về
                    {
                        result = SqlDataToJson(reader);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                result = "Error " + ex.Number + " has occurred: " + ex.Message;
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }
            finally
            {
                _con.Close();
                _con.Dispose();
                _con = null;
            }
            return result;
        }
    }
}
