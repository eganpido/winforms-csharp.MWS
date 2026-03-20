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
    public partial class ProductionRemarksView : Form
    {
        ProductionDetailView productionDetailView;
        public int productionItemId;
        public ProductionRemarksView(int _productionItemId, ProductionDetailView _productionDetailView)
        {
            InitializeComponent();

            productionDetailView = _productionDetailView;
            productionItemId = _productionItemId;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
            trnProductionItemController.UpdateProductionItemRemarks(productionItemId, textBoxRemarks.Text);
            Close();
            productionDetailView.UpdateProductionItemListDataSource();
            productionDetailView.textBoxWeight.Text = "";
            productionDetailView.textBoxWeight.Focus();
        }
    }
}
