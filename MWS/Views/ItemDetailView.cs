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
    public partial class ItemDetailView : Form
    {
        SystemTableView systemTableView;
        MstItemModel mstItemModel;
        public ItemDetailView(SystemTableView _systemTableView, MstItemModel _mstItemModel)
        {
            InitializeComponent();

            systemTableView = _systemTableView;
            mstItemModel = _mstItemModel;
            Controllers.MstItemController mstItemController = new Controllers.MstItemController();
            if (mstItemModel != null)
            {
                textBoxItem.Text = mstItemModel.Item;
            }
            else
            {
                textBoxItem.Text = "NA";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult saveDialogResult = MessageBox.Show("Confirm?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (saveDialogResult == DialogResult.Yes)
            {
                Controllers.MstItemController mstItemController = new Controllers.MstItemController();
                Models.MstItemModel newItemModel = new Models.MstItemModel()
                {
                    Item = textBoxItem.Text
                };

                if(mstItemModel == null)
                {
                    String[] addItem = mstItemController.AddItem(newItemModel);
                    if (addItem[1].Equals("0") == false)
                    {
                        Close();
                        systemTableView.UpdateItemListDataSource();
                    }
                    else
                    {
                        MessageBox.Show(addItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    String[] saveItem = mstItemController.SaveItem(mstItemModel.Id, newItemModel);
                    if (saveItem[1].Equals("0") == false)
                    {
                        Close();
                        systemTableView.UpdateItemListDataSource();
                    }
                    else
                    {
                        MessageBox.Show(saveItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
