using MWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class MstItemController
    {
        // ============
        // Data Context
        // ============
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // Item List 
        public List<Models.MstItemModel> ItemList(String filter)
        {
            var items = from d in db.MstItems
                        where d.IsLocked == true
                        && d.ItemDescription.Contains(filter)
                        select new Models.MstItemModel
                        {
                            Id = d.Id,
                            Item = d.ItemDescription,
                        };

            return items.OrderByDescending(d => d.Id).ToList();
        }

        // Item Detail 
        public Models.MstItemModel ItemDetail(Int32 id)
        {
            var item = from d in db.MstItems
                       where d.Id == id
                       select new Models.MstItemModel
                       {
                           Id = d.Id,
                           Item = d.ItemDescription
                       };

            return item.FirstOrDefault();
        }
        // Add Item
        public String[] AddItem(MstItemModel objItem)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                DB.MstItem newItem = new DB.MstItem()
                {
                    ItemDescription = objItem.Item,
                    IsLocked = true
                };

                db.MstItems.InsertOnSubmit(newItem);
                db.SubmitChanges();

                return new String[] { "", newItem.Id.ToString() };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Save Item
        public String[] SaveItem(Int32 id, MstItemModel objItem)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var item = from d in db.MstItems
                              where d.Id == id
                              select d;

                if (item.Any())
                {
                    var saveItem = item.FirstOrDefault();
                    saveItem.ItemDescription = objItem.Item;
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Item not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Delete Item
        public String[] DeleteItem(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var item = from d in db.MstItems
                              where d.Id == id
                              select d;

                if (item.Any())
                {
                    if (item.FirstOrDefault().IsLocked)
                    {
                        return new String[] { "Item is locked", "0" };
                    }

                    var deleteItem = item.FirstOrDefault();
                    db.MstItems.DeleteOnSubmit(deleteItem);
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Item not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }
    }
}
