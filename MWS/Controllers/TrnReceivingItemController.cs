using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace MWS.Controllers
{
    class TrnReceivingItemController
    {
        // Data Context
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());
        private Bitmap barcodeBitmap;
        public string barcodeItem;
        public string barcodeWeight;
        public string barcodeSize;
        public string barcodeClassification;
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

                string baseBarcode = "100000000000";

                var lastReceivingItem = db.TrnReceivingItems
                                          .OrderByDescending(d => d.Barcode)
                                          .FirstOrDefault();

                if (lastReceivingItem != null)
                {
                    string currentBarcode = lastReceivingItem.Barcode;

                    if (currentBarcode.Length >= 12)
                    {
                        long lastNumber = Convert.ToInt64(currentBarcode.Substring(0, 12));
                        long nextNumber = lastNumber + 1;
                        baseBarcode = nextNumber.ToString().PadLeft(12, '0');
                    }
                }

                string finalBarcode = CalculateEAN13(baseBarcode);

                DB.TrnReceivingItem newReceivingItem = new DB.TrnReceivingItem
                {
                    ReceivingId = receivingId,
                    ItemId = 1,
                    Barcode = finalBarcode, 
                    ItemDescription = "SLAB",
                    SizeId = GetSize(weight),
                    Weight = weight,
                    Classification = "NONE"
                };

                db.TrnReceivingItems.InsertOnSubmit(newReceivingItem);
                db.SubmitChanges();

                barcodeItem = newReceivingItem.ItemDescription;
                barcodeWeight = newReceivingItem.Weight.ToString();
                barcodeSize = newReceivingItem.MstSize.Size;
                barcodeClassification = newReceivingItem.Classification;

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
                MessageBox.Show("Error: Make sure there are 12 or 13 digits in EAN-13.\n" + ex.Message, "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    return new String[] { "Receiving item not found.", "0" };
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
