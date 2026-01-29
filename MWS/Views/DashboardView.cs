using MWS.Modules;
using MWS.Views;
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
    public partial class DashboardView : Form
    {
        public DashboardView()
        {
            InitializeComponent();

            SetFooter();

            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.RowCount = 2;

            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();

            for (int i = 0; i < 3; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 33.3333F)
                );
            }

            for (int i = 0; i < 2; i++)
            {
                tableLayoutPanel1.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 50F)
                );
            }

            Controllers.DashboardController dashboardController = new Controllers.DashboardController();
            int minis = dashboardController.GetQuantity(1);
            int extraSmall = dashboardController.GetQuantity(2);
            int small = dashboardController.GetQuantity(3);
            int medium = dashboardController.GetQuantity(4);
            int large = dashboardController.GetQuantity(5);
            int extraLarge = dashboardController.GetQuantity(6);

            tableLayoutPanel1.Controls.Add(CreateSizeCard("Minis", minis, Color.Gray), 0, 0);
            tableLayoutPanel1.Controls.Add(CreateSizeCard("Extra Small (XS)", extraSmall, Color.SlateBlue), 1, 0);
            tableLayoutPanel1.Controls.Add(CreateSizeCard("Small", small, Color.DodgerBlue), 2, 0);

            tableLayoutPanel1.Controls.Add(CreateSizeCard("Medium", medium, Color.LimeGreen), 0, 1);
            tableLayoutPanel1.Controls.Add(CreateSizeCard("Large", large, Color.Orange), 1, 1);
            tableLayoutPanel1.Controls.Add(CreateSizeCard("Extra Large (XL)", extraLarge, Color.Red), 2, 1);


            int total = minis + extraSmall + small + medium + large + extraLarge;

            labelTotal.Text = $"TOTAL CUTS: {total:N0}";
        }
        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Close();
            LoginView loginView = new LoginView();
            loginView.Show();
        }
        public void SetFooter()
        {
            var settings = SysCurrentModule.GetCurrentSettings();
            labelDeveloper.Text = settings.CurrentDeveloper;
            labelSupport.Text = settings.CurrentSupport;
            labelVersion.Text = settings.CurrentVersion;
            labelCurrentUser.Text = settings.CurrentUserName;
        }
        private Panel CreateSizeCard(string title, int value, Color accentColor)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.FromArgb(30, 41, 59); // dark panel
            panel.Padding = new Padding(10);
            panel.Margin = new Padding(10);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.ForeColor = Color.Gainsboro;
            lblTitle.Font = new Font("Open Sans", 14, FontStyle.Bold);
            lblTitle.Height = 25;

            Label lblValue = new Label();
            lblValue.Text = value.ToString("N0");
            lblValue.Dock = DockStyle.Fill;
            lblValue.ForeColor = accentColor;
            lblValue.Font = new Font("Open Sans", 28, FontStyle.Bold);
            lblValue.TextAlign = ContentAlignment.MiddleCenter;

            panel.Controls.Add(lblValue);
            panel.Controls.Add(lblTitle);

            return panel;
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            DialogResult proceed = MessageBox.Show("Confirm proceed to receiving?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (proceed == DialogResult.Yes)
            {
                Controllers.TrnReceivingController trnReceivingController = new Controllers.TrnReceivingController();
                String[] addReceiving = trnReceivingController.AddReceiving();
                if (addReceiving[1].Equals("0") == false)
                {
                    Close();
                    RecevingDetailView recevingDetailView = new RecevingDetailView(trnReceivingController.RecevingDetail(Convert.ToInt32(addReceiving[1])));
                    recevingDetailView.Show();
                }
                else
                {
                    MessageBox.Show(addReceiving[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonProduction_Click(object sender, EventArgs e)
        {
            DialogResult proceed = MessageBox.Show("Confirm proceed to production?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (proceed == DialogResult.Yes)
            {
                Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();
                String[] addProduction = trnProductionController.AddProduction();
                if (addProduction[1].Equals("0") == false)
                {
                    Close();
                    ProductionDetailView productionDetailView = new ProductionDetailView(trnProductionController.ProductionDetail(Convert.ToInt32(addProduction[1])));
                    productionDetailView.Show();
                }
                else
                {
                    MessageBox.Show(addProduction[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
