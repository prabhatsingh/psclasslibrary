using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XpsLibrary;
using Image = System.Drawing.Image;

namespace PsUtilities
{
    public class PdfUtilities
    {
        public bool IsXFA(string inputfile)
        {
            return new XfaForm(new PdfReader(inputfile)).XfaPresent;
        }

        public void PrintPdfToXps(string inputfilename)
        {
            string acropath = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\AcroRd32.exe").GetValue("").ToString();
            var printer = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList().Find(f => f.Contains("XPS"));
            var process = System.Diagnostics.Process.Start(acropath, string.Format("/N /T \"{0}\" \"{1}\"", inputfilename, printer));
            process.WaitForExit();
        }

        public string MergePdfTemp(List<string> inputfiles)
        {
            inputfiles.Sort();
            string outputpath = Path.GetTempPath() + Path.GetFileNameWithoutExtension(inputfiles.First()) + ".pdf";

            Document doc = null;
            PdfSmartCopy pdf = null;
            PdfReader.unethicalreading = true;

            try
            {
                var stream = new FileStream(outputpath, FileMode.Create);

                doc = new Document();
                pdf = new PdfSmartCopy(doc, stream);

                doc.Open();

                foreach (string file in inputfiles)
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

            return outputpath;
        }

        public string MergePdfHere(List<string> inputfiles)
        {
            string outputpath = Path.GetDirectoryName(inputfiles.First()) + "\\" + Path.GetFileNameWithoutExtension(inputfiles.First()) + "_merged.pdf";

            File.Copy(MergePdfTemp(inputfiles), outputpath, true);

            return outputpath;
        }

        public string ImageToPdfTemp(List<string> inputfiles)
        {
            string outputpath = Path.GetTempPath() + Path.GetFileNameWithoutExtension(inputfiles.First()) + ".pdf";

            Document doc = null;
            FileStream fs = null;
            PdfWriter writer = null;

            try
            {
                doc = new Document();
                fs = new FileStream(outputpath, FileMode.Create, FileAccess.Write, FileShare.None);
                writer = PdfWriter.GetInstance(doc, fs);

                doc.Open();

                inputfiles.Sort();
                foreach (var imgf in inputfiles)
                {
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imgf);
                    img.ScaleToFit(PageSize.A4.Width, PageSize.A4.Height);
                    img.SetAbsolutePosition((PageSize.A4.Width - img.ScaledWidth) / 2, (PageSize.A4.Height - img.ScaledHeight) / 2);

                    doc.NewPage();
                    writer.DirectContent.AddImage(img);
                }

                inputfiles.ForEach(i => File.Delete(i));
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

        public string ImageToPdfHere(List<string> inputfiles, string outputpath = "")
        {
            if (string.IsNullOrEmpty(outputpath))
                outputpath = Path.GetDirectoryName(inputfiles.First()) + "\\" + Path.GetFileNameWithoutExtension(inputfiles.First()) + "_itp.pdf";

            File.Copy(ImageToPdfTemp(inputfiles), outputpath, true);

            return outputpath;
        }

        public List<Image> PdfToImage(string inputfilename)
        {
            if (IsXFA(inputfilename))
                inputfilename = FlattenPdf(inputfilename, isTemp: true);

            var imagefiles = new GhostScriptHelper().PdfToImage(inputfilename);
            List<Image> optimizedimages = new List<Image>();
            var imageutil = new ImageUtilities();

            imagefiles.ForEach(f => optimizedimages.Add(imageutil.GetOptimizedImage(f)));

            return optimizedimages;
        }

        public List<string> PdfToImageFilesTemp(string inputfilename)
        {
            if (IsXFA(inputfilename))
                inputfilename = FlattenPdf(inputfilename, isTemp: true);

            var imagefiles = new GhostScriptHelper().PdfToImage(inputfilename);
            List<string> optimizedimagefiles = new List<string>();
            var imageutil = new ImageUtilities();

            imagefiles.ForEach(f => optimizedimagefiles.Add(imageutil.OptimizeToTemp(f)));

            return optimizedimagefiles;
        }

        public List<string> PdfToImageFilesHere(string inputfilename)
        {
            var images = PdfToImageFilesTemp(inputfilename);
            var outputdirectory = Path.GetDirectoryName(inputfilename);
            var outputfilename = Path.GetFileNameWithoutExtension(inputfilename) + "_Image_{0}" + Path.GetExtension(images.First());

            int imagecount = 1;
            List<string> outimages = new List<string>();
            images.ForEach(f =>
            {
                outimages.Add(outputdirectory + "\\" + string.Format(outputfilename, imagecount));
                File.Copy(f, outputdirectory + "\\" + string.Format(outputfilename, imagecount++), true);
            });

            return outimages;
        }

        public string FlattenPdf(string inputfile, bool isTemp = false)
        {
            var directoryname = (isTemp ? Path.GetTempPath() : Path.GetDirectoryName(inputfile));

            PrintPdfToXps(inputfile);

            //don't need xps when flatting pdf, expected output is pdf, so sending xps to temp
            if (File.Exists(Path.GetTempPath() + "\\to.xps"))
                File.Delete(Path.GetTempPath() + "\\to.xps");
            File.Move(Path.GetDirectoryName(inputfile) + "\\to.xps", Path.GetTempPath() + "\\to.xps");
            var xpspath = Path.GetTempPath() + "\\to.xps";

            var imagefiles = new XpsUtilities().XpsToImage(xpspath, true);

            var pdfoutputpath = directoryname + "\\" + Path.GetFileNameWithoutExtension(inputfile) + "_FLAT.pdf";
            return ImageToPdfHere(imagefiles, pdfoutputpath);
        }
    }
}
