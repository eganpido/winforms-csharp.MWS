using MWS.Controllers;
using MWS.Models;
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
    public partial class SystemTableView : Form
    {
        public static List<DgvMstItemModel> itemListData = new List<DgvMstItemModel>();
        public static Int32 itemPageNumber = 1;
        public static Int32 itemPageSize = 50;
        public PagedList<DgvMstItemModel> itemListPageList = new PagedList<DgvMstItemModel>(itemListData, itemPageNumber, itemPageSize);
        public BindingSource itemListDataSource = new BindingSource();

        public static List<DgvMstSupplierModel> supplierListData = new List<DgvMstSupplierModel>();
        public static Int32 supplierPageNumber = 1;
        public static Int32 supplierPageSize = 50;
        public PagedList<DgvMstSupplierModel> supplierListPageList = new PagedList<DgvMstSupplierModel>(supplierListData, supplierPageNumber, supplierPageSize);
        public BindingSource supplierListDataSource = new BindingSource();
        public SystemTableView()
        {
            InitializeComponent();

            CreateItemListDataGridView();
            CreateSupplierListDataGridView();
        }
        public void CreateItemListDataGridView()
        {
            UpdateItemListDataSource();

            dataGridViewItem.DataSource = itemListDataSource;
        }
        public void CreateSupplierListDataGridView()
        {
            UpdateSupplierListDataSource();

            dataGridViewSupplier.DataSource = supplierListDataSource;
        }
        public void UpdateItemListDataSource()
        {
            SetItemListDataSourceAsync();
        }
        public void UpdateSupplierListDataSource()
        {
            SetSupplierListDataSourceAsync();
        }
        public async void SetItemListDataSourceAsync()
        {
            List<DgvMstItemModel> getItemListData = await GetItemListDataTask();
            if (getItemListData.Any())
            {
                itemListData = getItemListData;
                itemListPageList = new PagedList<DgvMstItemModel>(itemListData, itemPageNumber, itemPageSize);

                if (itemListPageList.PageCount == 1)
                {
                    buttonItemFirst.Enabled = false;
                    buttonItemPrevious.Enabled = false;
                    buttonItemNext.Enabled = false;
                    buttonItemLast.Enabled = false;
                }
                else if (itemPageNumber == 1)
                {
                    buttonItemFirst.Enabled = false;
                    buttonItemPrevious.Enabled = false;
                    buttonItemNext.Enabled = true;
                    buttonItemLast.Enabled = true;
                }
                else if (itemPageNumber == itemListPageList.PageCount)
                {
                    buttonItemFirst.Enabled = true;
                    buttonItemPrevious.Enabled = true;
                    buttonItemNext.Enabled = false;
                    buttonItemLast.Enabled = false;
                }
                else
                {
                    buttonItemFirst.Enabled = true;
                    buttonItemPrevious.Enabled = true;
                    buttonItemNext.Enabled = true;
                    buttonItemLast.Enabled = true;
                }

                textBoxItemPageNumber.Text = itemPageNumber + " / " + itemListPageList.PageCount;
                itemListDataSource.DataSource = itemListPageList;
            }
            else
            {
                buttonItemFirst.Enabled = false;
                buttonItemPrevious.Enabled = false;
                buttonItemNext.Enabled = false;
                buttonItemLast.Enabled = false;

                itemPageNumber = 1;

                itemListData = new List<DgvMstItemModel>();
                itemListDataSource.Clear();
                textBoxItemPageNumber.Text = "1 / 1";
            }

        }

        public async void SetSupplierListDataSourceAsync()
        {
            List<DgvMstSupplierModel> getSupplierListData = await GetSupplierListDataTask();
            if (getSupplierListData.Any())
            {
                supplierListData = getSupplierListData;
                supplierListPageList = new PagedList<DgvMstSupplierModel>(supplierListData, supplierPageNumber, supplierPageSize);

                if (supplierListPageList.PageCount == 1)
                {
                    buttonSupplierFirst.Enabled = false;
                    buttonSupplierPrevious.Enabled = false;
                    buttonSupplierNext.Enabled = false;
                    buttonSupplierLast.Enabled = false;
                }
                else if (supplierPageNumber == 1)
                {
                    buttonSupplierFirst.Enabled = false;
                    buttonSupplierPrevious.Enabled = false;
                    buttonSupplierNext.Enabled = true;
                    buttonSupplierLast.Enabled = true;
                }
                else if (supplierPageNumber == supplierListPageList.PageCount)
                {
                    buttonSupplierFirst.Enabled = true;
                    buttonSupplierPrevious.Enabled = true;
                    buttonSupplierNext.Enabled = false;
                    buttonSupplierLast.Enabled = false;
                }
                else
                {
                    buttonSupplierFirst.Enabled = true;
                    buttonSupplierPrevious.Enabled = true;
                    buttonSupplierNext.Enabled = true;
                    buttonSupplierLast.Enabled = true;
                }

                textBoxSupplierPageNumber.Text = supplierPageNumber + " / " + supplierListPageList.PageCount;
                supplierListDataSource.DataSource = supplierListPageList;
            }
            else
            {
                buttonSupplierFirst.Enabled = false;
                buttonSupplierPrevious.Enabled = false;
                buttonSupplierNext.Enabled = false;
                buttonSupplierLast.Enabled = false;

                supplierPageNumber = 1;

                supplierListData = new List<DgvMstSupplierModel>();
                supplierListDataSource.Clear();
                textBoxSupplierPageNumber.Text = "1 / 1";
            }

        }
        public Task<List<DgvMstItemModel>> GetItemListDataTask()
        {
            String filter = txtItemSearch.Text;
            Controllers.MstItemController mstItemController = new Controllers.MstItemController();

            List<MstItemModel> listItem = mstItemController.ItemList(filter);
            if (listItem.Any())
            {
                var items = from d in listItem
                            select new DgvMstItemModel
                            {
                                ColumnId = d.Id,
                                ColumnItem = d.Item,
                                ColumnItemEdit = "Edit",
                            };

                return Task.FromResult(items.ToList());
            }
            else
            {
                return Task.FromResult(new List<DgvMstItemModel>());
            }
        }
        public Task<List<DgvMstSupplierModel>> GetSupplierListDataTask()
        {
            String filter = txtSupplierSearch.Text;
            Controllers.MstSupplierController mstSupplierController = new Controllers.MstSupplierController();

            List<MstSupplierModel> listSupplier = mstSupplierController.SupplierList(filter);
            if (listSupplier.Any())
            {
                var suppliers = from d in listSupplier
                                select new DgvMstSupplierModel
                            {
                                ColumnId = d.Id,
                                ColumnSupplier = d.Supplier,
                                ColumnSupplierEdit = "Edit",
                            };

                return Task.FromResult(suppliers.ToList());
            }
            else
            {
                return Task.FromResult(new List<DgvMstSupplierModel>());
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
            DashboardView dashboardView = new DashboardView();
            dashboardView.Show();
        }

        private void buttonItemFirst_Click(object sender, EventArgs e)
        {
            itemListPageList = new PagedList<DgvMstItemModel>(itemListData, 1, itemPageSize);
            itemListDataSource.DataSource = itemListPageList;

            buttonItemFirst.Enabled = false;
            buttonItemPrevious.Enabled = false;
            buttonItemNext.Enabled = true;
            buttonItemLast.Enabled = true;

            itemPageNumber = 1;
            textBoxItemPageNumber.Text = itemPageNumber + " / " + itemListPageList.PageCount;
        }

        private void buttonItemPrevious_Click(object sender, EventArgs e)
        {
            if (itemListPageList.HasPreviousPage == true)
            {
                itemListPageList = new PagedList<DgvMstItemModel>(itemListData, --itemPageNumber, itemPageSize);
                itemListDataSource.DataSource = itemListPageList;
            }

            buttonItemNext.Enabled = true;
            buttonItemLast.Enabled = true;

            if (itemPageNumber == 1)
            {
                buttonItemFirst.Enabled = false;
                buttonItemPrevious.Enabled = false;
            }

            textBoxItemPageNumber.Text = itemPageNumber + " / " + itemListPageList.PageCount;
        }

        private void buttonItemNext_Click(object sender, EventArgs e)
        {
            if (itemListPageList.HasNextPage == true)
            {
                itemListPageList = new PagedList<DgvMstItemModel>(itemListData, ++itemPageNumber, itemPageSize);
                itemListDataSource.DataSource = itemListPageList;
            }

            buttonItemFirst.Enabled = true;
            buttonItemPrevious.Enabled = true;

            if (itemPageNumber == itemListPageList.PageCount)
            {
                buttonItemNext.Enabled = false;
                buttonItemLast.Enabled = false;
            }

            textBoxItemPageNumber.Text = itemPageNumber + " / " + itemListPageList.PageCount;
        }

        private void buttonItemLast_Click(object sender, EventArgs e)
        {
            itemListPageList = new PagedList<DgvMstItemModel>(itemListData, itemListPageList.PageCount, itemPageSize);
            itemListDataSource.DataSource = itemListPageList;

            buttonItemFirst.Enabled = true;
            buttonItemPrevious.Enabled = true;
            buttonItemNext.Enabled = false;
            buttonItemLast.Enabled = false;

            itemPageNumber = itemListPageList.PageCount;
            textBoxItemPageNumber.Text = itemPageNumber + " / " + itemListPageList.PageCount;
        }

        private void txtItemSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateItemListDataSource();
            }
        }

        private void buttonSupplierFirst_Click(object sender, EventArgs e)
        {
            supplierListPageList = new PagedList<DgvMstSupplierModel>(supplierListData, 1, supplierPageSize);
            supplierListDataSource.DataSource = supplierListPageList;

            buttonSupplierFirst.Enabled = false;
            buttonSupplierPrevious.Enabled = false;
            buttonSupplierNext.Enabled = true;
            buttonSupplierLast.Enabled = true;

            supplierPageNumber = 1;
            textBoxSupplierPageNumber.Text = supplierPageNumber + " / " + supplierListPageList.PageCount;
        }

        private void buttonSupplierPrevious_Click(object sender, EventArgs e)
        {
            if (supplierListPageList.HasPreviousPage == true)
            {
                supplierListPageList = new PagedList<DgvMstSupplierModel>(supplierListData, --supplierPageNumber, supplierPageSize);
                supplierListDataSource.DataSource = supplierListPageList;
            }

            buttonSupplierNext.Enabled = true;
            buttonSupplierLast.Enabled = true;

            if (supplierPageNumber == 1)
            {
                buttonSupplierFirst.Enabled = false;
                buttonSupplierPrevious.Enabled = false;
            }

            textBoxSupplierPageNumber.Text = supplierPageNumber + " / " + supplierListPageList.PageCount;
        }

        private void buttonSupplierNext_Click(object sender, EventArgs e)
        {
            if (supplierListPageList.HasNextPage == true)
            {
                supplierListPageList = new PagedList<DgvMstSupplierModel>(supplierListData, ++supplierPageNumber, supplierPageSize);
                supplierListDataSource.DataSource = supplierListPageList;
            }

            buttonSupplierFirst.Enabled = true;
            buttonSupplierPrevious.Enabled = true;

            if (supplierPageNumber == supplierListPageList.PageCount)
            {
                buttonSupplierNext.Enabled = false;
                buttonSupplierLast.Enabled = false;
            }

            textBoxSupplierPageNumber.Text = supplierPageNumber + " / " + supplierListPageList.PageCount;
        }

        private void buttonSupplierLast_Click(object sender, EventArgs e)
        {
            supplierListPageList = new PagedList<DgvMstSupplierModel>(supplierListData, supplierListPageList.PageCount, supplierPageSize);
            supplierListDataSource.DataSource = supplierListPageList;

            buttonSupplierFirst.Enabled = true;
            buttonSupplierPrevious.Enabled = true;
            buttonSupplierNext.Enabled = false;
            buttonSupplierLast.Enabled = false;

            supplierPageNumber = supplierListPageList.PageCount;
            textBoxSupplierPageNumber.Text = supplierPageNumber + " / " + supplierListPageList.PageCount;
        }

        private void txtSupplierSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateSupplierListDataSource();
            }
        }

        private void dataGridViewItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridViewItem.CurrentCell.ColumnIndex == dataGridViewItem.Columns["ColumnItemEdit"].Index)
            {
                Controllers.MstItemController mstItemController = new Controllers.MstItemController();
                ItemDetailView itemDetailView = new ItemDetailView(this, mstItemController.ItemDetail(Convert.ToInt32(dataGridViewItem.Rows[e.RowIndex].Cells[0].Value)));
                itemDetailView.Show();
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            ItemDetailView itemDetailView = new ItemDetailView(this, null);
            itemDetailView.Show();
        }

        private void dataGridViewSupplier_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridViewSupplier.CurrentCell.ColumnIndex == dataGridViewSupplier.Columns["ColumnSupplierEdit"].Index)
            {
                Controllers.MstSupplierController mstSupplierController = new Controllers.MstSupplierController();
                SupplierDetailView supplierDetailView = new SupplierDetailView(this, mstSupplierController.SupplierDetail(Convert.ToInt32(dataGridViewSupplier.Rows[e.RowIndex].Cells[0].Value)));
                supplierDetailView.Show();
            }
        }

        private void btnAddSupplier_Click(object sender, EventArgs e)
        {
            SupplierDetailView supplierDetailView = new SupplierDetailView(this, null);
            supplierDetailView.Show();
        }
    }
}
