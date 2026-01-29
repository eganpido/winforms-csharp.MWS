using MWS.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MWS.Views
{
    public partial class LoginView : Form
    {
        public LoginView()
        {
            InitializeComponent();

            SetFooter();
        }
        public void SetFooter()
        {
            var settings = SysCurrentModule.GetCurrentSettings();
            labelDeveloper.Text = settings.CurrentDeveloper;
            labelSupport.Text = settings.CurrentSupport;
            labelVersion.Text = settings.CurrentVersion;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login();
        }

        public void Login()
        {
            Controllers.LoginController loginController = new Controllers.LoginController();
            String[] login = loginController.Login(textBoxUsername.Text, textBoxPassword.Text);
            if (login[1].Equals("0") == false)
            {
               Hide();
               DashboardView dashboardView = new DashboardView();
               dashboardView.Show();
            }
            else
            {
                MessageBox.Show(login[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login();
            }
        }
    }

}
