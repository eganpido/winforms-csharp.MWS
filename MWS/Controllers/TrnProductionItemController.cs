using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class TrnProductionItemController
    {
        // Data Context
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // List Production Item
        public List<Models.TrnProductionItemModel> ProductionItemList(Int32 productionId)
        {
            var productionItems = from d in db.TrnProductionItems

                                  select new Models.TrnProductionItemModel
                                  {
                                     Id = d.Id,
                                     ProductionId = d.ProductionId,
                                     ReceivingItemId = d.ReceivingItemId,
                                     ItemId = d.TrnReceivingItem.ItemId,
                                     Barcode = d.TrnReceivingItem.Barcode,
                                     ItemDescription = d.TrnReceivingItem.ItemDescription,
                                     SizeId = d.TrnReceivingItem.SizeId,
                                     Size = d.TrnReceivingItem.MstSize.Size,
                                     ReceivedWeight = d.TrnReceivingItem.Weight,
                                     ActualWeight = d.ActualWeight
                                  };

            return productionItems.Where(d => d.ProductionId == productionId).OrderByDescending(e => e.Id).ToList();
        }

        // Add Production Item
        public String[] AddProductionItem(int productionId, string barcode, decimal weight)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                DB.TrnProductionItem newProductionItem = new DB.TrnProductionItem
                {
                    ProductionId = productionId,
                    ReceivingItemId = GetReceivingItem(barcode),
                    ActualWeight = weight
                };

                db.TrnProductionItems.InsertOnSubmit(newProductionItem);
                db.SubmitChanges();

                return new String[] { "", "1" };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Delete Production Item
        public String[] DeleteProductionItem(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var productionItem = from d in db.TrnProductionItems
                                    where d.Id == id
                                    select d;

                if (productionItem.Any())
                {
                    var deleteProductionItem = productionItem.FirstOrDefault();
                    db.TrnProductionItems.DeleteOnSubmit(deleteProductionItem);
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Production item not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }
        public int GetReceivingItem(string barcode)
        {
            int receivingItemId = 0;
            var receivingItem = from d in db.TrnReceivingItems
                                where d.Barcode == barcode
                                select d;
            if (receivingItem.Any())
            {
                receivingItemId = receivingItem.FirstOrDefault().Id;
            }
            return receivingItemId;
        }
    }
}
