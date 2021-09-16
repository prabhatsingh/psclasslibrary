using PsUtilities.BaseClasses;
using PsUtilities.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace PsUtilities.Utilities
{
    public class FileUtilities : FileBase
    {
        public static List<string> filestodelete = new List<string>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static bool IsFileLocked(string filepath)
        {
            try
            {
                if (!File.Exists(filepath))
                    return false;

                FileInfo file = new FileInfo(filepath);

                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is still being written to
                //or being processed by another thread
                return true;
            }

            //file is not locked
            return false;
        }

        // The cryptographic service provider.
        private static readonly SHA256 sha256 = SHA256.Create();
        private static readonly MD5 md5 = MD5.Create();

        // Compute the file's hash.
        public static string GetHashSha256String(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return sha256.ComputeHash(stream).ToPlainString();
            }
        }

        public static string GetHashMd5String(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return md5.ComputeHash(stream).ToPlainString();
            }
        }

        public class FileDetails
        {
            public FileDetails(string f)
            {
                Filepath = f;
            }

            public string Filename
            {
                get
                {
                    return Path.GetFileNameWithoutExtension(Filepath);
                }
            }

            public string Filepath { get; set; }

            public string Extension
            {
                get
                {
                    return Path.GetExtension(Filepath);
                }
            }

            public FileType FType
            {
                get
                {
                    if (Extension.IsImage()) return FileType.IMAGE;
                    if (Extension.IsPdf()) return FileType.PDF;
                    if (Extension.IsXps()) return FileType.XPS;
                    return FileType.UNSUPPORTED;
                }
            }
        }
    }
}
