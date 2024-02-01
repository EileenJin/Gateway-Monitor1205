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
//using UST;

namespace Gateway
{
    ////// <summary>
    ////// 星纵网关驱动
    ////// </summary>
    public class cGateway_Milesight
    {
        ////// <summary>
        ////// 网关登录账户名
        ////// </summary>
        public static string Username = "admin";
        ////// <summary>
        ////// 网关登录密码
        ////// </summary>
        public static string Password = "password";
        // <summary>
        // 网关IP地址,Wifi相连用192.168.1.1,网线连接用192.168.23.150
        // </summary>
        public static string IP = "192.168.23.150";

        public static bool LoginState = false;

        // <summary>
        // 下发的数据类型
        // </summary>
        public enum eDownLinkType
        {
            Hex,
            ASCII,
            Base64
        }
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
            public bool active;////终端是否已经激活
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

        private HttpWebRequest reqqq;


        private static cGateway_Milesight _Instance;
        public static cGateway_Milesight Instance
        {
            get
            {
                if (_Instance is null)
                { _Instance = new cGateway_Milesight(); }
                return _Instance;
            }
        }

        //public void test()
        //{
        //    string PostData = "{\"id\":\"1\",\"execute\":1,\"core\":\"user\",\"function\":\"login\",\"values\":[{\"username\":\"admin\",\"password\":\"" + AesEncrypt(Password) + "\"}]}";
        //    byte[] bytearray = Encoding.UTF8.GetBytes(PostData);
        //    string url = @"http://" + IP + @"//cgi";
        //    reqqq = (HttpWebRequest)WebRequest.Create(url);
        //    reqqq.Method = "POST";
        //    reqqq.Headers.Add("Accept-Language", "");
        //    reqqq.Host = IP + @":8081";
        //    reqqq.Headers.Add("Origin", @"http://" + IP);
        //    reqqq.Referer = @"http://" + IP + @"/";
        //    reqqq.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
        //    reqqq.ContentType = @"application /x-www-form-urlencoded; charset=UTF-8";
        //    reqqq.Headers.Add("X-Requested-With", "XMLHttpRequest");
        //    reqqq.ContentLength = bytearray.Length;
        //    reqqq.Timeout = 5000;
        //    using (Stream reqstream = reqqq.GetRequestStream())
        //    {
        //        reqstream.Write(bytearray, 0, bytearray.Length);
        //        reqstream.Close();
        //    }
        //    HttpWebResponse bb = reqqq.GetResponse() as HttpWebResponse;
        //    Stream stream = bb.GetResponseStream();
        //    string res = string.Empty;
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        res = reader.ReadToEnd();
        //        JObject jo = JObject.Parse(res);
        //        if (jo.Property("status").Value.ToString() == "0")
        //        {
        //            string Cookie = bb.Headers["Set-Cookie"].ToString();
        //            string[] cookies = Cookie.Replace(@"; path=//", "").Split(',');
        //            my_Cookies = "language=zh-cn; " + string.Join(" ;", cookies); ////cookies.Join(';'); //// cookies.Join(cookies, new string[] { " ;"});    
        //            my_Cookies = string.Join(" ;", cookies); ////cookies.Join(';'); //// cookies.Join(cookies, new string[] { " ;"});  

        //        }
        //    }

        //    url = @"http://" + IP + @"//islogin";
        //    -------------------------------------------------------------------------
        //    reqqq = (HttpWebRequest)WebRequest.Create(url);
        //    reqqq.Method = "POST";
        //    reqqq.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        //    reqqq.Host = IP + ":8081";
        //    reqqq.Headers.Add("Origin", @"http://" + IP);
        //    reqqq.Referer = @"http://" + IP + @"/";
        //    reqqq.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
        //    -------------------------------------------------------------------------
        //    reqqq.Headers.Add("X-Requested-With", "XMLHttpRequest");
        //    reqqq.Headers.Add("Cookie", my_Cookies);
        //    req.CookieContainer = new CookieContainer();
        //    string[] Cookies = my_Cookies.Split(';');
        //    foreach (string Cookie in Cookies)
        //    {
        //        Cookie ck = new Cookie();
        //        ck.Name = Cookie.Split('=')[0].Trim();
        //        ck.Value = Cookie.Split('=')[1].Trim();
        //        req.CookieContainer.Add(ck);
        //    }
        //    reqqq.ContentLength = 0;
        //    reqqq.Timeout = 5000;
        //    using (Stream reqstream = req.GetRequestStream())
        //    {
        //        reqstream.Write(bytearray, 0, bytearray.Length);
        //        reqstream.Close();
        //    }
        //    bb = reqqq.GetResponse() as HttpWebResponse;

