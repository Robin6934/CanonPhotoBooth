using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using EOSDigital.API;
using EOSDigital.SDK;
using PhotoBooth;
using static PhotoBooth.PhotoBoothLib;
using System.Threading;
using PhotoBooth;
using System.Windows.Threading; 

namespace PhotoBooth
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		#region Variables

		CanonAPI APIHandler;
		Camera MainCamera;
		List<Camera> CamList;

		FocusInfo FocInfo;

		ImageBrush bgbrush = new ImageBrush();
		Action<BitmapImage> SetImageAction;

		int ErrCount;
		object ErrLock = new object();

		int PictureCount = 0;

		static string userName = Environment.UserName;
		string dir = $"C:\\Users\\{userName}\\Pictures\\Photobox";

		string CurrentPictureName = "";

		DispatcherTimer timer = new DispatcherTimer();
		int Timer = 0;
		int CountDown = 4;

		string jsonFilePath = "C:\\Users\\Robin\\Documents\\GitHub\\CanonPhotoBooth\\PhotoBooth\\Resources\\config.json";
		public ConfigLoader config { get; private set; }

		DispatcherTimer KeepAliveTimer = new DispatcherTimer();

		private FileSystemWatcher fileWatcher;


		#endregion

		public MainWindow()
		{
			try
			{
				InitializeComponent();

				ReadJson();

				CreateFilePaths(dir);
				APIHandler = new CanonAPI();
				APIHandler.CameraAdded += APIHandler_CameraAdded;
				ErrorHandler.SevereErrorHappened += ErrorHandler_SevereErrorHappened;
				ErrorHandler.NonSevereErrorHappened += ErrorHandler_NonSevereErrorHappened;
				SetImageAction = (BitmapImage img) => { bgbrush.ImageSource = img; };

				while (MainCamera == null)
				{
					RefreshCamera();
				}
				if (MainCamera == null) return;
				InitTimer();

				InitWindow();
				
				MainCamera.OpenSession();
				MainCamera.DownloadReady += MainCamera_DownloadReady;
				MainCamera.ProgressChanged += MainCamera_ProgressChanged;
				MainCamera.LiveViewUpdated += MainCamera_LiveViewUpdated;
				InitFilewatcher();

				MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Both);

				MainCamera.SetCapacity(4096, int.MaxValue);
				StartLV();


				SetCanvasSize();
				
			}
			catch (DllNotFoundException) { ReportError("Canon DLLs not found!", true); }
			catch (Exception ex) { ReportError(ex.Message, true); }
		}

		private void ReadJson()
		{
			config = ConfigLoader.LoadFromJsonFile(jsonFilePath);
			CountDown = config.CountDown;
			dir = config.BaseDirectory;
		}

		private void TakePicture()
		{
			try
			{

				//MainCamera.StopLiveView();

				MainCamera.SendCommand(CameraCommand.DoEvfAf, (int)EvfAFMode.Quick);

				//MainCamera.SendCommand(CameraCommand.DoEvfAf, (int)AFMode.OneShot);


				//CanonSDK.GetPropertyData(MainCamera.Reference, PropertyID.FocusInfo, 0,out FocInfo);

				//SaveFocusInfo(FocInfo);

				MainCamera.TakePhotoAsync();

				//MainCamera.StartLiveView();
			}
			catch (Exception ex) { ReportError(ex.Message, false); }
		}
		
		private void RefreshCamera()
		{
			int cnt = 0;
			bool CameraFound = false;
			while (!CameraFound)
			{
				try { CamList = APIHandler.GetCameraList(); }
				catch (Exception ex) { ReportError(ex.Message, false); }
				if (CamList.Count > 0)
				{
					try { MainCamera = CamList[0]; }
					catch (Exception ex) { ReportError(ex.Message, true); }
					CameraFound = true;
				}
				if(cnt>1000)
				{
					ReportError("No camera found", false);
					break;
				}
				cnt++;
			}

		}
		
		private void TakePictureButton_Click(object sender, RoutedEventArgs e)
		{
			TriggerPicture();
		}

		private void OpenSettingsButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// Check if the pressed key is the Spacebar
			if (e.Key == Key.Space)
			{
				TriggerPicture();
			}
		}		

		private void ShowImageViewer(string imagePath)
		{
			string imageName= System.IO.Path.GetFileName(imagePath);
			string TempPath = dir+"\\ShowTemp\\"+imageName;
			AddTextForPreview(imagePath, TempPath, config);
			//File.Copy(imagePath,TempPath , true);
			Dispatcher.Invoke((Action)(() =>
			{
				PhotoPrintPage imageViewerWindow = new PhotoPrintPage();
				imageViewerWindow.configLoader = config;
				imageViewerWindow.DisplayImage(TempPath);
				imageViewerWindow.Show();
			}));
		}

		#region Initialisations

		private void InitWindow()
		{
			this.WindowState = WindowState.Maximized;
			this.WindowStyle = WindowStyle.None;
			this.ResizeMode = ResizeMode.NoResize;
			// Subscribe to the PreviewKeyDown event
			this.PreviewKeyDown += MainWindow_PreviewKeyDown;

			// Set the window to focusable so that it can receive keyboard events
			this.Focusable = true;
			this.Focus();

			this.SizeChanged += MainWindow_SizeChanged;
			this.Closing += Window_Closing;
			this.Loaded += MainWindow_Loaded;
		}

		public void InitFilewatcher()
		{
			fileWatcher = new FileSystemWatcher();

			// Set the path to the directory you want to monitor
			fileWatcher.Path = dir + "\\Temp";

			// Subscribe to the "Created" event for files
			fileWatcher.Created += FileCreatedHandler;

			// Enable events for files
			fileWatcher.NotifyFilter = NotifyFilters.FileName;

			// Start monitoring
			fileWatcher.EnableRaisingEvents = true;

			// Clean up
			//fileWatcher.Dispose();
		}

		private void InitTimer()
		{
			timer.Tick += Timer_Tick;
			timer.Interval = TimeSpan.FromSeconds(1);

			KeepAliveTimer.Tick += KeepAliveTimer_Tick;
			KeepAliveTimer.Interval = TimeSpan.FromMinutes(1);
		}

		#endregion

		#region LiveView

		private void StartLV()
		{
			try
			{
				LVCanvas.Background = bgbrush;
				MainCamera.StartLiveView();
			}
			catch (Exception ex) { ReportError(ex.Message, false); }

		}

		private void MainCamera_LiveViewUpdated(Camera sender, Stream img)
		{
			try
			{
				using (WrapStream s = new WrapStream(img))
				{
					//TransformedBitmap mirroredBitmap = new TransformedBitmap();
					//mirroredBitmap.BeginInit();
					//ScaleTransform mirrorScale = new ScaleTransform(-1, 1);
					//mirroredBitmap.Transform = mirrorScale;

					img.Position = 0;
					BitmapImage EvfImage = new BitmapImage();
					EvfImage.BeginInit();
					EvfImage.StreamSource = s;
					EvfImage.CacheOption = BitmapCacheOption.OnLoad;
					EvfImage.EndInit();
					EvfImage.Freeze();
					try
					{
						Application.Current.Dispatcher.BeginInvoke(SetImageAction, EvfImage);
					}
					catch (System.NullReferenceException) { }
				}
			}
			catch (Exception ex) { ReportError(ex.Message, false); }
		}

		#endregion

		#region EventListeners

		private void Timer_Tick(object sender, EventArgs e)
		{

			if (Timer == 0)
			{
				TextBlockCountdown.Visibility = Visibility.Hidden;
				timer.Stop();
				TakePicture();
				TakePictureButton.Click += TakePictureButton_Click;
			}
			else
			{
				TextBlockCountdown.Text = Timer.ToString();
				Timer--;
			}
		}

		private void KeepAliveTimer_Tick(object sender, EventArgs e)
		{
			MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Both);
		}

		private void FileCreatedHandler(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType == WatcherChangeTypes.Created)
			{

				// A file was created, which could indicate a copy operation
				string filePath = e.FullPath;
				string fileName = e.Name;
				CurrentPictureName = fileName;
				WaitForFileToUnlock(filePath, TimeSpan.FromSeconds(10));

				ShowImageViewer(filePath);
			}
		}

		private void APIHandler_CameraAdded(CanonAPI sender)
		{
			try { Dispatcher.Invoke((Action)delegate { RefreshCamera(); }); }
			catch (Exception ex) { ReportError(ex.Message, false); }
		}

		private void MainCamera_ProgressChanged(object sender, int progress)
		{
			//try { if (progress >= 99) { ShowImageViewer(CurrentPictureName); } }
			//catch (Exception ex) { ReportError(ex.Message, false); }
		}

		private void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
		{
			try
			{
				//settings.SavePathTextBox.Dispatcher.Invoke((Action)delegate { dir = settings.SavePathTextBox.Text; });
				sender.DownloadFile(Info, dir + "\\Temp\\");
				//MainProgressBar.Dispatcher.Invoke((Action)delegate { MainProgressBar.Value = 0; });
			}
			catch (Exception ex) { ReportError(ex.Message, false); }
		}

		#endregion

		#region CameraInteractions
		private void TriggerPicture()
		{
			TextBlockCountdown.Visibility = Visibility.Visible;
			int CountDownTemp = CountDown + 1;
			TextBlockCountdown.Text = CountDownTemp.ToString();
			Timer = CountDown;
			TakePictureButton.Click -= TakePictureButton_Click;
			timer.Start();
		}

		#endregion

		#region ErrorHandling

		public void ReportError(string message, bool lockdown)
		{
			int errc;
			lock (ErrLock) { errc = ++ErrCount; }

			//if (lockdown) EnableUI(false);

			if (errc < 4) MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else if (errc == 4) MessageBox.Show("Many errors happened!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

			lock (ErrLock) { ErrCount--; }
		}

		private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
		{
			ReportError($"SDK Error code: {ex} ({((int)ex).ToString("X")})", false);
		}

		private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
		{
			ReportError(ex.Message, true);
		}

		#endregion

		#region WindowSize and Closing

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Update the canvas size when the window size changes
			SetCanvasSize();
			//SetButtonPosition();
		}

		private void SetCanvasSize()
		{
			// Calculate the desired canvas size based on the window's height
			double windowHeight = this.ActualHeight;
			double canvasHeight = windowHeight;
			double canvasWidth = (canvasHeight / 2) * 3; // 3:2 aspect ratio

			// Set the canvas size
			LVCanvas.Width = canvasWidth;
			LVCanvas.Height = canvasHeight;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			try
			{
				MainCamera?.Dispose();
				APIHandler?.Dispose();
				//Directory.Delete(dir+"\\Temp", true);
				//Directory.Delete(dir + "\\ShowTemp", true);
				MainCamera.LiveViewUpdated -= MainCamera_LiveViewUpdated;
			}
			catch (Exception ex) { ReportError(ex.Message, false); }
		}

		#endregion

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			TriggerPicture();
		}
	}
}
