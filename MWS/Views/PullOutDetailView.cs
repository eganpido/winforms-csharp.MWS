using MWS.Controllers;
using MWS.Models;
using MWS.Modules;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace MWS.Views
{
    public partial class PullOutDetailView : Form
    {
        public Models.TrnPullOutModel trnPullOutModel;
        public HistoryView historyView;
        public static List<Models.DgvTrnPullOutItemModel> pullOutItemData = new List<Models.DgvTrnPullOutItemModel>();
        public static Int32 pullOutItemPageNumber = 1;
        public static Int32 pullOutItemPageSize = 20;
        public PagedList<Models.DgvTrnPullOutItemModel> pullOutItemPageList = new PagedList<Models.DgvTrnPullOutItemModel>(pullOutItemData, pullOutItemPageNumber, pullOutItemPageSize);
        public BindingSource pullOutItemDataSource = new BindingSource();
        public PullOutDetailView(Models.TrnPullOutModel pullOutModel, HistoryView _historyView)
        {
            InitializeComponent();

            trnPullOutModel = pullOutModel;
            historyView = _historyView;
            var id = trnPullOutModel.Id;

            Controllers.TrnPullOutController trnPullOutController = new Controllers.TrnPullOutController();
            var detail = trnPullOutController.PullOutDetail(id);

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
            dataGridViewPullOutItem.EnableHeadersVisualStyles = true;
            dataGridViewPullOutItem.ColumnHeadersDefaultCellStyle.Font =
                         new Font("Open Sans", 11F, FontStyle.Regular);
            dataGridViewPullOutItem.ScrollBars = ScrollBars.Vertical;
            dataGridViewPullOutItem.Dock = DockStyle.None;
            dataGridViewPullOutItem.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridViewPullOutItem.AllowUserToResizeRows = false;
            dataGridViewPullOutItem.RowTemplate.Height = 32;

            GetPullOutDetail();
        }
        public void GetPullOutDetail()
        {
            UpdateComponents(trnPullOutModel.IsLocked);
            textBoxBarcode.Focus();

            CreatePullOutItemListDataGridView();
        }
        public void CreatePullOutItemListDataGridView()
        {
            UpdatePullOutItemListDataSource();

            dataGridViewPullOutItem.DataSource = pullOutItemDataSource;
        }
        public void UpdatePullOutItemListDataSource()
        {
            SetPullOutItemListDataSourceAsync();
        }
        public async void SetPullOutItemListDataSourceAsync()
        {
            List<Models.DgvTrnPullOutItemModel> getPullOutItemListData = await GetPullOutItemListDataTask();
            if (getPullOutItemListData.Any())
            {
                pullOutItemData = getPullOutItemListData;
                pullOutItemPageList = new PagedList<Models.DgvTrnPullOutItemModel>(pullOutItemData, pullOutItemPageNumber, pullOutItemPageSize);

                if (pullOutItemPageList.PageCount == 1)
                {
                    buttonFirst.Enabled = false;
                    buttonPrevious.Enabled = false;
                    buttonNext.Enabled = false;
                    buttonLast.Enabled = false;
                }
                else if (pullOutItemPageNumber == 1)
                {
                    buttonFirst.Enabled = false;
                    buttonPrevious.Enabled = false;
                    buttonNext.Enabled = true;
                    buttonLast.Enabled = true;
                }
                else if (pullOutItemPageNumber == pullOutItemPageList.PageCount)
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

                textBoxPageNumber.Text = pullOutItemPageNumber + " / " + pullOutItemPageList.PageCount;
                pullOutItemDataSource.DataSource = pullOutItemPageList;
            }
            else
            {
                buttonFirst.Enabled = false;
                buttonPrevious.Enabled = false;
                buttonNext.Enabled = false;
                buttonLast.Enabled = false;

                pullOutItemPageNumber = 1;

                pullOutItemData = new List<Models.DgvTrnPullOutItemModel>();
                pullOutItemDataSource.Clear();
                textBoxPageNumber.Text = "1 / 1";
            }
        }
        public Task<List<Models.DgvTrnPullOutItemModel>> GetPullOutItemListDataTask()
        {
            Controllers.TrnPullOutItemController trnPullOutItemController = new Controllers.TrnPullOutItemController();

            List<Models.TrnPullOutItemModel> listPullOutItem = trnPullOutItemController.PullOutItemList(trnPullOutModel.Id);
            if (listPullOutItem.Any())
            {
                var items = from d in listPullOutItem
                            select new Models.DgvTrnPullOutItemModel
                            {
                                ColumnId = d.Id,
                                ColumnPullOutId = d.PullOutId,
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
                return Task.FromResult(new List<Models.DgvTrnPullOutItemModel>());
            }
        }
        public void UpdateComponents(Boolean isLocked)
        {
            buttonSave.Enabled = !isLocked;
            buttonEdit.Enabled = isLocked;
            buttonPrint.Enabled = isLocked;
            textBoxBarcode.Enabled = !isLocked;

            dataGridViewPullOutItem.Columns[9].Visible = !isLocked;
            textBoxBarcode.Focus();

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
                historyView.UpdatePullOutListDataSource();
            }
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            pullOutItemPageList = new PagedList<Models.DgvTrnPullOutItemModel>(pullOutItemData, 1, pullOutItemPageSize);
            pullOutItemDataSource.DataSource = pullOutItemPageList;

            buttonFirst.Enabled = false;
            buttonPrevious.Enabled = false;
            buttonNext.Enabled = true;
            buttonLast.Enabled = true;

            pullOutItemPageNumber = 1;
            textBoxPageNumber.Text = pullOutItemPageNumber + " / " + pullOutItemPageList.PageCount;
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (pullOutItemPageList.HasPreviousPage == true)
            {
                pullOutItemPageList = new PagedList<Models.DgvTrnPullOutItemModel>(pullOutItemData, --pullOutItemPageNumber, pullOutItemPageSize);
                pullOutItemDataSource.DataSource = pullOutItemPageList;
            }

            buttonNext.Enabled = true;
            buttonLast.Enabled = true;

            if (pullOutItemPageNumber == 1)
            {
                buttonFirst.Enabled = false;
                buttonPrevious.Enabled = false;
            }

            textBoxPageNumber.Text = pullOutItemPageNumber + " / " + pullOutItemPageList.PageCount;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (pullOutItemPageList.HasNextPage == true)
            {
                pullOutItemPageList = new PagedList<Models.DgvTrnPullOutItemModel>(pullOutItemData, ++pullOutItemPageNumber, pullOutItemPageSize);
                pullOutItemDataSource.DataSource = pullOutItemPageList;
            }

            buttonFirst.Enabled = true;
            buttonPrevious.Enabled = true;

            if (pullOutItemPageNumber == pullOutItemPageList.PageCount)
            {
                buttonNext.Enabled = false;
                buttonLast.Enabled = false;
            }

            textBoxPageNumber.Text = pullOutItemPageNumber + " / " + pullOutItemPageList.PageCount;
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            pullOutItemPageList = new PagedList<Models.DgvTrnPullOutItemModel>(pullOutItemData, pullOutItemPageList.PageCount, pullOutItemPageSize);
            pullOutItemDataSource.DataSource = pullOutItemPageList;

            buttonFirst.Enabled = true;
            buttonPrevious.Enabled = true;
            buttonNext.Enabled = false;
            buttonLast.Enabled = false;

            pullOutItemPageNumber = pullOutItemPageList.PageCount;
            textBoxPageNumber.Text = pullOutItemPageNumber + " / " + pullOutItemPageList.PageCount;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult saveDialogResult = MessageBox.Show("Confirm save? This will lock the record.", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (saveDialogResult == DialogResult.Yes)
            {
                Controllers.TrnPullOutController trnPullOutController = new Controllers.TrnPullOutController();

                String[] savePullOut = trnPullOutController.LockPullOut(trnPullOutModel.Id);
                if (savePullOut[1].Equals("0") == false)
                {
                    UpdateComponents(true);
                }
                else
                {
                    MessageBox.Show(savePullOut[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult add = MessageBox.Show("Confirm add new record?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (add == DialogResult.Yes)
            {
                Controllers.TrnPullOutController trnPullOutController = new Controllers.TrnPullOutController();
                String[] addPullOut = trnPullOutController.AddPullOut();
                if (addPullOut[1].Equals("0") == false)
                {
                    Close();
                    PullOutDetailView pullOutDetailView = new PullOutDetailView(trnPullOutController.PullOutDetail(Convert.ToInt32(addPullOut[1])), null);
                    pullOutDetailView.Show();
                }
                else
                {
                    MessageBox.Show(addPullOut[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridViewPullOutItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridViewPullOutItem.CurrentCell.ColumnIndex == dataGridViewPullOutItem.Columns["ColumnDelete"].Index)
            {
                DialogResult deleteDialogResult = MessageBox.Show("Confirm delete?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (deleteDialogResult == DialogResult.Yes)
                {
                    var id = Convert.ToInt32(dataGridViewPullOutItem.Rows[e.RowIndex].Cells[dataGridViewPullOutItem.Columns["ColumnId"].Index].Value);

                    Controllers.TrnPullOutItemController trnPullOutItemController = new Controllers.TrnPullOutItemController();
                    String[] deletePullOutItem = trnPullOutItemController.DeletePullOutItem(id);
                    if (deletePullOutItem[1].Equals("0") == false)
                    {
                        pullOutItemPageNumber = 1;
                        UpdatePullOutItemListDataSource();
                        textBoxBarcode.Text = "";
                        textBoxBarcode.Focus();
                    }
                    else
                    {
                        MessageBox.Show(deletePullOutItem[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void textBoxBarcode_Click(object sender, EventArgs e)
        {
            
        }

        private void textBoxBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Controllers.TrnPullOutItemController trnPullOutItemController = new Controllers.TrnPullOutItemController();
                if (trnPullOutItemController.isAlreadyAdded(textBoxBarcode.Text, trnPullOutModel.Id) == false)
                {

                    int receivingItemId = trnPullOutItemController.GetProductionItem(textBoxBarcode.Text);
                    if (receivingItemId > 0)
                    {
                        trnPullOutItemController.AddPullOutItem(trnPullOutModel.Id, textBoxBarcode.Text);
                        UpdatePullOutItemListDataSource();
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
            Controllers.TrnPullOutController trnPullOutController = new Controllers.TrnPullOutController();

            String[] unlockPullOut = trnPullOutController.UnlockPullOut(trnPullOutModel.Id);
            if (unlockPullOut[1].Equals("0") == false)
            {
                UpdateComponents(false);
                if (historyView != null)
                {
                    historyView.UpdateReceivingListDataSource();
                }
            }
            else
            {
                MessageBox.Show(unlockPullOut[0], "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            DialogResult printDialogResult = MessageBox.Show("Confirm print?", "MWS", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (printDialogResult == DialogResult.Yes)
            {
                GenerateAndPrint();
            }
        }
        private void GenerateAndPrint()
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintBarcodeHandler);

                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PrintBarcodeHandler(object sender, PrintPageEventArgs e)
        {
            Font fontBold = new Font("Segoe UI", 18, FontStyle.Bold);
            Font fontRegular = new Font("Segoe UI", 13, FontStyle.Regular);

            int startX = 30;
            int baseStartY = 120; 
            int labelSpacing = 290;

            int barcodeWidth = 300;

            for (int i = 0; i < 1; i++)
            {
                int startY = baseStartY + (i * labelSpacing);

                string topText = "Pull Out No. : " + trnPullOutModel.PullOutNo;
                float topTextWidth = e.Graphics.MeasureString(topText, fontBold).Width;
                float topTextX = startX + (barcodeWidth - topTextWidth) / 2;
                e.Graphics.DrawString(topText, fontBold, Brushes.Black, topTextX, startY - 95);

                string countText = "Total Count : " + txtTotalCount.Text;
                float countTextWidth = e.Graphics.MeasureString(countText, fontBold).Width;
                float countTextX = startX + (barcodeWidth - countTextWidth) / 2;
                e.Graphics.DrawString(countText, fontBold, Brushes.Black, countTextX, 80);

                string weightText = "Total Weight : " + txtTotalWeight.Text;
                float weightTextWidth = e.Graphics.MeasureString(weightText, fontBold).Width;
                float weightTextX = startX + (barcodeWidth - weightTextWidth) / 2;
                e.Graphics.DrawString(weightText, fontBold, Brushes.Black, weightTextX, 135);
            }
        }
    }
}
