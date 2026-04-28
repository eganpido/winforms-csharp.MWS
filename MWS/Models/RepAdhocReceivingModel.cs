using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class RepAdhocReceivingModel
    {
        public string ReceivingBarcode { get; set; }
        public int ItemId { get; set; }
        public string Item { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public string Classification { get; set; }
    }
}
