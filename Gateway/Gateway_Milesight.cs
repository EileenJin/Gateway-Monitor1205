using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using log4net;
using static Gateway.Form1;


namespace Gateway
{
    public class Gateway_Milesight
    {
        ////// <summary>
        ////// 网关登录账户名
        ////// </summary>
        public static string Username = "admin";
        ////// <summary>
        ////// 网关登录密码
        ////// </summary>
        public static string Password = "mylora";
        // <summary>
        // 网关IP地址
        // </summary>
        public static string IP = "192.168.23.150";

        public static bool LoginState = false;
        public static string fileName = "";


        // <summary>
        // 网关设备格式
        // </summary>
        public class cGatewayDevice
        {
            public string DevEUI;////终端DevEUI
            public string Name;////此监听服务的名字
            public string ApplicationID;////网关App ID
            public string ApplicationName;////网关App名字
            public string ProfileID;////网关Profile ID
            public string ProfileName;////网关Profile 名字
            public string Appkey;////终端App key
            public string lastSeenAtTime;
            public bool active;////终端是否已经激活

        }
        /// <summary>
        /// 
        /// </summary>
       

        public class DeviceInfor
        {
            public string DevEUI;////终端DevEUI
            public string time;////数据发送时间
            public string type;////数据类型
            public string fCnt;////网关App名字
            public string payloadHex;////
            public string Battery;////终端电池电量
            public string Temperature;////温度   
            public string SIG_RMS;
            public string SIG_RMS_X;
            public string SIG_RMS_Y;
            public string SIG_RMS_Z;
            public string Datarate;
            public string Bandwidth; 
            public string Spreadfactor;
            public string Pressure;
            public string fPort;
            public string Timestamp;
            public string modulation;
            public string Bitrate;
            public string coderate;
            public string SNR;
            public string RSSI;
            public int size;
        }
      
        //-------1.重复登录第二次登录不了 获取Islogin，2.获取网关的区域版本,cgi

        // <summary>
        // 登录后返回的授权码
        // </summary>
        private string authorization = string.Empty;

        // <summary>
        // 密码加密Key
        // </summary>
        private readonly string Self_Key = "1111111111111111";
        // <summary>
        // 密码加密IV
        // </summary>
        private readonly string Self_IV = "2222222222222222";

        private string my_Cookies = string.Empty;
        private string LoginAuth = string.Empty;
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        private HttpWebRequest req;
        private HttpWebResponse aa;

        public void CreateReq(string url)
        {
            req = (HttpWebRequest)WebRequest.Create(url);
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Host = IP + @":8081";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";          
        }

        // <summary>
        // 获取登录密码
        // </summary>
        // <returns></returns>
        public bool PasswordValidate()
        {
            string PostData = "{\"id\":\"1\",\"execute\":1,\"core\":\"user\",\"function\":\"login\",\"values\":[{\"username\":\"admin\",\"password\":\"" + AesEncrypt(Password) + "\"}]}";           
            byte[] bytearray = Encoding.UTF8.GetBytes(PostData);
            string url = @"http://" + IP+ @"/cgi";
            CreateReq(url);         
            req.Method = "POST";
            req.ContentType = @"application /x-www-form-urlencoded; charset=UTF-8";
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            req.ContentLength = bytearray.Length;
            req.Timeout = 5000;
            using (Stream reqstream = req.GetRequestStream())
            {
                reqstream.Write(bytearray, 0, bytearray.Length);
                reqstream.Close();
            }
            aa = req.GetResponse() as HttpWebResponse;
            Stream stream = aa.GetResponseStream();
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();
                JObject jo = JObject.Parse(res);
                if (jo.Property("status").Value.ToString() == "0")
                {
                    string Cookie = aa.Headers["Set-Cookie"].ToString();   //Set-Cookie = "loginname=admin; path=/"
                    string[] cookies = Cookie.Replace(@"; path=//", "").Split(','); //删除 Set-Cookie中的"Path=/"
                    my_Cookies = "language=zh-cn; " + string.Join(" ;", cookies); ////cookies.Join(';'); //// cookies.Join(cookies, new string[] { " ;"});    
                    my_Cookies = string.Join(" ;", cookies); ////cookies.Join(';'); //// cookies.Join(cookies, new string[] { " ;"});  
                    return true;
                }
            }
            return false;
        }