        //    stream = bb.GetResponseStream();
        //    res = string.Empty;
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        res = reader.ReadToEnd();
        //    }

        //    url = @"http://" + IP + @"/";
        //    reqqq = (HttpWebRequest)WebRequest.Create(url);
        //    reqqq.Method = "GET";
        //    reqqq.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        //    reqqq.Host = IP + @":8081";
        //    reqqq.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
        //    reqqq.Headers.Add("Origin", @"http://" + IP);
        //    reqqq.Referer = @"http://" + IP + @"/";
        //    reqqq.Timeout = 5000;
        //    HttpWebResponse aa = reqqq.GetResponse() as HttpWebResponse;
        //    stream = aa.GetResponseStream();
        //    res = string.Empty;
        //    string Secondpassword = string.Empty;
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        res = reader.ReadToEnd();
        //        Regex reg = new Regex("{username:\"" + Username + "\",password:\".*?\"}");
        //        if (reg.IsMatch(res)) { Secondpassword = reg.Match(res).ToString().Insert(1, "\"").Insert(10, "\"").Insert(15 + Username.Length, "\"").Insert(15 + 9 + Username.Length, "\""); }
        //    }////{"username":"admin",password:"NicJjG18XOV3U1efQyo8AQ=="}

        //    PostData = Secondpassword;
        //    { "username":"admin","password":"NicJjG18XOV3U1efQyo8AQ=="}
        //    bytearray = Encoding.UTF8.GetBytes(PostData);
        //    url = @"http://" + IP + @":8081/api/internal//login";
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    req.Method = "POST";
        //    req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        //    req.Host = IP + ":8081";
        //    req.Headers.Add("Origin", @"http://" + IP);
        //    req.Referer = @"http://" + IP + @"/";
        //    req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
        //    req.ContentType = @"application//x-www-form-urlencoded; charset=UTF-8";
        //    req.ContentLength = bytearray.Length;
        //    req.Timeout = 5000;
        //    using (Stream reqstream = req.GetRequestStream())
        //    {
        //        reqstream.Write(bytearray, 0, bytearray.Length);
        //        reqstream.Close();
        //    }
        //    bb = req.GetResponse() as HttpWebResponse;
        //    stream = bb.GetResponseStream();
        //    res = string.Empty;
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        res = reader.ReadToEnd();
        //        if (res.Contains("jwt"))
        //        {
        //            JObject jo = JObject.Parse(res);
        //            authorization = jo.Property("jwt").Value.ToString();
        //        }
        //    }

        //    url = @"http://" + IP + @"//islogin";
        //    -------------------------------------------------------------------------
        //    reqqq = (HttpWebRequest)WebRequest.Create(url);
        //    reqqq.Method = "POST";
        //    reqqq.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        //    reqqq.Host = IP + ":8081";
        //    reqqq.Headers.Add("Origin", @"http://" + IP);
        //    reqqq.Referer = @"http://" + IP + @"/";
        //    reqqq.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
        //    -------------------------------------------------------------------------
        //    reqqq.Headers.Add("X-Requested-With", "XMLHttpRequest");
        //    reqqq.Headers.Add("Cookie", my_Cookies);
        //    req.CookieContainer = new CookieContainer();
        //    string[] Cookies = my_Cookies.Split(';');
        //    foreach (string Cookie in Cookies)
        //    {
        //        Cookie ck = new Cookie();
        //        ck.Name = Cookie.Split('=')[0].Trim();
        //        ck.Value = Cookie.Split('=')[1].Trim();
        //        req.CookieContainer.Add(ck);
        //    }
        //    reqqq.ContentLength = 0;
        //    reqqq.Timeout = 5000;
        //    using (Stream reqstream = req.GetRequestStream())
        //    {
        //        reqstream.Write(bytearray, 0, bytearray.Length);
        //        reqstream.Close();
        //    }
        //    bb = reqqq.GetResponse() as HttpWebResponse;

