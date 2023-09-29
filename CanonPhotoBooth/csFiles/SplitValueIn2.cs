using System;
using System.Globalization;
using System.Windows.Data;

namespace PhotoBooth
{
	public class SplitValueIn2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double canvasWidth)
			{
				// Calculate half of the width as row height
				return canvasWidth / 2;
			}

			return value; // Return the original value if it's not a valid width
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
