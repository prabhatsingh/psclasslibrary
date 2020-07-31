using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PsUtilities
{
    public class ImageUtilities
    {
        public Image GetOptimizedImage(string inputfile, int finalsize = 1024, float hdpi = 0, float vdpi = 0)
        {
            ISupportedImageFormat format = new JpegFormat();
            Size size = new Size(finalsize, 0);

            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {

                using (Image sourceImg = imageFactory.Load(inputfile).Resize(size).Format(format).Image)
                {
                    var clonedImg = new Bitmap(sourceImg.Width, sourceImg.Height, PixelFormat.Format32bppArgb);

                    clonedImg.SetResolution(hdpi == 0 ? Image.FromFile(inputfile).HorizontalResolution : hdpi, vdpi == 0 ? Image.FromFile(inputfile).VerticalResolution : vdpi);

                    using (var copy = Graphics.FromImage(clonedImg))
                    {
                        copy.DrawImage(sourceImg, 0, 0);
                    }

                    return clonedImg;
                }
            }
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

        public Image Rotate(string inputfile, float rotation)
        {
            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true).Resolution((int)Image.FromFile(inputfile).HorizontalResolution, (int)Image.FromFile(inputfile).VerticalResolution))
            {
                using (Image sourceImg = imageFactory.Load(inputfile).RotateBounded(rotation, true).Image)
                {
                    var clonedImg = new Bitmap(sourceImg.Width, sourceImg.Height, PixelFormat.Format32bppArgb);
                    clonedImg.SetResolution(Image.FromFile(inputfile).HorizontalResolution, Image.FromFile(inputfile).VerticalResolution);

                    using (var copy = Graphics.FromImage(clonedImg))
                    {
                        copy.DrawImage(sourceImg, 0, 0);
                    }

                    return clonedImg;
                }
            }
        }

        public string RotateHere(string inputfile, float rotation, string outputpath = "")
        {
            if (string.IsNullOrEmpty(outputpath))
                outputpath = Path.GetDirectoryName(inputfile) + "\\" + Path.GetFileNameWithoutExtension(inputfile) + "_Rotated" + Path.GetExtension(inputfile);

            Rotate(inputfile, rotation).Save(outputpath);
            return outputpath;
        }

        public string RotateToTemp(string inputfile, float rotation, string outputpath = "")
        {
            if (string.IsNullOrEmpty(outputpath))
                outputpath = Path.GetTempPath() + Path.GetFileNameWithoutExtension(inputfile) + "_Rotated" + Path.GetExtension(inputfile);

            Rotate(inputfile, rotation).Save(outputpath);
            return outputpath;
        }
    }
}
