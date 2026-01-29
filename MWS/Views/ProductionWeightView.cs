using MWS.Models;
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
    public partial class ProductionWeightView : Form
    {
        ProductionDetailView productionDetailView;
        TrnProductionModel trnProductionModel;
        public string barcode;
        public ProductionWeightView(ProductionDetailView _productionDetailView, TrnProductionModel _trnProductionModel, string _barcode)
        {
            InitializeComponent();

            productionDetailView = _productionDetailView;
            trnProductionModel = _trnProductionModel;
            barcode = _barcode;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxWeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult saveDialogResult = MessageBox.Show("Confirm weight?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (saveDialogResult == DialogResult.Yes)
                {
                    Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                    trnProductionItemController.AddProductionItem(trnProductionModel.Id, barcode, Convert.ToDecimal(textBoxWeight.Text));
                    Close();
                    productionDetailView.UpdateProductionItemListDataSource();
                    productionDetailView.textBoxBarcode.Text = "";
                    productionDetailView.textBoxBarcode.Focus();
                }
            }
        }
    }
}
