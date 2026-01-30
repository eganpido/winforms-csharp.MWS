using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class TrnPullOutItemModel
    {
        public Int32 Id { get; set; }
        public Int32 PullOutId { get; set; }
        public Int32 ReceivingItemId { get; set; }
        public Int32 ItemId { get; set; }
        public String Barcode { get; set; }
        public String ItemDescription { get; set; }
        public Int32 SizeId { get; set; }
        public String Size { get; set; }
    }
}
