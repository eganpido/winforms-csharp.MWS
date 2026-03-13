using MWS.Controllers;
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
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
            }

            for (int i = 0; i < 2; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            }

            Controllers.DashboardController db = new Controllers.DashboardController();

            int m1 = db.GetQuantity(1, 1); int m2 = db.GetQuantity(1, 2); // Minis
            int xs1 = db.GetQuantity(2, 1); int xs2 = db.GetQuantity(2, 2); // XS
            int s1 = db.GetQuantity(3, 1); int s2 = db.GetQuantity(3, 2); // Small
            int med1 = db.GetQuantity(4, 1); int med2 = db.GetQuantity(4, 2); // Medium
            int l1 = db.GetQuantity(5, 1); int l2 = db.GetQuantity(5, 2); // Large
            int xl1 = db.GetQuantity(6, 1); int xl2 = db.GetQuantity(6, 2); // XL

            tableLayoutPanel1.Controls.Clear();

            tableLayoutPanel1.Controls.Add(CreateSizeCard("Minis", "Commissary 1", m1, "Commissary 2", m2, Color.Gray), 0, 0);
            tableLayoutPanel1.Controls.Add(CreateSizeCard("Extra Small (XS)", "Commissary 1", xs1, "Commissary 2", xs2, Color.SlateBlue), 1, 0);
            tableLayoutPanel1.Controls.Add(CreateSizeCard("Small", "Commissary 1", s1, "Commissary 2", s2, Color.DodgerBlue), 2, 0);

            tableLayoutPanel1.Controls.Add(CreateSizeCard("Medium", "Commissary 1", med1, "Commissary 2", med2, Color.LimeGreen), 0, 1);
            tableLayoutPanel1.Controls.Add(CreateSizeCard("Large", "Commissary 1", l1, "Commissary 2", l2, Color.Orange), 1, 1);
            tableLayoutPanel1.Controls.Add(CreateSizeCard("Extra Large (XL)", "Commissary 1", xl1, "Commissary 2", xl2, Color.Red), 2, 1);

            int grandTotalB1 = m1 + xs1 + s1 + med1 + l1 + xl1;
            int grandTotalB2 = m2 + xs2 + s2 + med2 + l2 + xl2;

            labelTotal.Text = $"TOTAL Commissary 1:  {grandTotalB1:N0}  |  TOTAL Commissary 2:  {grandTotalB2:N0}";

            bool IsReceiver = Convert.ToBoolean(Modules.SysCurrentModule.GetCurrentSettings().IsReceiver);
            if (IsReceiver)
            {
                buttonProduction.Text = "       Production";
                buttonPullOut.Visible = false;
            }
            else
            {
                buttonProduction.Text = "       Processing";
                buttonPullOut.Visible = true;
            }
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
        private Panel CreateSizeCard(string mainSizeTitle, string title1, int value1, string title2, int value2, Color accentColor)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.FromArgb(30, 41, 59);
            panel.Padding = new Padding(10);
            panel.Margin = new Padding(10);

            Label lblMainTitle = new Label();
            lblMainTitle.Text = mainSizeTitle.ToUpper();
            lblMainTitle.Dock = DockStyle.Top;
            lblMainTitle.Height = 35;
            lblMainTitle.ForeColor = Color.White;
            lblMainTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblMainTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblMainTitle.Paint += (s, e) => {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(51, 65, 85), 2), 0, lblMainTitle.Height - 1, lblMainTitle.Width, lblMainTitle.Height - 1);
            };

            TableLayoutPanel innerLayout = new TableLayoutPanel();
            innerLayout.Dock = DockStyle.Fill;
            innerLayout.BackColor = Color.Transparent;
            innerLayout.ColumnCount = 2;
            innerLayout.RowCount = 2;
            innerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            innerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            innerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
            innerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));

            Label lblVal1 = new Label
            {
                Text = value1.ToString("N0"),
                Dock = DockStyle.Fill,
                ForeColor = accentColor,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomCenter
            };

            Label lblVal2 = new Label
            {
                Text = value2.ToString("N0"),
                Dock = DockStyle.Fill,
                ForeColor = accentColor,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomCenter
            };

            Label lblBranch1 = new Label
            {
                Text = title1.ToUpper(),
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.TopCenter
            };

            Label lblBranch2 = new Label
            {
                Text = title2.ToUpper(),
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.TopCenter
            };

            innerLayout.Controls.Add(lblVal1, 0, 0);
            innerLayout.Controls.Add(lblVal2, 1, 0);
            innerLayout.Controls.Add(lblBranch1, 0, 1);
            innerLayout.Controls.Add(lblBranch2, 1, 1);

            panel.Controls.Add(innerLayout);
            panel.Controls.Add(lblMainTitle);

            panel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle,
                    Color.FromArgb(51, 65, 85), ButtonBorderStyle.Solid);
            };

            return panel;
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            bool IsReceiver = Convert.ToBoolean(Modules.SysCurrentModule.GetCurrentSettings().IsReceiver);
            DialogResult proceed = MessageBox.Show("Confirm proceed to receiving?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (proceed == DialogResult.Yes)
            {
                if(IsReceiver)
                {
                    Controllers.TrnReceivingReceiverController trnReceivingController = new Controllers.TrnReceivingReceiverController();
                    String[] addReceiving = trnReceivingController.AddReceiving();
                    if (addReceiving[1].Equals("0") == false)
                    {
                        Close();
                        ReceivingDetailReceiverView recevingDetailView = new ReceivingDetailReceiverView(trnReceivingController.ReceivingDetail(Convert.ToInt32(addReceiving[1])), null);
                        recevingDetailView.Show();
                    }
                    else
                    {
                        MessageBox.Show(addReceiving[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Controllers.TrnReceivingController trnReceivingController = new Controllers.TrnReceivingController();
                    String[] addReceiving = trnReceivingController.AddReceiving();
                    if (addReceiving[1].Equals("0") == false)
                    {
                        Close();
                        RecevingDetailView recevingDetailView = new RecevingDetailView(trnReceivingController.ReceivingDetail(Convert.ToInt32(addReceiving[1])), null);
                        recevingDetailView.Show();
                    }
                    else
                    {
                        MessageBox.Show(addReceiving[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
                    ProductionDetailView productionDetailView = new ProductionDetailView(trnProductionController.ProductionDetail(Convert.ToInt32(addProduction[1])), null);
                    productionDetailView.Show();
                }
                else
                {
                    MessageBox.Show(addProduction[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonPullOut_Click(object sender, EventArgs e)
        {
            DialogResult proceed = MessageBox.Show("Confirm proceed to pull out?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (proceed == DialogResult.Yes)
            {
                Controllers.TrnPullOutController trnPullOutController = new Controllers.TrnPullOutController();
                String[] addPullOut = trnPullOutController.AddPullOut();
                if (addPullOut[1].Equals("0") == false)
                {
                    Close();
                    PullOutDetailView pullOutDetailView = new PullOutDetailView(trnPullOutController.PullOutDetail(Convert.ToInt32(addPullOut[1])), null);
                    pullOutDetailView.Show();
                }
                else
                {
                    MessageBox.Show(addPullOut[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonHistory_Click(object sender, EventArgs e)
        {
            Close();
            HistoryView historyView = new HistoryView();
            historyView.Show();
        }

        private void buttonSystemTables_Click(object sender, EventArgs e)
        {
            Close();
            SystemTableView systemTableView = new SystemTableView();
            systemTableView.Show();
        }

        private void buttonReports_Click(object sender, EventArgs e)
        {
            RepInventoryView repInventoryView = new RepInventoryView();
            repInventoryView.Show();
        }
    }
}
