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
        public int productionItemId;
        public ProductionWeightView(ProductionDetailView _productionDetailView, TrnProductionModel _trnProductionModel, string _barcode, int _productionItemId)
        {
            InitializeComponent();

            productionDetailView = _productionDetailView;
            trnProductionModel = _trnProductionModel;
            barcode = _barcode;
            productionItemId = _productionItemId;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
            productionDetailView.textBoxBarcode.Text = "";
            productionDetailView.textBoxBarcode.Focus();
        }

        private void textBoxWeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult saveDialogResult = MessageBox.Show("Confirm weight?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (saveDialogResult == DialogResult.Yes)
                {
                    var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
                    if (currentBranchId == 1)
                    {
                        Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                        String[] addItem = trnProductionItemController.AddProductionItem(trnProductionModel.Id, barcode, Convert.ToDecimal(textBoxWeight.Text));
                        if (addItem[1].Equals("0") == false)
                        {
                            Close();
                            ProductionDetailClassificationView productionDetailClassificationView = new ProductionDetailClassificationView(Convert.ToInt32(addItem[1]), productionDetailView);
                            productionDetailClassificationView.Show();
                        }
                        else
                        {
                            MessageBox.Show(addItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                        trnProductionItemController.UpdateProductionItemWeight(productionItemId, Convert.ToDecimal(textBoxWeight.Text));
                        Close();
                        productionDetailView.UpdateProductionItemListDataSource();
                    }
                }
            }
        }
    }
}
