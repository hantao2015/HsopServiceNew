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
        public static XmlDocument m_servicexlm = new XmlDocument();
        public static string m_servicename = "Shinemay";
       
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
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception ex = exception1;
                m_evtLog.WriteEntry("AutoImport unknown exception in OnStart(). Message: " + ex.Message);
                ProjectData.ClearProjectError();
            }
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
                VBt_refS0 = null;
                SLog.Crucial(m_servicename + " stopped.", ref VBt_refS0, false);
                if (m_evtLog != null)
                {
                    m_evtLog.WriteEntry(m_servicename+" stopped.");
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

