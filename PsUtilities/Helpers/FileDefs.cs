using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsUtilities.Helpers
{
    public enum FileType { PDF, IMAGE, XPS, COMBINED, UNSUPPORTED }

    public static class FileDefs
    {
        public static bool IsImage(this string extension)
        {
            return new string[] { "jpeg", "png", "jpg", "bmp", "tiff" }.Contains(extension.Trim(new char[] { '.', ' ' }).ToLowerInvariant());
        }

        public static bool IsPdf(this string extension)
        {
            return new string[] { "pdf" }.Contains(extension.Trim(new char[] { '.', ' ' }).ToLowerInvariant());
        }

        public static bool IsXps(this string extension)
        {
            return new string[] { "xps", "oxps" }.Contains(extension.Trim(new char[] { '.', ' ' }).ToLowerInvariant());
        }
    }
}
