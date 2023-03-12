namespace Hsop.Service
{
    using System;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.ServiceProcess;
    using hsopPlatform;
    using HS.Platform;
    using System.Windows.Forms;
    using System.Xml;
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {

        [AccessedThroughProperty("ServiceInstaller1")]
        private System.ServiceProcess.ServiceInstaller _ServiceInstaller1;
        [AccessedThroughProperty("ServiceProcessInstaller1")]
        private ServiceProcessInstaller _ServiceProcessInstaller1;
        private IContainer components;
        public static XmlDocument m_servicexlm = new XmlDocument();
        public static string m_servicename = "ShinemayService";
        public ServiceInstaller()
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
            m_servicexlm.Load( "d:\\config.xml");
            m_servicename = Convert.ToString(m_servicexlm.DocumentElement.SelectSingleNode("//ShinemayServiceName").InnerText);
            this.ServiceProcessInstaller1 = new ServiceProcessInstaller();
            this.ServiceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            this.ServiceProcessInstaller1.Account = ServiceAccount.LocalSystem;
            this.ServiceProcessInstaller1.Password = null;
            this.ServiceProcessInstaller1.Username = null;
            this.ServiceInstaller1.DisplayName = m_servicename;
            this.ServiceInstaller1.ServiceName = m_servicename;
            this.ServiceInstaller1.StartType = ServiceStartMode.Automatic;
            this.Installers.AddRange(new Installer[]{this.ServiceProcessInstaller1,this.ServiceInstaller1});
        }

        internal virtual System.ServiceProcess.ServiceInstaller ServiceInstaller1
        {
            [DebuggerNonUserCode]
            get
            {
                return this._ServiceInstaller1;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._ServiceInstaller1 = value;
            }
        }

        internal virtual ServiceProcessInstaller ServiceProcessInstaller1
        {
            [DebuggerNonUserCode]
            get
            {
                return this._ServiceProcessInstaller1;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._ServiceProcessInstaller1 = value;
            }
        }
        //为当前Windows服务设置可与桌面交互选项，否则winform程序没有显示界面
      
    }
}

