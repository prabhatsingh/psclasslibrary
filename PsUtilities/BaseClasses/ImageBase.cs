using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using System.Drawing.Imaging;

namespace PsUtilities.BaseClasses
{
    public class ImageBase
    {
        public Image Optimize(string inputfile, int finalsize = 1024, float hdpi = 0, float vdpi = 0)
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

        public Image OptimizeImageObject(Image inputfile, int finalsize = 1024, float hdpi = 0, float vdpi = 0)
        {
            ISupportedImageFormat format = new JpegFormat();
            Size size = new Size(finalsize, 0);

            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {
                using (Image sourceImg = imageFactory.Load(inputfile).Resize(size).Format(format).Image)
                {
                    var clonedImg = new Bitmap(sourceImg.Width, sourceImg.Height, PixelFormat.Format32bppArgb);

                    clonedImg.SetResolution(hdpi == 0 ? inputfile.HorizontalResolution : hdpi, vdpi == 0 ? inputfile.VerticalResolution : vdpi);

                    using (var copy = Graphics.FromImage(clonedImg))
                    {
                        copy.DrawImage(sourceImg, 0, 0);
                    }

                    return clonedImg;
                }
            }
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
    }
}
