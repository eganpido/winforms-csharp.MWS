using MWS.Models;
using MWS.Modules;
using PagedList;
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
    public partial class ProductionDetailView : Form
    {
        public Models.TrnProductionModel trnProductionModel;

        public static List<Models.DgvTrnProductionItemModel> productionItemData = new List<Models.DgvTrnProductionItemModel>();
        public static Int32 productionItemPageNumber = 1;
        public static Int32 productionItemPageSize = 20;
        public PagedList<Models.DgvTrnProductionItemModel> productionItemPageList = new PagedList<Models.DgvTrnProductionItemModel>(productionItemData, productionItemPageNumber, productionItemPageSize);
        public BindingSource productionItemDataSource = new BindingSource();
        public ProductionDetailView(Models.TrnProductionModel productionModel)
        {
            InitializeComponent();

            trnProductionModel = productionModel;

            var id = trnProductionModel.Id;

            Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();
            var detail = trnProductionController.ProductionDetail(id);

            SetFooter();
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
            dataGridViewProductionItem.Dock = DockStyle.Fill;
            dataGridViewProductionItem.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridViewProductionItem.AllowUserToResizeRows = false;
            dataGridViewProductionItem.RowTemplate.Height = 32;

            GetProductionDetail();
        }
        public void GetProductionDetail()
        {
            UpdateComponents(trnProductionModel.IsLocked);
            textBoxBarcode.Focus();

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
                                ColumnReceivingItemId = d.ReceivingItemId,
                                ColumnItemId = d.ItemId,
                                ColumnReceivingBarcode = d.ReceivingBarcode,
                                ColumnBarcode = d.Barcode,
                                ColumnItemDescription = d.ItemDescription,
                                ColumnSizeId = d.SizeId,
                                ColumnSize = d.Size,
                                ColumnReceivedWeight = d.ReceivedWeight.ToString("#,##0.00"),
                                ColumnActualWeight = d.ActualWeight.ToString("#,##0.00"),
                                ColumnDelete = "DELETE",
                            };

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
            textBoxBarcode.Enabled = !isLocked;

            dataGridViewProductionItem.Columns[10].Visible = !isLocked;
            textBoxBarcode.Focus();

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
            }
            else
            {
                labelProductionTitle.Text = "Meat Weighing System - Processing";
            }
        }
        private void textBoxBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                if(trnProductionItemController.isAlreadyAdded(textBoxBarcode.Text) ==  false)
                {
                    int receivingItemId = trnProductionItemController.GetReceivingItem(textBoxBarcode.Text);
                    if (receivingItemId > 0)
                    {
                        ProductionWeightView productionWeightView = new ProductionWeightView(this, trnProductionModel, textBoxBarcode.Text);
                        productionWeightView.Show();
                    }
                    else
                    {
                        MessageBox.Show("Barcode doesn't exist.", "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxBarcode.Text = "";
                        textBoxBarcode.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Barcode already exist.", "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxBarcode.Text = "";
                    textBoxBarcode.Focus();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
            DashboardView dashboardView = new DashboardView();
            dashboardView.Show();
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

                String[] saveProduction = trnProductionController.LockProduction(trnProductionModel.Id);
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
                    ProductionDetailView productionDetailView = new ProductionDetailView(trnProductionController.ProductionDetail(Convert.ToInt32(addProduction[1])));
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
            if (e.RowIndex > -1 && dataGridViewProductionItem.CurrentCell.ColumnIndex == dataGridViewProductionItem.Columns["ColumnDelete"].Index)
            {
                DialogResult deleteDialogResult = MessageBox.Show("Confirm delete?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (deleteDialogResult == DialogResult.Yes)
                {
                    var id = Convert.ToInt32(dataGridViewProductionItem.Rows[e.RowIndex].Cells[dataGridViewProductionItem.Columns["ColumnId"].Index].Value);

                    Controllers.TrnProductionItemController trnProductionItemController = new Controllers.TrnProductionItemController();
                    String[] deleteProductionItem = trnProductionItemController.DeleteProductionItem(id);
                    if (deleteProductionItem[1].Equals("0") == false)
                    {
                        productionItemPageNumber = 1;
                        UpdateProductionItemListDataSource();
                        textBoxBarcode.Text = "";
                        textBoxBarcode.Focus();
                    }
                    else
                    {
                        MessageBox.Show(deleteProductionItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
