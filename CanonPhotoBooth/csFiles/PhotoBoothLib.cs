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

		private static readonly Lazy<PhotoBoothLib> _instance = new Lazy<PhotoBoothLib> (() => new PhotoBoothLib());

		string userName = Environment.UserName;

		string basePath = "";

		private ConfigLoader? _configLoader;

		private MainWindow? mainWindow;

		public PhotoBoothLib() 
		{
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
			this.mainWindow = 
			(MainWindow)System.Windows.Application.Current.MainWindow);

			_configLoader = ConfigLoader.Instance;
        }

        public static PhotoBoothLib Instance { get { return _instance.Value; } }

        /// <summary>
        /// Can either Save, Print then Save or Delete the Picture
        /// </summary>
        /// <param name="CurrentPicturePath">The Path of the Picture to be used</param>
        /// <param name="Option">Enum to Specify what to do with the picture</param>
        public async void doPhotoboxThings(string CurrentPicturePath, PictureOptions Option)
		{
			string fileName = Path.GetFileName(CurrentPicturePath);
			string destinationPath = "";

            basePath = mainWindow.dir;

	
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


        /// <summary>
        /// Sends the picture to the Standart Printer specified in the print settings
        /// of your computer
        /// </summary>
        /// <param name="imagePath">Path of the picture to be printet</param>
        /// <returns></returns>
        public static async Task PrintImageAsync(string imagePath)
		{
			await Task.Run(() =>
			{
				imagePath = imagePath.Replace("downscaled", "");
				Bitmap image = new Bitmap(imagePath);
				using (PrintDocument pd = new PrintDocument())
				{
                    pd.PrintPage += (sender, e) =>
                    {
                        float width = e.PageSettings.PrintableArea.Width;
                        float height = e.PageSettings.PrintableArea.Height;
                        e.Graphics.DrawImage(image, 0, 0, height, width);
                    };
                    pd.Print();
                }
			});
		}

		/// <summary>
		/// Adds te
		/// </summary>
		/// <param name="ImagePath"></param>
		/// <param name="SavePath"></param>
		/// <param name="configLoader"></param>
		public static void AddTextForPreview(string ImagePath, string SavePath, ConfigLoader configLoader)
		{
			PhotoBoothLib photoBoothLib = Instance;
			photoBoothLib._configLoader = configLoader;
			photoBoothLib.AddTextToImage(ImagePath, SavePath);
		}

		/// <summary>
		/// Adds the text to the image like specified in the config
		/// </summary>
		/// <param name="ImagePath">Path to the image to which the text should be added</param>
		/// <param name="SavePath">Path to save the image with added text to</param>
		public void AddTextToImage(string ImagePath,string SavePath)
		{
			// Load the image
			Bitmap bitmap = new Bitmap(ImagePath);
			Graphics graphics = Graphics.FromImage(bitmap);

			// Define text color
			ColorConverter colorConverter = new ColorConverter();
			Color color = (Color)colorConverter.ConvertFromString(_configLoader.textOnPictureColor);
			Brush brush = new SolidBrush(color);

			// Define text font
			Font font = new Font(_configLoader.textOnPictureFont, _configLoader.textOnPictureFontSize, FontStyle.Regular);

			// Text to display
			string text = _configLoader.textOnPicture;

			// Measure the size of the text
			SizeF textSize = graphics.MeasureString(text, font);

			// Define rectangle based on text size
			float x = bitmap.Width - textSize.Width - _configLoader.textPositionFromRight;
			float y = bitmap.Height - textSize.Height - _configLoader.textPositionFromBottom;
			RectangleF rectangle = new RectangleF(new PointF(x, y), textSize);

			// Draw text on image
			graphics.DrawString(text, font, brush, rectangle);

			// Save the output file
			bitmap.Save(SavePath);
		}

        /// <summary>
        /// Checks if all the needed directories exist else creates them
        /// </summary>
        /// <param name="basePath">the base path where to create the paths in</param>
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

		public static void CopyPictureTo(string sourcePath, string destinationPath)
		{
			//WaitForFileToUnlock(sourcePath, TimeSpan.FromSeconds(10));
			File.Copy(sourcePath, destinationPath, true);
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
