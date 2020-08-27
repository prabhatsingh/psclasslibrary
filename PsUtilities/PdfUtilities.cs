using PsUtilities.BaseClasses;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Image = System.Drawing.Image;

namespace PsUtilities
{
    public class PdfUtilities : PdfBase
    {
        public string FlattenPdf(string pdffilepath, bool isTemp = false)
        {
            string tempflattenedpdf = base.FlattenPdf(pdffilepath);
            string flattenedpdf = Path.GetDirectoryName(pdffilepath) + "\\" + Path.GetFileNameWithoutExtension(pdffilepath) + "_flat.pdf";

            if (isTemp)
                flattenedpdf = tempflattenedpdf;
            else
                File.Move(tempflattenedpdf, flattenedpdf);

            return flattenedpdf;
        }

        public List<Image> ExtractImageObject(string inputfilename)
        {
            if (IsXFA(inputfilename))
                inputfilename = FlattenPdf(inputfilename, isTemp: true);

            return ExtractImage(inputfilename);
        }

        public List<string> ExtractImageAsFiles(string inputfilename, bool isTemp = false)
        {
            if (IsXFA(inputfilename))
                inputfilename = FlattenPdf(inputfilename, isTemp: true);

            List<Image> images = ExtractImage(inputfilename);
            List<string> imagefiles = new List<string>();
            string imagefilepathformat = (isTemp ? Path.GetTempPath() : Path.GetDirectoryName(inputfilename)) + "\\" + Path.GetFileNameWithoutExtension(inputfilename) + "_image_{0}.jpg";

            int count = 1;

            images.ForEach(img =>
            {
                imagefiles.Add(string.Format(imagefilepathformat, count));
                img.Save(string.Format(imagefilepathformat, count++));
            });

            return imagefiles;
        }

        public void PrintPdfToXps(string inputfilename)
        {
            string acropath = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\AcroRd32.exe").GetValue("").ToString();
            var printer = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList().Find(f => f.Contains("XPS"));
            var process = System.Diagnostics.Process.Start(acropath, string.Format("/N /T \"{0}\" \"{1}\"", inputfilename, printer));
            process.WaitForExit();
        }

        public string MergeToPdf(List<string> inputfiles, bool isTemp = false)
        {
            inputfiles.ForEach(pdffile =>
            {
                if (IsXFA(pdffile))
                    pdffile = FlattenPdf(pdffile, isTemp: true);
            });

            string tempmergedpdfpath = base.Merge(inputfiles);

            if (isTemp)
                return tempmergedpdfpath;

            string outputpath = Path.GetDirectoryName(inputfiles.First()) + "\\" + Path.GetFileNameWithoutExtension(inputfiles.First()) + "_merged.pdf";

            if (!FileUtilities.IsFileLocked(outputpath))
            {
                File.Delete(outputpath);
                File.Move(tempmergedpdfpath, outputpath);
            }

            return outputpath;
        }

        public string RotatePdf(string inputfile, float rotation, bool isTemp = false)
        {
            if (IsXFA(inputfile))
                inputfile = FlattenPdf(inputfile, isTemp: true);

            string temprotatedpdfpath = base.Rotate(inputfile, rotation);

            if (isTemp)
                return temprotatedpdfpath;

            string outputpath = Path.GetDirectoryName(inputfile) + "\\" + Path.GetFileNameWithoutExtension(inputfile) + "_rotated_" + rotation.ToString() + ".pdf";

            if (!FileUtilities.IsFileLocked(outputpath))
            {
                File.Delete(outputpath);
                File.Move(temprotatedpdfpath, outputpath);
            }

            return outputpath;
        }

        public string PrintToPdf(List<string> inputfiles, bool isTemp = false)
        {
            string temppdfpath = base.PrintToPdf(inputfiles);

            if (isTemp)
                return temppdfpath;

            string outputpath = Path.GetDirectoryName(inputfiles.First()) + "\\" + Path.GetFileNameWithoutExtension(inputfiles.First()) + "_printed.pdf";

            if (!FileUtilities.IsFileLocked(outputpath))
            {
                File.Delete(outputpath);
                File.Move(temppdfpath, outputpath);
            }

            return outputpath;
        }

        public List<string> Split(string inputfile, bool isTemp = false)
        {
            if (IsXFA(inputfile))
                inputfile = FlattenPdf(inputfile, isTemp: true);

            var tempsplitfiles = base.Split(inputfile);
            List<string> splitfiles = new List<string>();

            if (isTemp)
                return tempsplitfiles;

            string outputpath = Path.GetDirectoryName(inputfile) + "\\" + Path.GetFileNameWithoutExtension(inputfile) + "_page_{0}.pdf";

            int count = 1;

            tempsplitfiles.ForEach(img =>
            {
                splitfiles.Add(string.Format(outputpath, count));
                File.Move(img, string.Format(outputpath, count++));
            });

            return splitfiles;
        }

        public IEnumerable<SearchResult> SearchPdf(string directory, string searchtext)
        {
            string[] pdffiles = Directory.GetFiles(directory, "*.pdf", SearchOption.AllDirectories);

            foreach (SearchResult searchresult in base.NextSearchResult(pdffiles, searchtext))
            {
                yield return searchresult;
            }
        }
    }
}
