using MWS.Controllers;
using MWS.Models;
using MWS.Modules;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.QrCode.Internal;

namespace MWS.Views
{
    public partial class ProductionDetailView : Form
    {
        public Models.TrnProductionModel trnProductionModel;
        public HistoryView historyView;
        public static List<Models.DgvTrnProductionItemModel> productionItemData = new List<Models.DgvTrnProductionItemModel>();
        public static Int32 productionItemPageNumber = 1;
        public static Int32 productionItemPageSize = 20;
        public PagedList<Models.DgvTrnProductionItemModel> productionItemPageList = new PagedList<Models.DgvTrnProductionItemModel>(productionItemData, productionItemPageNumber, productionItemPageSize);
        public BindingSource productionItemDataSource = new BindingSource();
        public ProductionDetailView(Models.TrnProductionModel productionModel, HistoryView _historyView)
        {
            InitializeComponent();

            trnProductionModel = productionModel;
            historyView = _historyView;
            var id = trnProductionModel.Id;

            Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();
            var detail = trnProductionController.ProductionDetail(id);

            GetSupplierList();
        }
        public void GetSupplierList()
        {
            Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();
            if (trnProductionController.SupplierList().Any())
            {
                comboBoxSupplier.DataSource = trnProductionController.SupplierList();
                comboBoxSupplier.ValueMember = "Id";
                comboBoxSupplier.DisplayMember = "Supplier";

                SetFooter();
            }
        }
        public void SetFooter()
        {
            var settings = SysCurrentModule.GetCurrentSettings();
            labelDeveloper.Text = settings.CurrentDeveloper;
            labelSupport.Text = settings.CurrentSupport;
            labelVersion.Text = settings.CurrentVersion;
            labelCurrentUser.Text = settings.CurrentUserName;

            StyleDataGridViewHeader();
        }
        private void StyleDataGridViewHeader()
        {
            dataGridViewProductionItem.EnableHeadersVisualStyles = true;
            dataGridViewProductionItem.ColumnHeadersDefaultCellStyle.Font =
                         new Font("Open Sans", 11F, FontStyle.Regular);
            dataGridViewProductionItem.ScrollBars = ScrollBars.Vertical;
            dataGridViewProductionItem.Dock = DockStyle.None;
            dataGridViewProductionItem.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridViewProductionItem.AllowUserToResizeRows = false;
            dataGridViewProductionItem.RowTemplate.Height = 32;

            GetProductionDetail();
        }
        public void GetProductionDetail()
        {
            UpdateComponents(trnProductionModel.IsLocked);
            textBoxWeight.Focus();

            CreateProductionItemListDataGridView();
        }
        public void CreateProductionItemListDataGridView()
        {
            UpdateProductionItemListDataSource();

            dataGridViewProductionItem.DataSource = productionItemDataSource;
        }
        public void UpdateProductionItemListDataSource()
        {
            SetProductionItemListDataSourceAsync();
        }
        public async void SetProductionItemListDataSourceAsync()
        {
            List<Models.DgvTrnProductionItemModel> getProductionItemListData = await GetProductionItemListDataTask();
            if (getProductionItemListData.Any())
            {
                productionItemData = getProductionItemListData;
                productionItemPageList = new PagedList<Models.DgvTrnProductionItemModel>(productionItemData, productionItemPageNumber, productionItemPageSize);

                if (productionItemPageList.PageCount == 1)
                {
                    buttonFirst.Enabled = false;
                    buttonPrevious.Enabled = false;
                    buttonNext.Enabled = false;
                    buttonLast.Enabled = false;
                }
                else if (productionItemPageNumber == 1)
                {
                    buttonFirst.Enabled = false;
                    buttonPrevious.Enabled = false;
                    buttonNext.Enabled = true;
                    buttonLast.Enabled = true;
                }
                else if (productionItemPageNumber == productionItemPageList.PageCount)
                {
                    buttonFirst.Enabled = true;
                    buttonPrevious.Enabled = true;
                    buttonNext.Enabled = false;
                    buttonLast.Enabled = false;
                }
                else
                {
                    buttonFirst.Enabled = true;
                    buttonPrevious.Enabled = true;
                    buttonNext.Enabled = true;
                    buttonLast.Enabled = true;
                }

                textBoxPageNumber.Text = productionItemPageNumber + " / " + productionItemPageList.PageCount;
                productionItemDataSource.DataSource = productionItemPageList;
            }
            else
            {
                buttonFirst.Enabled = false;
                buttonPrevious.Enabled = false;
                buttonNext.Enabled = false;
                buttonLast.Enabled = false;

                productionItemPageNumber = 1;

                productionItemData = new List<Models.DgvTrnProductionItemModel>();
                productionItemDataSource.Clear();
                textBoxPageNumber.Text = "1 / 1";
            }
        }
        public Task<List<Models.DgvTrnProductionItemModel>> GetProductionItemListDataTask()
        {
            Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();

            List<Models.TrnProductionItemModel> listProductionItem = trnProductionItemController.ProductionItemList(trnProductionModel.Id);
            if (listProductionItem.Any())
            {
                var items = from d in listProductionItem
                            select new Models.DgvTrnProductionItemModel
                            {
                                ColumnId = d.Id,
                                ColumnProductionId = d.ProductionId,
                                ColumnItemId = d.ItemId,
                                ColumnBarcode = d.Barcode,
                                ColumnItemDescription = d.ItemDescription,
                                ColumnSizeId = d.SizeId,
                                ColumnSize = d.Size,
                                ColumnClassification = d.Classification,
                                ColumnReceivedWeight = d.ReceivedWeight.ToString("#,##0.000"),
                                ColumnInputWeight = "W",
                                ColumnActualWeight = d.ActualWeight.ToString("#,##0.000"),
                                ColumnInputRemarks = "R",
                                ColumnRemarks = d.Remarks,
                                ColumnDelete = "DELETE",
                            };

                txtTotalWeight.Text = items.Sum(a => Convert.ToDecimal(a.ColumnActualWeight)).ToString("#,##0.000");
                txtTotalCount.Text = items.Count().ToString();

                return Task.FromResult(items.ToList());
            }
            else
            {
                return Task.FromResult(new List<Models.DgvTrnProductionItemModel>());
            }
        }
        public void UpdateComponents(Boolean isLocked)
        {
            buttonSave.Enabled = !isLocked;
            buttonEdit.Enabled = isLocked;
            textBoxWeight.Enabled = !isLocked;
            comboBoxSupplier.Enabled = !isLocked;
            dataGridViewProductionItem.Columns[13].Visible = !isLocked;

            textBoxWeight.Focus();

            if (isLocked)
            {
                labelIndicator.Visible = true;
            }
            else
            {
                labelIndicator.Visible = false;
            }

            bool IsReceiver = Convert.ToBoolean(Modules.SysCurrentModule.GetCurrentSettings().IsReceiver);
            if (IsReceiver)
            {
                labelProductionTitle.Text = "Meat Weighing System - Production";
                labelEntry.Text = "Barcode";
            }
            else
            {
                labelProductionTitle.Text = "Meat Weighing System - Processing";
                labelEntry.Text = "Weight";
            }

            if(historyView == null)
            {
                btnAdd.Enabled = true;
                btnAddItem.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = false;
                btnAddItem.Enabled = false;
            }

            btnAddItem.Enabled = !isLocked;

            var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            if(currentBranchId == 1)
            {
                dataGridViewProductionItem.Columns[8].Visible = false;
                dataGridViewProductionItem.Columns[9].Visible = false;
                dataGridViewProductionItem.Columns[11].Visible = false;
                dataGridViewProductionItem.Columns[12].Visible = true;
                btnAddItem.Visible = true;
                labelSupplier.Visible = true;
                comboBoxSupplier.Visible = true;
            }
            else
            {
                dataGridViewProductionItem.Columns[8].Visible = true;
                dataGridViewProductionItem.Columns[9].Visible = true;
                dataGridViewProductionItem.Columns[11].Visible = false;
                dataGridViewProductionItem.Columns[12].Visible = false;
                btnAddItem.Visible = false;
                labelSupplier.Visible = false;
                comboBoxSupplier.Visible = false;
            }
        }
        private void textBoxBarcode_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (historyView == null)
            {
                Close();
                DashboardView dashboardView = new DashboardView();
                dashboardView.Show();
            }
            else
            {
                Close();
                historyView.UpdateProductionListDataSource();
            }
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            productionItemPageList = new PagedList<Models.DgvTrnProductionItemModel>(productionItemData, 1, productionItemPageSize);
            productionItemDataSource.DataSource = productionItemPageList;

