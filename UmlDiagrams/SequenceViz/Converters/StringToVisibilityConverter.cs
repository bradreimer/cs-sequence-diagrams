﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SequenceViz
{
	[ValueConversion(typeof(string), typeof(Visibility))]
	class StringToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
