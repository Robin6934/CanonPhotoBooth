using System.Text.Json;
using System;
using System.IO;

namespace PhotoBooth
{
	public class ConfigLoader
	{
		public string BaseDirectory { get; set; }
		public int MaxRetryAttempts { get; set; }
		public int CountDown { get; set; }
		public string TextOnPicture { get; set; }
		public string TextOnPictureFont { get; set; }
		public int TextOnPictureFontSize { get; set; }
		public string TextOnPictureColor { get; set; }
		public int TextPositionFromRight { get; set; }
		public int TextPositionFromBottom { get; set; }






		public static ConfigLoader LoadFromJsonFile(string filePath)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					MainWindow mainWindow = new MainWindow();
					mainWindow.ReportError($"Error reading JSON file: {filePath} does not exist",false);
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
