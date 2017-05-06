using System;
using System.Windows;
using System.Windows.Media;

namespace UmlDiagrams
{
	/// <summary>
	/// This interface is used to draw shapes on a Wpf drawing context.
	/// </summary>
	interface IDrawingCanvas : IDisposable
	{
		void DrawText(double x, double y, FormattedText formattedText);
		void DrawLine(double x1, double y1, double x2, double y2, SequenceLineType lineType, SequenceArrowType arrowHead);
		void DrawRectangle(Brush brush, Pen pen, Rect rect);
	}
}
