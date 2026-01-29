using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models 
{
    public class DgvTrnReceivingItemModel
    {
        public Int32 ColumnId { get; set; }
        public Int32 ColumnReceivingId { get; set; }
        public Int32 ColumnItemId { get; set; }
        public String ColumnBarcode { get; set; }
        public String ColumnItemDescription { get; set; }
        public Int32 ColumnSizeId { get; set; }
        public String ColumnSize { get; set; }
        public String ColumnWeight { get; set; }
        public String ColumnDelete { get; set; }
    }
}