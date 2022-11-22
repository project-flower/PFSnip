using System.Drawing.Imaging;
using System.Text;

namespace PFSnip
{
    internal class ImageType
    {
        #region Internal Fields

        internal static readonly ImageType[] All;
        internal const int DefaultIndex = 5;
        internal readonly string Description;
        internal readonly string[] Extensions;
        internal static readonly string Filter;
        internal readonly ImageFormat Format;

        #endregion

        #region Static Methods

        static ImageType()
        {
            All = new ImageType[]
            {
                new ImageType(ImageFormat.Bmp, "BMP Image", new string[]{ ".bmp", ".dib" })
                , new ImageType(ImageFormat.Jpeg, "JPEG Image", new string[]{ ".jpg", ".jpeg", ".jpe", ".jfif" })
                , new ImageType(ImageFormat.Gif, "GIF Image", new string[]{ ".gif" })
                , new ImageType(ImageFormat.Tiff, "TIFF Image", new string[]{ ".tif", ".tiff" })
                , new ImageType(ImageFormat.Png, "PNG Image", new string[]{ ".png" })
            };

            var filter = new StringBuilder();

            foreach (ImageType type in All)
            {
                if (filter.Length > 0)
                {
                    filter.Append("|");
                }

                filter.Append($"{type.Description}|");
                var extensions = new StringBuilder();

                foreach (string extension in type.Extensions)
                {
                    if (extensions.Length > 0)
                    {
                        extensions.Append(";");
                    }

                    extensions.Append("*");
                    extensions.Append(extension);
                }

                filter.Append(extensions.ToString());
            }

            if (filter.Length > 0)
            {
                filter.Append("|");
            }

            filter.Append("All Files|*.*");
            Filter = filter.ToString();
        }

        #endregion

        #region Internal Methods

        internal static ImageFormat GetFormat(string extension)
        {
            foreach (ImageType imageType in All)
            {
                foreach (string extension_ in imageType.Extensions)
                {
                    if (string.Compare(extension_, extension, true) == 0)
                    {
                        return imageType.Format;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Private Methods

        private ImageType(ImageFormat format, string description, string[] extensions)
        {
            Description = description;
            Extensions = extensions;
            Format = format;
        }

        #endregion
    }
}
