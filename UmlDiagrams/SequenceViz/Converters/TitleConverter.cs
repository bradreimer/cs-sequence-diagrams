using System;
using System.Globalization;
using System.Windows.Data;

namespace SequenceViz
{
	class TitleConverter : IValueConverter
	{
		private const string ApplicationTitle = "Sequence Visualizer";

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string title = (string)value;
			return string.IsNullOrWhiteSpace(title) ? ApplicationTitle : $"{ApplicationTitle} - {title}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
