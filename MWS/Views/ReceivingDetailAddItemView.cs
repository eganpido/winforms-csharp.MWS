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
    public partial class ReceivingDetailAddItemView : Form
    {
        RecevingDetailView receivingDetailView;
        TrnReceivingModel trnReceivingModel;
        public ReceivingDetailAddItemView(RecevingDetailView _receivingDetailView, TrnReceivingModel _trnReceivingModel)
        {
            InitializeComponent();

            receivingDetailView = _receivingDetailView;
            trnReceivingModel = _trnReceivingModel;

            GetItemList();
        }
        public void GetItemList()
        {
            Controllers.TrnReceivingItemController trnReceivingItemController = new Controllers.TrnReceivingItemController();
            if (trnReceivingItemController.DropDownItem().Any())
            {
                comboBoxItem.DataSource = trnReceivingItemController.DropDownItem();
                comboBoxItem.ValueMember = "Id";
                comboBoxItem.DisplayMember = "Item";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
            receivingDetailView.textBoxWeight.Text = "";
            receivingDetailView.textBoxWeight.Focus();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult saveDialogResult = MessageBox.Show("Confirm?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (saveDialogResult == DialogResult.Yes)
            {
                Controllers.TrnReceivingItemController trnReceivingItemController = new Controllers.TrnReceivingItemController();
                String[] addItem = trnReceivingItemController.AddReceivingItemOthers(trnReceivingModel.Id, Convert.ToInt32(comboBoxItem.SelectedValue), Convert.ToDecimal(textBoxWeight.Text));
                if (addItem[1].Equals("0") == false)
                {
                    Close();
                    receivingDetailView.UpdateReceivingItemListDataSource();
                    receivingDetailView.textBoxWeight.Text = "";
                    receivingDetailView.textBoxWeight.Focus();
                }
                else
                {
                    MessageBox.Show(addItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