            buttonFirst.Enabled = false;
            buttonPrevious.Enabled = false;
            buttonNext.Enabled = true;
            buttonLast.Enabled = true;

            productionItemPageNumber = 1;
            textBoxPageNumber.Text = productionItemPageNumber + " / " + productionItemPageList.PageCount;
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (productionItemPageList.HasPreviousPage == true)
            {
                productionItemPageList = new PagedList<Models.DgvTrnProductionItemModel>(productionItemData, --productionItemPageNumber, productionItemPageSize);
                productionItemDataSource.DataSource = productionItemPageList;
            }

            buttonNext.Enabled = true;
            buttonLast.Enabled = true;

            if (productionItemPageNumber == 1)
            {
                buttonFirst.Enabled = false;
                buttonPrevious.Enabled = false;
            }

            textBoxPageNumber.Text = productionItemPageNumber + " / " + productionItemPageList.PageCount;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (productionItemPageList.HasNextPage == true)
            {
                productionItemPageList = new PagedList<Models.DgvTrnProductionItemModel>(productionItemData, ++productionItemPageNumber, productionItemPageSize);
                productionItemDataSource.DataSource = productionItemPageList;
            }

            buttonFirst.Enabled = true;
            buttonPrevious.Enabled = true;

            if (productionItemPageNumber == productionItemPageList.PageCount)
            {
                buttonNext.Enabled = false;
                buttonLast.Enabled = false;
            }