        //    stream = bb.GetResponseStream();
        //    res = string.Empty;
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        res = reader.ReadToEnd();
        //    }
        //}

        // <summary>
        // 网关添加测试终端
        // </summary>
        // <param name = "DevEUI" ></ param >
        // < param name="AppKey"></param>
        // <returns></returns>
        //public bool Sensor_AddToGateway(string DevEUI, string AppKey)
        //{
        //    try
        //    {
        //        //test();
        //        if (!PasswordValidate()) { ATELog.Instance.Error("无法验证网关登录密码，请检查网关是否已经开启，账号密码是否正确！"); return false; }////先验证密码
        //        ATELog.Instance.Info("验证网关登录信息成功");
        //        // IsLogin();
        //        string LoginPassword = GetSecondPassword();////获取登录的密码
        //        if (string.IsNullOrEmpty(LoginPassword)) { ATELog.Instance.Error("未找到网关二级密码，可能网关固件更新或者更换新网关导致匹配变更，请联系管理员"); return false; }
        //        if (!Login(LoginPassword)) { ATELog.Instance.Error("登录失败，可能网关固件更新导致API变更，请联系管理员"); return false; }
        //        ATELog.Instance.Info("登录网关成功");
        //        // IsLogin();
        //        //------------获取ProfileID
        //        string ProfileID = Get_GatewayProfileID();
        //        if (string.IsNullOrEmpty(ProfileID)) { ATELog.Instance.Error("未找到网关ProfileID，请联系管理员查看是否Profile被删除！"); return false; }
        //        ATELog.Instance.Info($"网关ProfileID:{ProfileID}");
        //        //------------获取AppID
        //        string ApplicationID = Get_GatewayApplicationID();
        //        if (string.IsNullOrEmpty(ApplicationID)) { ATELog.Instance.Error("未找到网关ApplicationID，请联系管理员查看是否Application被删除！"); return false; }
        //        ATELog.Instance.Info($"网关ApplicationID:{ApplicationID}");
        //        // ------------清空旧监控终端
        //        if (!Delete_AllDeviceInlist()) { ATELog.Instance.Error("删除旧设备组失败"); return false; }
        //        ATELog.Instance.Info("清空旧设备信息成功");
        //        //------------添加新终端
        //        if (!Add_LoraDevice("LoraTest", "ProductionTest", DevEUI, ProfileID, ApplicationID, AppKey))
        //        { ATELog.Instance.Error("添加测试终端失败1"); return false; }
        //        ATELog.Instance.Info("恭喜，添加测试终端成功！");

        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //    finally { Logout(); }
        //}

