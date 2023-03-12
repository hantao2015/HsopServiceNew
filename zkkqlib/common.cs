using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Timers;
using System.Xml;
using System.Windows.Forms;

namespace Machinelib
{
    public class RecordTimer
    {
        private System.Timers.Timer tm;
        private static int[,] cPatterns = new int[,] { 
        { 2, 1, 2, 2, 2, 2, 0, 0 }, { 2, 2, 2, 1, 2, 2, 0, 0 }, { 2, 2, 2, 2, 2, 1, 0, 0 }, { 1, 2, 1, 2, 2, 3, 0, 0 }, { 1, 2, 1, 3, 2, 2, 0, 0 }, { 1, 3, 1, 2, 2, 2, 0, 0 }, { 1, 2, 2, 2, 1, 3, 0, 0 }, { 1, 2, 2, 3, 1, 2, 0, 0 }, { 1, 3, 2, 2, 1, 2, 0, 0 }, { 2, 2, 1, 2, 1, 3, 0, 0 }, { 2, 2, 1, 3, 1, 2, 0, 0 }, { 2, 3, 1, 2, 1, 2, 0, 0 }, { 1, 1, 2, 2, 3, 2, 0, 0 }, { 1, 2, 2, 1, 3, 2, 0, 0 }, { 1, 2, 2, 2, 3, 1, 0, 0 }, { 1, 1, 3, 2, 2, 2, 0, 0 }, 
        { 1, 2, 3, 1, 2, 2, 0, 0 }, { 1, 2, 3, 2, 2, 1, 0, 0 }, { 2, 2, 3, 2, 1, 1, 0, 0 }, { 2, 2, 1, 1, 3, 2, 0, 0 }, { 2, 2, 1, 2, 3, 1, 0, 0 }, { 2, 1, 3, 2, 1, 2, 0, 0 }, { 2, 2, 3, 1, 1, 2, 0, 0 }, { 3, 1, 2, 1, 3, 1, 0, 0 }, { 3, 1, 1, 2, 2, 2, 0, 0 }, { 3, 2, 1, 1, 2, 2, 0, 0 }, { 3, 2, 1, 2, 2, 1, 0, 0 }, { 3, 1, 2, 2, 1, 2, 0, 0 }, { 3, 2, 2, 1, 1, 2, 0, 0 }, { 3, 2, 2, 2, 1, 1, 0, 0 }, { 2, 1, 2, 1, 2, 3, 0, 0 }, { 2, 1, 2, 3, 2, 1, 0, 0 }, 
        { 2, 3, 2, 1, 2, 1, 0, 0 }, { 1, 1, 1, 3, 2, 3, 0, 0 }, { 1, 3, 1, 1, 2, 3, 0, 0 }, { 1, 3, 1, 3, 2, 1, 0, 0 }, { 1, 1, 2, 3, 1, 3, 0, 0 }, { 1, 3, 2, 1, 1, 3, 0, 0 }, { 1, 3, 2, 3, 1, 1, 0, 0 }, { 2, 1, 1, 3, 1, 3, 0, 0 }, { 2, 3, 1, 1, 1, 3, 0, 0 }, { 2, 3, 1, 3, 1, 1, 0, 0 }, { 1, 1, 2, 1, 3, 3, 0, 0 }, { 1, 1, 2, 3, 3, 1, 0, 0 }, { 1, 3, 2, 1, 3, 1, 0, 0 }, { 1, 1, 3, 1, 2, 3, 0, 0 }, { 1, 1, 3, 3, 2, 1, 0, 0 }, { 1, 3, 3, 1, 2, 1, 0, 0 }, 
        { 3, 1, 3, 1, 2, 1, 0, 0 }, { 2, 1, 1, 3, 3, 1, 0, 0 }, { 2, 3, 1, 1, 3, 1, 0, 0 }, { 2, 1, 3, 1, 1, 3, 0, 0 }, { 2, 1, 3, 3, 1, 1, 0, 0 }, { 2, 1, 3, 1, 3, 1, 0, 0 }, { 3, 1, 1, 1, 2, 3, 0, 0 }, { 3, 1, 1, 3, 2, 1, 0, 0 }, { 3, 3, 1, 1, 2, 1, 0, 0 }, { 3, 1, 2, 1, 1, 3, 0, 0 }, { 3, 1, 2, 3, 1, 1, 0, 0 }, { 3, 3, 2, 1, 1, 1, 0, 0 }, { 3, 1, 4, 1, 1, 1, 0, 0 }, { 2, 2, 1, 4, 1, 1, 0, 0 }, { 4, 3, 1, 1, 1, 1, 0, 0 }, { 1, 1, 1, 2, 2, 4, 0, 0 }, 
        { 1, 1, 1, 4, 2, 2, 0, 0 }, { 1, 2, 1, 1, 2, 4, 0, 0 }, { 1, 2, 1, 4, 2, 1, 0, 0 }, { 1, 4, 1, 1, 2, 2, 0, 0 }, { 1, 4, 1, 2, 2, 1, 0, 0 }, { 1, 1, 2, 2, 1, 4, 0, 0 }, { 1, 1, 2, 4, 1, 2, 0, 0 }, { 1, 2, 2, 1, 1, 4, 0, 0 }, { 1, 2, 2, 4, 1, 1, 0, 0 }, { 1, 4, 2, 1, 1, 2, 0, 0 }, { 1, 4, 2, 2, 1, 1, 0, 0 }, { 2, 4, 1, 2, 1, 1, 0, 0 }, { 2, 2, 1, 1, 1, 4, 0, 0 }, { 4, 1, 3, 1, 1, 1, 0, 0 }, { 2, 4, 1, 1, 1, 2, 0, 0 }, { 1, 3, 4, 1, 1, 1, 0, 0 }, 
        { 1, 1, 1, 2, 4, 2, 0, 0 }, { 1, 2, 1, 1, 4, 2, 0, 0 }, { 1, 2, 1, 2, 4, 1, 0, 0 }, { 1, 1, 4, 2, 1, 2, 0, 0 }, { 1, 2, 4, 1, 1, 2, 0, 0 }, { 1, 2, 4, 2, 1, 1, 0, 0 }, { 4, 1, 1, 2, 1, 2, 0, 0 }, { 4, 2, 1, 1, 1, 2, 0, 0 }, { 4, 2, 1, 2, 1, 1, 0, 0 }, { 2, 1, 2, 1, 4, 1, 0, 0 }, { 2, 1, 4, 1, 2, 1, 0, 0 }, { 4, 1, 2, 1, 2, 1, 0, 0 }, { 1, 1, 1, 1, 4, 3, 0, 0 }, { 1, 1, 1, 3, 4, 1, 0, 0 }, { 1, 3, 1, 1, 4, 1, 0, 0 }, { 1, 1, 4, 1, 1, 3, 0, 0 }, 
        { 1, 1, 4, 3, 1, 1, 0, 0 }, { 4, 1, 1, 1, 1, 3, 0, 0 }, { 4, 1, 1, 3, 1, 1, 0, 0 }, { 1, 1, 3, 1, 4, 1, 0, 0 }, { 1, 1, 4, 1, 3, 1, 0, 0 }, { 3, 1, 1, 1, 4, 1, 0, 0 }, { 4, 1, 1, 1, 3, 1, 0, 0 }, { 2, 1, 1, 4, 1, 2, 0, 0 }, { 2, 1, 1, 2, 1, 4, 0, 0 }, { 2, 1, 1, 2, 3, 2, 0, 0 }, { 2, 3, 3, 1, 1, 1, 2, 0 }
     };


