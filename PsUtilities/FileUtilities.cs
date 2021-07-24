using PsUtilities.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PsUtilities
{
    public class FileUtilities
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
