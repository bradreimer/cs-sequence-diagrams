using System;
using System.Globalization;
using System.Windows.Controls;

namespace UmlDiagrams
{
	/// <summary>
	/// Validates that a <see cref="String"/> can be converted to a <see cref="SequenceDiagramViewModel"/>.
	/// </summary>
	public sealed class SequenceDiagramGrammarValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			try
			{
				string inputText = value as string;
				(var seq, string error) = SequenceGrammar.Parse(inputText);

				return new ValidationResult(seq != null && string.IsNullOrEmpty(error), error);
			}
			catch (Exception ex)
			{
				return new ValidationResult(false, ex.Message);
			}
		}
	}
}