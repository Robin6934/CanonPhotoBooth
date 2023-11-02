using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PhotoBooth
{
    internal class ImageDownsampler
    {
        public static async Task DownsampleImageAspectRatioAsync(string sourcePath, string destinationPath, double divisionFactor)
        {
            await Task.Run(() =>
            {
                using (var originalImage = Image.FromFile(sourcePath))
                {
                    int newWidth = (int)(originalImage.Width / divisionFactor);
                    int newHeight = (int)(originalImage.Height / divisionFactor);

                    using (var resizedImage = new Bitmap(newWidth, newHeight))
                    {
                        using (var graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;

                            graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);

                            resizedImage.Save(destinationPath);
                        }
                    }
                }
            });
        }

    }
}
