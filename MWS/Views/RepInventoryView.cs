using MWS.Reports;
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
    public partial class RepInventoryView : Form
    {
        public RepInventoryView()
        {
            InitializeComponent();

            GetReportList();
        }
        public void GetReportList()
        {
            Controllers.RepInventoryController repInventoryController = new Controllers.RepInventoryController();
            if (repInventoryController.ReportList().Any())
            {
                comboBoxReport.DataSource = repInventoryController.ReportList();
                comboBoxReport.ValueMember = "Id";
                comboBoxReport.DisplayMember = "Report";

                GetBranchList();
            }
        }
        public void GetBranchList()
        {
            Controllers.RepInventoryController repInventoryController = new Controllers.RepInventoryController();
            if (repInventoryController.BranchList().Any())
            {
                comboBoxBranch.DataSource = repInventoryController.BranchList();
                comboBoxBranch.ValueMember = "Id";
                comboBoxBranch.DisplayMember = "Branch";
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            new RepInventoryPDFView(dtStartDate.Value.Date, dtEndDate.Value.Date, Convert.ToInt32(comboBoxBranch.SelectedValue));
        }
    }
}
