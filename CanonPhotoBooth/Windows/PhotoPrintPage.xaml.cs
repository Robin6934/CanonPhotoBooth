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
	public partial class PhotoPrintPage : Window, IDisposable
	{
		string ImagePath = "";

		public ConfigLoader? configLoader { get; set;}

        private bool _disposed = false;

		private PhotoBoothLib _photoBoothLib;

		public PhotoPrintPage()
		{
			InitializeComponent();

			this.WindowStyle = WindowStyle.None;

			this.WindowState = WindowState.Maximized;

            _photoBoothLib = PhotoBoothLib.Instance;
        }

		private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
		{
            // Create the Print button
            Button buttonPrint = new Button();
            buttonPrint.Name = "ButtonPrint";
            buttonPrint.Content = "Print";
            buttonPrint.Height = 150;
            buttonPrint.Width = 150;
            buttonPrint.FontSize = 40;
            buttonPrint.Click += ButtonPrint_Click;
            Canvas.SetLeft(buttonPrint, (MainCanvas.ActualWidth - buttonPrint.Width) / 2); // Centered
            Canvas.SetBottom(buttonPrint, 50);
            buttonPrint.SetValue(Button.TemplateProperty, GetRoundButtonTemplate(buttonPrint));

            // Create the Save button
            Button buttonSave = new Button();
            buttonSave.Name = "ButtonSave";
            buttonSave.Content = "Save";
            buttonSave.Height = 150;
            buttonSave.Width = 150;
            buttonSave.FontSize = 40;
            buttonSave.Click += ButtonSave_Click;
            Canvas.SetLeft(buttonSave, (MainCanvas.ActualWidth - buttonPrint.Width) / 2 - 180); // Centered
            Canvas.SetBottom(buttonSave, 50);
            buttonSave.SetValue(Button.TemplateProperty, GetRoundButtonTemplate(buttonSave));

            // Create the Delete button
            Button buttonDelete = new Button();
            buttonDelete.Name = "ButtonDelete";
            buttonDelete.Content = "Delete";
            buttonDelete.Height = 150;
            buttonDelete.Width = 150;
            buttonDelete.FontSize = 40;
            buttonDelete.Click += ButtonDelete_Click;
            Canvas.SetLeft(buttonDelete, (MainCanvas.ActualWidth - buttonPrint.Width) / 2 + 180); // Centered
            Canvas.SetBottom(buttonDelete, 50);
            buttonDelete.SetValue(Button.TemplateProperty, GetRoundButtonTemplate(buttonDelete));

            // Add the buttons to the canvas
            MainCanvas.Children.Add(buttonPrint);
			MainCanvas.Children.Add(buttonSave);
			MainCanvas.Children.Add(buttonDelete);

			SetCanvasSize();
		}

        private ControlTemplate GetRoundButtonTemplate(Button button)
        {
            ControlTemplate template = new ControlTemplate(typeof(Button));

            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "border";
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(button.Width / 2));
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(BackgroundProperty));

            FrameworkElementFactory contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);

            template.VisualTree = borderFactory;
            return template;
        }

        public void DisplayImage(string imagePath)
		{
			ImagePath = imagePath;
			this.SizeChanged += PhotoPrintPage_SizeChanged;

            Uri imageUri = new Uri(imagePath);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = imageUri;
            bitmap.EndInit();

            ImageViewer.Source = bitmap;
			
			MainCanvas.Loaded += MainCanvas_Loaded;
            this.Closing += PhotoPrintPage_Closing;
		}

        private void PhotoPrintPage_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }

        private void PhotoPrintPage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SetCanvasSize();
		}

		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			_photoBoothLib.doPhotoboxThings(ImagePath, PhotoBoothLib.PictureOptions.Save); // Call the method on the instance
			Close();
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
		{
			_photoBoothLib.doPhotoboxThings(ImagePath, PhotoBoothLib.PictureOptions.Print); // Call the method on the instance
			Close();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			_photoBoothLib.doPhotoboxThings(ImagePath, PhotoBoothLib.PictureOptions.Delete); // Call the method on the instance
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources here
                    // For example, unsubscribe from events and release objects.

                    this.SizeChanged -= PhotoPrintPage_SizeChanged;
                    // Unsubscribe from other events if necessary.

                    if (ImageViewer.Source is BitmapImage bitmapImage)
                    {
                        bitmapImage.StreamSource?.Close();
                        bitmapImage.StreamSource?.Dispose();
                        bitmapImage.StreamSource = null;
                    }
                    ImageViewer.Source = null;

                    // Dispose of any other disposable objects if used.
                    // For example, if `photoBoothLib` is IDisposable, call its Dispose method.
                }

                // Release unmanaged resources here, if any.

                _disposed = true;
            }
        }

        // Finalizer
        ~PhotoPrintPage()
        {
            Dispose(false);
        }

    }
}