        // <summary>
        // 判断终端是否与网关握手
        // </summary>
        // <param name = "DevEUI" ></param >
        // < param name="Appkey"></param>
        // <returns></returns>
        public  bool IsSensorActive()
        {
            if (!PasswordValidate()) { return false; }///{ ATELog.Instance.Error("无法验证网关登录密码，请检查网关是否已经开启，账号密码是否正确！"); return false; }//先验证密码
            //ATELog.Instance.Info("验证网关登录信息成功");
            string LoginPassword = GetSecondPassword();////获取登录的密码
            if (string.IsNullOrEmpty(LoginPassword)) { return false;  }///{ ATELog.Instance.Error("未找到网关二级密码，可能网关固件更新或者更换新网关导致匹配变更，请联系管理员"); return false; }
            if (!Login(LoginPassword)) { return false; } // {ATELog.Instance.Error("登录失败，可能网关固件更新导致API变更，请联系管理员"); return false; }
                                                         // ATELog.Instance.Info("登录网关成功");
            return true;
            //string ProfileID = Get_GatewayProfileID();
            //if (string.IsNullOrEmpty(ProfileID)) { ATELog.Instance.Error("未找到网关ProfileID，请联系管理员查看是否Profile被删除！"); return false; }
            //ATELog.Instance.Info($"网关ProfileID:{ProfileID}");
            //string ApplicationID = Get_GatewayApplicationID();
            //if (string.IsNullOrEmpty(ApplicationID)) { ATELog.Instance.Error("未找到网关ApplicationID，请联系管理员查看是否Application被删除！"); return false; }
            //ATELog.Instance.Info($"网关ApplicationID:{ProfileID}");
            //System.Threading.Thread.Sleep(500);
            //cGatewayDevice[] My_Devices = Get_GatewayDevicelist();
            //if (!(My_Devices is null)) { ATELog.Instance.Error("获取终端设备组信息失败"); return false; }
            //if (My_Devices.Length == 1 && My_Devices[0].DevEUI == DevEUI && My_Devices[0].Appkey == Appkey && My_Devices[0].active)
            //{ ATELog.Instance.Info("终端已经与网关握手成功！"); return true; }
            //ATELog.Instance.Info("网关未在线");
            //return false;
        }

        public string Read_UplinkPayload()
        {
            string url = @"http://" + IP + @":8081/api/urpackets?offset=0&limit=10&organizationID=1";
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
            if (aa.StatusCode.ToString() == "OK")
            {

            }
            Stream stream = aa.GetResponseStream();
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();////eg:"profileID": "a6dcb677-6a81-47c8-8b8b-d41543ec9aa2"
                Regex reg = new Regex("\"profileID\":\".*?\"");
                if (reg.IsMatch(res))
                {
                    string RegStr = reg.Match(res).ToString();
                    return RegStr.Substring(13, RegStr.Length - 14);
                }

                return string.Empty;
            }
        }

        //public bool IsLogin()
        //{
        //    string url = @"http://" + IP + @"/islogin";
        //    //-------------------------------------------------------------------------
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    req.Method = "POST";
        //    req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        //    req.Host = IP + ":8081";
        //    req.Headers.Add("Origin", @"http://" + IP);
        //    req.Referer = @"http://" + IP + @"/";
        //    req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
        //    //-------------------------------------------------------------------------
        //    req.Headers.Add("X-Requested-With", "XMLHttpRequest");
        //    req.Headers.Add("Cookie", my_Cookies);
        //    req.CookieContainer = new CookieContainer();
        //    string[] Cookies = my_Cookies.Split(';');
        //    foreach (string Cookie in Cookies)
        //    {
        //        Cookie ck = new Cookie();
        //        ck.Name = Cookie.Split('=')[0].Trim();
        //        ck.Value = Cookie.Split('=')[1].Trim();
        //        req.CookieContainer.Add(ck);
        //    }
        //    req.ContentLength = 0;
        //    req.Timeout = 5000;
        //    using (Stream reqstream = req.GetRequestStream())
        //    {
        //        reqstream.Write(bytearray, 0, bytearray.Length);
        //        reqstream.Close();
        //    }
        //    HttpWebResponse bb = req.GetResponse() as HttpWebResponse;

        //    Stream stream = bb.GetResponseStream();
        //    string res = string.Empty;
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        res = reader.ReadToEnd();
        //        return true;
        //    }
        //}

