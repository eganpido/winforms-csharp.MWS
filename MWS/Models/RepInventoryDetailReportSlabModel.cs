using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class RepInventoryDetailReportSlabModel
    {
        public int BranchId { get; set; }
        public int ItemId { get; set; }
        public string ItemDescription { get; set; }
        public string Classification { get; set; }
        public string Barcode { get; set; }
        public int Pundo { get; set; }
        public int Processing { get; set; }
        public int PullOut { get; set; }
        public int Receiving { get; set; }
        public int Production { get; set; }
        public int Balance { get; set; }
    }
}
