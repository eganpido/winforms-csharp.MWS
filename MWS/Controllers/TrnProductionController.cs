using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class TrnProductionController
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

        // Production List 
        public List<Models.TrnProductionModel> ProductionList(DateTime dateFilter, String filter)
        {
            var production = from d in db.TrnProductions
                           where d.ProductionDate == dateFilter
                           && (d.ProductionNo.Contains(filter)
                           || d.Remarks.Contains(filter))
                           select new Models.TrnProductionModel
                           {
                               Id = d.Id,
                               ProductionDate = d.ProductionDate.ToShortDateString(),
                               ProductionNo = d.ProductionNo,
                               Remarks = d.Remarks,
                               PreparedById = d.PrepareById,
                               PreparedBy = d.MstUser.FullName,
                               IsLocked = d.IsLocked
                           };

            return production.OrderByDescending(d => d.Id).ToList();
        }

        // Production Detail 
        public Models.TrnProductionModel ProductionDetail(Int32 id)
        {
            var production = from d in db.TrnProductions
                             where d.Id == id
                          select new Models.TrnProductionModel
                          {
                              Id = d.Id,
                              ProductionDate = d.ProductionDate.ToShortDateString(),
                              ProductionNo = d.ProductionNo,
                              Remarks = d.Remarks,
                              PreparedById = d.PrepareById,
                              PreparedBy = d.MstUser.FullName,
                              IsLocked = d.IsLocked
                          };

            return production.FirstOrDefault();
        }
        // Add Production
        public String[] AddProduction()
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                String productionNumber = "0000000001";
                var lastProduction = from d in db.TrnProductions.OrderByDescending(d => d.Id) where d.ProductionNo.Contains("-") == false select d;
                if (lastProduction.Any())
                {
                    Int32 newProductionNumber = Convert.ToInt32(lastProduction.FirstOrDefault().ProductionNo) + 1;
                    productionNumber = FillLeadingZeroes(newProductionNumber, 10);
                }

                DB.TrnProduction newProduction = new DB.TrnProduction()
                {
                    ProductionDate = DateTime.Today,
                    ProductionNo = productionNumber,
                    Remarks = "NA",
                    PrepareById = currentUserLogin.FirstOrDefault().Id,
                    IsLocked = false
                };

                db.TrnProductions.InsertOnSubmit(newProduction);
                db.SubmitChanges();

                return new String[] { "", newProduction.Id.ToString() };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Lock Production
        public String[] LockProduction(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var production = from d in db.TrnProductions
                              where d.Id == id
                              select d;

                if (production.Any())
                {
                    if (production.FirstOrDefault().IsLocked)
                    {
                        return new String[] { "Already locked.", "0" };
                    }

                    var lockProduction = production.FirstOrDefault();
                    lockProduction.ProductionDate = DateTime.Today;
                    lockProduction.IsLocked = true;
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Production not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Unlock Production
        public String[] UnlockProduction(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var production = from d in db.TrnProductions
                              where d.Id == id
                              select d;

                if (production.Any())
                {
                    if (production.FirstOrDefault().IsLocked == false)
                    {
                        return new String[] { "Already unlocked.", "0" };
                    }

                    var unlockProduction = production.FirstOrDefault();
                    unlockProduction.IsLocked = false;
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Production not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Delete Production
        public String[] DeleteProduction(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var production = from d in db.TrnProductions
                                where d.Id == id
                              select d;

                if (production.Any())
                {
                    if (production.FirstOrDefault().IsLocked)
                    {
                        return new String[] { "Production is locked", "0" };
                    }

                    var deleteProduction = production.FirstOrDefault();
                    db.TrnProductions.DeleteOnSubmit(deleteProduction);
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Production not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }
    }
}
