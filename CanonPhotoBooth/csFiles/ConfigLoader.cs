using System.Text.Json;
using System;
using System.IO;
using System.Windows;

namespace PhotoBooth
{
	public class ConfigLoader
	{
		private FileSystemWatcher _watcher = new FileSystemWatcher();
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

		/*
        private void SettingsChangesHandler(object sender, FileSystemEventArgs e)
        {
            ReadJson(e.FullPath);
        }

        private void ReadJson(string jsonFilePath)
        {
            Application.Current.Dispatcher.Invoke(() => ((MainWindow)Application.Current.MainWindow).config = LoadFromJsonFile(jsonFilePath, (MainWindow)Application.Current.MainWindow));
        }

        public void InitJsonReader(string jsonFilePath)
        {

            string absolutePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), jsonFilePath);

            string FileName = System.IO.Path.GetFileName(absolutePath);

            absolutePath = System.IO.Path.GetDirectoryName(absolutePath);

            ReadJson(jsonFilePath);

            _watcher.Path = absolutePath;

            _watcher.Filter = FileName;

            _watcher.Changed += SettingsChangesHandler;

            _watcher.NotifyFilter = NotifyFilters.LastWrite;

            _watcher.EnableRaisingEvents = true;
        }
		*/
    }
}
