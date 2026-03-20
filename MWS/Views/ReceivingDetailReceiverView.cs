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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MWS.Views
{
    public partial class ReceivingDetailReceiverView : Form
    {
        public Models.TrnReceivingModel trnReceivingModel;
        public HistoryView historyView;
        public static List<Models.DgvTrnReceivingReceiverItemModel> receivingItemData = new List<Models.DgvTrnReceivingReceiverItemModel>();
        public static Int32 receivingItemPageNumber = 1;
        public static Int32 receivingItemPageSize = 20;
        public PagedList<Models.DgvTrnReceivingReceiverItemModel> receivingItemPageList = new PagedList<Models.DgvTrnReceivingReceiverItemModel>(receivingItemData, receivingItemPageNumber, receivingItemPageSize);
        public BindingSource receivingItemDataSource = new BindingSource();
        public ReceivingDetailReceiverView(Models.TrnReceivingModel receivingModel, HistoryView _historyView)
        {
            InitializeComponent();

            trnReceivingModel = receivingModel;
            historyView = _historyView;
            var id = trnReceivingModel.Id;
            Controllers.TrnReceivingReceiverController trnReceivingController = new Controllers.TrnReceivingReceiverController();
            var detail = trnReceivingController.ReceivingDetail(id);

            GetSupplierList();
        }
        public void GetSupplierList()
        {
            Controllers.TrnReceivingController trnReceivingController = new Controllers.TrnReceivingController();
            if (trnReceivingController.SupplierList().Any())
            {
                comboBoxSupplier.DataSource = trnReceivingController.SupplierList();
                comboBoxSupplier.ValueMember = "Id";
                comboBoxSupplier.DisplayMember = "Supplier";

                GetPullOutList();
            }
        }
        public void GetPullOutList()
        {
            Controllers.TrnReceivingReceiverController trnReceivingController = new Controllers.TrnReceivingReceiverController();
            if (trnReceivingController.PullOutList().Any())
            {
                comboBoxPullOutNumber.DataSource = trnReceivingController.PullOutList();
                comboBoxPullOutNumber.ValueMember = "Id";
                comboBoxPullOutNumber.DisplayMember = "PullOutNo";
                comboBoxPullOutNumber.SelectedValue = 0;

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
            dataGridViewReceivingItem.EnableHeadersVisualStyles = true;
            dataGridViewReceivingItem.ColumnHeadersDefaultCellStyle.Font =
                new Font("Open Sans", 11F, FontStyle.Regular);
            dataGridViewReceivingItem.ScrollBars = ScrollBars.Vertical;
            dataGridViewReceivingItem.Dock = DockStyle.Fill;
            dataGridViewReceivingItem.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridViewReceivingItem.AllowUserToResizeRows = false;
            dataGridViewReceivingItem.RowTemplate.Height = 32;

            GetReceivingDetail();
        }
        public void GetReceivingDetail()
        {
            UpdateComponents(trnReceivingModel.IsLocked);

            comboBoxSupplier.SelectedValue = trnReceivingModel.SupplierId;
            textBoxRemarks.Text = trnReceivingModel.Remarks;
            if (trnReceivingModel != null && trnReceivingModel.PullOutId != null)
            {
                comboBoxPullOutNumber.SelectedValue = trnReceivingModel.PullOutId;
            }
            else
            {
                comboBoxPullOutNumber.SelectedIndex = -1;
            }
            comboBoxPullOutNumber.Focus();

            CreateReceivingItemListDataGridView();
        }
        public void UpdateComponents(Boolean isLocked)
        {
            buttonSave.Enabled = !isLocked;
            comboBoxSupplier.Enabled = !isLocked;
            textBoxRemarks.Enabled = !isLocked;
            comboBoxPullOutNumber.Enabled = !isLocked;
            textBoxBarcode.Enabled = !isLocked;

            dataGridViewReceivingItem.Columns[9].Visible = !isLocked;
            comboBoxPullOutNumber.Focus();

            if (isLocked)
            {
                labelIndicator.Visible = true;
            }
            else
            {
                labelIndicator.Visible = false;
            }

            if (historyView == null)
            {
                btnAdd.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = false;
            }
        }
        public void CreateReceivingItemListDataGridView()
        {
            UpdateReceivingItemListDataSource();

            dataGridViewReceivingItem.DataSource = receivingItemDataSource;
        }
        public void UpdateReceivingItemListDataSource()
        {
            SetReceivingItemListDataSourceAsync();
        }
        public async void SetReceivingItemListDataSourceAsync()
        {
            List<Models.DgvTrnReceivingReceiverItemModel> getReceivingItemListData = await GetReceivingItemListDataTask();
            if (getReceivingItemListData.Any())
            {
                receivingItemData = getReceivingItemListData;
                receivingItemPageList = new PagedList<Models.DgvTrnReceivingReceiverItemModel>(receivingItemData, receivingItemPageNumber, receivingItemPageSize);

                if (receivingItemPageList.PageCount == 1)
                {
                    buttonFirst.Enabled = false;
                    buttonPrevious.Enabled = false;
                    buttonNext.Enabled = false;
                    buttonLast.Enabled = false;
                }
                else if (receivingItemPageNumber == 1)
                {
                    buttonFirst.Enabled = false;
                    buttonPrevious.Enabled = false;
                    buttonNext.Enabled = true;
                    buttonLast.Enabled = true;
                }
                else if (receivingItemPageNumber == receivingItemPageList.PageCount)
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

                textBoxPageNumber.Text = receivingItemPageNumber + " / " + receivingItemPageList.PageCount;
                receivingItemDataSource.DataSource = receivingItemPageList;
            }
            else
            {
                buttonFirst.Enabled = false;
                buttonPrevious.Enabled = false;
                buttonNext.Enabled = false;
                buttonLast.Enabled = false;

                receivingItemPageNumber = 1;

                receivingItemData = new List<Models.DgvTrnReceivingReceiverItemModel>();
                receivingItemDataSource.Clear();
                textBoxPageNumber.Text = "1 / 1";
            }
        }
        public Task<List<Models.DgvTrnReceivingReceiverItemModel>> GetReceivingItemListDataTask()
        {
            Controllers.TrnReceivingItemController trnReceivingItemController = new Controllers.TrnReceivingItemController();

            List<Models.TrnReceivingItemModel> listReceivingItem = trnReceivingItemController.ReceivingItemList(trnReceivingModel.Id);
            if (listReceivingItem.Any())
            {
                var items = from d in listReceivingItem
                            select new Models.DgvTrnReceivingReceiverItemModel
                            {
                                ColumnId = d.Id,
                                ColumnReceivingId = d.ReceivingId,
                                ColumnItemId = d.ItemId,
                                ColumnBarcode = d.Barcode,
                                ColumnItemDescription = d.ItemDescription,
                                ColumnSizeId = d.SizeId,
                                ColumnSize = d.Size,
                                ColumnClassification = d.Classification,
                                ColumnWeight = d.Weight.ToString("#,##0.000"),
                                ColumnDelete = "DELETE",
                            };

                txtTotalWeight.Text = items.Sum(a => Convert.ToDecimal(a.ColumnWeight)).ToString("#,##0.000");
                txtTotalCount.Text = items.Count().ToString();

                return Task.FromResult(items.ToList());
            }
            else
            {
                return Task.FromResult(new List<Models.DgvTrnReceivingReceiverItemModel>());
            }
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

            }
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            receivingItemPageList = new PagedList<Models.DgvTrnReceivingReceiverItemModel>(receivingItemData, 1, receivingItemPageSize);
            receivingItemDataSource.DataSource = receivingItemPageList;

            buttonFirst.Enabled = false;
            buttonPrevious.Enabled = false;
            buttonNext.Enabled = true;
            buttonLast.Enabled = true;

            receivingItemPageNumber = 1;
            textBoxPageNumber.Text = receivingItemPageNumber + " / " + receivingItemPageList.PageCount;
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (receivingItemPageList.HasPreviousPage == true)
            {
                receivingItemPageList = new PagedList<Models.DgvTrnReceivingReceiverItemModel>(receivingItemData, --receivingItemPageNumber, receivingItemPageSize);
                receivingItemDataSource.DataSource = receivingItemPageList;
            }

            buttonNext.Enabled = true;
            buttonLast.Enabled = true;

            if (receivingItemPageNumber == 1)
            {
                buttonFirst.Enabled = false;
                buttonPrevious.Enabled = false;
            }

            textBoxPageNumber.Text = receivingItemPageNumber + " / " + receivingItemPageList.PageCount;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (receivingItemPageList.HasNextPage == true)
            {
                receivingItemPageList = new PagedList<Models.DgvTrnReceivingReceiverItemModel>(receivingItemData, ++receivingItemPageNumber, receivingItemPageSize);
                receivingItemDataSource.DataSource = receivingItemPageList;
            }

            buttonFirst.Enabled = true;
            buttonPrevious.Enabled = true;

            if (receivingItemPageNumber == receivingItemPageList.PageCount)
            {
                buttonNext.Enabled = false;
                buttonLast.Enabled = false;
            }

            textBoxPageNumber.Text = receivingItemPageNumber + " / " + receivingItemPageList.PageCount;
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            receivingItemPageList = new PagedList<Models.DgvTrnReceivingReceiverItemModel>(receivingItemData, receivingItemPageList.PageCount, receivingItemPageSize);
            receivingItemDataSource.DataSource = receivingItemPageList;

            buttonFirst.Enabled = true;
            buttonPrevious.Enabled = true;
            buttonNext.Enabled = false;
            buttonLast.Enabled = false;

            receivingItemPageNumber = receivingItemPageList.PageCount;
            textBoxPageNumber.Text = receivingItemPageNumber + " / " + receivingItemPageList.PageCount;
        }

        private void comboBoxPullOutNumber_SelectedValueChanged(object sender, EventArgs e)
        {
            textBoxBarcode.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult add = MessageBox.Show("Confirm add new record?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (add == DialogResult.Yes)
            {
                Controllers.TrnReceivingReceiverController trnReceivingController = new Controllers.TrnReceivingReceiverController();
                String[] addReceiving = trnReceivingController.AddReceiving();
                if (addReceiving[1].Equals("0") == false)
                {
                    Close();
                    ReceivingDetailReceiverView recevingDetailView = new ReceivingDetailReceiverView(trnReceivingController.ReceivingDetail(Convert.ToInt32(addReceiving[1])), null);
                    recevingDetailView.Show();
                }
                else
                {
                    MessageBox.Show(addReceiving[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridViewReceivingItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridViewReceivingItem.CurrentCell.ColumnIndex == dataGridViewReceivingItem.Columns["ColumnDelete"].Index)
            {
                DialogResult deleteDialogResult = MessageBox.Show("Confirm delete?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (deleteDialogResult == DialogResult.Yes)
                {
                    var id = Convert.ToInt32(dataGridViewReceivingItem.Rows[e.RowIndex].Cells[dataGridViewReceivingItem.Columns["ColumnId"].Index].Value);

                    Controllers.TrnReceivingReceiverItemController trnReceivingItemController = new Controllers.TrnReceivingReceiverItemController();
                    String[] deleteReceivingItem = trnReceivingItemController.DeleteReceivingItem(id);
                    if (deleteReceivingItem[1].Equals("0") == false)
                    {
                        receivingItemPageNumber = 1;
                        UpdateReceivingItemListDataSource();
                        //comboBoxPullOutNumber.Text = "";
                        //comboBoxPullOutNumber.Focus();
                    }
                    else
                    {
                        MessageBox.Show(deleteReceivingItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult saveDialogResult = MessageBox.Show("Confirm save? This will lock the record.", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (saveDialogResult == DialogResult.Yes)
            {
                Controllers.TrnReceivingReceiverController trnReceivingController = new Controllers.TrnReceivingReceiverController();
                int pullOutItemsCount = trnReceivingController.GetTotalPullOutItemsCount(Convert.ToInt32(comboBoxPullOutNumber.SelectedValue));
                int receivingItemsCount = trnReceivingController.GetTotalReceivingItemsCount(trnReceivingModel.Id);
                if(pullOutItemsCount == receivingItemsCount)
                {
                    Models.TrnReceivingModel newReceivingModel = new Models.TrnReceivingModel()
                    {
                        SupplierId = Convert.ToInt32(comboBoxSupplier.SelectedValue),
                        PullOutId = Convert.ToInt32(comboBoxPullOutNumber.SelectedValue),
                        Remarks = textBoxRemarks.Text.Trim(),
                    };

                    String[] saveReceiving = trnReceivingController.LockReceiving(trnReceivingModel.Id, newReceivingModel);
                    if (saveReceiving[1].Equals("0") == false)
                    {
                        UpdateComponents(true);
                    }
                    else
                    {
                        MessageBox.Show(saveReceiving[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Validation Error: Please ensure the item count meets the required specifications before saving.", "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            //DialogResult downloadDialogResult = MessageBox.Show("Confirm download? This will delete the existing item records.", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (downloadDialogResult == DialogResult.Yes)
            //{
            //    Controllers.TrnReceivingReceiverItemController trnReceivingItemController = new Controllers.TrnReceivingReceiverItemController();
            //    trnReceivingItemController.DeleteAllReceivingItem(trnReceivingModel.Id);
            //    trnReceivingItemController.AddReceivingItem(trnReceivingModel.Id, Convert.ToInt32(comboBoxPullOutNumber.SelectedValue));
            //    UpdateReceivingItemListDataSource();
            //    comboBoxPullOutNumber.Text = "";
            //    comboBoxPullOutNumber.Focus();
            //}
        }

        private void textBoxBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Controllers.TrnReceivingReceiverItemController trnReceivingReceiverItemController = new Controllers.TrnReceivingReceiverItemController();
                if (trnReceivingReceiverItemController.isAlreadyAdded(textBoxBarcode.Text, trnReceivingModel.Id) == false)
                {
                    String[] addReceiving = trnReceivingReceiverItemController.AddReceivingItem(trnReceivingModel.Id, Convert.ToInt32(comboBoxPullOutNumber.SelectedValue), textBoxBarcode.Text);
                    if (addReceiving[1].Equals("0") == false)
                    {
                        UpdateReceivingItemListDataSource();
                        textBoxBarcode.Text = "";
                        textBoxBarcode.Focus();
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

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Controllers.TrnReceivingReceiverController trnReceivingController = new Controllers.TrnReceivingReceiverController();

            String[] unlockReceiving = trnReceivingController.UnlockReceving(trnReceivingModel.Id);
            if (unlockReceiving[1].Equals("0") == false)
            {
                UpdateComponents(false);
                if (historyView != null)
                {
                    historyView.UpdateReceivingListDataSource();
                }
            }
            else
            {
                MessageBox.Show(unlockReceiving[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