        /// <summary>
        /// 到达开始时间委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void StartTimeArrived(object sender, EventArgs e);
        /// <summary>
        /// 到达开始时间事件
        /// </summary>
        public event StartTimeArrived OnStartTimeArrived;

        private XmlDocument _xmlDoc;
        /// <summary>
        /// XML文档
        /// </summary>
        public XmlDocument xmlDoc
        {
            get
            {
                return _xmlDoc;
            }
            set
            {
                _xmlDoc = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RecordTimer()
        {
            tm = new System.Timers.Timer(1000);    //1秒钟执行1次的定时器
            tm.AutoReset = true;     //一直执行
        
            tm.Elapsed += new ElapsedEventHandler(tm_Elapsed);  //定时器事件
        }

        void tm_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_xmlDoc != null)
            {
                int Hour = e.SignalTime.Hour;
                int Minute = e.SignalTime.Minute;
                int Second = e.SignalTime.Second;
                XmlNodeList nl = _xmlDoc.DocumentElement.SelectSingleNode("SyncTime").ChildNodes;
                bool isArrived = false;
                for (int i = 0; i < nl.Count; i++)
                {
                    DateTime StartTime = Convert.ToDateTime(nl[i].Attributes["StartTime"].Value);
                    if ((StartTime.Hour == Hour) && (StartTime.Minute == Minute) && (StartTime.Second == Second))
                    {
                        isArrived=true;
                       
                    }
                }
                if (isArrived)
                {
                   
                    if (OnStartTimeArrived != null)
                    
                        OnStartTimeArrived(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            tm.Enabled = true;
        }

        /// <summary>
        /// 结束
        /// </summary>
        public void Stop()
        {
            tm.Enabled = false;
        }

    }
    public sealed class ParamConfig
    {

        private static string m_ApplicationPath = "";
        /// <summary>
        /// 应用程序路径
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
                return m_ApplicationPath;
            }
            set
            {
                m_ApplicationPath = value;
            }
        }

        private static string m_DBServer = "";//DB Server
        /// <summary>
        /// 数据库服务器
        /// </summary>
        public static string DBServer
        {
            get
            {
                return m_DBServer;
            }
            set
            {
                m_DBServer = value;
            }
        }

        private static string m_Version = "";
        /// <summary>
        /// 程序版本
        /// </summary>
        public static string Version
        {
            get
            {
                return m_Version;
            }
            set
            {
                m_Version = value;
            }
        }

        private static int m_QueryDayNum = 30;
        /// <summary>
        /// 查询日期
        /// </summary>
        public static int QueryDayNum
        {
            get
            {
                return m_QueryDayNum;
            }
            set
            {
                m_QueryDayNum = value;
            }
        }

        #region 登录相关
        private static bool m_LoginFlag = false;//是否登录
        /// <summary>
        /// 是否登录
        /// </summary>
        public static bool LoginFlag
        {
            get
            {
                return m_LoginFlag;
            }
            set
            {
                m_LoginFlag = value;
            }
        }

        private static bool m_DBConnFlag = false;//是否连接DB
        /// <summary>
        /// 是否连接DB
        /// </summary>
        public static bool DBConnFlag
        {
            get
            {
                return m_DBConnFlag;
            }
            set
            {
                m_DBConnFlag = value;
            }
        }

        private static string m_LoginName = "";
        /// <summary>
        /// 登录姓名
        /// </summary>
        public static string LoginName
        {
            get
            {
                return m_LoginName;
            }
            set
            {
                m_LoginName = value;
            }
        }

        private static string m_LoginPassword = "";
        /// <summary>
        /// 登录密码
        /// </summary>
        public static string LoginPassword
        {
            get
            {
                return m_LoginPassword;
            }
            set
            {
                m_LoginPassword = value;
            }
        }

        private static int m_LoginID = 0;
        /// <summary>
        /// 登陆ID
        /// </summary>
        public static int LoginID
        {
            get
            {
                return m_LoginID;
            }
            set
            {
                m_LoginID = value;
            }
        }

        private static string m_LoginCode = "";
        /// <summary>
        /// 登录帐号
        /// </summary>
        public static string LoginCode
        {
            get
            {
                return m_LoginCode;
            }
            set
            {
                m_LoginCode = value;
            }
        }

        private static DateTime m_LoginTime = DateTime.Now;
        /// <summary>
        /// 登录时间
        /// </summary>
        public static DateTime LoginTime
        {
            get
            {
                return m_LoginTime;
            }
            set
            {
                m_LoginTime = value;
            }
        }

        private static int m_LoginLogID = 0;
        /// <summary>
        /// 登陆日志ID
        /// </summary>
        public static int LoginLogID
        {
            get
            {
                return m_LoginLogID;
            }
            set
            {
                m_LoginLogID = value;
            }
        }
        #endregion

        #region 注册表相关
        private static RegistryHive m_DBRegRoot = RegistryHive.CurrentUser;
        /// <summary>
        /// 数据库根路径设置
        /// </summary>
        public static RegistryHive DBRegRoot
        {
            get
            {
                return m_DBRegRoot;
            }
            set
            {
                m_DBRegRoot = value;
            }
        }

        private static string m_DBRegSubKey = "Software\\HardSoft\\HSop";
        /// <summary>
        /// 数据库注册表子路径
        /// </summary>
        public static string DBRegSubKey
        {
            get
            {
                return m_DBRegSubKey;
            }
            set
            {
                m_DBRegSubKey = value;
            }
        }

        #endregion
    }
        public static class Common
        {
            #region 读当前路径

