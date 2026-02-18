using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class TrnProductionItemModel
    {
        public Int32 Id { get; set; }
        public Int32 ProductionId { get; set; }
        public Int32 ReceivingItemId { get; set; }
        public Int32 ItemId { get; set; }
        public String ReceivingBarcode { get; set; }
        public String Barcode { get; set; }
        public String ItemDescription { get; set; }
        public Int32 SizeId { get; set; }
        public String Size { get; set; }
        public Decimal ReceivedWeight { get; set; }
        public Decimal ActualWeight { get; set; }
    }
}
