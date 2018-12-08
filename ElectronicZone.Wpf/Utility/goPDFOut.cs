using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace ElectronicZone.Wpf.Utility
{
    /// <summary>
    /// Generate Invoice PDF
    /// Ref : http://www.aspmemo.net/2014/01/how-to-generate-invoice-form-as-pdf.html
    /// </summary>
    public class goPDFOut
    {
        #region Class Variables
        private string _orderNo, _orderDate, _filePath, _logoPath, _custName, _custAddress, _companyName, _companyAddress, _watermarkText, _itemTableFooterText, _invoiceFooterNote;
        decimal totalAmtStr = 0;
        List<Invoice> invoiceList = new List<Invoice>();  
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="saleIdArr"></param>
        public goPDFOut(int [] saleIdArr)
        {
            #region File & Logo Section
            string fileName = string.Format("{1}-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now, "Invoice");
            string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _filePath = Path.Combine(strPath, string.Format("{0}.pdf", fileName));
            _logoPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\Resources\\logo.png";
            // App Settings
            this._watermarkText = ConfigurationManager.AppSettings["InvoiceWatermarkText"];
            this._itemTableFooterText = ConfigurationManager.AppSettings["InvoiceItemTableFooterText"];
            this._invoiceFooterNote = ConfigurationManager.AppSettings["InvoiceFooterNote"];
            this._companyName = ConfigurationManager.AppSettings["InvoiceCompanyName"];
            this._companyAddress = ConfigurationManager.AppSettings["InvoiceCompanyAddress"];
            #endregion

            DataTable dtSaleInvoice;
            var saleIds = string.Join(",", saleIdArr);
            using (DataAccess da = new DataAccess()) {
                dtSaleInvoice = da.GetSaleInvoices(saleIds);
            }

            for (int i = 0; i < dtSaleInvoice.Rows.Count; ++i)
            {
                _orderDate = Convert.ToDateTime(dtSaleInvoice.Rows[i]["SaleDate"]).ToString(ConfigurationManager.AppSettings["InvoiceDateDisplayFormat"]);
                _orderNo = dtSaleInvoice.Rows[i]["InvoiceNumber"].ToString();
                _custName = dtSaleInvoice.Rows[i]["SaleTo"].ToString();
                _custAddress = dtSaleInvoice.Rows[i]["SaleAddress"].ToString();

                invoiceList.Add(new Invoice() {
                    SrNo = $"{i + 1}",
                    Item = $"{dtSaleInvoice.Rows[i]["Product"].ToString()}-{dtSaleInvoice.Rows[i]["ProductCode"].ToString()}",
                    Price = dtSaleInvoice.Rows[i]["SalePrice"].ToString(),
                    Quantity = dtSaleInvoice.Rows[i]["Quantity"].ToString(),
                    Total = dtSaleInvoice.Rows[i]["Total"].ToString()
                });
                totalAmtStr += Convert.ToDecimal(dtSaleInvoice.Rows[i]["Total"]);
            }
        }

        /// <summary>
        /// Generate PDF using iTextSharp
        /// </summary>
        public void GeneratePDF()
        {
            using (Document document = new Document(PageSize.A4, 70, 70, 70, 70))
            {
                using (PdfWriter docWriter = PdfWriter.GetInstance(document, new FileStream(_filePath, FileMode.Create)))
                {
                    // First, create our fonts
                    var titleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);
                    var boldTableFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
                    var _bodyFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                    Rectangle pageSize = docWriter.PageSize;

                    #region Watermark
                    if (!string.IsNullOrEmpty(_watermarkText))
                    {
                        PdfWriterEvents writerEvent = new PdfWriterEvents(_watermarkText);
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
                    PdfPCell nextPostCell1 = new PdfPCell(new Phrase(_companyName, _bodyFont));
                    nextPostCell1.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    nested.AddCell(nextPostCell1);
                    PdfPCell nextPostCell2 = new PdfPCell(new Phrase(_companyAddress, _bodyFont));
                    nextPostCell2.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    nested.AddCell(nextPostCell2);
                    //PdfPCell nextPostCell3 = new PdfPCell(new Phrase("Nonthaburi 11120", _bodyFont));
                    //nextPostCell3.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    //nested.AddCell(nextPostCell3);
                    PdfPCell nesthousing = new PdfPCell(nested);
                    nesthousing.Rowspan = 4;
                    nesthousing.Padding = 0f;
                    headertable.AddCell(nesthousing);

                    headertable.AddCell("");
                    PdfPCell invoiceCell = new PdfPCell(new Phrase("INVOICE", titleFont));
                    invoiceCell.HorizontalAlignment = 2;
                    invoiceCell.Border = Rectangle.NO_BORDER;
                    headertable.AddCell(invoiceCell);

                    PdfPCell noCell = new PdfPCell(new Phrase("No :", _bodyFont));
                    noCell.HorizontalAlignment = 2;
                    noCell.Border = Rectangle.NO_BORDER;
                    headertable.AddCell(noCell);
                    headertable.AddCell(new Phrase(_orderNo, _bodyFont));
                    PdfPCell dateCell = new PdfPCell(new Phrase("Date :", _bodyFont));
                    dateCell.HorizontalAlignment = 2;
                    dateCell.Border = Rectangle.NO_BORDER;
                    headertable.AddCell(dateCell);
                    headertable.AddCell(new Phrase(_orderDate, _bodyFont));
                    PdfPCell billCell = new PdfPCell(new Phrase("Billing To :", _bodyFont));
                    billCell.HorizontalAlignment = 2;
                    billCell.Border = Rectangle.NO_BORDER;
                    headertable.AddCell(billCell);
                    headertable.AddCell(new Phrase(_custName + "\n" + _custAddress, _bodyFont));
                    document.Add(headertable);
                    #endregion

                    #region Items Table
                    //Create body table
                    PdfPTable itemTable = new PdfPTable(5);
                    itemTable.HorizontalAlignment = 0;
                    itemTable.WidthPercentage = 100;
                    itemTable.SetWidths(new float[] { 10, 40, 15, 15, 20 });  // then set the column's __relative__ widths
                    itemTable.SpacingAfter = 40;
                    itemTable.DefaultCell.Border = Rectangle.BOX;
                    PdfPCell cell1 = new PdfPCell(new Phrase("NO", boldTableFont));
                    cell1.HorizontalAlignment = 1;
                    itemTable.AddCell(cell1);
                    PdfPCell cell2 = new PdfPCell(new Phrase("ITEM", boldTableFont));
                    cell2.HorizontalAlignment = 1;
                    itemTable.AddCell(cell2);
                    PdfPCell cell3 = new PdfPCell(new Phrase("PRICE", boldTableFont));
                    cell3.HorizontalAlignment = 1;
                    itemTable.AddCell(cell3);
                    PdfPCell cell4 = new PdfPCell(new Phrase("QUANTITY", boldTableFont));
                    cell4.HorizontalAlignment = 1;
                    itemTable.AddCell(cell4);
                    PdfPCell cell5 = new PdfPCell(new Phrase("TOTAL", boldTableFont));
                    cell5.HorizontalAlignment = 1;
                    itemTable.AddCell(cell5);

                    // foreach (DataRow row in dt.Rows) //invoiceList
                    foreach (Invoice invoice in invoiceList) {
                        PdfPCell numberCell = new PdfPCell(new Phrase(invoice.SrNo, _bodyFont));
                        numberCell.HorizontalAlignment = 0;
                        numberCell.PaddingLeft = 10f;
                        numberCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(numberCell);

                        PdfPCell descCell = new PdfPCell(new Phrase(invoice.Item, _bodyFont));
                        descCell.HorizontalAlignment = 0;
                        descCell.PaddingLeft = 10f;
                        descCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(descCell);

                        PdfPCell PriceCell = new PdfPCell(new Phrase(invoice.Price, _bodyFont));
                        PriceCell.HorizontalAlignment = 1;
                        PriceCell.PaddingLeft = 10f;
                        PriceCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(PriceCell);

                        PdfPCell qtyCell = new PdfPCell(new Phrase(invoice.Quantity, _bodyFont));
                        qtyCell.HorizontalAlignment = 0;
                        qtyCell.PaddingLeft = 10f;
                        qtyCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(qtyCell);

                        PdfPCell amtCell = new PdfPCell(new Phrase(invoice.Total, _bodyFont));
                        amtCell.HorizontalAlignment = 1;
                        amtCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        itemTable.AddCell(amtCell);

                    }

                    // Table footer
                    PdfPCell totalAmtCell1 = new PdfPCell(new Phrase(""));
                    totalAmtCell1.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                    itemTable.AddCell(totalAmtCell1);
                    PdfPCell totalAmtCell2 = new PdfPCell(new Phrase(""));
                    totalAmtCell2.Border = Rectangle.TOP_BORDER; //Rectangle.NO_BORDER;
                    itemTable.AddCell(totalAmtCell2);
                    itemTable.AddCell(totalAmtCell2);

                    PdfPCell totalAmtStrCell = new PdfPCell(new Phrase("Gross", boldTableFont));
                    totalAmtStrCell.Border = Rectangle.TOP_BORDER;   //Rectangle.NO_BORDER;
                    totalAmtStrCell.HorizontalAlignment = 1;
                    itemTable.AddCell(totalAmtStrCell);

                    PdfPCell totalAmtCell = new PdfPCell(new Phrase(totalAmtStr.ToString(ConfigurationManager.AppSettings["AmountDisplayPattern"]), boldTableFont));
                    totalAmtCell.HorizontalAlignment = 1;
                    itemTable.AddCell(totalAmtCell);

                    PdfPCell cell = new PdfPCell(new Phrase(_itemTableFooterText, _bodyFont));
                    cell.Colspan = 5;
                    cell.HorizontalAlignment = 1;
                    itemTable.AddCell(cell);
                    document.Add(itemTable);
                    #endregion

                    //Image Singature
                    //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(_logoPath);// Server.MapPath("~/Images/Bill_Gates2.png"));
                    //logo.SetAbsolutePosition(pageSize.GetLeft(300), 140);
                    //document.Add(logo);

                    PdfContentByte cb = new PdfContentByte(docWriter);
                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                    cb = docWriter.DirectContent;
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 10);
                    cb.SetTextMatrix(pageSize.GetLeft(70), 100);
                    cb.ShowText(_invoiceFooterNote);
                    cb.EndText();

                    document.Close();
                }
            }
            MessageBox.Show($"Invoice Exported to {System.Environment.SpecialFolder.Desktop.ToString()} Successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.None);
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
