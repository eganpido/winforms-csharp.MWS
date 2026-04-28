using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class RepReceivingModel
    {
        public int ReceivingId { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public string ReceivingNo { get; set; }
        public DateTime ReceivingDate { get; set; }
        public string Remarks { get; set; }
        public int ItemId { get; set; }
        public string Item { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public decimal Weight { get; set; }
    }
}
