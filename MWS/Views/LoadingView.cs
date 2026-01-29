using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MWS.Modules;

namespace MWS.Views
{
    public partial class LoadingView : Form
    {
        public LoadingView()
        {
            InitializeComponent();

            SetFooter();
        }

        private void LoadingView_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "Loading... Please wait...";

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            SmoothReportProgress(worker, 0, 10, "Loading connection settings...");
            Thread.Sleep(1000);

            string connStr = SysConnectionStringModule.GetConnectionString();
            Thread.Sleep(1000);

            SmoothReportProgress(worker, 10, 40, "Reading configuration...");
            Thread.Sleep(1000);

            SmoothReportProgress(worker, 40, 70, "Parsing connection settings...");
            Thread.Sleep(400);

            SmoothReportProgress(worker, 70, 90, "Building connection string...");
            Thread.Sleep(500);

            SmoothReportProgress(worker, 90, 100, "Connection to database successful");
            Thread.Sleep(1500);

            e.Result = connStr;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            coloredProgressBar1.Value = e.ProgressPercentage;
            labelStatus.Text = e.UserState?.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoginView loginView = new LoginView();
            loginView.Show();
            Hide();
        }
        private void SmoothReportProgress(BackgroundWorker worker, int from, int to, string message, int delayMs = 20)
        {
            for (int i = from; i <= to; i++)
            {
                worker.ReportProgress(i, message);
                Thread.Sleep(delayMs);
            }
        }
        
        public void SetFooter()
        {
            var settings = SysCurrentModule.GetCurrentSettings();
            labelDeveloper.Text = settings.CurrentDeveloper;
            labelSupport.Text = settings.CurrentSupport;
            labelVersion.Text = settings.CurrentVersion;
        }
    }
}
