using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class TrnReceivingItemController
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
        public String[] AddReceivingItem(int receivingId, decimal weight)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                String barcode = "1000000001";
                var lastReceivingItem = from d in db.TrnReceivingItems.OrderByDescending(d => d.Id) where d.Barcode.Contains("-") == false select d;
                if (lastReceivingItem.Any())
                {
                    Int32 newBarcode = Convert.ToInt32(lastReceivingItem.FirstOrDefault().Barcode) + 1;
                    barcode = newBarcode.ToString();
                }

                DB.TrnReceivingItem newReceivingItem = new DB.TrnReceivingItem
                {
                    ReceivingId = receivingId,
                    ItemId = 1,
                    Barcode = barcode,
                    ItemDescription = "SLOB",
                    SizeId = GetSize(weight),
                    Weight = weight
                };

                db.TrnReceivingItems.InsertOnSubmit(newReceivingItem);
                db.SubmitChanges();

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
                    return new String[] { "Stock-In line not found.", "0" };
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
