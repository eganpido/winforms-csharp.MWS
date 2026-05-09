using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MWS.Reports
{
    public partial class RepReceivingPDFView : Form
    {
        public DateTime dateStart;
        public DateTime dateEnd;
        public int branchId;
        public int supplierId;
        public string branchName;
        public RepReceivingPDFView(DateTime startDate, DateTime endDate, int _branchId, int _supplierId)
        {
            InitializeComponent();

            dateStart = startDate;
            dateEnd = endDate;
            branchId = _branchId;
            supplierId = _supplierId;

            PrintReport();
        }
        public void PrintReport()
        {
            try
            {
                DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

                iTextSharp.text.Font fontTimesNewRoman10 = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10);
                iTextSharp.text.Font fontTimesNewRoman10Italic = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.ITALIC);
                iTextSharp.text.Font fontTimesNewRoman10Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontTimesNewRoman11Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontTimesNewRoman12Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontTimesNewRoman14Bold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD);

                Paragraph line = new Paragraph(new iTextSharp.text.Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5F, 100.0F, BaseColor.DARK_GRAY, Element.ALIGN_MIDDLE, 10F)));

                var fileName = "ReceivingReport" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var currentUser = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;

                var systemCurrent = Modules.SysCurrentModule.GetCurrentSettings();

                Document document = new Document(PageSize.LETTER.Rotate());
                document.SetMargins(30f, 30f, 100f, 30f);

                var branches = from d in db.MstBranches
                               where d.Id == branchId
                               select d;
                if (branches.Any())
                {
                    branchName = branches.FirstOrDefault().Branch;
                }

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
                pdfWriter.PageEvent = new ConfigureHeaderFooter(dateStart, dateEnd, branchName);

                document.Open();

                Controllers.RepInventoryController repInventoryController = new Controllers.RepInventoryController();
                var receiving = repInventoryController.ReceivingReport(dateStart, dateEnd, branchId, supplierId);

                PdfPTable tableReceiving = new PdfPTable(3);
                tableReceiving.SetWidths(new float[] { 100f, 100f, 100f });
                tableReceiving.WidthPercentage = 100;

                var groupSupplier = from d in receiving
                                    group d by new
                                    {
                                        d.SupplierId,
                                        d.Supplier
                                    }
                                    into g
                                    select new
                                    {
                                        SupplierId = g.Key.SupplierId,
                                        Supplier = g.Key.Supplier
                                    };
                if (groupSupplier.Any())
                {
                    foreach(var supplier in groupSupplier)
                    {
                        tableReceiving.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 3, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });
                        tableReceiving.AddCell(new PdfPCell(new Phrase(supplier.Supplier, fontTimesNewRoman12Bold)) { Border = 0, Colspan = 3, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });

                        var groupReceivingNo = from d in receiving
                                               where d.SupplierId == supplier.SupplierId
                                               group d by new
                                               {
                                                   d.ReceivingId,
                                                   d.ReceivingNo,
                                                   d.ReceivingDate,
                                                   d.Remarks,
                                                   d.ExpectedWeight
                                               }
                                               into g
                                               select new
                                               {
                                                  ReceivingId = g.Key.ReceivingId,
                                                  ReceivingNo = g.Key.ReceivingNo,
                                                  ReceivingDate = g.Key.ReceivingDate,
                                                  Remarks = g.Key.Remarks,
                                                  ExpectedWeight = g.Key.ExpectedWeight
                                               };
                        if (groupReceivingNo.Any())
                        {
                            foreach( var receivingNo in groupReceivingNo)
                            {
                                decimal subTotal = 0;
                                tableReceiving.AddCell(new PdfPCell(new Phrase("Receiving No.: " + receivingNo.ReceivingNo, fontTimesNewRoman11Bold)) { Border = 0, Colspan = 1, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });
                                tableReceiving.AddCell(new PdfPCell(new Phrase("Receiving Date.: " + receivingNo.ReceivingDate.ToShortDateString(), fontTimesNewRoman11Bold)) { Border = 0, Colspan = 2, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });

                                var receivingItems = from d in receiving
                                                     where d.SupplierId == supplier.SupplierId
                                                     && d.ReceivingId == receivingNo.ReceivingId
                                                     select d;
                                if(receivingItems.Any())
                                {
                                    tableReceiving.AddCell(new PdfPCell(new Phrase("ITEM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                    tableReceiving.AddCell(new PdfPCell(new Phrase("SIZE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                    tableReceiving.AddCell(new PdfPCell(new Phrase("WEIGHT", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                                    int count = 0;
                                    foreach(var item in receivingItems)
                                    {
                                        tableReceiving.AddCell(new PdfPCell(new Phrase(item.Item, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                        tableReceiving.AddCell(new PdfPCell(new Phrase(item.Size, fontTimesNewRoman10)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                        tableReceiving.AddCell(new PdfPCell(new Phrase(item.Weight.ToString("#,##0.00"), fontTimesNewRoman10)) {Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                                        subTotal += item.Weight;
                                        count++;
                                    }
                                    tableReceiving.AddCell(new PdfPCell(new Phrase("Total Count : " + count + "      Remarks : " + receivingNo.Remarks + "      Expected Weight : " + receivingNo.ExpectedWeight, fontTimesNewRoman10Bold)) { Colspan = 2, Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });
                                    tableReceiving.AddCell(new PdfPCell(new Phrase("Sub-Total : " + subTotal.ToString("#,##0.00") + "       Variance : " + (receivingNo.ExpectedWeight - subTotal).ToString("#,##0.00"), fontTimesNewRoman10Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 0 });

                                    tableReceiving.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 3, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });
                                }
                            }
                        }
                    }
                }
                document.Add(tableReceiving);
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
            public string branchName;

            public DB.mwsdbDataContext db;

            public ConfigureHeaderFooter(DateTime startDate, DateTime endDate, string _branchName)
            {
                dateStart = startDate;
                dateEnd = endDate;
                branchName = _branchName;

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
                String documentTitle = "Receiving Report";

                PdfPTable tableHeader = new PdfPTable(4);
                tableHeader.SetWidths(new float[] { 20f, 30f, 20f, 50f });
                tableHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                tableHeader.AddCell(new PdfPCell(new Phrase(companyName, fontTimesNewRoman14Bold)) { Colspan = 2, Border = 0, Padding = 3f, PaddingBottom = 3f });
                tableHeader.AddCell(new PdfPCell(new Phrase(documentTitle, fontTimesNewRoman14Bold)) { HorizontalAlignment = 2, Colspan = 2, Border = 0, Padding = 3f, PaddingBottom = 3f });
                tableHeader.AddCell(new PdfPCell(new Phrase("From : " + dateStart.ToShortDateString() + " To: " + dateEnd.ToShortDateString() + "\n", fontTimesNewRoman10)) { Colspan = 4, Border = 0, Padding = 3f, PaddingBottom = 3f });
                tableHeader.AddCell(new PdfPCell(new Phrase(branchName + "\n", fontTimesNewRoman10)) { Colspan = 4, Border = 0, Padding = 3f, PaddingBottom = 3f });

                tableHeader.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetTop(document.TopMargin) + 67f, writer.DirectContent);
            }
        }
    }
}
