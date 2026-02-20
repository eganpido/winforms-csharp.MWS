using MWS.Models;
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

namespace MWS.Views
{
    public partial class HistoryView : Form
    {
        public static List<DgvTrnReceivingModel> receivingListData = new List<DgvTrnReceivingModel>();
        public static Int32 receivingPageNumber = 1;
        public static Int32 receivingPageSize = 50;
        public PagedList<DgvTrnReceivingModel> receivingListPageList = new PagedList<DgvTrnReceivingModel>(receivingListData, receivingPageNumber, receivingPageSize);
        public BindingSource receivingListDataSource = new BindingSource();

        public static List<DgvTrnProductionModel> productionListData = new List<DgvTrnProductionModel>();
        public static Int32 productionPageNumber = 1;
        public static Int32 productionPageSize = 50;
        public PagedList<DgvTrnProductionModel> productionListPageList = new PagedList<DgvTrnProductionModel>(productionListData, productionPageNumber, productionPageSize);
        public BindingSource productionListDataSource = new BindingSource();

        public static List<DgvTrnPullOutModel> pullOutListData = new List<DgvTrnPullOutModel>();
        public static Int32 pullOutPageNumber = 1;
        public static Int32 pullOutPageSize = 50;
        public PagedList<DgvTrnPullOutModel> pullOutListPageList = new PagedList<DgvTrnPullOutModel>(pullOutListData, pullOutPageNumber, pullOutPageSize);
        public BindingSource pullOutListDataSource = new BindingSource();