            textBoxPageNumber.Text = productionItemPageNumber + " / " + productionItemPageList.PageCount;
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            productionItemPageList = new PagedList<Models.DgvTrnProductionItemModel>(productionItemData, productionItemPageList.PageCount, productionItemPageSize);
            productionItemDataSource.DataSource = productionItemPageList;

            buttonFirst.Enabled = true;
            buttonPrevious.Enabled = true;
            buttonNext.Enabled = false;
            buttonLast.Enabled = false;

            productionItemPageNumber = productionItemPageList.PageCount;
            textBoxPageNumber.Text = productionItemPageNumber + " / " + productionItemPageList.PageCount;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult saveDialogResult = MessageBox.Show("Confirm save? This will lock the record.", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (saveDialogResult == DialogResult.Yes)
            {
                Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();

                Models.TrnProductionModel newProductionModel = new Models.TrnProductionModel()
                {
                    SupplierId = Convert.ToInt32(comboBoxSupplier.SelectedValue),
                };

                String[] saveProduction = trnProductionController.LockProduction(trnProductionModel.Id, newProductionModel);
                if (saveProduction[1].Equals("0") == false)
                {
                    UpdateComponents(true);
                }
                else
                {
                    MessageBox.Show(saveProduction[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult add = MessageBox.Show("Confirm add new record?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (add == DialogResult.Yes)
            {
                Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();
                String[] addProduction = trnProductionController.AddProduction();
                if (addProduction[1].Equals("0") == false)
                {
                    Close();
                    ProductionDetailView productionDetailView = new ProductionDetailView(trnProductionController.ProductionDetail(Convert.ToInt32(addProduction[1])), null);
                    productionDetailView.Show();
                }
                else
                {
                    MessageBox.Show(addProduction[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridViewProductionItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                var grid = dataGridViewProductionItem;

                if (grid.Columns["ColumnDelete"] != null && e.ColumnIndex == grid.Columns["ColumnDelete"].Index)
                {
                    DialogResult deleteDialogResult = MessageBox.Show("Confirm delete?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (deleteDialogResult == DialogResult.Yes)
                    {
                        var cellValue = grid.Rows[e.RowIndex].Cells["ColumnId"].Value;

                        if (cellValue != null)
                        {
                            int id = Convert.ToInt32(cellValue);

                            Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                            String[] deleteProductionItem = trnProductionItemController.DeleteProductionItem(id);

                            if (deleteProductionItem.Length > 1 && deleteProductionItem[1].Equals("0") == false)
                            {
                                productionItemPageNumber = 1;
                                UpdateProductionItemListDataSource();
                                textBoxWeight.Text = "";
                                textBoxWeight.Focus();
                            }
                            else
                            {
                                MessageBox.Show(deleteProductionItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

                else if (grid.Columns["ColumnInputWeight"] != null && e.ColumnIndex == grid.Columns["ColumnInputWeight"].Index)
                {
                    var cellValue = grid.Rows[e.RowIndex].Cells["ColumnId"].Value;

                    if (cellValue != null)
                    {
                        int id = Convert.ToInt32(cellValue);
                        ProductionWeightView productionWeightView = new ProductionWeightView(this, trnProductionModel, textBoxWeight.Text, id);
                        productionWeightView.Show();
                    }
                }

                else if (grid.Columns["ColumnInputRemarks"] != null && e.ColumnIndex == grid.Columns["ColumnInputRemarks"].Index)
                {
                    var cellValue = grid.Rows[e.RowIndex].Cells["ColumnId"].Value;

                    if (cellValue != null)
                    {
                        int id = Convert.ToInt32(cellValue);
                        ProductionRemarksView productionRemarksView = new ProductionRemarksView(id, this);
                        productionRemarksView.Show();
                    }
                }
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            ProductionDetailAddItemView productionDetailAddItemView = new ProductionDetailAddItemView(this, trnProductionModel);
            productionDetailAddItemView.Show();
        }

        private void textBoxWeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
                if (currentBranchId == 1)
                {
                    DialogResult saveDialogResult = MessageBox.Show("Confirm weight?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (saveDialogResult == DialogResult.Yes)
                    {
                        String[] addItem = trnProductionItemController.AddProductionItem(trnProductionModel.Id, Convert.ToDecimal(textBoxWeight.Text), "");
                        if (addItem[1].Equals("0") == false)
                        {
                            ProductionDetailClassificationView productionDetailClassificationView = new ProductionDetailClassificationView(Convert.ToInt32(addItem[1]), this);
                            productionDetailClassificationView.Show();
                        }
                        else
                        {
                            MessageBox.Show(addItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    if (trnProductionItemController.isAlreadyAdded(textBoxWeight.Text, trnProductionModel.Id) == false)
                    {
                        if (trnProductionItemController.IsExist(textBoxWeight.Text) == true)
                        {
                            trnProductionItemController.AddProductionItem(trnProductionModel.Id, 0, textBoxWeight.Text);
                            UpdateProductionItemListDataSource();
                            textBoxWeight.Text = "";
                            textBoxWeight.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Barcode not exist!", "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxWeight.Text = "";
                            textBoxWeight.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Barcode already exist!", "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxWeight.Text = "";
                        textBoxWeight.Focus();
                    }
                }
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();

            String[] unlockProduction = trnProductionController.UnlockProduction(trnProductionModel.Id);
            if (unlockProduction[1].Equals("0") == false)
            {
                UpdateComponents(false);
                if (historyView != null)
                {
                    historyView.UpdateReceivingListDataSource();
                }
            }
            else
            {
                MessageBox.Show(unlockProduction[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
