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
                                     ReceivingItemId = d.ReceivingItemId,
                                     ItemId = d.TrnReceivingItem.ItemId,
                                     Barcode = d.TrnReceivingItem.Barcode,
                                     ItemDescription = d.TrnReceivingItem.ItemDescription,
                                     SizeId = d.TrnReceivingItem.SizeId,
                                     Size = d.TrnReceivingItem.MstSize.Size,
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
                    ReceivingItemId = GetReceivingItem(barcode),
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
