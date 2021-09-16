using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PsUtilities.BaseClasses
{
    public class FileBase
    {
        // The cryptographic service provider.
        private static SHA256 Sha256 = SHA256.Create();
        private static MD5 Md5 = MD5.Create();

        // Compute the file's hash.
        public static byte[] GetHashSha256(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return Sha256.ComputeHash(stream);
            }
        }

        public static byte[] GetHashMd5(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return Md5.ComputeHash(stream);
            }
        }
    }
}
