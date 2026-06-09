using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using System.Drawing;
using System.Data.SqlClient;
using MWS.Models;

namespace MWS.Controllers
{
    class TrnProductionItemController
    {
        // Data Context
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());
        private Bitmap barcodeBitmap;
        public string barcodeItem;
        public string barcodeWeight;
        public string barcodeSize;
        public string barcodeClassification;
        public string barcodeRemarks;

        // Classification - List
        public List<MstClassificationModel> DropDownClassification()
        {
            List<MstClassificationModel> classifications = new List<MstClassificationModel>();

            classifications.Add(new MstClassificationModel { Classification = "CLASSIC" });
            classifications.Add(new MstClassificationModel { Classification = "SPICY" });

            return classifications;
        }
        // Item List
        public List<Models.MstItemModel> DropDownItem()
        {
            var items = from d in db.MstItems
                        where d.ItemDescription != "SLAB"
                        select new Models.MstItemModel
                        {
                            Id = d.Id,
                            Item = d.ItemDescription
                        };

            return items.OrderBy(d => d.Item).ToList();
        }
        // List Production Item
        public List<Models.TrnProductionItemModel> ProductionItemList(Int32 productionId, string filter)
        {
            var productionItems = from d in db.TrnProductionItems
                                  where d.ProductionBarcode.Contains(filter)
                                  select new Models.TrnProductionItemModel
                                  {
                                      Id = d.Id,
                                      ProductionId = d.ProductionId,
                                      ItemId = d.ItemId,
                                      Barcode = d.ProductionBarcode,
                                      ItemDescription = d.MstItem.ItemDescription,
                                      SizeId = d.SizeId,
                                      Size = d.MstSize.Size,
                                      Classification = d.Classification,
                                      ReceivedWeight = d.ReceivedWeight,
                                      ActualWeight = d.ActualWeight,
                                      Remarks = d.Remarks
                                  };

            return productionItems.Where(d => d.ProductionId == productionId).OrderByDescending(e => e.Id).ToList();
        }

