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
using ZXing.QrCode.Internal;

namespace MWS.Views
{
    public partial class ProductionDetailClassificationView : Form
    {
        ProductionDetailView productionDetailView;
        public int productionItemId;
        public ProductionDetailClassificationView(int _productionItemId, ProductionDetailView _productionDetailView)
        {
            InitializeComponent();

            productionDetailView = _productionDetailView;
            productionItemId = _productionItemId;

            GetClassificationList();
        }
        public void GetClassificationList()
        {
            Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
            if (trnProductionItemController.DropDownClassification().Any())
            {
                comboBoxClassification.DataSource = trnProductionItemController.DropDownClassification();
                comboBoxClassification.ValueMember = "Classification";
                comboBoxClassification.DisplayMember = "Classification";
            }
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
            trnProductionItemController.UpdateProductionItemClassification(productionItemId, comboBoxClassification.SelectedValue.ToString());
            Close();
            productionDetailView.UpdateProductionItemListDataSource();
            productionDetailView.textBoxBarcode.Text = "";
            productionDetailView.textBoxBarcode.Focus();
        }
    }
}
