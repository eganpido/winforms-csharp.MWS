using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class TrnReceivingModel
    {
        public Int32 Id { get; set; }
        public Int32 BranchId { get; set; }
        public String ReceivingDate { get; set; }
        public String ReceivingNo { get; set; }
        public Int32 SupplierId { get; set; }
        public String Supplier { get; set; }
        public String Remarks { get; set; }
        public Int32 PreparedById { get; set; }
        public String PreparedBy { get; set; }
        public Decimal TotalWeight { get; set; }
        public Boolean IsLocked { get; set; }
    }
}
