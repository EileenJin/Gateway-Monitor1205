using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gateway
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //cGateway_Milesight ms = new cGateway_Milesight();
            //ms.IsSensorActive();
            InitialLog4net();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void InitialLog4net()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string log4netConfigFile = Path.Combine(baseDirectory, "log4net.config");
            FileInfo fileInfo = new FileInfo(log4netConfigFile);
            XmlConfigurator.ConfigureAndWatch(fileInfo);
        }
    }
}
