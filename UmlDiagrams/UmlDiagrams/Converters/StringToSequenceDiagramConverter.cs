using System;
using System.Globalization;
using System.Windows.Data;

namespace UmlDiagrams
{
	/// <summary>
	/// Converts a <see cref="String"/> to a <see cref="SequenceDiagramViewModel"/>.
	/// </summary>
	public sealed class StringToSequenceDiagramConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string inputText = value as string;
			if (inputText != null)
			{
				(var seq, string error) = SequenceGrammar.Parse(inputText);
				if (seq != null)
					return seq;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}