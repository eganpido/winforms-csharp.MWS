using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models 
{
    public class DgvTrnProductionModel
    {
        public Int32 ColumnProcessingId { get; set; }
        public String ColumnProcessingDate { get; set; }
        public String ColumnProcessingNo { get; set; }
        public String ColumnProcessingRemarks { get; set; }
        public Int32 ColumnProcessingPreparedById { get; set; }
        public String ColumnProcessingPreparedBy { get; set; }
        public String ColumnProcessingTotalWeight { get; set; }
        public Boolean ColumnProcessingIsLocked { get; set; }
        public String ColumnProcessingView { get; set; }
    }
}