using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class TrnReceivingReceiverItemController
    {
        // Data Context
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // List Receiving Item
        public List<Models.TrnReceivingItemModel> ReceivingItemList(Int32 receivingId)
        {
            var receivingItems = from d in db.TrnReceivingItems

                                 select new Models.TrnReceivingItemModel
                                 {
                                     Id = d.Id,
                                     ReceivingId = d.ReceivingId,
                                     ItemId = d.ItemId,
                                     Barcode = d.Barcode,
                                     ItemDescription = d.ItemDescription,
                                     SizeId = d.SizeId,
                                     Size = d.MstSize.Size,
                                     Weight = d.Weight
                                 };

            return receivingItems.Where(d => d.ReceivingId == receivingId).OrderByDescending(e => e.Id).ToList();
        }

        // Add Receiving Item
        public String[] AddReceivingItem(int receivingId, int pullOutId)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var pullOutItems = from d in db.TrnPullOutItems
                               where d.PullOutId == pullOutId select d;
                if (pullOutItems.Any())
                {
                    foreach (var item in pullOutItems)
                    {
                        DB.TrnReceivingItem newReceivingItem = new DB.TrnReceivingItem
                        {
                            ReceivingId = receivingId,
                            ItemId = 1,
                            Barcode = item.TrnProductionItem.ProductionBarcode,
                            ItemDescription = "SLOB",
                            SizeId = item.TrnProductionItem.TrnReceivingItem.SizeId,
                            Weight = item.TrnProductionItem.ActualWeight
                        };

                        db.TrnReceivingItems.InsertOnSubmit(newReceivingItem);
                        db.SubmitChanges();
                    }
                }

                return new String[] { "", "1" };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Delete Receiving Item
        public String[] DeleteReceivingItem(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var receivingItem = from d in db.TrnReceivingItems
                                    where d.Id == id
                                    select d;

                if (receivingItem.Any())
                {
                    var deleteReceivingItem = receivingItem.FirstOrDefault();
                    db.TrnReceivingItems.DeleteOnSubmit(deleteReceivingItem);
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Receiving item not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }
        // Delete All Receiving Item
        public String[] DeleteAllReceivingItem(Int32 receivingId)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var receivingItems = from d in db.TrnReceivingItems
                                    where d.Id == receivingId
                                    select d;

                if (receivingItems.Any())
                {
                    db.TrnReceivingItems.DeleteAllOnSubmit(receivingItems);
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Receiving items not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        public int GetSize(decimal weight)
        {
            var size = db.MstSizes
                     .Where(s => weight >= s.MinWeight && weight <= s.MaxWeight)
                     .OrderBy(s => s.Id)
                     .Select(s => s.Id)
                     .FirstOrDefault();

            return size;
        }
    }
}
