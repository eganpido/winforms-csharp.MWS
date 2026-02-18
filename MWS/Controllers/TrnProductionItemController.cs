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

namespace MWS.Controllers
{
    class TrnProductionItemController
    {
        // Data Context
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());
        private Bitmap barcodeBitmap;
        // List Production Item
        public List<Models.TrnProductionItemModel> ProductionItemList(Int32 productionId)
        {
            var productionItems = from d in db.TrnProductionItems

                                  select new Models.TrnProductionItemModel
                                  {
                                     Id = d.Id,
                                     ProductionId = d.ProductionId,
                                     ReceivingItemId = d.ReceivingItemId,
                                     ItemId = d.TrnReceivingItem.ItemId,
                                     ReceivingBarcode = d.TrnReceivingItem.Barcode,
                                     Barcode = d.ProductionBarcode,
                                     ItemDescription = d.TrnReceivingItem.ItemDescription,
                                     SizeId = d.TrnReceivingItem.SizeId,
                                     Size = d.TrnReceivingItem.MstSize.Size,
                                     ReceivedWeight = d.TrnReceivingItem.Weight,
                                     ActualWeight = d.ActualWeight
                                  };

            return productionItems.Where(d => d.ProductionId == productionId).OrderByDescending(e => e.Id).ToList();
        }

        // Add Production Item
        public String[] AddProductionItem(int productionId, string barcode, decimal weight)
        {
            try
            {
                var currentUserLogin = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;
                if (currentUserLogin.Any() == false)
                {
                    return new String[] { "Current login user not found.", "0" };
                }

                String productionBarcode = "100000000000";
                var lastProductionItem = db.TrnProductionItems
                                          .OrderByDescending(d => d.Id)
                                          .FirstOrDefault();
                if (lastProductionItem != null)
                {
                    long lastNumber = Convert.ToInt64(lastProductionItem.ProductionBarcode.Substring(0, 12));
                    long nextNumber = lastNumber + 1;
                    productionBarcode = nextNumber.ToString().PadLeft(12, '0');
                }

                string finalBarcode = CalculateEAN13(productionBarcode);

                DB.TrnProductionItem newProductionItem = new DB.TrnProductionItem
                {
                    ProductionId = productionId,
                    ReceivingItemId = GetReceivingItem(barcode),
                    ProductionBarcode = finalBarcode,
                    ActualWeight = weight
                };

                db.TrnProductionItems.InsertOnSubmit(newProductionItem);
                db.SubmitChanges();

                GenerateAndPrintBarcode(finalBarcode);

                return new String[] { "", "1" };
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
                MessageBox.Show("Error: Siguradua nga 12 o 13 digits ang EAN-13.\n" + ex.Message, "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PrintBarcodeHandler(object sender, PrintPageEventArgs e)
        {
            if (barcodeBitmap != null)
            {
                e.Graphics.DrawImage(barcodeBitmap, 100, 100); // 100, 100 ang position sa papel
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
        public int GetReceivingItem(string barcode)
        {
            int receivingItemId = 0;
            var receivingItem = from d in db.TrnReceivingItems
                                where d.Barcode == barcode
                                select d;
            if (receivingItem.Any())
            {
                receivingItemId = receivingItem.FirstOrDefault().Id;
            }
            return receivingItemId;
        }

        public bool isAlreadyAdded(string barcode)
        {
            bool added = false;

            var barcodeExist = from d in db.TrnProductionItems
                               where d.TrnReceivingItem.Barcode == barcode
                               && d.TrnProduction.IsLocked == true
                               select d;
            if (barcodeExist.Any())
            {
                added = true;
            }

            return added;
        }
    }
}
