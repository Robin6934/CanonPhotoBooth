using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using static PhotoBooth.PhotoBoothLib;
using PhotoBooth;

namespace PhotoBooth
{
	/// <summary>
	/// Interaction logic for PhotoPrintPage.xaml
	/// </summary>
	public partial class PhotoPrintPage : Window
	{
		string ImagePath = "";
		public ConfigLoader configLoader { get; set;}
		PhotoBoothLib photoBoothLib = new PhotoBoothLib();
		public PhotoPrintPage()
		{
			InitializeComponent();
			this.WindowStyle = WindowStyle.None;
			this.WindowState = WindowState.Maximized;

		}

		private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
		{
			// Create the Print button
			Button buttonPrint = new Button();
			buttonPrint.Name = "ButtonPrint";
			buttonPrint.Content = "Print";
			buttonPrint.Height = 40;
			buttonPrint.Width = 50;
			buttonPrint.FontSize = 20;
			buttonPrint.Click += ButtonPrint_Click;
			Canvas.SetLeft(buttonPrint, (MainCanvas.ActualWidth - buttonPrint.Width) / 2); // Centered
			Canvas.SetBottom(buttonPrint, 50);


			// Create the Save button
			Button buttonSave = new Button();
			buttonSave.Name = "ButtonSave";
			buttonSave.Content = "Save";
			buttonSave.Height = 40;
			buttonSave.Width = 50;
			buttonSave.FontSize = 20;
			buttonSave.Click += ButtonSave_Click;
			Canvas.SetLeft(buttonSave, (MainCanvas.ActualWidth - buttonPrint.Width) / 2 - 75); // Centered
			Canvas.SetBottom(buttonSave, 50);

			// Create the Delete button
			Button buttonDelete = new Button();
			buttonDelete.Name = "ButtonDelete";
			buttonDelete.Content = "Delete";
			buttonDelete.Height = 40;
			buttonDelete.Width = 70;
			buttonDelete.FontSize = 20;
			buttonDelete.Click += ButtonDelete_Click;
			Canvas.SetLeft(buttonDelete, (MainCanvas.ActualWidth - buttonPrint.Width) / 2 + 75); // Centered
			Canvas.SetBottom(buttonDelete, 50);


			// Add the buttons to the canvas
			MainCanvas.Children.Add(buttonPrint);
			MainCanvas.Children.Add(buttonSave);
			MainCanvas.Children.Add(buttonDelete);

			SetCanvasSize();
		}

		public void DisplayImage(string imagePath)
		{
			photoBoothLib.configLoader = configLoader;
			ImagePath = imagePath;
			this.SizeChanged += PhotoPrintPage_SizeChanged;
			ImageViewer.Source = new BitmapImage(new Uri(imagePath));
			
			MainCanvas.Loaded += MainCanvas_Loaded;
		}

		private void PhotoPrintPage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SetCanvasSize();
		}

		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{

			photoBoothLib.doPhotoboxThings(ImagePath, PhotoBoothLib.PictureOptions.Save); // Call the method on the instance
			Close();

		}

		private void ButtonPrint_Click(object sender, RoutedEventArgs e)
		{

			photoBoothLib.doPhotoboxThings(ImagePath, PhotoBoothLib.PictureOptions.Print); // Call the method on the instance
			Close();
		}

		private void ButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			photoBoothLib.doPhotoboxThings(ImagePath, PhotoBoothLib.PictureOptions.Delete); // Call the method on the instance
			Close();
		}


		private void SetCanvasSize()
		{
			// Calculate the desired canvas size based on the window's height
			double windowHeight = this.ActualHeight;
			double canvasHeight = windowHeight;
			double canvasWidth = (canvasHeight / 2) * 3; // 3:2 aspect ratio

			// Set the canvas size
			ImageViewer.Width = canvasWidth;
			ImageViewer.Height = canvasHeight;

			ImageViewer.SetValue(Canvas.LeftProperty, (this.ActualWidth - canvasWidth) / 2);
		}
	}
}
