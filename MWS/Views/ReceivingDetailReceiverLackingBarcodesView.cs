using MWS.Controllers;
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
    public partial class ReceivingDetailReceiverLackingBarcodesView : Form
    {
        public ReceivingDetailReceiverLackingBarcodesView(int pullOutId, int receivingId)
        {
            InitializeComponent();

            LackingBarcodes(pullOutId, receivingId);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        public void LackingBarcodes(int pullOutId, int receivingId)
        {
            TrnReceivingReceiverController trnReceivingReceiverController = new TrnReceivingReceiverController();
            var lacking = trnReceivingReceiverController.LackingBarcodes(pullOutId, receivingId);
            if (lacking.Any())
            {
                foreach (var lack in lacking)
                {
                    textBoxBarcodes.Text += lack.Barcode + Environment.NewLine;
                }
            }
        }
    }
}
