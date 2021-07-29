using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
namespace MyService
{
    public partial class Service1 : ServiceBase
    {

        [DllImport(@"D:\GitLab\tower_bridge_4\Debug\PBUIWork.dll",  CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PBLogin(string strPBPbPath, string filePath, string userName, string password);

        [DllImport(@"D:\GitLab\tower_bridge_4\Debug\PBUIWork.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Send_Instruction(string strProductNo, string strUnitNo, string strStockNo, string strEntrustDir, string strPriceType, string strEntrustPrice, string strEntrustAmount, StringBuilder APIResult, StringBuilder APIErrInfo);

        const int MSG_BUFF_SIZE = 1024 * 1024;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {          
            FileStream fs = new FileStream(@"d:\xx.txt", FileMode.OpenOrCreate| FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("WindowsService: Service Start" + DateTime.Now.ToString() + "\n");


            StringBuilder Result = new StringBuilder(MSG_BUFF_SIZE);
            StringBuilder ErrInfo = new StringBuilder(MSG_BUFF_SIZE);

            
            bool bRtn = PBLogin(@"D:\Software\pbrc-光大证券\pbrc.exe", @"D:\GitLab\tower_bridge_4\Debug", "89050001", "1q");
            //sw.WriteLine("PBLogin=" + bRtn.ToString()+ DateTime.Now.ToString() + "\n");
            //if (!bRtn)
            //{
            //    sw.WriteLine("demoWork登录不成功" + DateTime.Now.ToString()+"\n");
            //    //logger.Info("demoWork登录不成功");
            //}
            //else
            //{
            //    sw.WriteLine("demoWork登录成功" + DateTime.Now.ToString()+"\n");
            //    //logger.Info("demoWork成功");
            //}
            Thread.Sleep(10000);

            bRtn = Send_Instruction("300 华润测试产品", "1", "600555", "买入", "限价", "20", "200", Result, ErrInfo);
            sw.WriteLine("Send_Instruction=" + bRtn.ToString() + DateTime.Now.ToString()+"\n");
            if (!bRtn)
            {
                sw.WriteLine("demoWork指令下达失败！" + DateTime.Now.ToString()+ "\n");
                //logger.Info("demoWork指令下达失败！");
            }
            else
            {
                sw.WriteLine("demoWork指令下达OK！" + DateTime.Now.ToString()+"\n");
                // logger.Info("demoWork指令下达ok！");
            }
            string strResult = Result.ToString();
            string strErrInfo = ErrInfo.ToString();

            sw.Flush();
            sw.Close();
            fs.Close();
        }

        protected override void OnStop()
        {
            FileStream fs = new FileStream(@"d:\xx.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            //sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("WindowsService: Service Stopped" + DateTime.Now.ToString() + "\n");
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
    }
}
