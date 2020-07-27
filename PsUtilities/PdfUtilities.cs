using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsUtilities
{
    public class PdfUtilities
    {
        public static void PrintPdfToXps(string inputfilename)
        {
            string acropath = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\AcroRd32.exe").GetValue("").ToString();
            var printer = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList().Find(f => f.Contains("XPS"));
            System.Diagnostics.Process.Start(acropath, string.Format("/p /h /t \"{0}\" \"{1}\"", inputfilename, printer));
        }
    }
}