            public static void GetApplicationPath()
            {
                string StartupPath = Application.StartupPath;
                if (StartupPath[StartupPath.Length - 1].ToString() != "\\")
                {
                    StartupPath = StartupPath + "\\";
                }
                ParamConfig.ApplicationPath = StartupPath;
            }
            #endregion

            #region 读写注册表
            /// <summary>
            /// 读注册表
            /// </summary>
            /// <param name="p_RegRoot">注册表根路径</param>
            /// <param name="p_SubKeyStr">子路径</param>
            /// <param name="p_KeyName">键名</param>
            /// <returns>结果</returns>
            public static string ReadReg(string p_KeyName)
            {
                try
                {
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey(ParamConfig.DBRegSubKey, false);
                    return regKey.GetValue(p_KeyName).ToString().Trim();
                }
                catch
                {
                    return "";
                }
            }

            /// <summary>
            /// 写注册表
            /// </summary>
            /// <param name="p_RegRoot">注册表根路径</param>
            /// <param name="p_SubKeyStr">子路径</param>
            /// <param name="p_KeyName">键名</param>
            /// <param name="p_Value">值</param>
            public static void WriteReg(string p_KeyName, string p_Value)
            {
                RegistryKey regKey = Registry.CurrentUser.CreateSubKey(ParamConfig.DBRegSubKey);
                regKey.SetValue(p_KeyName, p_Value);
            }
            #endregion

            #region 写日志文件
            /// <summary>
            /// 写日志文件
            /// </summary>
            /// <param name="p_Str">文件内容</param>
            //public static void WriteLog(string p_Str)
            //{
            //    try
            //    {
            //        string logpath = System.Windows.Forms.Application.StartupPath + SystemConfiguration.LogFile;
            //        string movelogpath = System.Windows.Forms.Application.StartupPath + SystemConfiguration.MoveLogFile;
            //        if (File.Exists(logpath))
            //        {
            //            FileInfo fi = new FileInfo(logpath);
            //            long mn = fi.Length;
            //            if (mn > SystemConfiguration.FrameowrkLogFileLength)//大于则删除文件
            //            {
            //                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\Log"))
            //                {
            //                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + @"\Log");
            //                }
            //                fi.MoveTo(movelogpath);
            //            }
            //        }
            //        StreamWriter sw = new StreamWriter(logpath, true, Encoding.Unicode);
            //        sw.WriteLine("\r\n" + DateTime.Now.ToString() + "=====================================\r\n" + p_Str);
            //        sw.Close();
            //    }
            //    catch
            //    {
            //    }
            //}
            #endregion

            #region 测试和设置数据库连接
            /// <summary>
            /// 测试数据库是否可以连接
            /// </summary>
            /// <returns>true/false</returns>
            //public static bool TestConnDB()
            //{
            //    try
            //    {
            //        SysUtil.Fill("SELECT N'Testing Connection...'");
            //        return true;
            //    }
            //    catch
            //    {
            //        return false;
            //    }
            //}

            /// <summary>
            /// 设置第一个数据库连接串
            /// </summary>
            //public static void SetDBFirst()
            //{
            //    string dbconn = string.Empty;
            //    string server = Common.ReadReg(PageConst.DBServer);
            //    string db = Common.ReadReg(PageConst.DBDataBase);
            //    string account = Common.ReadReg(PageConst.DBAccount);
            //    string pwd = Common.ReadReg(PageConst.DBPassword);
            //    if (account == "")
            //    {
            //        account = "sa";
            //    }
            //    if (pwd == "")
            //    {
            //        pwd = "";
            //    }
            //    Common.SetDBFirst(server, db, account, pwd);
            //}

            //public static void SetDBServer()
            //{
            //    string server = Common.ReadReg(PageConst.DBServer);
            //    ParamConfig.DBServer = server;
            //}