        // <summary>
        // 通过网关给终端发数据
        // </summary>
        // <param name = "DevEUI" ></ param >
        // < param name="Data"></param>
        // <param name = "Port" ></ param >
        // < returns ><// returns >
        public bool Write_DownlinkCmd(string DevEUI, string Data, int Port)////Hex Type Downlink
        {
            byte[] HexByte = strToHexByte(Data);
            string New_Data = Convert.ToBase64String(HexByte, 0, HexByte.Length);
            string PostData = "{\"devEUI\":\"" + DevEUI + "\",\"data\":\"" + New_Data + "\",\"fPort\":" + Port.ToString() + ",\"confirmed\":false}";
            byte[] bytearray = Encoding.UTF8.GetBytes(PostData);
            string url = @"http://" + IP + @"/api/devices/" + DevEUI + @"/queue";
            //-------------------------------------------------------------------------
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Host = IP + ":8081";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            //-------------------------------------------------------------------------
            req.Headers.Add("authorization", "Bearer " + authorization);
            req.ContentType = @"application /x-www-form-urlencoded; charset=UTF-8";
            req.ContentLength = bytearray.Length;
            req.Timeout = 5000;
            using (Stream reqstream = req.GetRequestStream())
            {
                reqstream.Write(bytearray, 0, bytearray.Length);
                reqstream.Close();
            }
            HttpWebResponse bb = req.GetResponse() as HttpWebResponse;
            return bb.StatusCode.ToString() == "OK";
        }

        // <summary>
        // 添加一个Lora终端，就是8911Ex传感器
        // </summary>
        // <param name = "DeviceName" ></ param >
        // < param name="Description"></param>
        // <param name = "Dev_EUI" ></ param >
        // < param name="ProfileID"></param>
        // <param name = "ApplicationID" ></ param >
        // < param name="Appkey"></param>
        // <returns></returns>
        public bool Add_LoraDevice(string DeviceName, string Description, string Dev_EUI, string ProfileID, string ApplicationID, string Appkey)
        {
            string PostData = "{\"name\":\"" + DeviceName + "\",\"description\":\"" + Description + "\",\"devEUI\":\"" + Dev_EUI + "\",\"applicationID\":\"" + ApplicationID + "\",\"appKey\":\"" + Appkey + "\",\"profileID\":\"" + ProfileID + "\",\"skipFCntCheck\":true}";
            byte[] bytearray = Encoding.UTF8.GetBytes(PostData);
            string url = @"http://" + IP + @":8081/api/urdevices";
            //-------------------------------------------------------------------------
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Host = IP + ":8081";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            //-------------------------------------------------------------------------
            req.ContentType = @"application /x-www-form-urlencoded; charset=UTF-8";
            req.Headers.Add("authorization", "Bearer " + authorization);
            req.ContentLength = bytearray.Length;
            req.Timeout = 5000;
            using (Stream reqstream = req.GetRequestStream())
            {
                reqstream.Write(bytearray, 0, bytearray.Length);
                reqstream.Close();
            }
            HttpWebResponse bb = req.GetResponse() as HttpWebResponse;
            return bb.StatusCode.ToString() == "OK";
        }

        // <summary>
        // 删除所有终端
        // </summary>
        // <returns></returns>
        public bool Delete_AllDeviceInlist()
        {
            string url = @"http://" + IP + @":8081/api/urdevicesall";
            //-------------------------------------------------------------------------
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "DELETE";
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Host = IP + ":8081";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            //-------------------------------------------------------------------------
            req.Headers.Add("authorization", "Bearer " + authorization);
            HttpWebResponse aa = req.GetResponse() as HttpWebResponse;
            return aa.StatusCode.ToString() == "OK";
        }
        
