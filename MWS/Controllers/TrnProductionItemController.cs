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

        // Classification - List
        public List<MstClassificationModel> DropDownClassification()
        {
            List<MstClassificationModel> classifications = new List<MstClassificationModel>();

            classifications.Add(new MstClassificationModel { Classification = "NONE" });
            classifications.Add(new MstClassificationModel { Classification = "CUT" });
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
        public List<Models.TrnProductionItemModel> ProductionItemList(Int32 productionId)
        {
            var productionItems = from d in db.TrnProductionItems

                                  select new Models.TrnProductionItemModel
                                  {
                                      Id = d.Id,
                                      ProductionId = d.ProductionId,
                                      ReceivingItemId = d.ReceivingItemId,
                                      ItemId = d.ItemId,
                                      ReceivingBarcode = d.TrnReceivingItem.Barcode,
                                      Barcode = d.ProductionBarcode,
                                      ItemDescription = d.MstItem.ItemDescription,
                                      SizeId = d.SizeId,
                                      Size = d.MstSize.Size,
                                      Classification = d.Classification,
                                      ReceivedWeight = d.ReceivedWeight,
                                      ActualWeight = d.ActualWeight
                                  };

            return productionItems.Where(d => d.ProductionId == productionId).OrderByDescending(e => e.Id).ToList();
        }

        // Add Production Item
        public String[] AddProductionItem(int productionId, string barcode, decimal weight)
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
                        ReceivingItemId = GetReceivingItem(barcode).Id,
                        ItemId = GetReceivingItem(barcode).ItemId,
                        SizeId = GetReceivingItem(barcode).SizeId,
                        ProductionBarcode = finalBarcode,
                        ActualWeight = weight,
                        ReceivedWeight = GetReceivingItem(barcode).Weight,
                        Classification = "NONE"
                    };

                    db.TrnProductionItems.InsertOnSubmit(newProductionItem);
                    db.SubmitChanges();

                    return new String[] { "", newProductionItem.Id.ToString() };
                }
                else
                {
                    DB.TrnProductionItem newProductionItem = new DB.TrnProductionItem
                    {
                        ProductionId = productionId,
                        ReceivingItemId = GetReceivingItem(barcode).Id,
                        ItemId = GetReceivingItem(barcode).ItemId,
                        SizeId = GetReceivingItem(barcode).SizeId,
                        ProductionBarcode = barcode,
                        ActualWeight = 0,
                        ReceivedWeight = GetReceivingItem(barcode).Weight,
                        Classification = GetClassification(barcode)
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

                DB.TrnProductionItem newProductionItem = new DB.TrnProductionItem
                {
                    ProductionId = productionId,
                    ItemId = itemId,
                    SizeId = 7,
                    ProductionBarcode = finalBarcode,
                    ActualWeight = weight,
                    ReceivedWeight = 0,
                    Classification = "CUT"
                };

                db.TrnProductionItems.InsertOnSubmit(newProductionItem);
                db.SubmitChanges();

                barcodeItem = newProductionItem.MstItem.ItemDescription;
                barcodeWeight = newProductionItem.ActualWeight.ToString();
                barcodeSize = newProductionItem.MstSize.Size;
                barcodeClassification = newProductionItem.Classification;

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
        public String[] UpdateProductionItemClassification(int id, string classification)
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
                    db.SubmitChanges();

                    barcodeItem = updateProductionItem.MstItem.ItemDescription;
                    barcodeWeight = updateProductionItem.ActualWeight.ToString();
                    barcodeSize = updateProductionItem.MstSize.Size;
                    barcodeClassification = updateProductionItem.Classification;

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
                        Height = 150,
                        Margin = 10
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
                // Settings para sa font ug position
                Font fontBold = new Font("Segoe UI", 18, FontStyle.Bold);
                Font fontRegular = new Font("Segoe UI", 13, FontStyle.Regular);

                int startX = 100;
                // Gihimo natong 180 ang startY para naay dako nga space sa taas para sa mga text
                int startY = 180;
                int barcodeWidth = 500;
                int barcodeHeight = 200;

                // 1. "SLAB" (Center, Pinaka babaw)
                string topText = barcodeItem;
                float topTextWidth = e.Graphics.MeasureString(topText, fontBold).Width;
                float topTextX = startX + (barcodeWidth - topTextWidth) / 2;
                // Gi-move nato sa -120 gikan sa barcode
                e.Graphics.DrawString(topText, fontBold, Brushes.Black, topTextX, startY - 95);

                // 2. "8.9" (Center, Ubos sa SLAB)
                string sizeText = barcodeWeight;
                float sizeTextWidth = e.Graphics.MeasureString(sizeText, fontBold).Width;
                float sizeTextX = startX + (barcodeWidth - sizeTextWidth) / 2;
                // Gi-move sa -85 para naay space gikan sa topText
                e.Graphics.DrawString(sizeText, fontBold, Brushes.Black, sizeTextX, startY - 55);

                // 4. (KANI IMONG GIPANGITA) "SLAB" (Left) ug "SPICY" (Right)
                // Gi-move nato sa -45 para naay dako nga space sa tunga nila ug sa barcode
                int textAboveBarcodeY = startY - 35;
                e.Graphics.DrawString(barcodeSize, fontRegular, Brushes.Black, startX + 90, textAboveBarcodeY);

                string rightText = barcodeClassification;
                float rightTextWidth = e.Graphics.MeasureString(rightText, fontRegular).Width;
                e.Graphics.DrawString(rightText, fontRegular, Brushes.Black, startX + 365, textAboveBarcodeY);

                // 3. Ang Barcode (Magsugod sa startY)
                e.Graphics.DrawImage(barcodeBitmap, startX, startY, barcodeWidth, barcodeHeight);
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
        public TrnReceivingItemModel GetReceivingItem(string barcode)
        {
            var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            var receivingItem = from d in db.TrnReceivingItems
                                where d.Barcode == barcode
                                && d.TrnReceiving.IsLocked == true
                                && d.TrnReceiving.BranchId == currentBranchId
                                select new TrnReceivingItemModel
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
            if (receivingItem.Any())
            {
                return receivingItem.FirstOrDefault();
            }
            return null;
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
        public bool isAlreadyAdded(string barcode)
        {
            var currentBranchId = Modules.SysCurrentModule.GetCurrentSettings().BranchId;
            bool added = false;
            var barcodeExist = from d in db.TrnProductionItems
                               where d.TrnReceivingItem.Barcode == barcode
                               && d.TrnProduction.IsLocked == true
                               && d.TrnProduction.BranchId == currentBranchId
                               select d;
            if (barcodeExist.Any())
            {
                added = true;
            }

            return added;
        }
    }
}
