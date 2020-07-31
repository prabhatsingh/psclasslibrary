using PsUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClassLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"\\gwusrfsp2\pnppps$\Documents\Personal\SpouseVisa\IMM5707  New - Family Info Form May 2018.pdf";
            //new PdfUtilities().PdfToImageFilesHere(path);
            new PdfUtilities().FlattenPdf(path);
            Console.Read();
        }
    }
}