            /// <summary>
            /// 设置第一个数据库连接串
            /// </summary>
            /// <param name="p_Server">数据库服务器</param>
            /// <param name="p_DB">数据库</param>
            /// <param name="p_Account">帐号</param>
            /// <param name="p_Pwd">密码</param>
            //public static void SetDBFirst(string p_Server, string p_DB, string p_Account, string p_Pwd)
            //{
            //    string dbconn = string.Empty;
            //    dbconn = "server=" + p_Server + ";database=" + p_DB + ";uid=" + p_Account + ";pwd=" + p_Pwd + ";";
            //    SystemConfiguration.ConnectionString = dbconn;
            //}
            #endregion


            #region 通用数据绑定
            //public static void LoadDropRepositoryComb(RepositoryItemComboBox p_Drp, DataTable p_Dt, string p_DisplayField, bool p_ShowBlank)
            //{
            //    p_Drp.Items.Clear();
            //    if (p_ShowBlank)
            //    {
            //        p_Drp.Items.Add("");
            //    }
            //    foreach (DataRow dr in p_Dt.Rows)
            //    {
            //        p_Drp.Items.Add(dr[p_DisplayField]);
            //    }

            //    int i = 0;
            //    if (p_ShowBlank)
            //    {
            //        i = 1;
            //    }

            //    if (p_Dt.Rows.Count + i < 10)
            //    {
            //        p_Drp.DropDownRows = p_Dt.Rows.Count + i;
            //    }
            //    else
            //    {
            //        p_Drp.DropDownRows = 10;
            //    }
            //}

            //public static void LoadDropComb(ComboBoxEdit p_Drp, DataTable p_Dt, string p_DisplayField, bool p_ShowBlank)
            //{
            //    p_Drp.Properties.Items.Clear();
            //    if (p_ShowBlank)
            //    {
            //        p_Drp.Properties.Items.Add("");
            //    }

            //    foreach (DataRow dr in p_Dt.Rows)
            //    {
            //        p_Drp.Properties.Items.Add(dr[p_DisplayField]);
            //    }

            //    int i = 0;
            //    if (p_ShowBlank)
            //    {
            //        i = 1;
            //    }

            //    if (p_Dt.Rows.Count + i < 10)
            //    {
            //        p_Drp.Properties.DropDownRows = p_Dt.Rows.Count + i;
            //    }
            //    else
            //    {
            //        p_Drp.Properties.DropDownRows = 10;
            //    }
            //}

            //public static void LoadDropItemComb(RepositoryItemComboBox p_Drp, DataTable p_Dt, string p_DisplayField, bool p_ShowBlank)
            //{
            //    p_Drp.Items.Clear();
            //    if (p_ShowBlank)
            //    {
            //        p_Drp.Items.Add("");
            //    }

            //    foreach (DataRow dr in p_Dt.Rows)
            //    {
            //        p_Drp.Items.Add(dr[p_DisplayField]);

            //    }

            //    int i = 0;
            //    if (p_ShowBlank)
            //    {
            //        i = 1;
            //    }


            //    if (p_Dt.Rows.Count + i < 10)
            //    {
            //        p_Drp.DropDownRows = p_Dt.Rows.Count + i;
            //    }
            //    else
            //    {
            //        p_Drp.DropDownRows = 10;
            //    }
            //}

            //public static void LoadDropLookUP(LookUpEdit p_Drp, DataTable p_Dt, string p_DisplayField, string p_ValueField, bool p_ShowBlank)
            //{
            //    p_Drp.Properties.ValueMember = p_ValueField;
            //    p_Drp.Properties.DisplayMember = p_DisplayField;
            //    if (p_ShowBlank)
            //    {
            //        DataRow dr = p_Dt.NewRow();
            //        p_Dt.Rows.Add(dr);
            //    }

            //    int i = 0;
            //    if (p_ShowBlank)
            //    {
            //        i = 1;
            //    }

            //    p_Drp.Properties.DataSource = p_Dt;
            //    if (p_Dt.Rows.Count + i < 10)
            //    {
            //        p_Drp.Properties.DropDownRows = p_Dt.Rows.Count + i;
            //    }
            //    else
            //    {
            //        p_Drp.Properties.DropDownRows = 10;
            //    }
            //}

            //public static void LoadDropLookUP(RepositoryItemLookUpEdit p_Drp, DataTable p_Dt, string p_DisplayField, string p_ValueField, bool p_ShowBlank)
            //{
            //    p_Drp.ValueMember = p_ValueField;
            //    p_Drp.DisplayMember = p_DisplayField;
            //    if (p_ShowBlank)
            //    {
            //        DataRow dr = p_Dt.NewRow();
            //        p_Dt.Rows.Add(dr);
            //    }

            //    p_Drp.DataSource = p_Dt;

            //    int i = 0;
            //    if (p_ShowBlank)
            //    {
            //        i = 1;
            //    }


            //    if (p_Dt.Rows.Count + i < 10)
            //    {
            //        p_Drp.DropDownRows = p_Dt.Rows.Count + i;
            //    }
            //    else
            //    {
            //        p_Drp.DropDownRows = 10;
            //    }
            //}

            //public static void LoadDropRepositoryLookUP(RepositoryItemLookUpEdit p_Drp, DataTable p_Dt, string p_DisplayField, string p_ValueField, bool p_ShowBlank)
            //{
            //    p_Drp.ValueMember = p_ValueField;
            //    p_Drp.DisplayMember = p_DisplayField;
            //    if (p_ShowBlank)
            //    {
            //        DataRow dr = p_Dt.NewRow();
            //        p_Dt.Rows.Add(dr);
            //    }

            //    int i = 0;
            //    if (p_ShowBlank)
            //    {
            //        i = 1;
            //    }

