using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models 
{
    public class DgvTrnPullOutModel
    {
        public Int32 ColumnPullOutId { get; set; }
        public String ColumnPullOutDate { get; set; }
        public String ColumnPullOutNo { get; set; }
        public String ColumnPullOutRemarks { get; set; }
        public Int32 ColumnPullOutPreparedById { get; set; }
        public String ColumnPullOutPreparedBy { get; set; }
        public Boolean ColumnPullOutIsLocked { get; set; }
        public String ColumnPullOutView { get; set; }
    }
}