        public HistoryView()
        {
            InitializeComponent();

            CreateReceivingListDataGridView();
            CreateProductionListDataGridView();
            CreatePullOutListDataGridView();

            var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            if(currentBranchId == 1)
            {
                tabPageProcessing.Text = "Processing";
            }
            else
            {
                tabPageProcessing.Text = "Production";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
            DashboardView dashboardView = new DashboardView();
            dashboardView.Show();
        }

        public void CreateReceivingListDataGridView()
        {
            UpdateReceivingListDataSource();

            dataGridViewReceiving.DataSource = receivingListDataSource;
        }
        public void CreateProductionListDataGridView()
        {
            UpdateProductionListDataSource();

            dataGridViewProcessing.DataSource = productionListDataSource;
        }
        public void CreatePullOutListDataGridView()
        {
            UpdatePullOutListDataSource();

            dataGridViewPullOut.DataSource = pullOutListDataSource;
        }
        public void UpdateReceivingListDataSource()
        {
            SetReceivingListDataSourceAsync();
        }
        public void UpdateProductionListDataSource()
        {
            SetProductionListDataSourceAsync();
        }
        public void UpdatePullOutListDataSource()
        {
            SetPullOutListDataSourceAsync();
        }

        public async void SetReceivingListDataSourceAsync()
        {
            List<DgvTrnReceivingModel> getReceivingListData = await GetReceivingListDataTask();
            if (getReceivingListData.Any())
            {
                receivingListData = getReceivingListData;
                receivingListPageList = new PagedList<DgvTrnReceivingModel>(receivingListData, receivingPageNumber, receivingPageSize);

                if (receivingListPageList.PageCount == 1)
                {
                    buttonReceivingFirst.Enabled = false;
                    buttonReceivingPrevious.Enabled = false;
                    buttonReceivingNext.Enabled = false;
                    buttonReceivingLast.Enabled = false;
                }
                else if (receivingPageNumber == 1)
                {
                    buttonReceivingFirst.Enabled = false;
                    buttonReceivingPrevious.Enabled = false;
                    buttonReceivingNext.Enabled = true;
                    buttonReceivingLast.Enabled = true;
                }
                else if (receivingPageNumber == receivingListPageList.PageCount)
                {
                    buttonReceivingFirst.Enabled = true;
                    buttonReceivingPrevious.Enabled = true;
                    buttonReceivingNext.Enabled = false;
                    buttonReceivingLast.Enabled = false;
                }
                else
                {
                    buttonReceivingFirst.Enabled = true;
                    buttonReceivingPrevious.Enabled = true;
                    buttonReceivingNext.Enabled = true;
                    buttonReceivingLast.Enabled = true;
                }

                textBoxReceivingPageNumber.Text = receivingPageNumber + " / " + receivingListPageList.PageCount;
                receivingListDataSource.DataSource = receivingListPageList;
            }
            else
            {
                buttonReceivingFirst.Enabled = false;
                buttonReceivingPrevious.Enabled = false;
                buttonReceivingNext.Enabled = false;
                buttonReceivingLast.Enabled = false;

                receivingPageNumber = 1;

                receivingListData = new List<DgvTrnReceivingModel>();
                receivingListDataSource.Clear();
                textBoxReceivingPageNumber.Text = "1 / 1";
            }

        }

        public async void SetProductionListDataSourceAsync()
        {
            List<DgvTrnProductionModel> getProductionListData = await GetProductionListDataTask();
            if (getProductionListData.Any())
            {
                productionListData = getProductionListData;
                productionListPageList = new PagedList<DgvTrnProductionModel>(productionListData, productionPageNumber, productionPageSize);

                if (productionListPageList.PageCount == 1)
                {
                    buttonProcessingFirst.Enabled = false;
                    buttonProcessingPrevious.Enabled = false;
                    buttonProcessingNext.Enabled = false;
                    buttonProcessingLast.Enabled = false;
                }
                else if (productionPageNumber == 1)
                {
                    buttonProcessingFirst.Enabled = false;
                    buttonProcessingPrevious.Enabled = false;
                    buttonProcessingNext.Enabled = true;
                    buttonProcessingLast.Enabled = true;
                }
                else if (productionPageNumber == productionListPageList.PageCount)
                {
                    buttonProcessingFirst.Enabled = true;
                    buttonProcessingPrevious.Enabled = true;
                    buttonProcessingNext.Enabled = false;
                    buttonProcessingLast.Enabled = false;
                }
                else
                {
                    buttonProcessingFirst.Enabled = true;
                    buttonProcessingPrevious.Enabled = true;
                    buttonProcessingNext.Enabled = true;
                    buttonProcessingLast.Enabled = true;
                }

                textBoxProcessingPageNumber.Text = productionPageNumber + " / " + productionListPageList.PageCount;
                productionListDataSource.DataSource = productionListPageList;
            }
            else
            {
                buttonProcessingFirst.Enabled = false;
                buttonProcessingPrevious.Enabled = false;
                buttonProcessingNext.Enabled = false;
                buttonProcessingLast.Enabled = false;

                productionPageNumber = 1;

                productionListData = new List<DgvTrnProductionModel>();
                productionListDataSource.Clear();
                textBoxProcessingPageNumber.Text = "1 / 1";
            }

        }
        public async void SetPullOutListDataSourceAsync()
        {
            List<DgvTrnPullOutModel> getPullOutListData = await GetPullOutListDataTask();
            if (getPullOutListData.Any())
            {
                pullOutListData = getPullOutListData;
                pullOutListPageList = new PagedList<DgvTrnPullOutModel>(pullOutListData, pullOutPageNumber, pullOutPageSize);

                if (pullOutListPageList.PageCount == 1)
                {
                    buttonPullOutFirst.Enabled = false;
                    buttonPullOutPrevious.Enabled = false;
                    buttonPullOutNext.Enabled = false;
                    buttonPullOutLast.Enabled = false;
                }
                else if (pullOutPageNumber == 1)
                {
                    buttonPullOutFirst.Enabled = false;
                    buttonPullOutPrevious.Enabled = false;
                    buttonPullOutNext.Enabled = true;
                    buttonPullOutLast.Enabled = true;
                }
                else if (pullOutPageNumber == pullOutListPageList.PageCount)
                {
                    buttonPullOutFirst.Enabled = true;
                    buttonPullOutPrevious.Enabled = true;
                    buttonPullOutNext.Enabled = false;
                    buttonPullOutLast.Enabled = false;
                }
                else
                {
                    buttonPullOutFirst.Enabled = true;
                    buttonPullOutPrevious.Enabled = true;
                    buttonPullOutNext.Enabled = true;
                    buttonPullOutLast.Enabled = true;
                }

                textBoxPullOutPageNumber.Text = pullOutPageNumber + " / " + pullOutListPageList.PageCount;
                pullOutListDataSource.DataSource = pullOutListPageList;
            }
            else
            {
                buttonPullOutFirst.Enabled = false;
                buttonPullOutPrevious.Enabled = false;
                buttonPullOutNext.Enabled = false;
                buttonPullOutLast.Enabled = false;

                pullOutPageNumber = 1;

                pullOutListData = new List<DgvTrnPullOutModel>();
                pullOutListDataSource.Clear();
                textBoxPullOutPageNumber.Text = "1 / 1";
            }

        }
        public Task<List<DgvTrnReceivingModel>> GetReceivingListDataTask()
        {
            DateTime startDateFilter = dtReceivingStartDate.Value.Date;
            DateTime endDateFilter = dtReceivingEndDate.Value.Date;
            String filter = txtReceivingSearch.Text;
            Controllers.TrnReceivingController trnReceivingController = new Controllers.TrnReceivingController();

            List<TrnReceivingModel> listReceiving = trnReceivingController.ReceivingList(startDateFilter, endDateFilter, filter);
            if (listReceiving.Any())
            {
                var items = from d in listReceiving
                            select new DgvTrnReceivingModel
                            {
                                ColumnId = d.Id,
                                ColumnReceivingBranchId = d.BranchId,
                                ColumnReceivingDate = d.ReceivingDate,
                                ColumnReceivingNo = d.ReceivingNo,
                                ColumnReceivingSupplierId = d.SupplierId,
                                ColumnReceivingSupplier = d.Supplier,
                                ColumnReceivingRemarks = d.Remarks,
                                ColumnReceivingPreparedById = d.PreparedById,
                                ColumnReceivingPreparedBy = d.PreparedBy.ToString(),
                                ColumnReceivingTotalWeight = d.TotalWeight.ToString(),
                                ColumnReceivingIsLocked = d.IsLocked,
                                ColumnReceivingView = "View",
                            };

                return Task.FromResult(items.ToList());
            }
            else
            {
                return Task.FromResult(new List<DgvTrnReceivingModel>());
            }
        }
        public Task<List<DgvTrnProductionModel>> GetProductionListDataTask()
        {
            DateTime startDateFilter = dtProcessingStartDate.Value.Date;
            DateTime endDateFilter = dtProcessingEndDate.Value.Date;
            String filter = txtProcessingSearch.Text;
            Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();

            List<TrnProductionModel> listProcessing = trnProductionController.ProductionList(startDateFilter, endDateFilter, filter);
            if (listProcessing.Any())
            {
                var items = from d in listProcessing
                            select new DgvTrnProductionModel
                            {
                                ColumnProcessingId = d.Id,
                                ColumnProcessingDate = d.ProductionDate,
                                ColumnProcessingNo = d.ProductionNo,
                                ColumnProcessingRemarks = d.Remarks,
                                ColumnProcessingPreparedById = d.PreparedById,
                                ColumnProcessingPreparedBy = d.PreparedBy.ToString(),
                                ColumnProcessingTotalWeight = d.TotalWeight.ToString(),
                                ColumnProcessingIsLocked = d.IsLocked,
                                ColumnProcessingView = "View",
                            };

                return Task.FromResult(items.ToList());
            }
            else
            {
                return Task.FromResult(new List<DgvTrnProductionModel>());
            }
        }
        public Task<List<DgvTrnPullOutModel>> GetPullOutListDataTask()
        {
            DateTime startDateFilter = dtPullStartDate.Value.Date;
            DateTime endDateFilter = dtPullEndDate.Value.Date;
            String filter = txtPullSearch.Text;
            Controllers.TrnPullOutController trnPullOutController = new Controllers.TrnPullOutController();

            List<TrnPullOutModel> listPullOut = trnPullOutController.PullOutList(startDateFilter, endDateFilter, filter);
            if (listPullOut.Any())
            {
                var items = from d in listPullOut
                            select new DgvTrnPullOutModel
                            {
                                ColumnPullOutId = d.Id,
                                ColumnPullOutDate = d.PullOutDate,
                                ColumnPullOutNo = d.PullOutNo,
                                ColumnPullOutRemarks = d.Remarks,
                                ColumnPullOutPreparedById = d.PreparedById,
                                ColumnPullOutPreparedBy = d.PreparedBy.ToString(),
                                ColumnPullOutIsLocked = d.IsLocked,
                                ColumnPullOutView = "View",
                            };

                return Task.FromResult(items.ToList());
            }
            else
            {
                return Task.FromResult(new List<DgvTrnPullOutModel>());
            }
        }

        private void buttonReceivingFirst_Click(object sender, EventArgs e)
        {
            receivingListPageList = new PagedList<DgvTrnReceivingModel>(receivingListData, 1, receivingPageSize);
            receivingListDataSource.DataSource = receivingListPageList;

            buttonReceivingFirst.Enabled = false;
            buttonReceivingPrevious.Enabled = false;
            buttonReceivingNext.Enabled = true;
            buttonReceivingLast.Enabled = true;

            receivingPageNumber = 1;
            textBoxReceivingPageNumber.Text = receivingPageNumber + " / " + receivingListPageList.PageCount;
        }

        private void buttonReceivingPrevious_Click(object sender, EventArgs e)
        {
            if (receivingListPageList.HasPreviousPage == true)
            {
                receivingListPageList = new PagedList<DgvTrnReceivingModel>(receivingListData, --receivingPageNumber, receivingPageSize);
                receivingListDataSource.DataSource = receivingListPageList;
            }

            buttonReceivingNext.Enabled = true;
            buttonReceivingLast.Enabled = true;

            if (receivingPageNumber == 1)
            {
                buttonReceivingFirst.Enabled = false;
                buttonReceivingPrevious.Enabled = false;
            }

            textBoxReceivingPageNumber.Text = receivingPageNumber + " / " + receivingListPageList.PageCount;
        }

        private void buttonReceivingNext_Click(object sender, EventArgs e)
        {
            if (receivingListPageList.HasNextPage == true)
            {
                receivingListPageList = new PagedList<DgvTrnReceivingModel>(receivingListData, ++receivingPageNumber, receivingPageSize);
                receivingListDataSource.DataSource = receivingListPageList;
            }

            buttonReceivingFirst.Enabled = true;
            buttonReceivingPrevious.Enabled = true;

            if (receivingPageNumber == receivingListPageList.PageCount)
            {
                buttonReceivingNext.Enabled = false;
                buttonReceivingLast.Enabled = false;
            }

            textBoxReceivingPageNumber.Text = receivingPageNumber + " / " + receivingListPageList.PageCount;
        }

        private void buttonReceivingLast_Click(object sender, EventArgs e)
        {
            receivingListPageList = new PagedList<DgvTrnReceivingModel>(receivingListData, receivingListPageList.PageCount, receivingPageSize);
            receivingListDataSource.DataSource = receivingListPageList;

            buttonReceivingFirst.Enabled = true;
            buttonReceivingPrevious.Enabled = true;
            buttonReceivingNext.Enabled = false;
            buttonReceivingLast.Enabled = false;

            receivingPageNumber = receivingListPageList.PageCount;
            textBoxReceivingPageNumber.Text = receivingPageNumber + " / " + receivingListPageList.PageCount;
        }

        private void txtReceivingSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateReceivingListDataSource();
            }
        }

