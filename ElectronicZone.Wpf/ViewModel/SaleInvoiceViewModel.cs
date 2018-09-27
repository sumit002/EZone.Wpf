using ElectronicZone.Wpf.Helper;
using System;
using ElectronicZone.Wpf.Utility;
using System.IO;
using System.Windows.Input;

namespace ElectronicZone.Wpf.ViewModel
{
    public class SaleInvoiceViewModel : ViewModelBase
    {
        ILogger logger = new Logger(typeof(LoginWindow));
        // Commands
        public ICommand GenerateInvoiceCmd { get; set; }


        public SaleInvoiceViewModel()
        {
            this.GenerateInvoiceCmd = new CommandHandler(OnGeneratePdf, CanExecuteGeneratePdf);
        }

        private bool CanExecuteGeneratePdf(object arg)
        {
            // throw new NotImplementedException();
            return true;
        }

        public void OnGeneratePdf(object obj) {
            try
            {
                string fileName = string.Format("{1}-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now, "Invoice");
                string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(strPath, string.Format("{0}.pdf", fileName));
                string logoPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).Replace("\\bin\\Debug", "")
                 + "\\logo\\logo.png";

                goPDFOut invoice = new goPDFOut();
                invoice.GeneratePDF(filePath, logoPath, "[ PAID ]");
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }
    }
}
