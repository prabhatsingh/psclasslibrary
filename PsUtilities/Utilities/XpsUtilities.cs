using PsUtilities.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PsUtilities.Utilities
{
    public class XpsUtilities
    {
        public List<string> XpsToImage(string inputfile, bool istemp = false)
        {
            var imagefiles = new List<string>();
            string outputpath = (istemp ? Path.GetTempPath() : Path.GetDirectoryName(inputfile)) + "\\" + "{0}_Image_{1}.png";

            using (var xpsConverter = new Xps2Image(inputfile))
            {
                var images = xpsConverter.ToBitmap(new Parameters
                {
                    ImageType = ImageType.Png,
                    Dpi = 300
                });

                int count = 0;
                images.ToList().ForEach(f =>
                {
                    var imgfilename = string.Format(outputpath, Path.GetFileNameWithoutExtension(inputfile), ++count);
                    imagefiles.Add(imgfilename);
                    f.Save(imgfilename);
                });
            }

            return imagefiles;
        }
    }
}