        private void buttonProcessingFirst_Click(object sender, EventArgs e)
        {
            productionListPageList = new PagedList<DgvTrnProductionModel>(productionListData, 1, productionPageSize);
            productionListDataSource.DataSource = productionListPageList;

            buttonProcessingFirst.Enabled = false;
            buttonProcessingPrevious.Enabled = false;
            buttonProcessingNext.Enabled = true;
            buttonProcessingLast.Enabled = true;

            productionPageNumber = 1;
            textBoxProcessingPageNumber.Text = productionPageNumber + " / " + receivingListPageList.PageCount;
        }

        private void buttonProcessingPrevious_Click(object sender, EventArgs e)
        {
            if (productionListPageList.HasPreviousPage == true)
            {
                productionListPageList = new PagedList<DgvTrnProductionModel>(productionListData, --productionPageNumber, productionPageSize);
                productionListDataSource.DataSource = productionListPageList;
            }

            buttonProcessingNext.Enabled = true;
            buttonProcessingLast.Enabled = true;

            if (productionPageNumber == 1)
            {
                buttonProcessingFirst.Enabled = false;
                buttonProcessingPrevious.Enabled = false;
            }

            textBoxProcessingPageNumber.Text = productionPageNumber + " / " + productionListPageList.PageCount;
        }

