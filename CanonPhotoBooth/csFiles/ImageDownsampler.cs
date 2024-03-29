﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PhotoBooth
{
    internal class ImageDownsampler
    {
        /// <summary>
        /// Takes the specified image and downsamples it with the specified division factor
        /// </summary>
        /// <param name="sourcePath">Path of the image to be downscaled</param>
        /// <param name="destinationPath">Path to save the downscaled image to</param>
        /// <param name="divisionFactor">Factor by which the old images resolution is divided by</param>
        /// <returns>Task of the downscaler</returns>
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
