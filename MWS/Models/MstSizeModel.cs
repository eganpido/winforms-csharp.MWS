using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Models
{
    public class MstSizeModel
    {
        public Int32 Id { get; set; }
        public String Size { get; set; }
        public Decimal MinWeight { get; set; }
        public Decimal MaxWeight { get; set; }
    }
}
