using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class RepInventoryReportCutModel
    {
        public int ItemId { get; set; }
        public string ItemDescription { get; set; }
        public decimal Weight { get; set; }
    }
}