            //    p_Drp.DataSource = p_Dt;
            //    if (p_Dt.Rows.Count + i < 10)
            //    {
            //        p_Drp.DropDownRows = p_Dt.Rows.Count + i;
            //    }
            //    else
            //    {
            //        p_Drp.DropDownRows = 10;
            //    }
            //}

            //public static void LoadListBoxControl(ListBoxControl p_Drp, DataTable p_Dt, string p_DisplayField, string p_ValueField)
            //{
            //    p_Drp.ValueMember = p_ValueField;
            //    p_Drp.DisplayMember = p_DisplayField;

            //    p_Drp.DataSource = p_Dt;
            //}

            //public static void LoadCheckBoxControl(CheckedListBoxControl p_Chk, DataTable p_Dt, string p_DisplayField, string p_ValueField)
            //{
            //    p_Chk.DisplayMember = p_DisplayField;
            //    p_Chk.ValueMember = p_ValueField;

            //    p_Chk.DataSource = p_Dt;
            //}
            //#endregion

            //#region 数据绑定
            ///// <summary>
            ///// 绑定用户
            ///// </summary>
            ///// <param name="p_Drp">下拉框</param>
            ///// <param name="p_Dt">数据源</param>
            ///// <param name="p_DisplayField">显示字段</param>
            ///// <param name="p_ValueField">值字段</param>
            ///// <param name="p_ShowFlag">是否显示空行</param>
            //public static void BindUser(LookUpEdit p_Drp, bool p_ShowFlag)
            //{
            //    string sql = "SELECT ID,Name FROM Data_User WHERE DeleteFlag=0 AND DefaultFlag = 0 ";
            //    DataTable dt = SysUtil.Fill(sql);
            //    Common.LoadDropLookUP(p_Drp, dt, "Name", "ID", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定角色
            ///// </summary>
            ///// <param name="lbcRole"></param>
            ///// <param name="p_ShowFlag"></param>
            //public static void BindRole(ListBoxControl lbcRole, bool p_ShowFlag)
            //{
            //    string sql = "SELECT ID,Name FROM Data_Role WHERE DeleteFlag = 0 ";
            //    DataTable dt = SysUtil.Fill(sql);
            //    Common.LoadListBoxControl(lbcRole, dt, "Name", "ID");
            //}

            ///// <summary>
            ///// 绑定角色
            ///// </summary>
            ///// <param name="lbcRole"></param>
            ///// <param name="p_ShowFlag"></param>
            //public static void BindRole(CheckedListBoxControl lbcRole, bool p_ShowFlag)
            //{
            //    string sql = "SELECT ID,Name FROM Data_Role WHERE DeleteFlag = 0 ";
            //    DataTable dt = SysUtil.Fill(sql);
            //    Common.LoadCheckBoxControl(lbcRole, dt, "Name", "ID");
            //}

            ///// <summary>
            ///// 绑定用户
            ///// </summary>
            ///// <param name="p_Drp">下拉框</param>
            ///// <param name="p_Dt">数据源</param>
            ///// <param name="p_DisplayField">显示字段</param>
            ///// <param name="p_ValueField">值字段</param>
            ///// <param name="p_ShowFlag">是否显示空行</param>
            //public static void BindUser(RepositoryItemLookUpEdit p_Drp, bool p_ShowFlag)
            //{
            //    string sql = "SELECT ID,Name FROM Data_User WHERE DeleteFlag=0 AND DefaultFlag = 0 ";
            //    DataTable dt = SysUtil.Fill(sql);
            //    Common.LoadDropRepositoryLookUP(p_Drp, dt, "Name", "ID", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定菜单
            ///// </summary>
            ///// <param name="p_Chk">绑定控件</param>
            ///// <param name="p_DisplayField">显示字段</param>
            ///// <param name="p_ValueField">值字段</param>
            //public static void BindMenu(CheckedListBoxControl p_Chk)
            //{
            //    string sql = "SELECT ID,Name FROM Sys_SystemMenu WHERE ParentID NOT IN(0,1) AND ShowFlag=1 ";
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadCheckBoxControl(p_Chk, dt, "Name", "ID");
            //}

            ///// <summary>
            ///// 绑定类别
            ///// </summary>
            ///// <param name="lbcType">列表控件</param>
            ///// <param name="p_DisplayField">显示字段</param>
            ///// <param name="p_ValueField">值字段</param>
            //public static void BindType(ListBoxControl lbcType)
            //{
            //    string sql = "SELECT ID,Name FROM Data_DownType WHERE AllowEdit=0 ";
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadListBoxControl(lbcType, dt, "Name", "ID");
            //}

