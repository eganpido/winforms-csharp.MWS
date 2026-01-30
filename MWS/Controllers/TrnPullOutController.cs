using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class TrnPullOutController
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

        // Pull Out List 
        public List<Models.TrnPullOutModel> PullOutList(DateTime dateFilter, String filter)
        {
            var pullOut = from d in db.TrnPullOuts
                           where d.PullOutDate == dateFilter
                           && (d.PullOutNo.Contains(filter)
                           || d.Remarks.Contains(filter))
                           select new Models.TrnPullOutModel
                           {
                               Id = d.Id,
                               PullOutDate = d.PullOutDate.ToShortDateString(),
                               PullOutNo = d.PullOutNo,
                               Remarks = d.Remarks,
                               PreparedById = d.PrepareById,
                               PreparedBy = d.MstUser.FullName,
                               IsLocked = d.IsLocked
                           };

            return pullOut.OrderByDescending(d => d.Id).ToList();
        }

        // Pull Out Detail 
        public Models.TrnPullOutModel PullOutDetail(Int32 id)
        {
            var pullOut = from d in db.TrnPullOuts
                             where d.Id == id
                          select new Models.TrnPullOutModel
                          {
                              Id = d.Id,
                              PullOutDate = d.PullOutDate.ToShortDateString(),
                              PullOutNo = d.PullOutNo,
                              Remarks = d.Remarks,
                              PreparedById = d.PrepareById,
                              PreparedBy = d.MstUser.FullName,
                              IsLocked = d.IsLocked
                          };

            return pullOut.FirstOrDefault();
        }
        // Add Pull Out
        public String[] AddPullOut()
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                String pullOutNumber = "0000000001";
                var lastPullOut = from d in db.TrnPullOuts.OrderByDescending(d => d.Id) where d.PullOutNo.Contains("-") == false select d;
                if (lastPullOut.Any())
                {
                    Int32 newPullOutNumber = Convert.ToInt32(lastPullOut.FirstOrDefault().PullOutNo) + 1;
                    pullOutNumber = FillLeadingZeroes(newPullOutNumber, 10);
                }

                DB.TrnPullOut newPullOut = new DB.TrnPullOut()
                {
                    PullOutDate = DateTime.Today,
                    PullOutNo = pullOutNumber,
                    Remarks = "NA",
                    PrepareById = currentUserLogin.FirstOrDefault().Id,
                    IsLocked = false
                };

                db.TrnPullOuts.InsertOnSubmit(newPullOut);
                db.SubmitChanges();

                return new String[] { "", newPullOut.Id.ToString() };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }

        // Lock Pull Out
        public String[] LockPullOut(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var pullOut = from d in db.TrnPullOuts
                              where d.Id == id
                              select d;

                if (pullOut.Any())
                {
                    if (pullOut.FirstOrDefault().IsLocked)
                    {
                        return new String[] { "Already locked.", "0" };
                    }

                    var lockPullOut = pullOut.FirstOrDefault();
                    lockPullOut.PullOutDate = DateTime.Today;
                    lockPullOut.IsLocked = true;
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

        // Unlock Pull Out
        public String[] UnlockPullOut(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var pullOut = from d in db.TrnPullOuts
                              where d.Id == id
                              select d;

                if (pullOut.Any())
                {
                    if (pullOut.FirstOrDefault().IsLocked == false)
                    {
                        return new String[] { "Already unlocked.", "0" };
                    }

                    var unlockPullOut = pullOut.FirstOrDefault();
                    unlockPullOut.IsLocked = false;
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

        // Delete Pull Out
        public String[] DeletePullOut(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var pullOut = from d in db.TrnPullOuts
                                where d.Id == id
                              select d;

                if (pullOut.Any())
                {
                    if (pullOut.FirstOrDefault().IsLocked)
                    {
                        return new String[] { "Pull Out is locked", "0" };
                    }

                    var deletePullOut = pullOut.FirstOrDefault();
                    db.TrnPullOuts.DeleteOnSubmit(deletePullOut);
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
                else
                {
                    return new String[] { "Pull Out not found.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }
    }
}
