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
    public partial class ProductionDetailAddItemView : Form
    {
        ProductionDetailView productionDetailView;
        TrnProductionModel trnProductionModel;
        public ProductionDetailAddItemView(ProductionDetailView _productionDetailView, TrnProductionModel _trnProductionModel)
        {
            InitializeComponent();

            productionDetailView = _productionDetailView;
            trnProductionModel = _trnProductionModel;

            GetItemList();
        }
        public void GetItemList()
        {
            Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
            if (trnProductionItemController.DropDownItem().Any())
            {
                comboBoxItem.DataSource = trnProductionItemController.DropDownItem();
                comboBoxItem.ValueMember = "Id";
                comboBoxItem.DisplayMember = "Item";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
            productionDetailView.textBoxWeight.Text = "";
            productionDetailView.textBoxWeight.Focus();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult saveDialogResult = MessageBox.Show("Confirm?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (saveDialogResult == DialogResult.Yes)
            {
                Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                String[] addItem = trnProductionItemController.AddProductionItemOthers(trnProductionModel.Id, Convert.ToInt32(comboBoxItem.SelectedValue), Convert.ToDecimal(textBoxWeight.Text));
                if (addItem[1].Equals("0") == false)
                {
                    Close();
                    productionDetailView.UpdateProductionItemListDataSource();
                    productionDetailView.textBoxWeight.Text = "";
                    productionDetailView.textBoxWeight.Focus();
                }
                else
                {
                    MessageBox.Show(addItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
