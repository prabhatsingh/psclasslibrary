using PsUtilities;
using PsUtilities.Utilities;
using System;
using System.IO;

namespace TestClassLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\pnppps\Downloads\Kiran_Passport.pdf";

            var result = new PdfUtilities().OptimizePdf(path);

            //File.Move(result, @"C:\SourceCode\UtilityTools\RoughPage\Kiran_Passport_Optimized.pdf");
        }

        public void searchpdf()
        {
            string path = @"C:\SourceCode\UtilityTools\RoughPage\GroupAgreement_PlanSummaryReport_115056 UAT EPS.pdf";
            //new PdfUtilities().PdfToImageFilesHere(path);
            //new PdfUtilities().ExtractImageAsFiles(path, true);

            int rowcount = 0;
            string column1 = "";
            string column2 = "";
            foreach (var result in new PdfUtilities().SearchPdf(@"\\gwlanfs4\Griddata\LI Test\Load Status Reports\uat\", "System Exception at Pas Service Layer"))
            {
                if (rowcount == 0)
                {
                    column1 = "─".PadRight(result.filename.Length + 8, '─');
                    column2 = "─".PadRight(result.pagenumber.ToString().Length + 8, '─');

                    Console.WriteLine("┌" + column1 + "┬" + column2 + "┐");
                }

                string output = "│" + result.filename.PadRight(column1.Length, ' ') + "│" + result.pagenumber.ToString().PadLeft(column2.Length, ' ') + "│";

                Console.WriteLine(output);

                Console.WriteLine("├" + column1 + "┼" + column2 + "┤");
                rowcount++;
            }

            Console.WriteLine("└" + column1 + "┴" + column2 + "┘");

            Console.Read();
        }
    }
}
