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
    public partial class SupplierDetailView : Form
    {
        SystemTableView systemTableView;
        MstSupplierModel mstSupplierModel;
        public SupplierDetailView(SystemTableView _systemTableView, MstSupplierModel _mstSupplierModel)
        {
            InitializeComponent();

            systemTableView = _systemTableView;
            mstSupplierModel = _mstSupplierModel;
            Controllers.MstSupplierController mstSupplierController = new Controllers.MstSupplierController();
            if (mstSupplierModel != null)
            {
                textBoxSupplier.Text = mstSupplierModel.Supplier;
            }
            else
            {
                textBoxSupplier.Text = "NA";
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
                Controllers.MstSupplierController mstSupplierController = new Controllers.MstSupplierController();
                Models.MstSupplierModel newSupplierModel = new Models.MstSupplierModel()
                {
                    Supplier = textBoxSupplier.Text
                };

                if (mstSupplierModel == null)
                {
                    String[] addSupplier = mstSupplierController.AddSupplier(newSupplierModel);
                    if (addSupplier[1].Equals("0") == false)
                    {
                        Close();
                        systemTableView.UpdateSupplierListDataSource();
                    }
                    else
                    {
                        MessageBox.Show(addSupplier[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    String[] saveSupplier = mstSupplierController.SaveSupplier(mstSupplierModel.Id, newSupplierModel);
                    if (saveSupplier[1].Equals("0") == false)
                    {
                        Close();
                        systemTableView.UpdateSupplierListDataSource();
                    }
                    else
                    {
                        MessageBox.Show(saveSupplier[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
