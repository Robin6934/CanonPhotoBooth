using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace PhotoBooth
{
    class BitmapImageLib
    {
        public static bool BitmapImageIsTooDark(BitmapImage bitmapImage)
        {
            // Step 1: Convert BitmapImage to Bitmap
            BitmapSource bitmapSource = bitmapImage;
            FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap(bitmapSource, PixelFormats.Bgr24, null, 0);

            // Step 2: Extract pixel data from the middle portion
            int width = convertedBitmap.PixelWidth;
            int height = convertedBitmap.PixelHeight;

            int startX = width / 2 - 20 / 2;   // Adjust as needed
            int startY = height / 2 - 20 / 2;  // Adjust as needed
            int cropWidth = 20; // Adjust as needed
            int cropHeight = 20; // Adjust as needed

            CroppedBitmap croppedBitmap = new CroppedBitmap(convertedBitmap, new Int32Rect(startX, startY, cropWidth, cropHeight));

            int stride = croppedBitmap.PixelWidth * (croppedBitmap.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[croppedBitmap.PixelHeight * stride];
            croppedBitmap.CopyPixels(pixels, stride, 0);

            // Step 3: Calculate the average color
            int totalRed = 0, totalGreen = 0, totalBlue = 0;
            int pixelCount = pixels.Length / 3; // 3 bytes per pixel (BGR)

            for (int i = 0; i < pixels.Length; i += 3)
            {
                totalBlue += pixels[i];
                totalGreen += pixels[i + 1];
                totalRed += pixels[i + 2];
            }

            byte averageRed = (byte)(totalRed / pixelCount);
            byte averageGreen = (byte)(totalGreen / pixelCount);
            byte averageBlue = (byte)(totalBlue / pixelCount);

            // Step 4: Determine whether the average color is too dark based on a threshold
            const int darknessThreshold = 128; // Adjust as needed

            // Step 5: Set the boolean value accordingly
            return averageRed < darknessThreshold && averageGreen < darknessThreshold && averageBlue < darknessThreshold;
        }

    }
}
