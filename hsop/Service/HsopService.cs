namespace Hsop.Service
{
    using hsopPlatform;
    using HS.Platform;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceProcess;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
    [DesignerGenerated]
    public class ShinemayService : ServiceBase
    {
        private IContainer components;
        private static EventLog m_evtLog = null;
        private static Thread m_thdAutoImport = null;
        private static Thread m_thdAutoRun = null;
        private static Thread m_thdAutosendEmail = null;
        private static Thread m_thdAutosendSms = null;
        private static Thread m_thdDictSave = null;
        private static Thread m_thdAutosendSmsWs = null;
        private static Process m_process = null;
         
        public static XmlDocument m_servicexlm = new XmlDocument();
        
        public static string m_servicename = "Shinemay";
        string path = AppDomain.CurrentDomain.BaseDirectory;

        [DebuggerNonUserCode]
        public ShinemayService()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ServiceName = m_servicename;
        }

        [DebuggerNonUserCode, MTAThread]
        public static void Main()
        {
            


            m_servicexlm.Load("d:\\config.xml");
            m_servicename = Convert.ToString(m_servicexlm.DocumentElement.SelectSingleNode("//ShinemayServiceName").InnerText); 
            ServiceBase.Run(new ServiceBase[] { new ShinemayService() });
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                m_evtLog = new EventLog();
                if (!EventLog.SourceExists(m_servicename))
                {
                    EventLog.CreateEventSource(m_servicename, "Application");
                }
                m_evtLog.Source = m_servicename;
                m_evtLog.WriteEntry(m_servicename + " Started");

                HsopCmsEnvironment.InitForClientApplication(Application.StartupPath, m_servicename + ".log", true);
                SLog.LogLevel = SLog.LOG_LEVEL.Err;
                SLogDb.LogLevel = SLog.LOG_LEVEL.NoLog;
             
                m_evtLog.WriteEntry("Hsop app folder: " + CmsConfig.GetFolderOfProjectRoot);
                Exception VBt_refS0 = null;
                SLog.Crucial("Hsop app folder: " + CmsConfig.GetFolderOfProjectRoot, ref VBt_refS0, false);
                m_evtLog.WriteEntry("Hsop database config: " + CmsDbConfig.GetConfigFilePath());
                VBt_refS0 = null;
                SLog.Crucial("Hsop app_database: " + CmsDbConfig.GetConfigFilePath(), ref VBt_refS0, false);

                m_evtLog.WriteEntry("Hsop db1: " + CmsDbConfig.GetDbHost() + " / " + CmsDbConfig.GetDbPort() + " / " + CmsDbConfig.GetCmsDbName() + " / " + CmsDbConfig.GetCmsDocDbName());
                VBt_refS0 = null;
                SLog.Crucial("Hsop db1: " + CmsDbConfig.GetDbHost() + " / " + CmsDbConfig.GetDbPort() + " / " + CmsDbConfig.GetCmsDbName() + " / " + CmsDbConfig.GetCmsDocDbName(), ref VBt_refS0, false);
                VBt_refS0 = null;
                SLog.Crucial("Hsop app_config: " + CmsConfig.GetConfigFilePath(), ref VBt_refS0, false);
                VBt_refS0 = null;
                SLog.Crucial("Hsop app_msg_cn: " + CmsMessage.GetConfigFilePathCn(), ref VBt_refS0, false);
                VBt_refS0 = null;
                SLog.Crucial("Hsop app_msg_en: " + CmsMessage.GetConfigFilePathEn(), ref VBt_refS0, false);
                m_thdAutoImport = new Thread(new ThreadStart(AutoImportLogic.Run));
                m_thdAutoImport.Start();
                m_thdDictSave = new Thread(new ThreadStart(DictionarySaveLogic.Run));
                m_thdDictSave.Start();
                m_thdAutosendEmail = new Thread(new ThreadStart(AutoSendEmailLogic.Run));
                m_thdAutosendEmail.Start();
                m_thdAutosendSms = new Thread(new ThreadStart(AutoSendSmsLogic.Run));
                m_thdAutosendSms.Start();
                m_thdAutoRun = new Thread(new ThreadStart(AutoRunCodeLogic.Run));
                m_thdAutoRun.Start();
                m_thdAutosendSmsWs = new Thread(new ThreadStart(AutoSendSmsWsLogic.Run));
                m_thdAutosendSmsWs.Start();
                path = CmsConfig.GetFolderOfProjectRoot + "collector\\";
                Exception ex = null;
                SLog.Crucial("Hsop StartProgram: " + path + @"RealsunMiniServerDac.exe",ref ex);
                StartProgram(path + @"RealsunMiniServerDac.exe");
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception ex = exception1;
                m_evtLog.WriteEntry("AutoImport unknown exception in OnStart(). Message: " + ex.Message);
                ProjectData.ClearProjectError();
            }
        }
        /// <summary>
        /// 启动所有要启动的程序 ProgramPath：完整路径
        /// </summary>
        private void StartProgram(string ProgramPath)
        {
            try
            {

              
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(ProgramPath);
                    if (!IsExistProcess(fileName))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(ProgramPath);
                        startInfo.WorkingDirectory = path;
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        m_process = Process.Start(startInfo);
                        LogWrite("Winform: " + fileName + " started.");
                        LogWrite("startInfo.WorkingDirectory: " + startInfo.WorkingDirectory);
                    }
 

            }
            catch (Exception err)
            {
                LogWrite(err.Message);
            }
        }
        /// <summary>
        /// 写日志
        /// </summary>
        public void LogWrite(string str)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path + @"DemoLog.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + str);
            }
        }


        /// <summary>
        /// 检查该进程是否已启动
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        private bool IsExistProcess(string processName)
        {
            Process[] MyProcesses = Process.GetProcesses();
            foreach (Process MyProcess in MyProcesses)
            {
                if (MyProcess.ProcessName.CompareTo(processName) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnStop()
        {
            Exception VBt_refS0;
            try
            {
                m_thdAutoImport.Abort();
                m_thdDictSave.Abort();
                m_thdAutosendEmail.Abort();
                m_thdAutosendSms.Abort();
                m_thdAutoRun.Abort();
                m_process.Close();
                VBt_refS0 = null;
                SLog.Crucial(m_servicename + " stopped.", ref VBt_refS0, false);
                SLog.Crucial(m_process.ProcessName + " Closed.", ref VBt_refS0, false);
                if (m_evtLog != null)
                {
                    m_evtLog.WriteEntry(m_servicename+" stopped.");
                    m_evtLog.WriteEntry(m_process.ProcessName + " Closed.");
                }
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception ex = exception1;
                string strErrMsg = "Error to stop " + m_servicename + ". Message: " + ex.Message;
                VBt_refS0 = null;
                SLog.Crucial(strErrMsg, ref VBt_refS0, false);
                if (m_evtLog != null)
                {
                    m_evtLog.WriteEntry(strErrMsg);
                }
                ProjectData.ClearProjectError();
            }
        }
    }
}

