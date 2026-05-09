using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ZXing.OneD;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using Chunk = iTextSharp.text.Chunk;

namespace MWS.Reports
{
    public partial class RepInventoryDetailPDFView : Form
    {
        public DateTime dateStart;
        public DateTime dateEnd;
        public int branchId;
        public RepInventoryDetailPDFView(DateTime startDate, DateTime endDate, int _branchId)
        {
            InitializeComponent();

            dateStart = startDate;
            dateEnd = endDate;
            branchId = _branchId;

            PrintReport();
        }
        public void PrintReport()
        {
            try
            {
                DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());
                DB.mwsdbDataContext db2 = new DB.mwsdbDataContext(Modules.SysConnectionString2Module.GetConnectionString());

                iTextSharp.text.Font fontTimesNewRoman10 = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10);
                iTextSharp.text.Font fontTimesNewRoman10Italic = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.ITALIC);
                iTextSharp.text.Font fontTimesNewRoman10Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontTimesNewRoman12Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontTimesNewRoman14Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD);

                Paragraph line = new Paragraph(new iTextSharp.text.Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5F, 100.0F, BaseColor.DARK_GRAY, Element.ALIGN_MIDDLE, 10F)));

                var fileName = "InventoryDetailReport" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var currentUser = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;

                var systemCurrent = Modules.SysCurrentModule.GetCurrentSettings();

                Document document = new Document(PageSize.LETTER.Rotate());
                document.SetMargins(30f, 30f, 100f, 30f);

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
                pdfWriter.PageEvent = new ConfigureHeaderFooter(dateStart, dateEnd);

                document.Open();

                Controllers.RepInventoryController repInventoryController = new Controllers.RepInventoryController();
                var commissary1 = repInventoryController.Commissary1DetailList(dateStart, dateEnd);
                var commissary2 = repInventoryController.Commissary2DetailList(dateStart, dateEnd);

                PdfPTable tableCommissary1 = new PdfPTable(5);
                tableCommissary1.SetWidths(new float[] { 150f, 200f, 60f, 60f, 60f });
                tableCommissary1.WidthPercentage = 100;

                PdfPTable tableCommissary2 = new PdfPTable(5);
                tableCommissary2.SetWidths(new float[] { 150f, 200f, 60f, 60f, 60f });
                tableCommissary2.WidthPercentage = 100;

                if (branchId == 0)
                {
                    tableCommissary1.AddCell(new PdfPCell(new Phrase("COMMISSARY 1", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });

                    if (commissary1.Any())
                    {
                        var groupCommissary1 = from d in commissary1
                                               orderby d.Barcode
                                               group d by new
                                               {
                                                   d.Barcode,
                                                   d.Classification,
                                                   d.Pundo
                                               }
                                          into g
                                               select new
                                               {
                                                   Barcode = g.Key.Barcode,
                                                   Classification = g.Key.Classification,
                                                   Pundo = g.Key.Pundo,
                                                   Processing = g.Sum(a => a.Processing),
                                                   PullOut = g.Sum(a => a.PullOut),
                                                   Balance = g.Sum(a => a.Balance)
                                               };

                        if (groupCommissary1.Any())
                        {
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("SLAB  PUNDO : " + groupCommissary1.FirstOrDefault().Pundo, fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                            tableCommissary1.AddCell(new PdfPCell(new Phrase("BARCODE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("CLASSIFICATION", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("PROCESSING", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("PULL-OUT", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("BALANCE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                            int totalCount = 0;
                            int totalClassic = 0;
                            int totalSpicy = 0;
                            int totalProcessing = 0;
                            int totalPullOut = 0;
                            int totalBalance = 0;
                            foreach (var item in groupCommissary1)
                            {
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Barcode, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 0 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Classification, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 0 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Processing.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.PullOut.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Balance.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });

                                totalCount++;
                                totalProcessing += item.Processing;
                                totalPullOut += item.PullOut;
                                totalBalance += item.Balance;
                                if (item.Classification == "CLASSIC") totalClassic++;
                                else totalSpicy++;
                            }
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                            tableCommissary1.AddCell(new PdfPCell(new Phrase("TOTAL COUNT :" + totalCount, fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("CLASSIC : " + totalClassic + "  SPICY : " + totalSpicy, fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(totalProcessing.ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(totalPullOut.ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase((totalBalance + groupCommissary1.FirstOrDefault().Pundo).ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                            document.Add(tableCommissary1);
                        }
                    }

                    if (commissary2.Any())
                    {
                        tableCommissary2.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("COMMISSARY 2", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });

                        if (commissary2.Any())
                        {
                            var groupCommissary2 = from d in commissary2
                                                   orderby d.Barcode
                                                   group d by new
                                                   {
                                                       d.Barcode,
                                                       d.Classification,
                                                       d.Pundo
                                                   }
                                              into g
                                                   select new
                                                   {
                                                       Barcode = g.Key.Barcode,
                                                       Classification = g.Key.Classification,
                                                       Pundo = g.Key.Pundo,
                                                       Receiving = g.Sum(a => a.Receiving),
                                                       Production = g.Sum(a => a.Production),
                                                       Balance = g.Sum(a => a.Balance)
                                                   };

                            if (groupCommissary2.Any())
                            {
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("SLAB  PUNDO : " + groupCommissary2.FirstOrDefault().Pundo, fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                                tableCommissary2.AddCell(new PdfPCell(new Phrase("BARCODE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("CLASSIFICATION", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("RECEIVING", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("PRODUCTION", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("BALANCE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                                int totalCount = 0;
                                int totalClassic = 0;
                                int totalSpicy = 0;
                                int totalReceiving = 0;
                                int totalProduction = 0;
                                int totalBalance = 0;
                                foreach (var item in groupCommissary2)
                                {
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Barcode, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 0 });
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Classification, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 0 });
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Receiving.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Production.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Balance.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });

                                    totalCount++;
                                    totalReceiving += item.Receiving;
                                    totalProduction += item.Production;
                                    totalBalance += item.Balance;
                                    if (item.Classification == "CLASSIC") totalClassic++;
                                    else totalSpicy++;
                                }
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                                tableCommissary2.AddCell(new PdfPCell(new Phrase("TOTAL COUNT :" + totalCount, fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("CLASSIC : " + totalClassic + "  SPICY : " + totalSpicy, fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(totalReceiving.ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(totalProduction.ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase((totalBalance + groupCommissary2.FirstOrDefault().Pundo).ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                                document.Add(tableCommissary2);
                            }
                        }
                    }
                }
                else
                {
                    if (branchId == 1)
                    {
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("COMMISSARY 1", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });

                        if (commissary1.Any())
                        {
                            var groupCommissary1 = from d in commissary1
                                                   orderby d.Barcode
                                                   group d by new
                                                   {
                                                       d.Barcode,
                                                       d.Classification,
                                                       d.Pundo
                                                   }
                                              into g
                                                   select new
                                                   {
                                                       Barcode = g.Key.Barcode,
                                                       Classification = g.Key.Classification,
                                                       Pundo = g.Key.Pundo,
                                                       Processing = g.Sum(a => a.Processing),
                                                       PullOut = g.Sum(a => a.PullOut),
                                                       Balance = g.Sum(a => a.Balance)
                                                   };

                            if (groupCommissary1.Any())
                            {
                                tableCommissary1.AddCell(new PdfPCell(new Phrase("SLAB  PUNDO : " + groupCommissary1.FirstOrDefault().Pundo, fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                                tableCommissary1.AddCell(new PdfPCell(new Phrase("BARCODE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase("CLASSIFICATION", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase("PROCESSING", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase("PULL-OUT", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase("BALANCE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                                int totalCount = 0;
                                int totalClassic = 0;
                                int totalSpicy = 0;
                                int totalProcessing = 0;
                                int totalPullOut = 0;
                                int totalBalance = 0;
                                foreach (var item in groupCommissary1)
                                {
                                    tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Barcode, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 0 });
                                    tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Classification, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 0 });
                                    tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Processing.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });
                                    tableCommissary1.AddCell(new PdfPCell(new Phrase(item.PullOut.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });
                                    tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Balance.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });

                                    totalCount++;
                                    totalProcessing += item.Processing;
                                    totalPullOut += item.PullOut;
                                    totalBalance += item.Balance;
                                    if (item.Classification == "CLASSIC") totalClassic++;
                                    else totalSpicy++;
                                }
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                                tableCommissary1.AddCell(new PdfPCell(new Phrase("TOTAL COUNT :" + totalCount, fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase("CLASSIC : " + totalClassic + "  SPICY : " + totalSpicy, fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(totalProcessing.ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(totalPullOut.ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase((totalBalance + groupCommissary1.FirstOrDefault().Pundo).ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                                document.Add(tableCommissary1);
                            }
                        }
                    }
                    else
                    {
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("COMMISSARY 2", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });

                        if (commissary2.Any())
                        {
                            var groupCommissary2 = from d in commissary2
                                                   orderby d.Barcode
                                                   group d by new
                                                   {
                                                       d.Barcode,
                                                       d.Classification,
                                                       d.Pundo
                                                   }
                                              into g
                                                   select new
                                                   {
                                                       Barcode = g.Key.Barcode,
                                                       Classification = g.Key.Classification,
                                                       Pundo = g.Key.Pundo,
                                                       Receiving = g.Sum(a => a.Receiving),
                                                       Production = g.Sum(a => a.Production),
                                                       Balance = g.Sum(a => a.Balance)
                                                   };

                            if (groupCommissary2.Any())
                            {
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("SLAB PUNDO : " + groupCommissary2.FirstOrDefault().Pundo, fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                                tableCommissary2.AddCell(new PdfPCell(new Phrase("BARCODE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("CLASSIFICATION", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("RECEIVING", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("PRODUCTION", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("BALANCE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                                int totalCount = 0;
                                int totalClassic = 0;
                                int totalSpicy = 0;
                                int totalReceiving = 0;
                                int totalProduction = 0;
                                int totalBalance = 0;
                                foreach (var item in groupCommissary2)
                                {
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Barcode, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 0 });
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Classification, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 0 });
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Receiving.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Production.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });
                                    tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Balance.ToString(), fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f, HorizontalAlignment = 1 });

                                    totalCount++;
                                    totalReceiving += item.Receiving;
                                    totalProduction += item.Production;
                                    totalBalance += item.Balance;
                                    if (item.Classification == "CLASSIC") totalClassic++;
                                    else totalSpicy++;
                                }
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                                tableCommissary2.AddCell(new PdfPCell(new Phrase("TOTAL COUNT :" + totalCount, fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase("CLASSIC : " + totalClassic + "  SPICY : " + totalSpicy, fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(totalReceiving.ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(totalProduction.ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase((totalBalance + groupCommissary2.FirstOrDefault().Pundo).ToString(), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                                document.Add(tableCommissary2);
                            }
                        }
                    }
                }

                document.Close();

                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MWS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        class ConfigureHeaderFooter : PdfPageEventHelper
        {
            public DateTime dateStart;
            public DateTime dateEnd;

            public DB.mwsdbDataContext db;

            public ConfigureHeaderFooter(DateTime startDate, DateTime endDate)
            {
                dateStart = startDate;
                dateEnd = endDate;

                db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                iTextSharp.text.Font fontTimesNewRoman10 = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10);
                iTextSharp.text.Font fontTimesNewRoman10Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontTimesNewRoman14Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD);

                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0F, 100.0F, BaseColor.BLACK, Element.ALIGN_MIDDLE, 7F)));

                var systemCurrent = Modules.SysCurrentModule.GetCurrentSettings();

                String companyName = systemCurrent.CompanyName;
                String documentTitle = "Inventory Detail Report";

                PdfPTable tableHeader = new PdfPTable(4);
                tableHeader.SetWidths(new float[] { 20f, 30f, 20f, 50f });
                tableHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                tableHeader.AddCell(new PdfPCell(new Phrase(companyName, fontTimesNewRoman14Bold)) { Colspan = 2, Border = 0, Padding = 3f, PaddingBottom = 3f });
                tableHeader.AddCell(new PdfPCell(new Phrase(documentTitle, fontTimesNewRoman14Bold)) { HorizontalAlignment = 2, Colspan = 2, Border = 0, Padding = 3f, PaddingBottom = 3f });
                tableHeader.AddCell(new PdfPCell(new Phrase("From : " + dateStart.ToShortDateString() + " To: " + dateEnd.ToShortDateString() + "\n", fontTimesNewRoman10)) { Colspan = 4, Border = 0, Padding = 3f, PaddingBottom = -5f });

                tableHeader.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetTop(document.TopMargin) + 67f, writer.DirectContent);
            }
        }
    }
}
