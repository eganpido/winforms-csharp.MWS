using MWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class MstSupplierController
    {
        // ============
        // Data Context
        // ============
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // Supplier List 
        public List<Models.MstSupplierModel> SupplierList(String filter)
        {
            var suppliers = from d in db.MstSuppliers
                        where d.IsLocked == true
                        && d.Supplier.Contains(filter)
                        select new Models.MstSupplierModel
                        {
                            Id = d.Id,
                            Supplier = d.Supplier,
                        };

            return suppliers.OrderByDescending(d => d.Id).ToList();
        }

        // Supplier Detail 
        public Models.MstSupplierModel SupplierDetail(Int32 id)
        {
            var supplier = from d in db.MstSuppliers
                       where d.Id == id
                       select new Models.MstSupplierModel
                       {
                           Id = d.Id,
                           Supplier = d.Supplier
                       };

            return supplier.FirstOrDefault();
        }
        // Add Supplier
        public String[] AddSupplier(MstSupplierModel objSupplier)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                DB.MstSupplier newSupplier = new DB.MstSupplier()
                {
                    Supplier = objSupplier.Supplier,
                    IsLocked = true
                };

                db.MstSuppliers.InsertOnSubmit(newSupplier);
                db.SubmitChanges();

                return new String[] { "", newSupplier.Id.ToString() };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Save Supplier
        public String[] SaveSupplier(Int32 id, MstSupplierModel objSupplier)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var supplier = from d in db.MstSuppliers
                              where d.Id == id
                              select d;

                if (supplier.Any())
                {
                    var saveSupplier = supplier.FirstOrDefault();
                    saveSupplier.Supplier = objSupplier.Supplier;
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

        // Delete Supplier
        public String[] DeleteSupplier(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var supplier = from d in db.MstSuppliers
                              where d.Id == id
                              select d;

                if (supplier.Any())
                {
                    if (supplier.FirstOrDefault().IsLocked)
                    {
                        return new String[] { "Item is locked", "0" };
                    }

                    var deleteSupplier = supplier.FirstOrDefault();
                    db.MstSuppliers.DeleteOnSubmit(deleteSupplier);
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