        private void buttonProcessingNext_Click(object sender, EventArgs e)
        {
            if (productionListPageList.HasNextPage == true)
            {
                productionListPageList = new PagedList<DgvTrnProductionModel>(productionListData, ++productionPageNumber, productionPageSize);
                productionListDataSource.DataSource = productionListPageList;
            }

            buttonProcessingFirst.Enabled = true;
            buttonProcessingPrevious.Enabled = true;

            if (productionPageNumber == productionListPageList.PageCount)
            {
                buttonProcessingNext.Enabled = false;
                buttonProcessingLast.Enabled = false;
            }

            textBoxProcessingPageNumber.Text = productionPageNumber + " / " + productionListPageList.PageCount;
        }

        private void buttonProcessingLast_Click(object sender, EventArgs e)
        {
            productionListPageList = new PagedList<DgvTrnProductionModel>(productionListData, productionListPageList.PageCount, productionPageSize);
            productionListDataSource.DataSource = productionListPageList;

            buttonProcessingFirst.Enabled = true;
            buttonProcessingPrevious.Enabled = true;
            buttonProcessingNext.Enabled = false;
            buttonProcessingLast.Enabled = false;

            productionPageNumber = productionListPageList.PageCount;
            textBoxProcessingPageNumber.Text = productionPageNumber + " / " + productionListPageList.PageCount;
        }

