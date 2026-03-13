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
        public int GetQuantity(int sizeId, int branchId)
        {
            int totalSlabs = 0;

            if(branchId == 1)
            {
                var receivingItem = from d in db.TrnReceivingItems
                                    where d.TrnReceiving.IsLocked == true
                                    && d.SizeId == sizeId
                                    && d.TrnReceiving.BranchId == branchId
                                    && d.ItemId == 1
                                    select d;
                if (receivingItem.Any())
                {
                    totalSlabs = receivingItem.Count();

                    var pullOutItem = from d in db.TrnPullOutItems
                                        where d.TrnPullOut.IsLocked == true
                                        && d.TrnProductionItem.SizeId == sizeId
                                        && d.TrnPullOut.BranchId == branchId
                                        && d.TrnProductionItem.ItemId == 1
                                        select d;
                    if (pullOutItem.Any())
                    {
                        totalSlabs = receivingItem.Count() - pullOutItem.Count();
                    }
                }
            }
            else
            {
                var receivingItem = from d in db.TrnReceivingItems
                                    where d.TrnReceiving.IsLocked == true
                                    && d.SizeId == sizeId
                                    && d.TrnReceiving.BranchId == branchId
                                    && d.ItemId == 1
                                    select d;
                if (receivingItem.Any())
                {
                    totalSlabs = receivingItem.Count();

                    var productionItems = from d in db.TrnProductionItems
                                      where d.TrnProduction.IsLocked == true
                                      && d.SizeId == sizeId
                                      && d.TrnProduction.BranchId == branchId
                                      && d.ItemId == 1
                                      select d;
                    if (productionItems.Any())
                    {
                        totalSlabs = receivingItem.Count() - productionItems.Count();
                    }
                }
            }

            return totalSlabs < 0 ? 0 : totalSlabs;
        }
    }
}