        // <summary>
        // 获取已经添加的终端信息
        // </summary>
        // <returns></returns>
        public cGatewayDevice[] Get_GatewayDevicelist()////OK
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
                    for (int i = 0; i < match.Count; i++)
                    {
                        JObject jo = JObject.Parse(match[i].Value);
                        cGatewayDevice Device = new cGatewayDevice();
                        Device.active = Convert.ToBoolean(jo.Property("active").Value.ToString());
                        Device.Appkey = jo.Property("appKey").Value.ToString();
                        Device.ApplicationID = jo.Property("applicationID").Value.ToString();
                        Device.ApplicationName = jo.Property("appName").Value.ToString();
                        Device.DevEUI = jo.Property("devEUI").Value.ToString();
                        Device.Name = jo.Property("name").Value.ToString();
                        Device.ProfileID = jo.Property("profileID").Value.ToString();
                        Device.ProfileName = jo.Property("profileName").Value.ToString();
                        my_Devices.Add(Device);
                    }
                    return my_Devices.ToArray();
                }
            }
            return null;

        }
        // <summary>
        // 获取GateWay Application ID
        // </summary>
        // <returns></returns>
        public string Get_GatewayApplicationID()////OK
        {
            string url = @"http://" + IP + @":8081/api/urapplications?limit=9999&offset=0&organizationID=1";
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
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();////"id":"6"
                Regex reg = new Regex("\"id\":\".*?\"");
                if (reg.IsMatch(res))
                {
                    string RegStr = reg.Match(res).ToString();
                    return RegStr.Substring(6, RegStr.Length - 7);
                }
                return string.Empty;
            }
        }
        // <summary>
        // 获取网关ProfileID
        // </summary>
        // <returns></returns>
        public string Get_GatewayProfileID()////OK
        {
            string url = @"http://" + IP + @":8081/api/urprofiles?limit=9999&offset=0&organizationID=1";
            // -------------------------------------------------------------------------
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
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();////eg:"profileID": "a6dcb677-6a81-47c8-8b8b-d41543ec9aa2"
                Regex reg = new Regex("\"profileID\":\".*?\"");
                if (reg.IsMatch(res))
                {
                    string RegStr = reg.Match(res).ToString();
                    return RegStr.Substring(13, RegStr.Length - 14);
                }

                return string.Empty;
            }
        }

        public string GetGatewayInfo()////未调试好
        {
            // { "id":1,"execute":1,"core":"yruo","function":"menu","values":[]}
            //string PostData = "{\"id\":0,\"execute\":3,\"core\":\"yruo_status\",\"function\":\"get\",\"values\":[{\"base\":\"summary\"}]}";
            string PostData = "{\"id\":3,\"execute\":1,\"core\":\"yruo\",\"function\":\"menu\",\"values\":[]}";
            byte[] bytearray = Encoding.UTF8.GetBytes(PostData);
            string url = @"http://" + IP + @"/cgi";
            //-------------------------------------------------------------------------
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Host = IP + ":8081";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            //-------------------------------------------------------------------------
            req.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            req.ContentLength = bytearray.Length;
            req.Timeout = 5000;
            using (Stream reqstream = req.GetRequestStream())
            {
                reqstream.Write(bytearray, 0, bytearray.Length);
                reqstream.Close();
            }
            HttpWebResponse bb = req.GetResponse() as HttpWebResponse;
            Stream stream = bb.GetResponseStream();
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();
                return res;
            }
        }

        // <summary>
        // Post登录密码
        // </summary>
        // <returns></returns>
        public bool Login(string SecondPassword)
        {
            string PostData = SecondPassword;
            //{ "username":"admin","password":"NicJjG18XOV3U1efQyo8AQ=="}
            byte[] bytearray = Encoding.UTF8.GetBytes(PostData);
            string url = @"http://" + IP + @":8081/api/internal//login";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Host = IP + ":8081";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            req.ContentType = @"application//x-www-form-urlencoded; charset=UTF-8";
            req.ContentLength = bytearray.Length;
           // ATELog.Instance.Info($"{bytearray.Length}");
            req.Timeout = 10000;
            using (Stream reqstream = req.GetRequestStream())
            {
                reqstream.Write(bytearray, 0, bytearray.Length);
                reqstream.Close();
            }
            HttpWebResponse bb = req.GetResponse() as HttpWebResponse;
            Stream stream = bb.GetResponseStream();
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();
                if (res.Contains("jwt"))
                {
                    JObject jo = JObject.Parse(res);
                    authorization = jo.Property("jwt").Value.ToString();
                    //ATELog.Instance.Info($"Authorization:{authorization}");
                    return true;
                }
            }
            return false;
        }

        public bool Logout()
        {
            try
            {
                //{ "id":5,"execute":1,"core":"","function":""}
                string PostData = "{\"id\":1,\"execute\":1,\"core\":\"\",\"function\":\"\"}";
                byte[] bytearray = Encoding.UTF8.GetBytes(PostData);
                string url = @"http://" + IP + @"//logout";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                req.Host = IP;
                req.Headers.Add("Origin", @"http://" + IP);
                req.Referer = @"http://" + IP + @"/";
                req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
                req.ContentType = @"application//x-www-form-urlencoded; charset=UTF-8";
                req.Headers.Add("X-Requested-With", "XMLHttpRequest");
                req.ContentLength = bytearray.Length;
                req.Headers.Add("Cookie", my_Cookies + "; token=" + authorization);
                req.Timeout = 5000;
                using (Stream reqstream = req.GetRequestStream())
                {
                    reqstream.Write(bytearray, 0, bytearray.Length);
                    reqstream.Close();
                }
                HttpWebResponse bb = req.GetResponse() as HttpWebResponse;
                return bb.StatusCode.ToString() == "OK";
            }
            catch (Exception e)
            {
                return false;
            }

        }

        // <summary>
        // 获取登录的密码
        // </summary>
        // <returns></returns>
        public string GetSecondPassword()
        {
            string url = @"http://" + IP + @"/";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            req.Host = IP + @":8081";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";
            req.Timeout = 5000;
            HttpWebResponse aa = req.GetResponse() as HttpWebResponse;
            Stream stream = aa.GetResponseStream();
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();
                Regex reg = new Regex("{username:\"" + Username + "\",password:\".*?\"}");
                if (reg.IsMatch(res))
                {
                    string ret = reg.Match(res).ToString().Insert(1, "\"").Insert(10, "\"").Insert(15 + Username.Length, "\"").Insert(15 + 9 + Username.Length, "\"");
                   // ATELog.Instance.Error(ret);
                    return ret;
                }
                else { }
            }////{"username":"admin",password:"NicJjG18XOV3U1efQyo8AQ=="}
            return "";
        }

        // <summary>
        // 获取登录密码
        // </summary>
        // <returns></returns>
        public bool PasswordValidate()
        {
            string PostData = "{\"id\":\"1\",\"execute\":1,\"core\":\"user\",\"function\":\"login\",\"values\":[{\"username\":\"admin\",\"password\":\"" + AesEncrypt(Password) + "\"}]}";
            byte[] bytearray = Encoding.UTF8.GetBytes(PostData);
            string url = @"http://" + IP + @"/cgi";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("Accept-Language", "");
            req.Host = IP + @":8081";
            req.Headers.Add("Origin", @"http://" + IP);
            req.Referer = @"http://" + IP + @"/";
            req.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36";
            req.ContentType = @"application /x-www-form-urlencoded; charset=UTF-8";
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            req.ContentLength = bytearray.Length;
            req.Timeout = 5000;
            using (Stream reqstream = req.GetRequestStream())
            {
                reqstream.Write(bytearray, 0, bytearray.Length);
                reqstream.Close();
            }
            HttpWebResponse bb = req.GetResponse() as HttpWebResponse;
            Stream stream = bb.GetResponseStream();
            string res = string.Empty;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                res = reader.ReadToEnd();
                JObject jo = JObject.Parse(res);
                if (jo.Property("status").Value.ToString() == "0")
                {
                    string Cookie = bb.Headers["Set-Cookie"].ToString();
                    string[] cookies = Cookie.Replace(@"; path=//", "").Split(',');
                    my_Cookies = "language=zh-cn; " + string.Join(" ;", cookies); ////cookies.Join(';'); //// cookies.Join(cookies, new string[] { " ;"});    
                    my_Cookies = string.Join(" ;", cookies); ////cookies.Join(';'); //// cookies.Join(cookies, new string[] { " ;"});  
                    return true;
                }
            }
            return false;
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
    }
}

