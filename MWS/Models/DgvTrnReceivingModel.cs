using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models 
{
    public class DgvTrnReceivingModel
    {
        public Int32 ColumnId { get; set; }
        public Int32 ColumnReceivingBranchId { get; set; }
        public String ColumnReceivingDate { get; set; }
        public String ColumnReceivingNo { get; set; }
        public Int32 ColumnReceivingSupplierId { get; set; }
        public String ColumnReceivingSupplier { get; set; }
        public String ColumnReceivingRemarks { get; set; }
        public Int32 ColumnReceivingPreparedById { get; set; }
        public String ColumnReceivingPreparedBy { get; set; }
        public Boolean ColumnReceivingIsLocked { get; set; }
        public String ColumnReceivingView { get; set; }
    }
}