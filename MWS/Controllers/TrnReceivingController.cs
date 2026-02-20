using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class TrnReceivingController
    {
        // ============
        // Data Context
        // ============
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // ===================
        // Fill Leading Zeroes
        // ===================
        public String FillLeadingZeroes(Int32 number, Int32 length)
        {
            var result = number.ToString();
            var pad = length - result.Length;
            while (pad > 0)
            {
                result = '0' + result;
                pad--;
            }

            return result;
        }

        // Receiving List 
        public List<Models.TrnReceivingModel> ReceivingList(DateTime startDate, DateTime endDate, String filter)
        {
            var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            var receiving = from d in db.TrnReceivings
                           where d.ReceivingDate >= startDate
                           && d.ReceivingDate <= endDate
                           && d.BranchId == currentBranchId
                           && d.IsLocked == true
                           && (d.ReceivingNo.Contains(filter)
                           || d.MstSupplier.Supplier.Contains(filter)
                           || d.Remarks.Contains(filter))
                            select new Models.TrnReceivingModel
                           {
                               Id = d.Id,
                               BranchId = d.BranchId,
                               ReceivingDate = d.ReceivingDate.ToShortDateString(),
                               ReceivingNo = d.ReceivingNo,
                               SupplierId = d.SupplierId,
                               Supplier = d.MstSupplier.Supplier,
                               Remarks = d.Remarks,
                               PreparedById = d.PrepareById,
                               PreparedBy = d.MstUser.FullName,
                               TotalWeight = d.TrnReceivingItems.Sum(a => a.Weight),
                               IsLocked = d.IsLocked
                           };

            return receiving.OrderByDescending(d => d.Id).ToList();
        }

        // Receiving Detail
        public Models.TrnReceivingModel ReceivingDetail(Int32 id)
        {
            var receiving = from d in db.TrnReceivings
                          where d.Id == id
                          select new Models.TrnReceivingModel
                          {
                              Id = d.Id,
                              ReceivingDate = d.ReceivingDate.ToShortDateString(),
                              ReceivingNo = d.ReceivingNo,
                              SupplierId = d.SupplierId,
                              Supplier = d.MstSupplier.Supplier,
                              Remarks = d.Remarks,
                              PreparedById = d.PrepareById,
                              PreparedBy = d.MstUser.FullName,
                              IsLocked = d.IsLocked
                          };

            return receiving.FirstOrDefault();
        }

        // Supplier List
        public List<Models.MstSupplierModel> SupplierList()
        {
            var suppliers = from d in db.MstSuppliers
                            select new Models.MstSupplierModel
                            {
                                Id = d.Id,
                                Supplier = d.Supplier
                            };

            return suppliers.OrderBy(d => d.Supplier).ToList();
        }

        // Add Receiving
        public String[] AddReceiving()
        {
            try
            {
                var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var supplier = from d in db.MstSuppliers select d;
                if (supplier.Any() == false)
                {
                    return new String[] { "Supplier not found.", "0" };
                }

                String receivingNumber = "0000000001";
                var lastReceiving = from d in db.TrnReceivings.OrderByDescending(d => d.Id) where d.ReceivingNo.Contains("-") == false && d.BranchId == currentBranchId select d;
                if (lastReceiving.Any())
                {
                    Int32 newReceivingNumber = Convert.ToInt32(lastReceiving.FirstOrDefault().ReceivingNo) + 1;
                    receivingNumber = FillLeadingZeroes(newReceivingNumber, 10);
                }

                DB.TrnReceiving newReceiving = new DB.TrnReceiving()
                {
                    BranchId = currentBranchId,
                    ReceivingDate = DateTime.Today,
                    ReceivingNo = receivingNumber,
                    SupplierId = supplier.FirstOrDefault().Id,
                    Remarks = "NA",
                    PrepareById = currentUserLogin.FirstOrDefault().Id,
                    IsLocked = false
                };

                db.TrnReceivings.InsertOnSubmit(newReceiving);
                db.SubmitChanges();

                return new String[] { "", newReceiving.Id.ToString() };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Lock Receiving
        public String[] LockReceiving(Int32 id, Models.TrnReceivingModel objReceiving)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var supplier = from d in db.MstSuppliers
                               where d.Id == objReceiving.SupplierId
                               select d;

                if (supplier.Any() == false)
                {
                    return new String[] { "Supplier not found.", "0" };
                }

                var receiving = from d in db.TrnReceivings
                              where d.Id == id
                              select d;

                if (receiving.Any())
                {
                    if (receiving.FirstOrDefault().IsLocked)
                    {
                        return new String[] { "Already locked.", "0" };
                    }

                    var lockReceiving = receiving.FirstOrDefault();
                    lockReceiving.ReceivingDate = DateTime.Today;
                    lockReceiving.SupplierId = objReceiving.SupplierId;
                    lockReceiving.Remarks = objReceiving.Remarks;
                    lockReceiving.IsLocked = true;
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Receiving not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Unlock Receiving
        public String[] UnlockReceving(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var receiving = from d in db.TrnReceivings
                              where d.Id == id
                              select d;

                if (receiving.Any())
                {
                    if (receiving.FirstOrDefault().IsLocked == false)
                    {
                        return new String[] { "Already unlocked.", "0" };
                    }

                    var unlockReceiving = receiving.FirstOrDefault();
                    unlockReceiving.IsLocked = false;
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Receving not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Delete Receiving
        public String[] DeleteReceiving(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var receiving = from d in db.TrnReceivings
                              where d.Id == id
                              select d;

                if (receiving.Any())
                {
                    if (receiving.FirstOrDefault().IsLocked)
                    {
                        return new String[] { "Receiving is locked", "0" };
                    }

                    var deleteReceiving = receiving.FirstOrDefault();
                    db.TrnReceivings.DeleteOnSubmit(deleteReceiving);
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Receiving not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }
    }
}
