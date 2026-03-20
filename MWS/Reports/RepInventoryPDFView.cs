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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ZXing.OneD;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using Chunk = iTextSharp.text.Chunk;

namespace MWS.Reports
{
    public partial class RepInventoryPDFView : Form
    {
        public DateTime dateStart;
        public DateTime dateEnd;
        public int branchId;
        public RepInventoryPDFView(DateTime startDate, DateTime endDate, int _branchId)
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

                var fileName = "InventoryReport" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                var currentUser = from d in db.MstUsers where d.Id == Convert.ToInt32(Modules.SysCurrentModule.GetCurrentSettings().CurrentUserId) select d;

                var systemCurrent = Modules.SysCurrentModule.GetCurrentSettings();

                Document document = new Document(PageSize.LETTER.Rotate());
                document.SetMargins(30f, 30f, 100f, 30f);

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
                pdfWriter.PageEvent = new ConfigureHeaderFooter(dateStart, dateEnd);

                document.Open();

                Controllers.RepInventoryController repInventoryController = new Controllers.RepInventoryController();
                var commissary1 = repInventoryController.Commissary1List(dateStart, dateEnd);
                var commissary1Cut = repInventoryController.Commissary1CutList(dateStart, dateEnd);
                var commissary2 = repInventoryController.Commissary2List(dateStart, dateEnd);

                PdfPTable tableCommissary1 = new PdfPTable(7);
                tableCommissary1.SetWidths(new float[] { 150f, 100f, 100f, 100f, 100f, 100f, 100f });
                tableCommissary1.WidthPercentage = 100;

                PdfPTable tableCommissary1Cut = new PdfPTable(7);
                tableCommissary1Cut.SetWidths(new float[] { 150f, 100f, 100f, 100f, 100f, 100f, 100f });
                tableCommissary1Cut.WidthPercentage = 100;

                PdfPTable tableCommissary2 = new PdfPTable(7);
                tableCommissary2.SetWidths(new float[] { 150f, 100f, 100f, 100f, 100f, 100f, 100f });
                tableCommissary2.WidthPercentage = 100;

                if (branchId == 0)
                {
                    tableCommissary1.AddCell(new PdfPCell(new Phrase("COMMISSARY 1", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 7, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });
                    if (commissary1.Any())
                    {
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("ITEM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("MINIS", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("XS", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("SMALL", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("MEDIUM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("LARGE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("X-LARGE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        //tableCommissary1.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 7, PaddingLeft = 0, PaddingRight = 0, PaddingTop = 0, PaddingBottom = 0 });

                        foreach (var item in commissary1)
                        {
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(item.ItemDescription, fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Minis.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(item.ExtraSmall.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Small.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Medium.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Large.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase(item.ExtraLarge.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                        }
                        document.Add(tableCommissary1);
                    }

                    if (commissary1Cut.Any())
                    {
                        tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 7, PaddingLeft = 0, PaddingRight = 0, PaddingTop = 3f, PaddingBottom = 0 });
                        tableCommissary1Cut.AddCell(new PdfPCell(new Phrase("ITEM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary1Cut.AddCell(new PdfPCell(new Phrase("CUT(S)", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        //tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 2, PaddingLeft = 0, PaddingRight = 0, PaddingTop = 0, PaddingBottom = 0 });

                        foreach (var item in commissary1Cut)
                        {
                            tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(item.ItemDescription, fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(item.Weight.ToString("#,##0.00"), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                        }
                        document.Add(tableCommissary1Cut);
                    }

                    tableCommissary2.AddCell(new PdfPCell(new Phrase("COMMISSARY 2", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 7, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 30f, PaddingBottom = 10f, HorizontalAlignment = 0 });
                    if (commissary2.Any())
                    {
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("ITEM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("MINIS", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("XS", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("SMALL", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("MEDIUM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("LARGE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("X-LARGE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                        //tableCommissary2.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 7, PaddingLeft = 0, PaddingRight = 0, PaddingTop = 0, PaddingBottom = 0 });

                        foreach (var item in commissary2)
                        {
                            tableCommissary2.AddCell(new PdfPCell(new Phrase(item.ItemDescription, fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Minis.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase(item.ExtraSmall.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Small.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Medium.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Large.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase(item.ExtraLarge.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                        }
                        document.Add(tableCommissary2);
                    }
                }
                else
                {
                    if (branchId == 1)
                    {
                        tableCommissary1.AddCell(new PdfPCell(new Phrase("COMMISSARY 1", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 7, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });
                        if (commissary1.Any())
                        {
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("ITEM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("MINIS", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("XS", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("SMALL", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("MEDIUM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("LARGE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1.AddCell(new PdfPCell(new Phrase("X-LARGE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            //tableCommissary1.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 7, PaddingLeft = 0, PaddingRight = 0, PaddingTop = 0, PaddingBottom = 0 });

                            foreach (var item in commissary1)
                            {
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.ItemDescription, fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Minis.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.ExtraSmall.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Small.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Medium.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.Large.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary1.AddCell(new PdfPCell(new Phrase(item.ExtraLarge.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            }
                            document.Add(tableCommissary1);
                        }

                        if (commissary1Cut.Any())
                        {
                            tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 7, PaddingLeft = 0, PaddingRight = 0, PaddingTop = 30f, PaddingBottom = 0 });
                            tableCommissary1Cut.AddCell(new PdfPCell(new Phrase("ITEM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1Cut.AddCell(new PdfPCell(new Phrase("CUT(S)", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 30f, PaddingBottom = 5f, HorizontalAlignment = 1 });

                            foreach (var item in commissary1Cut)
                            {
                                tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(item.ItemDescription, fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(item.Weight.ToString("#,##0.00"), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary1Cut.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 5, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            }
                            document.Add(tableCommissary1Cut);
                        }
                    }
                    else
                    {
                        tableCommissary2.AddCell(new PdfPCell(new Phrase("COMMISSARY 2", fontTimesNewRoman12Bold)) { Border = 0, Colspan = 7, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 10f, HorizontalAlignment = 0 });
                        if (commissary2.Any())
                        {
                            tableCommissary2.AddCell(new PdfPCell(new Phrase("ITEM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase("MINIS", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase("XS", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase("SMALL", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase("MEDIUM", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase("LARGE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            tableCommissary2.AddCell(new PdfPCell(new Phrase("X-LARGE", fontTimesNewRoman10Bold)) { PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 5f, HorizontalAlignment = 1 });
                            //tableCommissary2.AddCell(new PdfPCell(new Phrase(" ", fontTimesNewRoman10Bold)) { Border = 0, Colspan = 7, PaddingLeft = 0, PaddingRight = 0, PaddingTop = 0, PaddingBottom = 0 });

                            foreach (var item in commissary2)
                            {
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(item.ItemDescription, fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Minis.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(item.ExtraSmall.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Small.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Medium.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(item.Large.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                                tableCommissary2.AddCell(new PdfPCell(new Phrase(item.ExtraLarge.ToString(), fontTimesNewRoman12Bold)) { Border = 0, PaddingLeft = 3f, PaddingRight = 3f, PaddingTop = 3f, PaddingBottom = 0f });
                            }
                            document.Add(tableCommissary2);
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
                String documentTitle = "Inventory Report";

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
