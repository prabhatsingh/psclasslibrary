using System.Drawing;

namespace PsUtilities.Helpers
{
    public class Parameters
    {
        public ImageType ImageType { get; set; }
        public ImageOptions ImageOptions { get; set; }
        public Size? RequiredSize { get; set; }
        public int Dpi { get; set; }
    }
}