        // Add Production Item
        public String[] AddProductionItem(int productionId, decimal weight, string barcode)
        {
            try
            {
                var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                String productionBarcode = "100000000000";

                var lastProductionItem = db.TrnProductionItems
                                           .OrderByDescending(d => d.ProductionBarcode)
                                           .FirstOrDefault();

                if (lastProductionItem != null)
                {
                    string currentBarcode = lastProductionItem.ProductionBarcode ?? "";

                    if (currentBarcode.Length >= 12)
                    {
                        long lastNumber = Convert.ToInt64(currentBarcode.Substring(0, 12));
                        long nextNumber = lastNumber + 1;
                        productionBarcode = nextNumber.ToString().PadLeft(12, '0');
                    }
                }

                string finalBarcode = CalculateEAN13(productionBarcode);

                if (currentBranchId == 1)
                {
                    DB.TrnProductionItem newProductionItem = new DB.TrnProductionItem
                    {
                        ProductionId = productionId,
                        ItemId = 1,
                        SizeId = GetSize(weight),
                        ProductionBarcode = finalBarcode,
                        ActualWeight = weight,
                        Classification = "CLASSIC",
                        Remarks = "NA"
                    };

                    db.TrnProductionItems.InsertOnSubmit(newProductionItem);
                    db.SubmitChanges();

                    return new String[] { "", newProductionItem.Id.ToString() };
                }
                else
                {
                    var receivingItem = from d in db.TrnReceivingItems
                                        where d.TrnReceiving.IsLocked == true
                                        && d.Barcode == barcode
                                        && d.TrnReceiving.BranchId != 1
                                        select d;
                    var item = receivingItem.FirstOrDefault();
                    DB.TrnProductionItem newProductionItem = new DB.TrnProductionItem
                    {
                        ProductionId = productionId,
                        ItemId = item.ItemId,
                        SizeId = item.SizeId,
                        ProductionBarcode = barcode,
                        ActualWeight = 0,
                        ReceivedWeight = item.Weight,
                        Classification = item.Classification,
                        Remarks = "NA"
                    };

                    db.TrnProductionItems.InsertOnSubmit(newProductionItem);
                    db.SubmitChanges();

                    return new String[] { "", "1" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }
        // Add Production Item except SLAB
        public String[] AddProductionItemOthers(int productionId, int itemId, decimal weight)
        {
            try
            {
                var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                String productionBarcode = "100000000000";

                var lastProductionItem = db.TrnProductionItems
                                           .OrderByDescending(d => d.ProductionBarcode)
                                           .FirstOrDefault();

                if (lastProductionItem != null)
                {
                    string currentBarcode = lastProductionItem.ProductionBarcode ?? "";

                    if (currentBarcode.Length >= 12)
                    {
                        long lastNumber = Convert.ToInt64(currentBarcode.Substring(0, 12));
                        long nextNumber = lastNumber + 1;
                        productionBarcode = nextNumber.ToString().PadLeft(12, '0');
                    }
                }

                string finalBarcode = CalculateEAN13(productionBarcode);

                var item = db.MstItems.FirstOrDefault(d => d.Id == itemId);

                DB.TrnProductionItem newProductionItem = new DB.TrnProductionItem
                {
                    ProductionId = productionId,
                    ItemId = itemId,
                    SizeId = 7,
                    ProductionBarcode = finalBarcode,
                    ActualWeight = weight,
                    Classification = item.ItemDescription,
                    Remarks = "NA"
                };

                db.TrnProductionItems.InsertOnSubmit(newProductionItem);
                db.SubmitChanges();

                barcodeItem = newProductionItem.MstItem.ItemDescription;
                barcodeWeight = newProductionItem.ActualWeight.ToString();
                barcodeSize = newProductionItem.MstSize.Size;
                barcodeClassification = newProductionItem.Classification;
                barcodeRemarks = newProductionItem.Remarks;

                GenerateAndPrintBarcode(finalBarcode);

                return new String[] { "", newProductionItem.Id.ToString() };
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }


        // Update Production Item Weight
        public String[] UpdateProductionItemWeight(int id, decimal weight)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var productionItem = from d in db.TrnProductionItems
                                     where d.Id == id
                                     select d;

                if (productionItem.Any())
                {
                    var updateProductionItem = productionItem.FirstOrDefault();
                    updateProductionItem.ActualWeight = weight;
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

        // Update Production Item Classification
        public String[] UpdateProductionItemClassification(int id, string classification, string remarks)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var productionItem = from d in db.TrnProductionItems
                                     where d.Id == id
                                     select d;

                if (productionItem.Any())
                {
                    var updateProductionItem = productionItem.FirstOrDefault();
                    updateProductionItem.Classification = classification;
                    updateProductionItem.Remarks = remarks;
                    db.SubmitChanges();

                    barcodeItem = updateProductionItem.MstItem.ItemDescription;
                    barcodeWeight = updateProductionItem.ActualWeight.ToString();
                    barcodeSize = updateProductionItem.MstSize.Size;
                    barcodeClassification = updateProductionItem.Classification;
                    barcodeRemarks = updateProductionItem.Remarks;

                    GenerateAndPrintBarcode(updateProductionItem.ProductionBarcode);

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
        // Update Production Item Remarks
        public String[] UpdateProductionItemRemarks(int id, string remarks)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var productionItem = from d in db.TrnProductionItems
                                     where d.Id == id
                                     select d;

                if (productionItem.Any())
                {
                    var updateProductionItem = productionItem.FirstOrDefault();
                    updateProductionItem.Remarks = remarks;
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
        public string CalculateEAN13(string input)
        {
            if (input.Length < 12) return "Invalid Length";

            string first12 = input.Substring(0, 12);
            int sum = 0;

            for (int i = 0; i < 12; i++)
            {
                int digit = int.Parse(first12[i].ToString());
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            int checksum = (10 - (sum % 10)) % 10;
            return first12 + checksum.ToString();
        }
        private void GenerateAndPrintBarcode(string data)
        {
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.EAN_13,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 300,
                        Height = 110,
                        Margin = 0
                    }
                };

                barcodeBitmap = writer.Write(data);

                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintBarcodeHandler);

                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Make sure it is 12 or 13 digits ang EAN-13.\n" + ex.Message, "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PrintBarcodeHandler(object sender, PrintPageEventArgs e)
        {
            if (barcodeBitmap != null)
            {
                Font fontBold = new Font("Segoe UI", 18, FontStyle.Bold);
                Font fontRegular = new Font("Segoe UI", 13, FontStyle.Regular);

                int startX = 30;
                int baseStartY = 120; // Ang original nga Y position
                int labelSpacing = 290; // Distansya gikan sa unang label padulong sa ikaduha

                int barcodeWidth = 300;
                int barcodeHeight = 110;

                // Mag-loop ta og kaduha (0 ug 1)
                for (int i = 0; i < 2; i++)
                {
                    // Kada tuyok, madugangan ang startY base sa labelSpacing
                    int startY = baseStartY + (i * labelSpacing);

                    // 1. Item (Center, Pinaka babaw)
                    string topText = barcodeItem;
                    float topTextWidth = e.Graphics.MeasureString(topText, fontBold).Width;
                    float topTextX = startX + (barcodeWidth - topTextWidth) / 2;
                    e.Graphics.DrawString(topText, fontBold, Brushes.Black, topTextX, startY - 95);

                    // 2. Weight (Center, Ubos sa SLAB)
                    string sizeText = barcodeWeight;
                    float sizeTextWidth = e.Graphics.MeasureString(sizeText, fontBold).Width;
                    float sizeTextX = startX + (barcodeWidth - sizeTextWidth) / 2;
                    e.Graphics.DrawString(sizeText, fontBold, Brushes.Black, sizeTextX, startY - 55);

                    // 4. Size (Left) ug Classification (Right)
                    int textAboveBarcodeY = startY - 35;
                    e.Graphics.DrawString(barcodeSize, fontRegular, Brushes.Black, startX + 15, textAboveBarcodeY);

                    string rightText = barcodeClassification;
                    float rightTextWidth = e.Graphics.MeasureString(rightText, fontRegular).Width;
                    e.Graphics.DrawString(rightText, fontRegular, Brushes.Black, startX + 235, textAboveBarcodeY);

                    string remarksText = barcodeRemarks;
                    float remarksTextWidth = e.Graphics.MeasureString(remarksText, fontRegular).Width;
                    float remarksTextX = startX + (barcodeWidth - remarksTextWidth) / 2;
                    e.Graphics.DrawString(remarksText, fontRegular, Brushes.Black, remarksTextX, startY);

                    // 3. Ang Barcode
                    e.Graphics.DrawImage(barcodeBitmap, startX, startY + 30, barcodeWidth, barcodeHeight);
                }
            }
        }

        // Delete Production Item
        public String[] DeleteProductionItem(Int32 id)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                var productionItem = from d in db.TrnProductionItems
                                     where d.Id == id
                                     select d;

                if (productionItem.Any())
                {
                    var deleteProductionItem = productionItem.FirstOrDefault();
                    db.TrnProductionItems.DeleteOnSubmit(deleteProductionItem);
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
        public string GetClassification(string barcode)
        {
            var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            var processingItem = from d in db.TrnProductionItems
                                where d.ProductionBarcode == barcode
                                && d.TrnProduction.IsLocked == true
                                && d.TrnProduction.BranchId != currentBranchId
                                select d;
            if (processingItem.Any())
            {
                return processingItem.FirstOrDefault().Classification;
            }
            return null;
        }
        public ExistingItemInfo GetExistingItemDetails(string barcode)
        {
            var item = (from d in db.TrnProductionItems
                        where d.ProductionBarcode == barcode
                        select new ExistingItemInfo
                        {
                            IsAdded = true,
                            Barcode = d.ProductionBarcode,
                            ProductionNo = d.TrnProduction.ProductionNo,
                            ProductionDate = d.TrnProduction.ProductionDate
                        }).FirstOrDefault();

            return item ?? new ExistingItemInfo { IsAdded = false };
        }
        public class ExistingItemInfo
        {
            public bool IsAdded { get; set; }
            public string Barcode { get; set; }
            public string ProductionNo { get; set; }
            public DateTime ProductionDate { get; set; }
        }
        public bool isAlreadyAddedInHere(string barcode, int productionId)
        {
            bool added = false;
            var barcodeExist = from d in db.TrnProductionItems
                               where d.ProductionBarcode == barcode
                               && d.ProductionId == productionId
                               select d;
            if (barcodeExist.Any())
            {
                added = true;
            }

            return added;
        }
        public bool IsExist(string barcode)
        {
            bool exist = false;
            var barcodeExist = from d in db.TrnReceivingItems
                               where d.TrnReceiving.IsLocked == true
                               && d.Barcode == barcode
                               && d.TrnReceiving.BranchId != 1
                               select d;
            if (barcodeExist.Any())
            {
                exist = true;
            }

            return exist;
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
