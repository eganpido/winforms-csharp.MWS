using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class SysCurrentModel
    {
        public String CompanyName { get; set; }
        public String Address { get; set; }
        public String ContactNo { get; set; }
        public Int32 CurrentUserId { get; set; }
        public String CurrentUserName { get; set; }
        public String CurrentVersion { get; set; }
        public String CurrentDeveloper { get; set; }
        public String CurrentSupport { get; set; }
        public int BranchId { get; set; }
        public bool IsReceiver { get; set; }
    }
}