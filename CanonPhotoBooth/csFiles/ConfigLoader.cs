using System.Text.Json;
using System;
using System.IO;
using System.Windows;

namespace PhotoBooth
{
	public class ConfigLoader
	{
		public int countDown { get; set; }
		public string? textOnPicture { get; set; }
		public string? textOnPictureFont { get; set; }
		public int textOnPictureFontSize { get; set; }
		public string? textOnPictureColor { get; set; }
		public int textPositionFromRight { get; set; }
		public int textPositionFromBottom { get; set; }

		public static ConfigLoader LoadFromJsonFile(string filePath, MainWindow mainWindow)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					mainWindow.ReportError($"Error reading JSON file: {filePath} does not exist");
					return null;
				}
				// Read the JSON file using System.Text.Json
				string json = File.ReadAllText(filePath);

				// Deserialize the JSON string into an instance of ConfigLoader
				return JsonSerializer.Deserialize<ConfigLoader>(json);
			}
			catch (Exception ex)
			{
				// Handle any exceptions, such as file not found or invalid JSON format
				Console.WriteLine($"Error reading JSON file: {ex.Message}");
				return null;
			}
		}
    }
}