            ///// <summary>
            ///// 绑定卡批次
            ///// </summary>
            ///// <param name="p_Drp">列表控件</param>
            ///// <param name="p_ShowFlag">是否显示空行</param>
            //public static void BindCardPC(LookUpEdit p_Drp, bool p_ShowFlag)
            //{
            //    string sql = "SELECT CardPC_ID,CardPC_Name FROM ICYQSHSF..T_CardPc";
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropLookUP(p_Drp, dt, "CardPC_Name", "CardPC_ID", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定身份类型
            ///// </summary>
            ///// <param name="p_Drp">列表控件</param>
            ///// <param name="p_ShowFlag">是否显示空行</param>
            //public static void BindIDType(LookUpEdit p_Drp, bool p_ShowFlag)
            //{
            //    string sql = "SELECT ID_TypeID,ID_TypeName FROM ICYQSHSF..T_ManType";
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropLookUP(p_Drp, dt, "ID_TypeName", "ID_TypeID", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定读者类型
            ///// </summary>
            ///// <param name="p_Drp">列表控件</param>
            ///// <param name="p_ShowFlag">是否显示空行</param>
            //public static void BindReaderType(ComboBoxEdit p_Drp, bool p_ShowFlag)
            //{
            //    string sql = "SELECT Zdmc FROM YQSHTS..Tsys_zd WHERE Zdlb='读者身份'";
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropComb(p_Drp, dt, "Zdmc", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定类别明细
            ///// </summary>
            ///// <param name="lbcType">列表控件</param>
            ///// <param name="p_DisplayField">显示字段</param>
            ///// <param name="p_ValueField">值字段</param>
            //public static void BindTypeDts(LookUpEdit p_Drp, int typeID, bool p_ShowFlag)
            //{
            //    string sql = "SELECT TypeID,TypeIndex,TypeValue,TypeValue1,TypeValue2,TypeValue3,TypeValue4,TypeValue5,TypeValue+CASE WHEN TypeValue1 IS NULL THEN '' WHEN TypeValue1 = '' THEN '' WHEN TypeValue1 IS NOT NULL THEN '→'+TypeValue1 END+CASE WHEN TypeValue2 IS NULL THEN '' WHEN TypeValue2 = '' THEN '' WHEN TypeValue2 IS NOT NULL THEN '→'+TypeValue2 END+CASE WHEN TypeValue3 IS NULL THEN '' WHEN TypeValue3 = '' THEN '' WHEN TypeValue3 IS NOT NULL THEN '→'+TypeValue3 END+CASE WHEN TypeValue4 IS NULL THEN '' WHEN TypeValue4 = '' THEN '' WHEN TypeValue4 IS NOT NULL THEN '→'+TypeValue4 END+CASE WHEN TypeValue5 IS NULL THEN '' WHEN TypeValue5 = '' THEN '' WHEN TypeValue5 IS NOT NULL THEN '→'+TypeValue5 END AS AllValue FROM Data_DownTypeDts WHERE TypeID=" + typeID;
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropLookUP(p_Drp, dt, "TypeValue", "TypeIndex", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定部门类型
            ///// </summary>
            ///// <param name="lbcType">列表控件</param>
            //public static void BindDeptType(LookUpEdit p_Drp, bool p_ShowFlag)
            //{
            //    string sql = "SELECT ID,DeptTypeName FROM Data_DeptType";
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropLookUP(p_Drp, dt, "DeptTypeName", "ID", p_ShowFlag);
            //}
            ///// <summary>
            ///// 绑定部门类型
            ///// </summary>
            ///// <param name="lbcType">列表控件</param>
            //public static void BindDeptType(CheckedListBoxControl p_Drp)
            //{
            //    string sql = "SELECT ID,DeptTypeName FROM Data_DeptType";
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadCheckBoxControl(p_Drp, dt, "DeptTypeName", "ID");
            //}
            ///// <summary>
            ///// 绑定部门
            ///// </summary>
            ///// <param name="p_Drp">列表控件</param>
            ///// <param name="p_ShowFlag">是否显示空行</param>
            //public static void BindDept(RepositoryItemLookUpEdit p_Drp, bool p_ShowFlag)
            //{
            //    string sql = "SELECT ID,DeptName FROM Data_Dept WHERE DeleteFlag=0 ORDER BY SortIndex";
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropLookUP(p_Drp, dt, "DeptName", "ID", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定类别明细
            ///// </summary>
            ///// <param name="lbcType">列表控件</param>
            ///// <param name="p_DisplayField">显示字段</param>
            ///// <param name="p_ValueField">值字段</param>
            //public static void BindTypeDts(RepositoryItemLookUpEdit p_Drp, int typeID, bool p_ShowFlag)
            //{
            //    string sql = "SELECT TypeID,TypeIndex,TypeValue,TypeValue1,TypeValue2,TypeValue3,TypeValue4,TypeValue5,TypeValue+CASE WHEN TypeValue1 IS NULL THEN '' WHEN TypeValue1 = '' THEN '' WHEN TypeValue1 IS NOT NULL THEN '→'+TypeValue1 END+CASE WHEN TypeValue2 IS NULL THEN '' WHEN TypeValue2 = '' THEN '' WHEN TypeValue2 IS NOT NULL THEN '→'+TypeValue2 END+CASE WHEN TypeValue3 IS NULL THEN '' WHEN TypeValue3 = '' THEN '' WHEN TypeValue3 IS NOT NULL THEN '→'+TypeValue3 END+CASE WHEN TypeValue4 IS NULL THEN '' WHEN TypeValue4 = '' THEN '' WHEN TypeValue4 IS NOT NULL THEN '→'+TypeValue4 END+CASE WHEN TypeValue5 IS NULL THEN '' WHEN TypeValue5 = '' THEN '' WHEN TypeValue5 IS NOT NULL THEN '→'+TypeValue5 END AS AllValue FROM Data_DownTypeDts WHERE TypeID=" + typeID;
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropRepositoryLookUP(p_Drp, dt, "TypeValue", "TypeIndex", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定类别明细
            ///// </summary>
            ///// <param name="lbcType">列表控件</param>
            ///// <param name="p_DisplayField">显示字段</param>
            ///// <param name="p_ValueField">值字段</param>
            //public static void BindWorkUnit(ComboBoxEdit p_Drp, int typeID, bool p_ShowFlag)
            //{
            //    string sql = "SELECT TypeID,TypeIndex,TypeValue,TypeValue1,TypeValue2,TypeValue3,TypeValue4,TypeValue5,TypeValue+CASE WHEN TypeValue1 IS NULL THEN '' WHEN TypeValue1 = '' THEN '' WHEN TypeValue1 IS NOT NULL THEN '→'+TypeValue1 END+CASE WHEN TypeValue2 IS NULL THEN '' WHEN TypeValue2 = '' THEN '' WHEN TypeValue2 IS NOT NULL THEN '→'+TypeValue2 END+CASE WHEN TypeValue3 IS NULL THEN '' WHEN TypeValue3 = '' THEN '' WHEN TypeValue3 IS NOT NULL THEN '→'+TypeValue3 END+CASE WHEN TypeValue4 IS NULL THEN '' WHEN TypeValue4 = '' THEN '' WHEN TypeValue4 IS NOT NULL THEN '→'+TypeValue4 END+CASE WHEN TypeValue5 IS NULL THEN '' WHEN TypeValue5 = '' THEN '' WHEN TypeValue5 IS NOT NULL THEN '→'+TypeValue5 END AS AllValue FROM Data_DownTypeDts WHERE TypeID=" + typeID;
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropComb(p_Drp, dt, "AllValue", p_ShowFlag);
            //}

            ///// <summary>
            ///// 绑定类别明细
            ///// </summary>
            ///// <param name="lbcType">列表控件</param>
            ///// <param name="p_DisplayField">显示字段</param>
            ///// <param name="p_ValueField">值字段</param>
            //public static void BindWorkUnit(RepositoryItemComboBox p_Drp, int typeID, bool p_ShowFlag)
            //{
            //    string sql = "SELECT TypeID,TypeIndex,TypeValue,TypeValue1,TypeValue2,TypeValue3,TypeValue4,TypeValue5,TypeValue+CASE WHEN TypeValue1 IS NULL THEN '' WHEN TypeValue1 = '' THEN '' WHEN TypeValue1 IS NOT NULL THEN '→'+TypeValue1 END+CASE WHEN TypeValue2 IS NULL THEN '' WHEN TypeValue2 = '' THEN '' WHEN TypeValue2 IS NOT NULL THEN '→'+TypeValue2 END+CASE WHEN TypeValue3 IS NULL THEN '' WHEN TypeValue3 = '' THEN '' WHEN TypeValue3 IS NOT NULL THEN '→'+TypeValue3 END+CASE WHEN TypeValue4 IS NULL THEN '' WHEN TypeValue4 = '' THEN '' WHEN TypeValue4 IS NOT NULL THEN '→'+TypeValue4 END+CASE WHEN TypeValue5 IS NULL THEN '' WHEN TypeValue5 = '' THEN '' WHEN TypeValue5 IS NOT NULL THEN '→'+TypeValue5 END AS AllValue FROM Data_DownTypeDts WHERE TypeID=" + typeID;
            //    DataTable dt = SysUtil.Fill(sql);
            //    LoadDropRepositoryComb(p_Drp, dt, "AllValue", p_ShowFlag);
            //}
            //#endregion

            //#region 校验身份证号码
            ///// <summary>
            ///// 校验身份证号
            ///// </summary>
            ///// <param name="IDCard">18位身份证号</param>
            ///// <returns></returns>
            //public static string CheckIDCard(string IDCard)
            //{
            //    string[] aCity = new string[] { null, null, null, null, null, null, null, null, null, null, null, 
            //    "北京", "天津", "河北", "山西", "内蒙古", null, null, null, null, null, "辽宁", "吉林", "黑龙江", 
            //    null, null, null, null, null, null, null, "上海", "江苏", "浙江", "安微", "福建", "江西", "山东", 
            //    null, null, null, "河南", "湖北", "湖南", "广东", "广西", "海南", null, null, null, "重庆", "四川", 
            //    "贵州", "云南", "西藏", null, null, null, null, null, null, "陕西", "甘肃", "青海", "宁夏", "新疆", 
            //    null, null, null, null, null, "台湾", null, null, null, null, null, null, null, null, null, "香港", 
            //    "澳门", null, null, null, null, null, null, null, null, "国外" };

            //    double iSum = 0;
            //    System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^\d{17}(\d|x)$");
            //    System.Text.RegularExpressions.Match mc = rg.Match(IDCard.ToLower());
            //    if (!mc.Success)
            //    {
            //        return "身份证号错误";
            //    }
            //    IDCard = IDCard.Replace("x", "a").Replace("X", "a");
            //    if (aCity[int.Parse(IDCard.Substring(0, 2))] == null)
            //    {
            //        return "身份证号错误";
            //    }
            //    try
            //    {
            //        DateTime.Parse(IDCard.Substring(6, 4) + "-" + IDCard.Substring(10, 2) + "-" + IDCard.Substring(12, 2));
            //    }
            //    catch
            //    {
            //        return "身份证号错误";
            //    }
            //    for (int i = 17; i >= 0; i--)
            //    {
            //        iSum += (System.Math.Pow(2, i) % 11) * int.Parse(IDCard[17 - i].ToString(), System.Globalization.NumberStyles.HexNumber);
            //    }
            //    if (iSum % 11 != 1)
            //        return "身份证号错误";

            //    return string.Empty;
            //    //return (aCity[int.Parse(IDCard.Substring(0, 2))] + "," + IDCard.Substring(6, 4) + "-" + IDCard.Substring(10, 2) + "-" + IDCard.Substring(12, 2) + "," + (int.Parse(IDCard.Substring(16, 1)) % 2 == 1 ? "男" : "女"));
            //}

            ///// <summary>
            ///// 身份证15位转18位
            ///// </summary>
            //public static string ToEighteen(string IDCard)
            //{
            //    string[] aCity = new string[] { null, null, null, null, null, null, null, null, null, null, null, 
            //    "北京", "天津", "河北", "山西", "内蒙古", null, null, null, null, null, "辽宁", "吉林", "黑龙江", 
            //    null, null, null, null, null, null, null, "上海", "江苏", "浙江", "安微", "福建", "江西", "山东", 
            //    null, null, null, "河南", "湖北", "湖南", "广东", "广西", "海南", null, null, null, "重庆", "四川", 
            //    "贵州", "云南", "西藏", null, null, null, null, null, null, "陕西", "甘肃", "青海", "宁夏", "新疆", 
            //    null, null, null, null, null, "台湾", null, null, null, null, null, null, null, null, null, "香港", 
            //    "澳门", null, null, null, null, null, null, null, null, "国外" };

            //    double iSum = 0;
            //    System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^\d{15}$");
            //    System.Text.RegularExpressions.Match mc = rg.Match(IDCard);
            //    if (!mc.Success)
            //    {
            //        return "身份证号错误";
            //    }
            //    IDCard = IDCard.ToLower();
            //    if (aCity[int.Parse(IDCard.Substring(0, 2))] == null)
            //    {
            //        return "身份证号错误";
            //    }
            //    try
            //    {
            //        DateTime.Parse("19" + IDCard.Substring(6, 2) + "-" + IDCard.Substring(8, 2) + "-" + IDCard.Substring(10, 2));
            //    }
            //    catch
            //    {
            //        return "身份证号错误";
            //    }
            //    IDCard = IDCard.Substring(0, 6) + "19" + IDCard.Substring(6);
            //    for (int i = 17; i > 0; i--)
            //    {
            //        iSum += (Math.Pow(2, i) % 11) * SysConvert.ToInt32(IDCard[17 - i].ToString());
            //    }
            //    double spe = iSum % 11;
            //    string speStr = string.Empty;

            //    IDictionary<int, string> dic = new Dictionary<int, string>();

            //    dic.Add(0, "1");
            //    dic.Add(1, "0");
            //    dic.Add(2, "X");
            //    dic.Add(3, "9");
            //    dic.Add(4, "8");
            //    dic.Add(5, "7");
            //    dic.Add(6, "6");
            //    dic.Add(7, "5");
            //    dic.Add(8, "4");
            //    dic.Add(9, "3");
            //    dic.Add(10, "2");

            //    for (int i = 0; i < 11; i++)
            //    {
            //        if (i == spe)
            //        {
            //            speStr = dic[i];
            //        }
            //    }

            //    return IDCard + speStr;
            //}
            //#endregion

            //#region 生成系统编号
            ///// <summary>
            ///// 获得系统配置编号
            ///// </summary>
            ///// <returns></returns>
            //public static string GetSystemCode(int paramCodeID)
            //{
            //    string ParamCode = string.Empty;
            //    string sql = "SELECT * FROM Data_ParamCode WHERE IsEnable=1 AND ID=" + paramCodeID;
            //    DataTable dtTemp = SysUtil.Fill(sql);
            //    if (dtTemp.Rows.Count != 0)
            //    {
            //        string SpeStr = string.Empty;
            //        string Year = string.Empty;
            //        string Month = string.Empty;
            //        string Day = string.Empty;
            //        int CurNum = 0;
            //        int CurNumLength = 0;

            //        SpeStr = SysConvert.ToString(dtTemp.Rows[0]["ConstChar"]);
            //        Year = SysConvert.ToString(dtTemp.Rows[0]["Year"]);
            //        Month = SysConvert.ToString(dtTemp.Rows[0]["Month"]);
            //        Day = SysConvert.ToString(dtTemp.Rows[0]["Day"]);
            //        CurNum = SysConvert.ToInt32(dtTemp.Rows[0]["CurNum"]);
            //        CurNumLength = SysConvert.ToInt32(dtTemp.Rows[0]["CurNumLength"]);

            //        bool NeedUpdate = false;

            //        if (Year != "0" && Year != DateTime.Now.Year.ToString())
            //        {
            //            Year = DateTime.Now.Year.ToString();
            //            NeedUpdate = true;
            //        }
            //        if (Month != "0" && Month != DateTime.Now.Month.ToString().PadLeft(2, '0'))
            //        {
            //            Month = DateTime.Now.Month.ToString().PadLeft(2, '0');
            //            NeedUpdate = true;
            //        }
            //        if (Day != "0" && Day != DateTime.Now.Day.ToString().PadLeft(2, '0'))
            //        {
            //            Day = DateTime.Now.Day.ToString().PadLeft(2, '0');
            //            NeedUpdate = true;
            //        }
            //        if (NeedUpdate)
            //        {
            //            CurNum = 1;//Year='" + Year + "',Month='" + Month + "',Day='" + Day + "',
            //            sql = "UPDATE Data_ParamCode SET CurNum=" + CurNum;
            //            if (Year != "0")//更新年
            //            {
            //                sql += ",Year='" + Year + "'";
            //            }
            //            if (Month != "0")
            //            {
            //                sql += ",Month='" + Month + "'";
            //            }
            //            if (Day != "0")
            //            {
            //                sql += ",Day='" + Day + "'";
            //            }
            //            sql += " WHERE ID=" + paramCodeID;
            //            SysUtil.Update(sql);
            //        }
            //        else
            //        {
            //            CurNum = CurNum + 1;
            //            sql = "UPDATE Data_ParamCode SET CurNum=" + CurNum + " WHERE ID=" + paramCodeID;
            //            SysUtil.Update(sql);
            //        }
            //        ParamCode = SpeStr + (Year == "0" ? "" : Year.Substring(2)) + (Month == "0" ? "" : Month) + (Day == "0" ? "" : Day) + CurNum.ToString().PadLeft(CurNumLength, '0');
            //    }
            //    else
            //    {
            //        ParamCode = "SystemConfigError";
            //    }
            //    return ParamCode;
            //}
            #endregion
        }
    }
 
