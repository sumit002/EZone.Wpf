using System;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using ElectronicZone.Wpf.DataAccessLayer;
//http://www.aspmemo.net/2014/01/how-to-generate-invoice-form-as-pdf.html

namespace ElectronicZone.Wpf.Utility
{
    public class goPDFOut
    {
        private string _filePath;
        //Class Variables
        string orderNo = DateTime.Now.Ticks.ToString().Substring(0, 6);
        string orderDate = DateTime.Now.ToString("dd MMM yyyy");
        string _custAddress = "#113, Sharada Nilaya, 2nd Floor, J P Nagar 6th Phase, Karnataka";
        string _custName = "Sumit Kumar Das";
        decimal totalAmtStr = 200;
        string accountNo = "0123456789012";
        string accountName = "Sumit Das";
        string branch = "Phahon Yothin Branch";
        string bank = "Kasikorn Bank";

        // for Gridview
        DataTable dt = new DataTable();

        public goPDFOut()
        {

            DataAccess da = new DataAccess();
            DataTable dtSaleInvoice = da.GetSaleInvoice("139");

            dt.Columns.Add("NO", Type.GetType("System.String"));
            dt.Columns.Add("ITEM", Type.GetType("System.String"));
            dt.Columns.Add("QUANTITY", Type.GetType("System.String"));
            dt.Columns.Add("AMOUNT", Type.GetType("System.String"));

            for (int i = 0; i < 10; ++i)
            {
                dt.Rows.Add();
                dt.Rows[i]["NO"] = (i + 1).ToString();
                dt.Rows[i]["ITEM"] = "Item " + i.ToString();
                dt.Rows[i]["QUANTITY"] = (i + 1).ToString();
                dt.Rows[i]["AMOUNT"] = (i + 1).ToString();
                totalAmtStr += (i + 1);
            }
        }

        /*private void button_Click(object sender, RoutedEventArgs e)
        {
            string fileName = string.Format("{1}-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now, "Invoice");
            string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(strPath, string.Format("{0}.pdf", fileName));
            string logoPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).Replace("\\bin\\Debug", "")
             + "\\logo\\logo.png";

            GeneratePDF(filePath, logoPath, "[ PAID ]");
        }*/

