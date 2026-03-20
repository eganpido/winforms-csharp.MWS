using MWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class RepInventoryController
    {
        // ============
        // Data Context
        // ============
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // Item List 
        public List<Models.RepInventoryModel> ReportList()
        {
            var reports = from d in db.RepInventories
                          where d.IsVisible == true
                          select new Models.RepInventoryModel
                          {
                              Id = d.Id,
                              Report = d.Report
                          };

            return reports.OrderBy(d => d.Id).ToList();
        }
        // Branch List 
        public List<Models.MstBranchModel> BranchList()
        {
            var branches = (from d in db.MstBranches
                            select new Models.MstBranchModel
                            {
                                Id = d.Id,
                                Branch = d.Branch
                            })
                            .OrderBy(d => d.Id)
                            .ToList();

            branches.Insert(0, new Models.MstBranchModel
            {
                Id = 0,
                Branch = "All"
            });

            return branches;
        }
        public List<Models.RepInventoryReportSlabModel> Commissary1List(DateTime startDate, DateTime endDate)
        {
            List<Models.RepInventoryReportSlabModel> repInventoryCommissary1 = new List<RepInventoryReportSlabModel>();

            var branchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            if (branchId == 1)
            {
                db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());
                var receivings = from d in db.TrnReceivingItems
                                 where d.ItemId == 1
                                 && d.TrnReceiving.ReceivingDate >= startDate
                                 && d.TrnReceiving.ReceivingDate <= endDate
                                 && d.TrnReceiving.IsLocked == true
                                 select d;
                if (receivings.Any())
                {
                    var pullOuts = from d in db.TrnPullOutItems
                                   where d.TrnProductionItem.ItemId == 1
                                   && d.TrnPullOut.PullOutDate >= startDate
                                   && d.TrnPullOut.PullOutDate <= endDate
                                   && d.TrnPullOut.IsLocked == true
                                   select d;
                    if (pullOuts.Any())
                    {
                        repInventoryCommissary1.Add(new Models.RepInventoryReportSlabModel
                        {
                            ItemId = 1,
                            ItemDescription = "SLAB",
                            Minis = receivings.Where(a => a.SizeId == 1).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 1).Count(),
                            ExtraSmall = receivings.Where(a => a.SizeId == 2).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 2).Count(),
                            Small = receivings.Where(a => a.SizeId == 3).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 3).Count(),
                            Medium = receivings.Where(a => a.SizeId == 4).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 4).Count(),
                            Large = receivings.Where(a => a.SizeId == 5).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 5).Count(),
                            ExtraLarge = receivings.Where(a => a.SizeId == 6).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 6).Count()
                        });
                    }
                    else
                    {
                        repInventoryCommissary1.Add(new Models.RepInventoryReportSlabModel
                        {
                            ItemId = 1,
                            ItemDescription = "SLAB",
                            Minis = receivings.Where(a => a.SizeId == 1).Count(),
                            ExtraSmall = receivings.Where(a => a.SizeId == 2).Count(),
                            Small = receivings.Where(a => a.SizeId == 3).Count(),
                            Medium = receivings.Where(a => a.SizeId == 4).Count(),
                            Large = receivings.Where(a => a.SizeId == 5).Count(),
                            ExtraLarge = receivings.Where(a => a.SizeId == 6).Count()
                        });
                    }
                }
            }
            else
            {
                db = new DB.mwsdbDataContext(Modules.SysConnectionString2Module.GetConnectionString());
                var receivings = from d in db.TrnReceivingItems
                                 where d.ItemId == 1
                                 && d.TrnReceiving.ReceivingDate >= startDate
                                 && d.TrnReceiving.ReceivingDate <= endDate
                                 && d.TrnReceiving.IsLocked == true
                                 select d;
                if (receivings.Any())
                {
                    var pullOuts = from d in db.TrnPullOutItems
                                   where d.TrnProductionItem.ItemId == 1
                                   && d.TrnPullOut.PullOutDate >= startDate
                                   && d.TrnPullOut.PullOutDate <= endDate
                                   && d.TrnPullOut.IsLocked == true
                                   select d;
                    if (pullOuts.Any())
                    {
                        repInventoryCommissary1.Add(new Models.RepInventoryReportSlabModel
                        {
                            ItemId = 1,
                            ItemDescription = "SLAB",
                            Minis = receivings.Where(a => a.SizeId == 1).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 1).Count(),
                            ExtraSmall = receivings.Where(a => a.SizeId == 2).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 2).Count(),
                            Small = receivings.Where(a => a.SizeId == 3).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 3).Count(),
                            Medium = receivings.Where(a => a.SizeId == 4).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 4).Count(),
                            Large = receivings.Where(a => a.SizeId == 5).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 5).Count(),
                            ExtraLarge = receivings.Where(a => a.SizeId == 6).Count() - pullOuts.Where(a => a.TrnProductionItem.SizeId == 6).Count()
                        });
                    }
                    else
                    {
                        repInventoryCommissary1.Add(new Models.RepInventoryReportSlabModel
                        {
                            ItemId = 1,
                            ItemDescription = "SLAB",
                            Minis = receivings.Where(a => a.SizeId == 1).Count(),
                            ExtraSmall = receivings.Where(a => a.SizeId == 2).Count(),
                            Small = receivings.Where(a => a.SizeId == 3).Count(),
                            Medium = receivings.Where(a => a.SizeId == 4).Count(),
                            Large = receivings.Where(a => a.SizeId == 5).Count(),
                            ExtraLarge = receivings.Where(a => a.SizeId == 6).Count()
                        });
                    }
                }
            }

            return repInventoryCommissary1.ToList();
        }
        public List<Models.RepInventoryReportCutModel> Commissary1CutList(DateTime startDate, DateTime endDate)
        {
            List<Models.RepInventoryReportCutModel> repInventoryCommissary1Cut = new List<RepInventoryReportCutModel>();

            var branchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            if (branchId == 1)
            {
                db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

                var processings = from d in db.TrnProductionItems
                                  where d.ItemId == 2
                                  && d.TrnProduction.ProductionDate >= startDate
                                  && d.TrnProduction.ProductionDate <= endDate
                                  && d.TrnProduction.IsLocked == true
                                  select d;
                if (processings.Any())
                {
                    var pullOuts = from d in db.TrnPullOutItems
                                   where d.TrnProductionItem.ItemId == 2
                                   && d.TrnPullOut.PullOutDate >= startDate
                                   && d.TrnPullOut.PullOutDate <= endDate
                                   && d.TrnPullOut.IsLocked == true
                                   select d;
                    if (pullOuts.Any())
                    {
                        repInventoryCommissary1Cut.Add(new Models.RepInventoryReportCutModel
                        {
                            ItemId = 2,
                            ItemDescription = "CUT",
                            Weight = processings.Where(a => a.ItemId == 2).Sum(a => a.ActualWeight) - pullOuts.Where(a => a.TrnProductionItem.ItemId == 2).Sum(a => a.TrnProductionItem.ActualWeight)
                        });
                    }
                    else
                    {
                        repInventoryCommissary1Cut.Add(new Models.RepInventoryReportCutModel
                        {
                            ItemId = 2,
                            ItemDescription = "CUT",
                            Weight = processings.Where(a => a.ItemId == 2).Sum(a => a.ActualWeight)
                        });
                    }
                }
            }
            else
            {
                db = new DB.mwsdbDataContext(Modules.SysConnectionString2Module.GetConnectionString());

                var processings = from d in db.TrnProductionItems
                                  where d.ItemId == 2
                                  && d.TrnProduction.ProductionDate >= startDate
                                  && d.TrnProduction.ProductionDate <= endDate
                                  && d.TrnProduction.IsLocked == true
                                  select d;
                if (processings.Any())
                {
                    var pullOuts = from d in db.TrnPullOutItems
                                   where d.TrnProductionItem.ItemId == 2
                                   && d.TrnPullOut.PullOutDate >= startDate
                                   && d.TrnPullOut.PullOutDate <= endDate
                                   && d.TrnPullOut.IsLocked == true
                                   select d;
                    if (pullOuts.Any())
                    {
                        repInventoryCommissary1Cut.Add(new Models.RepInventoryReportCutModel
                        {
                            ItemId = 2,
                            ItemDescription = "CUT",
                            Weight = processings.Where(a => a.ItemId == 2).Sum(a => a.ActualWeight) - pullOuts.Where(a => a.TrnProductionItem.ItemId == 2).Sum(a => a.TrnProductionItem.ActualWeight),
                        });
                    }
                    else
                    {
                        repInventoryCommissary1Cut.Add(new Models.RepInventoryReportCutModel
                        {
                            ItemId = 2,
                            ItemDescription = "CUT",
                            Weight = processings.Where(a => a.ItemId == 2).Sum(a => a.ActualWeight)
                        });
                    }
                }
            }
           

            return repInventoryCommissary1Cut.ToList();
        }
        public List<Models.RepInventoryReportSlabModel> Commissary2List(DateTime startDate, DateTime endDate)
        {
            List<Models.RepInventoryReportSlabModel> repInventoryCommissary2 = new List<RepInventoryReportSlabModel>();

            var branchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            if (branchId == 1)
            {
                db = new DB.mwsdbDataContext(Modules.SysConnectionString2Module.GetConnectionString());
                var receivings = from d in db.TrnReceivingItems
                                 where d.ItemId == 1
                                 && d.TrnReceiving.ReceivingDate >= startDate
                                 && d.TrnReceiving.ReceivingDate <= endDate
                                 && d.TrnReceiving.IsLocked == true
                                 select d;
                if (receivings.Any())
                {
                    var productions = from d in db.TrnProductionItems
                                      where d.ItemId == 1
                                      && d.TrnProduction.ProductionDate >= startDate
                                      && d.TrnProduction.ProductionDate <= endDate
                                      && d.TrnProduction.IsLocked == true
                                      select d;
                    if (productions.Any())
                    {
                        repInventoryCommissary2.Add(new Models.RepInventoryReportSlabModel
                        {
                            ItemId = 1,
                            ItemDescription = "SLAB",
                            Minis = receivings.Where(a => a.SizeId == 1).Count() - productions.Where(a => a.SizeId == 1).Count(),
                            ExtraSmall = receivings.Where(a => a.SizeId == 2).Count() - productions.Where(a => a.SizeId == 2).Count(),
                            Small = receivings.Where(a => a.SizeId == 3).Count() - productions.Where(a => a.SizeId == 3).Count(),
                            Medium = receivings.Where(a => a.SizeId == 4).Count() - productions.Where(a => a.SizeId == 4).Count(),
                            Large = receivings.Where(a => a.SizeId == 5).Count() - productions.Where(a => a.SizeId == 5).Count(),
                            ExtraLarge = receivings.Where(a => a.SizeId == 6).Count() - productions.Where(a => a.SizeId == 6).Count()
                        });
                    }
                    else
                    {
                        repInventoryCommissary2.Add(new Models.RepInventoryReportSlabModel
                        {
                            ItemId = 1,
                            ItemDescription = "SLAB",
                            Minis = receivings.Where(a => a.SizeId == 1).Count(),
                            ExtraSmall = receivings.Where(a => a.SizeId == 2).Count(),
                            Small = receivings.Where(a => a.SizeId == 3).Count(),
                            Medium = receivings.Where(a => a.SizeId == 4).Count(),
                            Large = receivings.Where(a => a.SizeId == 5).Count(),
                            ExtraLarge = receivings.Where(a => a.SizeId == 6).Count()
                        });
                    }
                }
            }
            else
            {
                db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());
                var receivings = from d in db.TrnReceivingItems
                                 where d.ItemId == 1
                                 && d.TrnReceiving.ReceivingDate >= startDate
                                 && d.TrnReceiving.ReceivingDate <= endDate
                                 && d.TrnReceiving.IsLocked == true
                                 select d;
                if (receivings.Any())
                {
                    var productions = from d in db.TrnProductionItems
                                      where d.ItemId == 1
                                      && d.TrnProduction.ProductionDate >= startDate
                                      && d.TrnProduction.ProductionDate <= endDate
                                      && d.TrnProduction.IsLocked == true
                                      select d;
                    if (productions.Any())
                    {
                        repInventoryCommissary2.Add(new Models.RepInventoryReportSlabModel
                        {
                            ItemId = 1,
                            ItemDescription = "SLAB",
                            Minis = receivings.Where(a => a.SizeId == 1).Count() - productions.Where(a => a.SizeId == 1).Count(),
                            ExtraSmall = receivings.Where(a => a.SizeId == 2).Count() - productions.Where(a => a.SizeId == 2).Count(),
                            Small = receivings.Where(a => a.SizeId == 3).Count() - productions.Where(a => a.SizeId == 3).Count(),
                            Medium = receivings.Where(a => a.SizeId == 4).Count() - productions.Where(a => a.SizeId == 4).Count(),
                            Large = receivings.Where(a => a.SizeId == 5).Count() - productions.Where(a => a.SizeId == 5).Count(),
                            ExtraLarge = receivings.Where(a => a.SizeId == 6).Count() - productions.Where(a => a.SizeId == 6).Count()
                        });
                    }
                    else
                    {
                        repInventoryCommissary2.Add(new Models.RepInventoryReportSlabModel
                        {
                            ItemId = 1,
                            ItemDescription = "SLAB",
                            Minis = receivings.Where(a => a.SizeId == 1).Count(),
                            ExtraSmall = receivings.Where(a => a.SizeId == 2).Count(),
                            Small = receivings.Where(a => a.SizeId == 3).Count(),
                            Medium = receivings.Where(a => a.SizeId == 4).Count(),
                            Large = receivings.Where(a => a.SizeId == 5).Count(),
                            ExtraLarge = receivings.Where(a => a.SizeId == 6).Count()
                        });
                    }
                }
            }

            return repInventoryCommissary2.ToList();
        }
    }
}
