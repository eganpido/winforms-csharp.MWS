using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class SysConnectionStringModel
    {
        public String DataSource { get; set; }
        public String InitialCatalog { get; set; }
        public String UserId { get; set; }
        public String Password { get; set; }
    }
}