        private void buttonPullOutFirst_Click(object sender, EventArgs e)
        {
            pullOutListPageList = new PagedList<DgvTrnPullOutModel>(pullOutListData, 1, pullOutPageSize);
            pullOutListDataSource.DataSource = pullOutListPageList;

            buttonPullOutFirst.Enabled = false;
            buttonPullOutPrevious.Enabled = false;
            buttonPullOutNext.Enabled = true;
            buttonPullOutLast.Enabled = true;

            pullOutPageNumber = 1;
            textBoxPullOutPageNumber.Text = pullOutPageNumber + " / " + pullOutListPageList.PageCount;
        }

        private void buttonPullOutPrevious_Click(object sender, EventArgs e)
        {
            if (pullOutListPageList.HasPreviousPage == true)
            {
                pullOutListPageList = new PagedList<DgvTrnPullOutModel>(pullOutListData, --pullOutPageNumber, pullOutPageSize);
                pullOutListDataSource.DataSource = pullOutListPageList;
            }

            buttonPullOutNext.Enabled = true;
            buttonPullOutLast.Enabled = true;

            if (pullOutPageNumber == 1)
            {
                buttonPullOutFirst.Enabled = false;
                buttonPullOutPrevious.Enabled = false;
            }

            textBoxPullOutPageNumber.Text = pullOutPageNumber + " / " + pullOutListPageList.PageCount;
        }

        private void buttonPullOutNext_Click(object sender, EventArgs e)
        {
            if (pullOutListPageList.HasNextPage == true)
            {
                pullOutListPageList = new PagedList<DgvTrnPullOutModel>(pullOutListData, ++pullOutPageNumber, pullOutPageSize);
                pullOutListDataSource.DataSource = pullOutListPageList;
            }

            buttonPullOutFirst.Enabled = true;
            buttonPullOutPrevious.Enabled = true;

            if (pullOutPageNumber == pullOutListPageList.PageCount)
            {
                buttonPullOutNext.Enabled = false;
                buttonPullOutLast.Enabled = false;
            }

            textBoxPullOutPageNumber.Text = pullOutPageNumber + " / " + pullOutListPageList.PageCount;
        }

