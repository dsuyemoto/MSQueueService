namespace MSMQHandlerService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.MSMQCicHandler = new System.ServiceProcess.ServiceInstaller();
            this.MSMQServiceNowHandler = new System.ServiceProcess.ServiceInstaller();
            this.MSMQImanageHandler = new System.ServiceProcess.ServiceInstaller();
            this.MSMQEwsHandler = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // MSMQCicHandler
            // 
            this.MSMQCicHandler.DelayedAutoStart = true;
            this.MSMQCicHandler.ServiceName = "MSMQ Cic Handler";
            this.MSMQCicHandler.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // MSMQServiceNowHandler
            // 
            this.MSMQServiceNowHandler.DelayedAutoStart = true;
            this.MSMQServiceNowHandler.ServiceName = "MSMQ ServiceNow Handler";
            this.MSMQServiceNowHandler.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // MSMQImanageHandler
            // 
            this.MSMQImanageHandler.DelayedAutoStart = true;
            this.MSMQImanageHandler.ServiceName = "MSMQ Imanage Handler";
            this.MSMQImanageHandler.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // MSMQEwsHandler
            // 
            this.MSMQEwsHandler.DelayedAutoStart = true;
            this.MSMQEwsHandler.ServiceName = "MSMQ Ews Handler";
            this.MSMQEwsHandler.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.MSMQCicHandler,
            this.MSMQEwsHandler,
            this.MSMQImanageHandler,
            this.MSMQServiceNowHandler});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller MSMQCicHandler;
        private System.ServiceProcess.ServiceInstaller MSMQServiceNowHandler;
        private System.ServiceProcess.ServiceInstaller MSMQImanageHandler;
        private System.ServiceProcess.ServiceInstaller MSMQEwsHandler;
    }
}