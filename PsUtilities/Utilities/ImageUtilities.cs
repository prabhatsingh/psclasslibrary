using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using PsUtilities.BaseClasses;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PsUtilities.Utilities
{
    public class ImageUtilities : ImageBase
    {
        public Image GetOptimizedImage(string inputfile, int finalsize = 1024, float hdpi = 0, float vdpi = 0)
        {
            return Optimize(inputfile, finalsize, hdpi, vdpi);
        }

        public string OptimizeHere(string inputfile, int finalsize = 1024, float hdpi = 0, float vdpi = 0, string outputpath = "")
        {
            if (string.IsNullOrEmpty(outputpath))
                outputpath = Path.GetDirectoryName(inputfile) + "\\" + Path.GetFileNameWithoutExtension(inputfile) + "_Optimized" + Path.GetExtension(inputfile);

            GetOptimizedImage(inputfile, finalsize, hdpi, vdpi).Save(outputpath);
            return outputpath;
        }

        public string OptimizeToTemp(string inputfile, int finalsize = 1024, float hdpi = 0, float vdpi = 0, string outputpath = "")
        {
            if (string.IsNullOrEmpty(outputpath))
                outputpath = Path.GetTempPath() + Path.GetFileNameWithoutExtension(inputfile) + "_Optimized" + Path.GetExtension(inputfile);

            GetOptimizedImage(inputfile, finalsize, hdpi, vdpi).Save(outputpath);
            return outputpath;
        }

        public Image RotateToObject(string inputfile, float rotation)
        {
            return Rotate(inputfile, rotation);
        }

        public string RotateHere(string inputfile, float rotation, string outputpath = "")
        {
            if (string.IsNullOrEmpty(outputpath))
                outputpath = Path.GetDirectoryName(inputfile) + "\\" + Path.GetFileNameWithoutExtension(inputfile) + "_Rotated" + Path.GetExtension(inputfile);

            RotateToObject(inputfile, rotation).Save(outputpath);
            return outputpath;
        }

        public string RotateToTemp(string inputfile, float rotation, string outputpath = "")
        {
            if (string.IsNullOrEmpty(outputpath))
                outputpath = Path.GetTempPath() + Path.GetFileNameWithoutExtension(inputfile) + "_Rotated" + Path.GetExtension(inputfile);

            RotateToObject(inputfile, rotation).Save(outputpath);
            return outputpath;
        }
    }
}
