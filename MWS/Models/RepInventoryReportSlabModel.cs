using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class RepInventoryReportSlabModel
    {
        public int ItemId { get; set; }
        public string ItemDescription { get; set; }
        public int Minis { get; set; }
        public int ExtraSmall { get; set; }
        public int Small { get; set; }
        public int Medium { get; set; }
        public int Large { get; set; }
        public int ExtraLarge { get; set; }
    }
}
