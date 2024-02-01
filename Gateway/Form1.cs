using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Gateway
{
    public partial class Form1 : Form
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Form1 frm1;
        public static bool LoginFlag = false;
        bool SQLFlag;
        bool TEPFlag = false;
        public string fileName = string.Empty;
        /// <summary>
        /// 定时器
        /// </summary>
        System.Timers.Timer query_interval;

        public static int count = 0;
        
        public static string BeginTime;
        public static string EndTime;

        

        Gateway_Milesight G1 = new Gateway_Milesight();      

        public enum TestType
        {
            Pressure,
            Vibration,
            VibrationBEQ,   
            Temp,
            Humi
        };
        public TestType testtype;

        public Form1()
        {
            InitializeComponent();
            frm1 = this;

        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            txtBox_Usename.Text = Gateway_Milesight.Username;
            txtBox_Secret.Text = Gateway_Milesight.Password;
            comBox_Time.Items.Add("1");
            comBox_Time.Items.Add("10");
            comBox_Time.Items.Add("15");
            comBox_Time.Items.Add("20");
            comBox_Time.Items.Add("30");
            ProductTypeCmb.Items.Add("Pressure-69xx");
            ProductTypeCmb.Items.Add("Vibration-8911");
            ProductTypeCmb.Items.Add("Vibration-8931 BEQ");
            ProductTypeCmb.Items.Add("Temperature");
            output("software open");
            comBox_Time.Enabled = false;
            btn_GetValue.Enabled = false;
            btn_stop.Enabled = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }



        private void btn_Login_Click_1(object sender, EventArgs e)
        {
            //UInt32 X;
            //double fy, xy,zy;
            //string HexStr;
            //string payload = "000a8509088d07b0e4c9b49231a4f60548c0539492ea697947";//"13210e0a80637fff7fffffff";
            //fy = G1.Temperture_8931BEQ(payload);
            //xy = G1.Battery_8931BEQ(payload);
            //zy = G1.SIG_8931BEQ(payload);
            //Hex = "0x" + payload.Substring(3, 1);  //截取字符串前一位
            //bat = System.Convert.ToInt32(Hex, 16) * 10;  //将十六进制转为十进制
            //HexStr = payload.Substring(16, 8);
            //X = Convert.ToUInt32(HexStr, 16);
            //fy = BitConverter.ToSingle(BitConverter.GetBytes(X), 0);
            //string str = fy.ToString();
            if (ProductTypeCmb.Text == string.Empty)
            {
                MessageBox.Show("请选择你的测试类型");
                return;
            }
            else
            {
                switch(ProductTypeCmb.Text)
                {
                    case "Pressure-69xx":
                        testtype = TestType.Pressure;
                        break;
                    case "Vibration-8911":
                        testtype = TestType.Vibration;
                        break;
                    case "Vibration-8931 BEQ":
                        testtype = TestType.VibrationBEQ;
                        break;
                    case "Temperature":
                        testtype = TestType.Temp;
                        break;

                }
            }
            if (G1.Login())
            {
                lbl_Note.Text = "登录成功";
                LoginFlag = true;
                groupBox2.BackColor = Color.Green;
                comBox_Time.Enabled = true;
                btn_GetValue.Enabled = true;
                btn_stop.Enabled = true;
            }
            else
            {
                lbl_Note.Text = "登录失败";
                groupBox2.BackColor = Color.Red;
            }

        }

        private void btn_GetValue_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (LoginFlag == true)
            {                            
                if (comBox_Time.Text != "")
                {
                    if (SQLRadioBtn.Checked)
                    {
                        DialogResult result = MessageBox.Show("Are you sure to save data to the SQL Serve ? ", "Warning", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            SQLFlag = true;
                        }
                        else
                        {
                            SQLFlag = false;
                        }

                    }

                    if (TEPRadioBtn.Checked)
                    {
                        TEPFlag = true;
                        fileName = G1.CreateCsv(true,testtype);
                    }
                    else
                    {
                        TEPFlag = false;
                        fileName = G1.CreateCsv(false, testtype);
                    }

                    G1.Read_UplinkPayload(fileName, SQLFlag, testtype);
                    output("保存网关数据成功");
                    G1.DeleteDataFlow();
                    output("清空网关数据成功");
                    int interval_Time;   //Time：持续时间，interval_Time:间隔时间
                    interval_Time = Convert.ToInt32(comBox_Time.Text);
                    query_interval = new System.Timers.Timer(interval_Time * 60000);            //查询间隔定时器，实例化timer类，设置间隔时间为  interval_Time*1000       
                    query_interval.AutoReset = true;                                            //每隔Time*60000ms执行一次
                    query_interval.Elapsed += new System.Timers.ElapsedEventHandler(Save_Date); //到达时间的时候执行事件Save_Date；
                    query_interval.Start();

                }
                else
                {
                    MessageBox.Show("请先设置时间");
                }
            }
            else
            {
                MessageBox.Show("请先登录");
            }
        }

        public void Save_Date(object source, System.Timers.ElapsedEventArgs e)
        {
            //lbl_save.Text = "正在保存....";
            //更新设备最新活跃时间
            //G1.Get_GatewayDevicelist();
            //获取新的设备信息
            output("开始保存....");
            G1.Login();           
            G1.Read_UplinkPayload(fileName, SQLFlag, testtype);
            output("保存成功");
            G1.DeleteDataFlow();
            output("清空数据流成功");
            System.Threading.Thread.Sleep(50);
        }

      

        /// <summary>
        /// 停止获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stop_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result;
                result = MessageBox.Show("Sure to exit?", "Ask", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    query_interval.Stop();
                    output("已停止运行");
                }
                groupBox2.BackColor = Color.LightSteelBlue;
            }
            catch(Exception ex)
            {
                output($"{ex}");
            }
            
        }

        /// <summary>
        /// 界面log输出
        /// </summary>
        /// <param name="log"></param>
        public void output(string data)
        {
            //if the length of the log message over 100 rows, it will auto clear.
            if (this.txt_Log.GetLineFromCharIndex(txt_Log.Text.Length) > 100)
                this.txt_Log.Text = "";
            this.txt_Log.AppendText(DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss ") + data + "\r\n");
            log.Info(data);
        }
        
    }
}

