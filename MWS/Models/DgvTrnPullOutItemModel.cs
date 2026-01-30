using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models 
{
    public class DgvTrnPullOutItemModel
    {
        public Int32 ColumnId { get; set; }
        public Int32 ColumnPullOutId { get; set; }
        public Int32 ColumnReceivingItemId { get; set; }
        public Int32 ColumnItemId { get; set; }
        public String ColumnBarcode { get; set; }
        public String ColumnItemDescription { get; set; }
        public Int32 ColumnSizeId { get; set; }
        public String ColumnSize { get; set; }
        public String ColumnDelete { get; set; }
    }
}