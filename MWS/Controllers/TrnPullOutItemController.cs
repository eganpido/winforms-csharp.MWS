using MWS.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class TrnPullOutItemController
    {
        // Data Context
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // List Pull Out Item
        public List<Models.TrnPullOutItemModel> PullOutItemList(Int32 pullOutId)
        {
            var pullOutItems = from d in db.TrnPullOutItems
                                  select new Models.TrnPullOutItemModel
                                  {
                                     Id = d.Id,
                                     PullOutId = d.PullOutId,
                                     ReceivingItemId = d.TrnProductionItem.TrnReceivingItem.ReceivingId,
                                     ItemId = d.TrnProductionItem.TrnReceivingItem.ItemId,
                                     Barcode = d.TrnProductionItem.ProductionBarcode,
                                     ItemDescription = d.TrnProductionItem.TrnReceivingItem.ItemDescription,
                                     SizeId = d.TrnProductionItem.TrnReceivingItem.SizeId,
                                     Size = d.TrnProductionItem.TrnReceivingItem.MstSize.Size,
                                  };

            return pullOutItems.Where(d => d.PullOutId == pullOutId).OrderByDescending(e => e.Id).ToList();
        }

        // Add Pull Out Item
        public String[] AddPullOutItem(int pullOutId, string barcode)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                DB.TrnPullOutItem newPullOutItem = new DB.TrnPullOutItem
                {
                    PullOutId = pullOutId,
                    ProductionItemId = GetProductionItem(barcode),
                };

                db.TrnPullOutItems.InsertOnSubmit(newPullOutItem);
                db.SubmitChanges();

                return new String[] { "", "1" };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Delete Pull Out Item
        public String[] DeletePullOutItem(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var pullOutItem = from d in db.TrnPullOutItems
                                    where d.Id == id
                                    select d;

                if (pullOutItem.Any())
                {
                    var deletePullOutItem = pullOutItem.FirstOrDefault();
                    db.TrnPullOutItems.DeleteOnSubmit(deletePullOutItem);
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
        public int GetProductionItem(string barcode)
        {
            int productionItemId = 0;
            var productionItem = from d in db.TrnProductionItems
                                where d.ProductionBarcode == barcode
                                select d;
            if (productionItem.Any())
            {
                productionItemId = productionItem.FirstOrDefault().Id;
            }
            return productionItemId;
        }
        public bool isAlreadyAdded(string barcode)
        {
            bool added = false;

            var barcodeExist = from d in db.TrnPullOutItems
                               where d.TrnProductionItem.ProductionBarcode == barcode
                               select d;
            if (barcodeExist.Any())
            {
                added = true;
            }

            return added;
        }
    }
}