        // <summary>
        // 获取登录的密码
        // </summary>
        // <returns></returns>
        public string GetSecondPassword()
        {
            string url = @"http://" + IP + @"/";
            CreateReq(url);
            req.Method = "GET";          
            req.Timeout = 5000;
            try
            {
                aa = req.GetResponse() as HttpWebResponse;
                Stream stream = aa.GetResponseStream();
                string res = string.Empty;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    res = reader.ReadToEnd();   //表示取得网页的源码
                    Regex reg = new Regex("{username:\"" + Username + "\",password:\".*?\"}");
                    if (reg.IsMatch(res))
                    {
                        string ret = reg.Match(res).ToString().Insert(1, "\"").Insert(10, "\"").Insert(15 + Username.Length, "\"").Insert(15 + 9 + Username.Length, "\"");
                        return ret;
                    }
                    else
                    {

                    }
                }
            }
            catch(Exception ex)
            {
               // Logger.Debug(DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + $"create getsecondpassword failed");
            }
          
           return "";
        }

        public bool Login()
        {            
            //"{\"username\":\"admin\",\"password\":\"NicJjG18XOV3U1efQyo8AQ==\"}";
            string PostData = GetSecondPassword();   ////获取网站登录账号以及密码
            byte[] bytearray = Encoding.UTF8.GetBytes(PostData);       
            string url = @"http://" + IP + @":8081/api/internal/login";
            CreateReq(url);
            req.Method = "POST";           
            req.ContentType = @"application//x-www-form-urlencoded; charset=UTF-8";
            req.ContentLength = bytearray.Length;            
            req.Timeout = 10000;
            try
            {
                using (Stream reqstream = req.GetRequestStream())
                {
                    reqstream.Write(bytearray, 0, bytearray.Length);
                    reqstream.Close();
                 }            
                aa = req.GetResponse() as HttpWebResponse;
                _log.Info("Login request create success");
                Stream stream = aa.GetResponseStream();
                string res = string.Empty;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    res = reader.ReadToEnd();
                    if (res.Contains("jwt"))
                    {
                        JObject jo = JObject.Parse(res);
                        authorization = jo.Property("jwt").Value.ToString();
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                _log.Info("Login request create failed");
            }
            return false;

        }


        /// <summary>
        /// 获取设备payload
        /// </summary>
        /// <returns></returns>
        public DeviceInfor[] Read_UplinkPayload(string fileName,bool SQLFlag,TestType testtype)
        {
            string url = @"http://" + IP + @":8081/api/urpackets?offset=0&limit=10&organizationID=1";
            CreateReq(url);
            req.Method = "GET";
            req.Headers.Add("authorization", "Bearer " + authorization);
            req.Timeout = 5000;
            HttpWebResponse aa = req.GetResponse() as HttpWebResponse;
            Stream stream = aa.GetResponseStream();
            _log.Info("Read_UplinkPayload request create success");
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();
                if (res.Contains("devEUI"))
                {
                    List<DeviceInfor> Devices = new List<DeviceInfor>() { };
                    Regex re = new Regex("{\"frequency\":.*?}");
                    MatchCollection match = re.Matches(res);
                    for (int i = 0; i < match.Count; i++)
                    {
                        JObject jo = JObject.Parse(match[i].Value);
                        DeviceInfor Device = new DeviceInfor();
                        Device.DevEUI = jo.Property("devEUI").Value.ToString();
                        Device.fCnt = jo.Property("fCnt").Value.ToString();
                        Device.payloadHex = jo.Property("payloadHex").Value.ToString();
                        Device.time = jo.Property("time").Value.ToString();
                        Device.type = jo.Property("type").Value.ToString();
                        Device.Datarate = jo.Property("dataRate").Value.ToString();
                        Device.Spreadfactor = jo.Property("spreadFactor").Value.ToString();
                        Device.Bandwidth = jo.Property("bandwidth").Value.ToString();
                        Device.Timestamp = jo.Property("timestamp").Value.ToString();
                        Device.fPort = jo.Property("fPort").Value.ToString();
                        Device.modulation = jo.Property("modulation").Value.ToString();
                        Device.Bitrate = jo.Property("bitRate").Value.ToString();
                        Device.coderate = jo.Property("codeRate").Value.ToString();
                        Device.SNR = jo.Property("loraSNR").Value.ToString();
                        Device.RSSI = jo.Property("rssi").Value.ToString();
                        Device.size = Device.payloadHex.Length;
                        if (Device.payloadHex != "")
                        {
                            switch (testtype)
                            {
                                case TestType.Pressure:
                                    Device.Battery = Battery_Pressure(Device.payloadHex).ToString();
                                    Device.Temperature = Temp_Pressure(Device.payloadHex).ToString();
                                    Device.Pressure = Pressure(Device.payloadHex).ToString();
                                    break;
                                case TestType.VibrationBEQ:
                                    Device.Battery = Battery_8931BEQ(Device.payloadHex).ToString();
                                    Device.Temperature = Temperture_8931BEQ(Device.payloadHex).ToString();
                                    if(Convert.ToInt16(Device.payloadHex.Substring(8,1)) >= 8 && Convert.ToInt16(Device.payloadHex.Substring(8, 1)) < 12)
                                        Device.SIG_RMS_Z = SIG_8931BEQ(Device.payloadHex).ToString();
                                    else if(Convert.ToInt16(Device.payloadHex.Substring(8, 1)) >= 4 && Convert.ToInt16(Device.payloadHex.Substring(8, 1)) < 8)
                                        Device.SIG_RMS_Y = SIG_8931BEQ(Device.payloadHex).ToString();
                                    else if(Convert.ToInt16(Device.payloadHex.Substring(8, 1)) < 4)
                                        Device.SIG_RMS_X = SIG_8931BEQ(Device.payloadHex).ToString();
                                    break;
                                case TestType.Vibration:
                                    Device.Battery = Battery(Device.payloadHex).ToString();
                                    Device.Temperature = Temp(Device.payloadHex).ToString();
                                    Device.SIG_RMS = SIG_RMS(Device.payloadHex).ToString();
                                    break;
                                case TestType.Temp:
                                    Device.Battery = Battery_TempP(Device.payloadHex).ToString();
                                    Device.Temperature = Temp_TempP(Device.payloadHex).ToString();
                                   // Device.SIG_RMS = SIG_RMS(Device.payloadHex).ToString();
                                    break;
                            }
                        }

                        Devices.Add(Device);
                    }
                    Devices.Reverse();   //反转列表元素
                    if (SQLFlag)
                    {
                        Database.DbInsertSever(Devices);  //数据存放至数据库
                    }
                    StreamWriter sw = new StreamWriter(fileName, true, System.Text.Encoding.UTF8);
                    for (int j = 0; j < Devices.Count; j++)
                    {
                        string data;
                        switch (testtype)
                        {
                            case TestType.Pressure:
                                data = Devices[j].DevEUI + "," + Devices[j].time + "," + Devices[j].payloadHex + "," + Devices[j].size + "," + Devices[j].type + "," + Devices[j].fCnt + "," + Devices[j].Datarate + "," + Devices[j].Bandwidth + "," + Devices[j].Spreadfactor + "," + Devices[j].Timestamp + "," + Devices[j].fPort + "," + Devices[j].modulation
                                    + "," + Devices[j].Bitrate + "," + Devices[j].SNR + "," + Devices[j].RSSI + "," + Devices[j].Battery + "," + Devices[j].Temperature + "," + Devices[j].Pressure;
                                break;
                            case TestType.Temp:
                                data = Devices[j].DevEUI + "," + Devices[j].time + "," + Devices[j].payloadHex + "," + Devices[j].size + "," + Devices[j].type + "," + Devices[j].fCnt + "," + Devices[j].Datarate + "," + Devices[j].Bandwidth + "," + Devices[j].Spreadfactor + "," + Devices[j].Timestamp + "," + Devices[j].fPort + "," + Devices[j].modulation
                                    + "," + Devices[j].Bitrate + "," + Devices[j].SNR + "," + Devices[j].RSSI + "," + Devices[j].Battery + "," + Devices[j].Temperature;
                                break;
                            case TestType.VibrationBEQ:
                                data = Devices[j].DevEUI + "," + Devices[j].time + "," + Devices[j].payloadHex + "," + Devices[j].size + "," + Devices[j].type + "," + Devices[j].fCnt + "," + Devices[j].Datarate + "," + Devices[j].Bandwidth + "," + Devices[j].Spreadfactor + "," + Devices[j].Timestamp + "," + Devices[j].fPort + "," + Devices[j].modulation
                                     + "," + Devices[j].Bitrate + "," + Devices[j].SNR + "," + Devices[j].RSSI + "," + Devices[j].Battery + "," + Devices[j].Temperature + "," + Devices[j].SIG_RMS_X + "," + Devices[j].SIG_RMS_Y + "," + Devices[j].SIG_RMS_Z;
                                break;
                            default:
                                data = Devices[j].DevEUI + "," + Devices[j].time + "," + Devices[j].payloadHex + "," + Devices[j].size + "," + Devices[j].type + "," + Devices[j].fCnt + "," + Devices[j].Datarate + "," + Devices[j].Bandwidth + "," + Devices[j].Spreadfactor + "," + Devices[j].Timestamp + "," + Devices[j].fPort + "," + Devices[j].modulation
                                     + "," + Devices[j].Bitrate + "," + Devices[j].SNR + "," + Devices[j].RSSI + "," + Devices[j].Battery + "," + Devices[j].Temperature + "," + Devices[j].SIG_RMS;
                                break;
                        }
                       
                        sw.WriteLine(data);
                        sw.Flush();
                    }
                    sw.Close();
                    return Devices.ToArray();
                }
                return null;

            }
        }

        // <summary>
        // 获取已经添加的终端信息
        // </summary>
        // <returns></returns>
        public cGatewayDevice[] Get_GatewayDevicelist()
        {
            string url = @"http://" + IP + @":8081/api/urdevices?search=&order=asc&offset=0&limit=10&organizationID=1";
            //-------------------------------------------------------------------------
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Host = IP + ":8081";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            //-------------------------------------------------------------------------
            req.Headers.Add("authorization", "Bearer " + authorization);
            req.Timeout = 5000;
            HttpWebResponse aa = req.GetResponse() as HttpWebResponse;
            Stream stream = aa.GetResponseStream();
            //Logger.Debug(DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + $"create Get_GatewayDevicelist request success");
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();
                if (res.Contains("devTotalCount"))
                {
                    Regex reg = new Regex("{\"devEUI\":\".*?\"}");
                    MatchCollection match = reg.Matches(res);
                    List<cGatewayDevice> my_Devices = new List<cGatewayDevice>() { };
                    //{
                    //    "devEUI":"1231231212312312","name":"asdrfdss","applicationID":"6","appName":"sdfsf","description":"fgjuyref","profileID":
                    //"a6dcb677-6a81-47c8-8b8b-d41543ec9aa2","profileName":"sdfsf","fCntUp":0,"fCntDown":0,"skipFCntCheck":true,
                    //"appKey":"65432109654321096543210965432109","devAddr":"","appSKey":"","nwkSKey":"","supportsJoin":true,"active":false,"lastSeenAt":"-","mbMode":"","mbFramePort":"0","mbTcpPort":"0","lastSeenAtTime":"-"}
                    //Form1.frm1.txtbox_DeviceList.Text = string.Empty;   //清空文本框
                    for (int i = 0; i < match.Count; i++)
                    {
                        JObject jo = JObject.Parse(match[i].Value);
                        cGatewayDevice Device = new cGatewayDevice();                      
                        Device.DevEUI = jo.Property("devEUI").Value.ToString();
                        Device.Name = jo.Property("name").Value.ToString();
                        Device.ProfileID = jo.Property("profileID").Value.ToString();
                        Device.ProfileName = jo.Property("profileName").Value.ToString();
                        Device.lastSeenAtTime = jo.Property("lastSeenAtTime").Value.ToString();
                        string[] str = Device.lastSeenAtTime.Split('.');
                        my_Devices.Add(Device);
                        
                     }
                    return my_Devices.ToArray();
                }
            }
            return null;

        }

        /// <summary>
        /// 保存DeviceInfo到csv文件中
        /// </summary>
        /// <param name="fileName"></param>
        public string CreateCsv(bool TepFlg,TestType testtype)      //OK
        {
            string date;
            if (TepFlg)
                date = "TEP" + DateTime.Now.ToString("yyyMMddHHmmss");
            else
                date = DateTime.Now.ToString("yyyMMddHHmmss");
            fileName = "D:\\PayloadData\\" + date + ".csv";
            if (!File.Exists(fileName)) //当文件不存在时创建文件
            {               
                StreamWriter sw = new StreamWriter(fileName, true, System.Text.Encoding.UTF8);
                string dataHeard = string.Empty;
                //写入CSV的标题栏
                switch(testtype)
                {
                    case TestType.Pressure:
                        dataHeard = "DevEUI,Time,Payload,Payload Size,Type,fCnt,DataRate,BandWidth,SpreadFactor,TimeStamp,fport,Modulation,BitRate,SNR,RSSI,Battery(%),Temperature(℃),Pressure";
                        break;
                    case TestType.Temp:
                        dataHeard = "DevEUI,Time,Payload,Payload Size,Type,fCnt,DataRate,BandWidth,SpreadFactor,TimeStamp,fport,Modulation,BitRate,SNR,RSSI,Battery(%),Temperature(℃)";
                        break;
                    case TestType.VibrationBEQ:
                        dataHeard = "DevEUI,Time,Payload,Payload Size,Type,fCnt,DataRate,BandWidth,SpreadFactor,TimeStamp,fport,Modulation,BitRate,SNR,RSSI,Battery(%),Temperature(℃),SIG_RMS_X,SIG_RMS_Y,SIG_RMS_Z";
                        break;
                    default:
                        dataHeard = "DevEUI,Time,Payload,Payload Size,Type,fCnt,DataRate,BandWidth,SpreadFactor,TimeStamp,fport,Modulation,BitRate,SNR,RSSI,Battery(%),Temperature(℃),SIG_RMS";
                        break;
                }
                
                sw.WriteLine(dataHeard);                
                sw.Close();
                // fs.Close();
                return fileName;   
            }
            return null;

        }

        /// <summary>
        /// 清空数据流
        /// </summary>
        public void DeleteDataFlow()   ///OK
        {
            string url = @"http://" + IP + @":8081/api/urpackets";
            CreateReq(url);
            req.Method = "DELETE";
            req.Headers.Add("authorization", "Bearer " + authorization);
            req.Timeout = 5000;
            HttpWebResponse aa = req.GetResponse() as HttpWebResponse;
            Stream stream = aa.GetResponseStream();
            //Logger.Debug(DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + $"create deletedataflow request success");
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();
                _log.Info("Delete Dataflow request create success");
            }
        }

            // <summary>
            // 登录密码Aes加密
            // </summary>
            // <param name = "Password" ></ param >
            // < returns ><// returns >
            private string AesEncrypt(string Password)
        {
            SymmetricAlgorithm crypt = Aes.Create();
            crypt.Key = Encoding.UTF8.GetBytes(Self_Key);
            crypt.IV = Encoding.UTF8.GetBytes(Self_IV);
            crypt.Mode = CipherMode.CBC;
            crypt.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = crypt.CreateEncryptor();
            byte[] res = cryptoTransform.TransformFinalBlock(Encoding.UTF8.GetBytes(Password), 0, Encoding.UTF8.GetBytes(Password).Length);
            return Convert.ToBase64String(res, 0, res.Length);
        }



        // <summary>
        // 字符串转16进制字节数组
        // </summary>
        // <param name = "hexString" ></ param >
        // < returns ><// returns >
        private byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 电量转换公式
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private int Battery(string payload)
        {
            try
            {
                string Hex;
                int bat;
                Hex = "0x" + payload.Substring(0, 2);  //截取字符串前两位
                bat = System.Convert.ToInt32(Hex, 16);  //将十六进制转为十进制
                return bat;
            }
            catch
            {
                _log.Error("payload异常");
                return 0;
            }

        }

        /// <summary>
        /// 温度转换公式
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private double Temp(string payload)
        {
            try
            {
                string Hex;
                double temp;
                Hex = "0x" + payload.Substring(6, 2) + payload.Substring(4, 2);
                temp = (System.Convert.ToInt32(Hex, 16) - 1000) / (double)10;
                return temp;
            }
            catch
            {
                _log.Error("payload异常");
                return 0;
            }
            
        }

        /// <summary>
        /// 加速度转换公式
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private double SIG_RMS(string payload)
        {
            try
            {
                string Hex;
                double sig;
                Hex = "0x" + payload.Substring(10, 2) + payload.Substring(8, 2);
                sig = System.Convert.ToInt32(Hex, 16) / (double)1000;
                return sig;
            }
            catch
            {
                _log.Error("payload异常");
                return 0;
            }
            
        }


        /// <summary>
        /// Pressure电量转换公式
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public int Battery_Pressure(string payload)
        {
            try
            {
                string Hex;
                int bat;
                //Hex = "0x" + payload.Substring(3, 1);  //截取字符串前一位
                //bat = System.Convert.ToInt32(Hex, 16) * 10;  //将十六进制转为十进制
                Hex = "0x" + payload.Substring(10, 2);
                bat = System.Convert.ToInt32(Hex,16);
                return bat;
            }
            catch
            {
                _log.Error("payload异常");
                return 0;
            }
            
        }

        /// <summary>
        /// 温度转换公式
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public double Temp_Pressure(string payload)
        {
            try
            {
                double bat;
                int Dec;
                string Hex;
                Hex = "0x" + payload.Substring(12, 4);
                Dec = System.Convert.ToInt32(Hex,16);
                if(Dec < Math.Pow(2,15))
                {
                    bat = (double)Dec / 100;
                    return bat;
                }
                else
                {
                    bat = (Dec - Math.Pow(2, 16))/100;
                    return bat;
                }
               
            }
            catch
            {
                _log.Error("payload异常");
                return 0;
            }
            
        }

        /// <summary>
        /// 压力转换公式
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public float Pressure(string payload)
        {
            try
            {
                UInt32 X;
                float fy;
                string HexStr;
                //HexStr = payload.Substring(10, 2) + payload.Substring(8, 2) + payload.Substring(6, 2) + payload.Substring(4, 2);
                //X = Convert.ToUInt32(HexStr, 16);
                //fy = BitConverter.ToSingle(BitConverter.GetBytes(X), 0);
                HexStr = payload.Substring(16, 8);
                X = Convert.ToUInt32(HexStr, 16);
                fy = BitConverter.ToSingle(BitConverter.GetBytes(X), 0);
                return fy;
            }
            catch
            {
                _log.Error("payload异常");
                return 0;
            }
           
        }

        public int Battery_8931BEQ(string payload)
        {
            string BattHex = "0x" + payload.Substring(3, 1);
            int Battery = Convert.ToInt32(BattHex, 16) * 10;
            return Battery;
        }

        public double Temperture_8931BEQ(string payload)
        {
            string TempHex = "0x" + payload.Substring(4, 2);
            double Temp = Convert.ToInt32(TempHex, 16) * 0.5 - 40;
            return Temp;
        }

        public double SIG_8931BEQ(string payload)
        {
            string SIGHex = "0x" + payload.Substring(10, 2);
            double SIG = Math.Pow(10, (Convert.ToInt32(SIGHex, 16) * 0.3149606 - 49.0298) / 20);
            double sig = Math.Round(SIG, 6);
            return sig;
        }

        public double Battery_TempP(string payload)
        {
            try
            {
                string Hex;
                int bat;
                //Hex = "0x" + payload.Substring(3, 1);  //截取字符串前一位
                //bat = System.Convert.ToInt32(Hex, 16) * 10;  //将十六进制转为十进制
                Hex = "0x" + payload.Substring(10, 2);
                bat = System.Convert.ToInt32(Hex, 16);
                return bat;
            }
            catch
            {
                _log.Error("payload异常");
                return -999;
            }
        }

        public double Temp_TempP(string payload)
        {
            try
            {
                string Hex;
                int bat;
                //Hex = "0x" + payload.Substring(3, 1);  //截取字符串前一位
                //bat = System.Convert.ToInt32(Hex, 16) * 10;  //将十六进制转为十进制
                Hex = "0x" + payload.Substring(12, 4);
                bat = System.Convert.ToInt32(Hex, 16);
                return bat * 0.01;
            }
            catch
            {
                _log.Error("payload异常");
                return -999;
            }
        }
    }
}