        public void GeneratePDF(string filePath, string logoPath, string watermarkText = null)
        {
            // using iTextSharp
            using (Document document = new Document(PageSize.A4, 70, 70, 70, 70))
            {
                using (PdfWriter docWriter = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                {
                    // First, create our fonts
                    var titleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
                    var boldTableFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
                    var bodyFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                    Rectangle pageSize = docWriter.PageSize;

                    #region Watermark
                    if (!string.IsNullOrEmpty(watermarkText))
                    {
                        PdfWriterEvents writerEvent = new PdfWriterEvents(watermarkText);
                        docWriter.PageEvent = writerEvent;
                    }
                    #endregion

                    // Open the Document for writing
                    document.Open();

                    //Add elements to the document here

                    #region Top table
                    // Create the header table 
                    PdfPTable headertable = new PdfPTable(3);
                    headertable.HorizontalAlignment = 0;
                    headertable.WidthPercentage = 100;
                    headertable.SetWidths(new float[] { 4, 2, 4 });  // then set the column's __relative__ widths
                    headertable.DefaultCell.Border = Rectangle.NO_BORDER;
                    //headertable.DefaultCell.Border = Rectangle.BOX; //for testing
                    headertable.SpacingAfter = 30;
                    PdfPTable nested = new PdfPTable(1);
                    nested.DefaultCell.Border = Rectangle.BOX;
                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase("ABC Co.,Ltd", bodyFont));
                    nextPostCell1.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    nested.AddCell(nextPostCell1);
                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase("111/206 Moo 9, Ramkhamheang Road,", bodyFont));
                    nextPostCell2.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    nested.AddCell(nextPostCell2);
                    PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Nonthaburi 11120", bodyFont));
                    nextPostCell3.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    nested.AddCell(nextPostCell3);
                    PdfPCell nesthousing = new PdfPCell(nested);
                    nesthousing.Rowspan = 4;
                    nesthousing.Padding = 0f;
                    headertable.AddCell(nesthousing);

                    headertable.AddCell("");
                    PdfPCell invoiceCell = new PdfPCell(new Phrase("INVOICE", titleFont));
                    invoiceCell.HorizontalAlignment = 2;
                    invoiceCell.Border = Rectangle.NO_BORDER;
                    headertable.AddCell(invoiceCell);
                    PdfPCell noCell = new PdfPCell(new Phrase("No :", bodyFont));
                    noCell.HorizontalAlignment = 2;
                    noCell.Border = Rectangle.NO_BORDER;
                    headertable.AddCell(noCell);
                    headertable.AddCell(new Phrase(orderNo, bodyFont));
                    PdfPCell dateCell = new PdfPCell(new Phrase("Date :", bodyFont));
                    dateCell.HorizontalAlignment = 2;
                    dateCell.Border = Rectangle.NO_BORDER;
                    headertable.AddCell(dateCell);
                    headertable.AddCell(new Phrase(orderDate, bodyFont));
                    PdfPCell billCell = new PdfPCell(new Phrase("Bill To :", bodyFont));
                    billCell.HorizontalAlignment = 2;
                    billCell.Border = Rectangle.NO_BORDER;
                    headertable.AddCell(billCell);
                    headertable.AddCell(new Phrase(_custName + "\n" + _custAddress, bodyFont));
                    document.Add(headertable);
                    #endregion

                    #region Items Table
                    //Create body table
                    PdfPTable itemTable = new PdfPTable(4);
                    itemTable.HorizontalAlignment = 0;
                    itemTable.WidthPercentage = 100;
                    itemTable.SetWidths(new float[] { 10, 40, 20, 30 });  // then set the column's __relative__ widths
                    itemTable.SpacingAfter = 40;
                    itemTable.DefaultCell.Border = Rectangle.BOX;
                    PdfPCell cell1 = new PdfPCell(new Phrase("NO", boldTableFont));
                    cell1.HorizontalAlignment = 1;
                    itemTable.AddCell(cell1);
                    PdfPCell cell2 = new PdfPCell(new Phrase("ITEM", boldTableFont));
                    cell2.HorizontalAlignment = 1;
                    itemTable.AddCell(cell2);
                    PdfPCell cell3 = new PdfPCell(new Phrase("QUANTITY", boldTableFont));
                    cell3.HorizontalAlignment = 1;
                    itemTable.AddCell(cell3);
                    PdfPCell cell4 = new PdfPCell(new Phrase("AMOUNT(USD)", boldTableFont));
                    cell4.HorizontalAlignment = 1;
                    itemTable.AddCell(cell4);

                    foreach (DataRow row in dt.Rows)
                    {
                        PdfPCell numberCell = new PdfPCell(new Phrase(row["NO"].ToString(), bodyFont));
                        numberCell.HorizontalAlignment = 0;
                        numberCell.PaddingLeft = 10f;
                        numberCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(numberCell);

                        PdfPCell descCell = new PdfPCell(new Phrase(row["ITEM"].ToString(), bodyFont));
                        descCell.HorizontalAlignment = 0;
                        descCell.PaddingLeft = 10f;
                        descCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(descCell);

                        PdfPCell qtyCell = new PdfPCell(new Phrase(row["QUANTITY"].ToString(), bodyFont));
                        qtyCell.HorizontalAlignment = 0;
                        qtyCell.PaddingLeft = 10f;
                        qtyCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(qtyCell);

                        PdfPCell amtCell = new PdfPCell(new Phrase(row["AMOUNT"].ToString(), bodyFont));
                        amtCell.HorizontalAlignment = 1;
                        amtCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(amtCell);

                    }
                    // Table footer
                    PdfPCell totalAmtCell1 = new PdfPCell(new Phrase(""));
                    totalAmtCell1.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                    itemTable.AddCell(totalAmtCell1);
                    PdfPCell totalAmtCell2 = new PdfPCell(new Phrase(""));
                    totalAmtCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                    itemTable.AddCell(totalAmtCell2);
                    PdfPCell totalAmtStrCell = new PdfPCell(new Phrase("Total Amount", boldTableFont));
                    totalAmtStrCell.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER; //Rectangle.TOP_BORDER;
                    totalAmtStrCell.HorizontalAlignment = 1;
                    itemTable.AddCell(totalAmtStrCell);
                    PdfPCell totalAmtCell = new PdfPCell(new Phrase(totalAmtStr.ToString("#,###.00"), boldTableFont));
                    totalAmtCell.HorizontalAlignment = 1;
                    itemTable.AddCell(totalAmtCell);

                    PdfPCell cell = new PdfPCell(new Phrase("*** Please note that ABC Co., Ltd’s bank account is USD Bank Account ***", bodyFont));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = 1;
                    itemTable.AddCell(cell);
                    document.Add(itemTable);
                    #endregion

                    ////Approved by
                    //PdfContentByte cb = new PdfContentByte(docWriter);
                    //BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                    //cb = docWriter.DirectContent;
                    //cb.BeginText();
                    //cb.SetFontAndSize(bf, 10);
                    //cb.SetTextMatrix(pageSize.GetLeft(300), 200);
                    //cb.ShowText("Approved by,");
                    //cb.EndText();
                    ////Image Singature
                    //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);// Server.MapPath("~/Images/Bill_Gates2.png"));
                    //logo.SetAbsolutePosition(pageSize.GetLeft(300), 140);
                    //document.Add(logo);

                    PdfContentByte cb = new PdfContentByte(docWriter);
                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                    cb = docWriter.DirectContent;
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 10);
                    cb.SetTextMatrix(pageSize.GetLeft(70), 100);
                    cb.ShowText("Thank you for your business! If you have any questions about your order, please contact us at 800-555-NORTH.");
                    cb.EndText();
                    // document.Close();
                }

            }
        }
    }

    public class PdfWriterEvents : IPdfPageEvent
    {
        string watermarkText = string.Empty;

        public PdfWriterEvents(string watermark)
        {
            watermarkText = watermark;
        }

        public void OnOpenDocument(PdfWriter writer, Document document) { }
        public void OnCloseDocument(PdfWriter writer, Document document) { }
        public void OnStartPage(PdfWriter writer, Document document)
        {
            float fontSize = 80;
            float xPosition = 300;
            float yPosition = 400;
            float angle = 45;
            try
            {
                PdfContentByte under = writer.DirectContentUnder;
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                under.BeginText();
                under.SetColorFill(BaseColor.LIGHT_GRAY);
                under.SetFontAndSize(baseFont, fontSize);
                under.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, xPosition, yPosition, angle);
                under.EndText();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
        public void OnEndPage(PdfWriter writer, Document document) { }
        public void OnParagraph(PdfWriter writer, Document document, float paragraphPosition) { }
        public void OnParagraphEnd(PdfWriter writer, Document document, float paragraphPosition) { }
        public void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title) { }
        public void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition) { }
        public void OnSection(PdfWriter writer, Document document, float paragraphPosition, int depth, Paragraph title) { }
        public void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition) { }
        public void OnGenericTag(PdfWriter writer, Document document, Rectangle rect, String text) { }
    }
}