        private void buttonPullOutLast_Click(object sender, EventArgs e)
        {
            pullOutListPageList = new PagedList<DgvTrnPullOutModel>(pullOutListData, pullOutListPageList.PageCount, pullOutPageSize);
            pullOutListDataSource.DataSource = pullOutListPageList;

            buttonPullOutFirst.Enabled = true;
            buttonPullOutPrevious.Enabled = true;
            buttonPullOutNext.Enabled = false;
            buttonPullOutLast.Enabled = false;

            pullOutPageNumber = pullOutListPageList.PageCount;
            textBoxPullOutPageNumber.Text = pullOutPageNumber + " / " + pullOutListPageList.PageCount;
        }

        private void txtProcessingSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateProductionListDataSource();
            }
        }

        private void txtPullSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdatePullOutListDataSource();
            }
        }

        private void dtReceivingStartDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateReceivingListDataSource();
        }

        private void dtReceivingEndDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateReceivingListDataSource();
        }

        private void dtProcessingStartDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateProductionListDataSource();
        }

        private void dtProcessingEndDate_ValueChanged(object sender, EventArgs e)
        {
            UpdateProductionListDataSource();
        }

        private void dtPullStartDate_ValueChanged(object sender, EventArgs e)
        {
            UpdatePullOutListDataSource();
        }

        private void dtPullEndDate_ValueChanged(object sender, EventArgs e)
        {
            UpdatePullOutListDataSource();
        }

        private void dataGridViewReceiving_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridViewReceiving.CurrentCell.ColumnIndex == dataGridViewReceiving.Columns["ColumnReceivingView"].Index)
            {
                int branchId = Convert.ToInt32(dataGridViewReceiving.Rows[e.RowIndex].Cells[1].Value);
                if (branchId == 1)
                {
                    Controllers.TrnReceivingController trnReceivingController = new Controllers.TrnReceivingController();
                    RecevingDetailView recevingDetailView = new RecevingDetailView(trnReceivingController.ReceivingDetail(Convert.ToInt32(dataGridViewReceiving.Rows[e.RowIndex].Cells[0].Value)), this);
                    recevingDetailView.Show();
                }
                else
                {
                    Controllers.TrnReceivingReceiverController trnReceivingReceiverController = new Controllers.TrnReceivingReceiverController();
                    ReceivingDetailReceiverView recevingDetailReceiverView = new ReceivingDetailReceiverView(trnReceivingReceiverController.ReceivingDetail(Convert.ToInt32(dataGridViewReceiving.Rows[e.RowIndex].Cells[0].Value)), this);
                    recevingDetailReceiverView.Show();
                }
            }
        }

        private void dataGridViewProcessing_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridViewProcessing.CurrentCell.ColumnIndex == dataGridViewProcessing.Columns["ColumnProcessingView"].Index)
            {
                Controllers.TrnProductionController trnProductionController = new Controllers.TrnProductionController();
                ProductionDetailView productionDetailView = new ProductionDetailView(trnProductionController.ProductionDetail(Convert.ToInt32(dataGridViewProcessing.Rows[e.RowIndex].Cells[0].Value)), this);
                productionDetailView.Show();
            }
        }

        private void dataGridViewPullOut_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridViewPullOut.CurrentCell.ColumnIndex == dataGridViewPullOut.Columns["ColumnPullOutView"].Index)
            {
                Controllers.TrnPullOutController trnPullOutController = new Controllers.TrnPullOutController();
                PullOutDetailView pullOutDetailView = new PullOutDetailView(trnPullOutController.PullOutDetail(Convert.ToInt32(dataGridViewPullOut.Rows[e.RowIndex].Cells[0].Value)), this);
                pullOutDetailView.Show();
            }
        }
    }
}
