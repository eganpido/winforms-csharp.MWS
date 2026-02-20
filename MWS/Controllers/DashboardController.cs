using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class DashboardController
    {
        // Data Context
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // Get Cut Quantity Per Sizes
        public int GetQuantity(int sizeId)
        {
            var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            int totalCut = 0;
            var receivingItem = from d in db.TrnReceivingItems
                                where d.TrnReceiving.IsLocked == true
                                && d.TrnReceiving.BranchId == currentBranchId
                                && d.SizeId == sizeId
                                select d;
            if (receivingItem.Any())
            {
                totalCut = receivingItem.Count();
            }

            return totalCut;
        }
    }
}
