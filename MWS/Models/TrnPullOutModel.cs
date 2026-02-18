using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class TrnPullOutModel
    {
        public Int32 Id { get; set; }
        public Int32 BranchId { get; set; }
        public String PullOutDate { get; set; }
        public String PullOutNo { get; set; }
        public String Remarks { get; set; }
        public Int32 PreparedById { get; set; }
        public String PreparedBy { get; set; }
        public Boolean IsLocked { get; set; }
    }
}
