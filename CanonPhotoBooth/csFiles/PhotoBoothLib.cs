using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using EOSDigital.API;
using EOSDigital.SDK;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using System.Printing.IndexedProperties;
using System.ComponentModel;

namespace PhotoBooth
{
	internal class PhotoBoothLib
	{
		string userName = Environment.UserName;
		string basePath = "";
		public ConfigLoader configLoader { get; set; }  

		public async void doPhotoboxThings(string CurrentPicturePath, PictureOptions Option)
		{
			string fileName = Path.GetFileName(CurrentPicturePath);
			string destinationPath = "";

			char[] charArray = fileName.ToCharArray();

            basePath = configLoader.BaseDirectory;

			if (Option == PictureOptions.Save)
			{
				// Copy picture to directory
				string sourcePath = CurrentPicturePath;
				destinationPath = $"{basePath}\\Photos\\{fileName}";
				AddTextToImage(sourcePath, destinationPath);

                //movePictureTo(sourcePath, destinationPath);
            }
            else if(Option == PictureOptions.Print)
			{
				// Copy picture to directory
				string sourcePath = CurrentPicturePath;
				destinationPath = $"{basePath}\\Photos\\{fileName}";
				AddTextToImage(sourcePath, destinationPath);

                await PrintImageAsync(destinationPath);
			}
			else if(Option == PictureOptions.Delete)
			{
				// Delete picture

				string sourcePath = CurrentPicturePath;
				destinationPath = $"{basePath}\\Deleted\\{fileName}";
				CopyPictureTo(sourcePath, destinationPath);
				return;
				
			}

			CopyPictureTo(destinationPath, $"{basePath}\\Static\\{fileName}");

			await ImageDownsampler.DownsampleImageAspectRatioAsync(destinationPath, $"{basePath}\\Static\\downscaled{fileName}",4);

        }



		public static async Task PrintImageAsync(string imagePath)
		{
			await Task.Run(() =>
			{
				Bitmap image = new Bitmap(imagePath);
				PrintDocument pd = new PrintDocument();
				pd.PrintPage += (sender, e) =>
				{
					float width = e.PageSettings.PrintableArea.Width;
					float height = e.PageSettings.PrintableArea.Height;
					e.Graphics.DrawImage(image, 0, 0, height, width);

				};

				pd.Print();
			});
		}

		public static void AddTextForPreview(string ImagePath, string SavePath, ConfigLoader configLoader)
		{
			PhotoBoothLib photoBoothLib = new PhotoBoothLib();
			photoBoothLib.configLoader = configLoader;
			photoBoothLib.AddTextToImage(ImagePath, SavePath);
		}

		public void AddTextToImage(string ImagePath,string SavePath)
		{
			// Load the image
			Bitmap bitmap = new Bitmap(ImagePath);
			Graphics graphics = Graphics.FromImage(bitmap);

			// Define text color
			ColorConverter colorConverter = new ColorConverter();
			Color color = (Color)colorConverter.ConvertFromString(configLoader.TextOnPictureColor);
			Brush brush = new SolidBrush(color);

			// Define text font
			Font font = new Font(configLoader.TextOnPictureFont, configLoader.TextOnPictureFontSize, FontStyle.Regular);

			// Text to display
			string text = configLoader.TextOnPicture;

			// Measure the size of the text
			SizeF textSize = graphics.MeasureString(text, font);

			// Define rectangle based on text size
			float x = bitmap.Width - textSize.Width - configLoader.TextPositionFromRight;
			float y = bitmap.Height - textSize.Height - configLoader.TextPositionFromBottom;
			RectangleF rectangle = new RectangleF(new PointF(x, y), textSize);

			// Draw text on image
			graphics.DrawString(text, font, brush, rectangle);

			// Save the output file
			bitmap.Save(SavePath);
		}


		public static void CreateFilePaths(string basePath)
		{
			string[] paths = { "Photos", "Deleted", "Temp" , "ShowTemp", "Static" };
			foreach (string path in paths)
			{
				string FullPath = basePath + "\\" + path;

				if (!Directory.Exists(FullPath))
				{
					Directory.CreateDirectory(FullPath);
				}
			}
		}

		public void CopyPictureTo(string sourcePath, string destinationPath)
		{
			//WaitForFileToUnlock(sourcePath, TimeSpan.FromSeconds(10));
			File.Copy(sourcePath, destinationPath);
		}

		public enum PictureOptions: Int32
		{
			Save = 1,
			Print = 2,
			Delete = 3
		}


		//public void SaveFocusInfo(FocusInfo focusInfo)
		//{
		//	string filePath = @"C:\Users\Robin\Documents\File.json"; // Update with your desired file path

		//	try
		//	{
		//		// Serialize the 'focusInfo' object to JSON with formatting
		//		string json = JsonConvert.SerializeObject(focusInfo, Formatting.Indented);

		//		// Write the JSON string to the specified file
		//		File.WriteAllText(filePath, json);

		//		Console.WriteLine("FocusInfo has been saved to JSON file.");
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine($"An error occurred: {ex.Message}");
		//	}
		//}



		public static bool IsFileLocked(string filePath)
		{
			try
			{
				using (FileStream stream = File.Open(filePath, FileMode.Open, System.IO.FileAccess.Read, FileShare.None))
				{
					stream.Close();
				}
			}
			catch (IOException)
			{
				return true; // File is locked
			}

			return false; // File is not locked
		}

		public static void WaitForFileToUnlock(string filePath, TimeSpan timeout)
		{
			DateTime startTime = DateTime.Now;

			while (IsFileLocked(filePath))
			{
				if (DateTime.Now - startTime >= timeout)
				{
					throw new TimeoutException("Timeout waiting for the file to be unlocked.");
				}

				// Sleep for a short interval before checking again
				Thread.Sleep(100);
			}
		}

	}
}
