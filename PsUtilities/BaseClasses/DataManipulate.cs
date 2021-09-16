using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsUtilities.BaseClasses
{
    public static class DataManipulate
    {
        // Return a byte array as a sequence of hex values.
        public static string ToPlainString(this byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }
    }
}
