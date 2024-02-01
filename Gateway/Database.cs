using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using static Gateway.Form1;

namespace Gateway
{
    class Database
    {
        public static string ServerName = "10.131.156.54"; // CNS30NBO21097   10.131.156.54
        public static string Account = "sa";
        public static string Password = "Password12";
        //public static string MyMasterDatabase = "NISO Data";
        public static string MyNewDb = "Engineering"; // 新数据库的名称
        public static string tableName= "IOT_PRESSURE";

        /// <summary>
        ///  创建数据库
        /// </summary>
        public static void DbCreateSever()
        {
            
            string query = $"CREATE DATABASE {MyNewDb};"; // 创建数据库的 SQL 命令
            if(!IsDatabaseExists(MyNewDb))
            {
                using (SqlConnection connect = new SqlConnection($"Server={ServerName};uid={Account};pwd={Password}"))
                {
                    connect.Open();
                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.ExecuteNonQuery();
                        Form1.frm1.output($"创建数据库{MyNewDb}成功");
                    }
                }
            }
            Form1.frm1.output($"数据库{MyNewDb}已存在");

        }

        /// <summary>
        ///  create new table
        /// </summary>
        public static void DbCreateTableSever()
        {
            tableName = "DAP82FTTestData";
            string columns = @"ID Int identity(1,1)Primary Key,
                           DevEUI NVARCHAR(50),
                           [Time] DATETIME,
                           Payload NVARCHAR(200),
                           [Payload Size] NVARCHAR(50),
                           [Type] NVARCHAR(50),
                           fCnt NVARCHAR(50),
                           DataRate NVARCHAR(50),
                           BandWidth NVARCHAR(50),
                           SpreadFactor NVARCHAR(50),
                           [TimeStamp] NVARCHAR(50),
                           fport NVARCHAR(50),
                           Modulation NVARCHAR(50),
                           BitRate NVARCHAR(50),
                           SNR NVARCHAR(50),
                           RSSI NVARCHAR(50),
                           [Battery(%)] NVARCHAR(50),
                           [Temperature(℃)] NVARCHAR(50),
                           Pressure NVARCHAR(50),                                                    
                           PC NVARCHAR(50)";// 列定义

            string query = $"CREATE TABLE {tableName} ({columns});"; // 创建表格的 SQL 命令
            using (SqlConnection connection = new SqlConnection($"Server={ServerName};Database={MyNewDb};uid={Account};pwd={Password}"))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                    Form1.frm1.output($"创建数据表{tableName}成功");

                }
            }

        }

        /// <summary>
        /// 插入sql语句
        /// </summary>
        /// <param name="ServerName"></param>
        /// <param name="DbName"></param>
        /// <param name="Account"></param>
        /// <param name="Password"></param>
        /// <param name="Devices"></param>
        public static void DbInsertSever(List<Gateway_Milesight.DeviceInfor> Devices)
        {
            using (SqlConnection connection = new SqlConnection($"Server={ServerName};Database={MyNewDb};uid={Account};pwd={Password}"))
            {
                connection.Open();
                string fields = "DevEUI,[Time],Payload,[Payload Size],[Type],fCnt,DataRate,BandWidth,SpreadFactor,[TimeStamp],fport,Modulation,BitRate,SNR,RSSI,[Battery(%)],[Temperature(℃)],Pressure,PC";
                foreach(Gateway_Milesight.DeviceInfor device in Devices)
                {
                    string value = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}'",device.DevEUI
                        ,device.time,device.payloadHex,device.size,device.type,device.fCnt,device.Datarate,device.Bandwidth,device.Spreadfactor,device.Timestamp,device.fPort,device.modulation
                        ,device.Bitrate,device.SNR,device.RSSI,device.Battery,device.Temperature,device.Pressure,Environment.MachineName);
                    string sql = string.Format("insert into {0} ({1}) values ({2})", tableName, fields, value);
                    SqlCommand cmd = new SqlCommand(sql, connection);
                   int rtn = cmd.ExecuteNonQuery();
                }
            }
        }

        // 检查数据库是否存在
        public static bool IsDatabaseExists(string databaseName)
        {
            using (SqlConnection connection = new SqlConnection($"Server={ServerName};uid={Account};pwd={Password}"))
            {
                connection.Open();
                string sql = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

    }
}
