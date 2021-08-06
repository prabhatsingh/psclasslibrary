using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PsUtilities.Helpers;
using PsUtilities.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Path = System.IO.Path;

namespace PsUtilities.BaseClasses
{
    public class PdfBase
    {
        public bool IsXFA(string inputfile)
        {
            return new XfaForm(new PdfReader(inputfile)).XfaPresent;
        }

        public string Merge(List<string> pdffiles)
        {
            pdffiles.Sort();
            string mergedpdf = Path.GetTempPath() + "\\pdfmerged" + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss") + ".pdf";

            Document doc = null;
            PdfSmartCopy pdf = null;
            PdfReader.unethicalreading = true;

            try
            {
                var stream = new FileStream(mergedpdf, FileMode.Create);

                doc = new Document();
                pdf = new PdfSmartCopy(doc, stream);

                doc.Open();

                foreach (string file in pdffiles)
                {
                    pdf.AddDocument(new PdfReader(file));
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                pdf?.Dispose();
                doc?.Dispose();
            }

            return mergedpdf;
        }

        public List<string> Split(string pdffile)
        {
            List<string> splittedpdf = new List<string>();

            using (PdfReader reader = new PdfReader(pdffile))
            {
                int pagecount = reader.NumberOfPages;

                if (pagecount == 1)
                {
                    return new List<string>() { pdffile };
                }

                var outputpath = "{0}_Page_{1}";

                for (int i = 1; i <= pagecount; i++)
                {
                    string outFile = string.Format(outputpath, Path.GetTempPath(), i);

                    FileStream stream = new FileStream(outFile, FileMode.Create);

                    Document doc = new Document();
                    PdfCopy pdf = new PdfCopy(doc, stream);

                    doc.Open();
                    PdfImportedPage page = pdf.GetImportedPage(reader, i);
                    pdf.AddPage(page);

                    pdf.Dispose();
                    doc.Dispose();

                    splittedpdf.Add(outFile);
                }
            }

            return splittedpdf;
        }

        public string Rotate(string inputfile, float desiredRot)
        {
            string rotatedpdf = Path.GetTempPath() + "\\pdfrotated" + "_" + desiredRot.ToString() + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss") + ".pdf";

            FileStream outStream = new FileStream(rotatedpdf, FileMode.Create);

            PdfReader reader = new PdfReader(inputfile);
            PdfStamper stamper = new PdfStamper(reader, outStream);

            int pageCount = reader.NumberOfPages;

            for (int n = 1; n <= pageCount; n++)
            {
                PdfDictionary pageDict = reader.GetPageN(n);

                PdfNumber rotation = pageDict.GetAsNumber(PdfName.ROTATE);
                float rotationrequired = desiredRot;
                if (rotation != null)
                {
                    rotationrequired += rotation.IntValue;
                    rotationrequired %= 360; // must be 0, 90, 180, or 270
                }
                pageDict.Put(PdfName.ROTATE, new PdfNumber(rotationrequired));
            }

            stamper.Close();
            reader.Close();

            return rotatedpdf;
        }

        public string PrintToPdf(List<string> imagefiles)
        {
            string outputpath = Path.GetTempPath() + "\\pdffile" + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss") + ".pdf";

            Document doc = null;
            FileStream fs = null;
            PdfWriter writer = null;

            try
            {
                doc = new Document();
                fs = new FileStream(outputpath, FileMode.Create, FileAccess.Write, FileShare.None);
                writer = PdfWriter.GetInstance(doc, fs);

                doc.Open();

                imagefiles.Sort();
                foreach (var imgf in imagefiles)
                {
                    Image img = iTextSharp.text.Image.GetInstance(imgf);
                    img.ScaleToFit(PageSize.A4.Width, PageSize.A4.Height);
                    img.SetAbsolutePosition((PageSize.A4.Width - img.ScaledWidth) / 2, (PageSize.A4.Height - img.ScaledHeight) / 2);

                    doc.NewPage();
                    writer.DirectContent.AddImage(img);
                }

                imagefiles.ForEach(i => File.Delete(i));
            }
            catch (Exception)
            {

            }
            finally
            {
                doc?.Dispose();
                fs?.Dispose();
                writer?.Dispose();
            }

            return outputpath;
        }

        public string FlattenPdf(string pdffile)
        {
            string acropath = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\AcroRd32.exe").GetValue("").ToString();
            var printer = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList().Find(f => f.Contains("XPS"));
            var process = System.Diagnostics.Process.Start(acropath, string.Format("/N /T \"{0}\" \"{1}\"", pdffile, printer));
            process.WaitForExit();

            if (File.Exists(Path.GetTempPath() + "\\tx.xps"))
                File.Delete(Path.GetTempPath() + "\\tx.xps");

            File.Move(Path.GetDirectoryName(pdffile) + "\\tx.xps", Path.GetTempPath() + "\\tx.xps");

            var xpspath = Path.GetTempPath() + "\\tx.xps";

            var imagefiles = new XpsUtilities().XpsToImage(xpspath, true);

            File.Delete(xpspath);

            return PrintToPdf(imagefiles);
        }

        public List<System.Drawing.Image> ExtractImage(string pdffile)
        {
            var imagefiles = new GhostScriptHelper().PdfToImage(pdffile);

            List<System.Drawing.Image> optimizedimages = new List<System.Drawing.Image>();

            var imageutil = new ImageUtilities();

            imagefiles.ForEach(f =>
            {
                optimizedimages.Add(imageutil.GetOptimizedImage(f));
                FileUtilities.filestodelete.Add(f);
            });

            return optimizedimages;
        }

        public IEnumerable<SearchResult> NextSearchResult(string[] files, string searchText)
        {
            foreach (string pdffile in files)
            {
                PdfReader pdfReader = null;
                int numpages = 0;
                try
                {
                    pdfReader = new PdfReader(pdffile);
                    numpages = pdfReader.NumberOfPages;
                }
                catch (InvalidPdfException)
                {
                    continue;
                }

                for (int page = 1; page <= numpages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                    string currentPageText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    string[] textlines = currentPageText.Split('\n');
                    if (currentPageText.Contains(searchText))
                    {
                        yield return new SearchResult { filename = pdffile, pagenumber = page, results = textlines.Where(f => f.Contains(searchText)).ToArray() };
                    }
                }

                pdfReader.Close();
            }
        }

        public string ImageToPdf(List<System.Drawing.Image> images)
        {
            string outputpath = Path.GetTempPath() + "\\pdffile" + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss") + ".pdf";

            Document doc = null;
            FileStream fs = null;
            PdfWriter writer = null;

            try
            {
                doc = new Document();
                fs = new FileStream(outputpath, FileMode.Create, FileAccess.Write, FileShare.None);
                writer = PdfWriter.GetInstance(doc, fs);

                doc.Open();

                foreach (var imgf in images)
                {
                    Image img = Image.GetInstance(imgf, System.Drawing.Imaging.ImageFormat.Jpeg);
                    img.ScaleToFit(PageSize.A4.Width, PageSize.A4.Height);
                    img.SetAbsolutePosition((PageSize.A4.Width - img.ScaledWidth) / 2, (PageSize.A4.Height - img.ScaledHeight) / 2);

                    doc.NewPage();
                    writer.DirectContent.AddImage(img);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                doc?.Dispose();
                fs?.Dispose();
                writer?.Dispose();
            }
            return outputpath;
        }

        public class SearchResult
        {
            public string filename;
            public FileInfo Fileinfo => new FileInfo(filename);
            public int pagenumber;
            public string[] results;
        }
    }
}