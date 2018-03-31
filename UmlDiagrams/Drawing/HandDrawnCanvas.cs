using System;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UmlDiagrams
{
	/// <summary>
	/// The hand drawn canvas draws shapes in a simplified (i.e. hand-drawn) style.
	/// </summary>
	sealed class HandDrawnCanvas : DefaultDrawingCanvas
	{
		private static readonly Random s_seedGenerator = new Random();
		private static readonly int s_seed = s_seedGenerator.Next();
		private Random m_random;

		public HandDrawnCanvas(DrawingContext drawingContext, Brush lineBrush)
			: base(drawingContext, lineBrush)
		{
			m_random = new Random(s_seed);
		}

		protected override void DrawLine(DrawingContext drawingContext, Pen pen, Point start, Point end)
		{
			var geometry = Geometry.Parse(HandLine(start.X, start.Y, end.X, end.Y));
			drawingContext.DrawGeometry(Brushes.Transparent, pen, geometry);
		}

		protected override void DrawRectangle(DrawingContext drawingContext, Brush brush, Pen pen, Rect rect)
		{
			var geometry = Geometry.Parse(HandRectangle(rect.X, rect.Y, rect.Width, rect.Height));
			drawingContext.DrawGeometry(brush, pen, geometry);
		}

		private string HandRectangle(double x, double y, double w, double h)
		{
			var path = new StringBuilder().Append('M').Append(x).Append(',').Append(y);
			Wobble(path, x, y, x + w, y);
			Wobble(path, x + w, y, x + w, y + h);
			Wobble(path, x + w, y + h, x, y + h);
			Wobble(path, x, y + h, x, y);
			return path.ToString();
		}

		private string HandLine(double x1, double y1, double x2, double y2)
		{
			var path = new StringBuilder().Append('M').Append(x1).Append(',').Append(y1);
			Wobble(path, x1, y1, x2, y2);
			return path.ToString();
		}

		private void Wobble(StringBuilder path, double x1, double y1, double x2, double y2)
		{
			if (double.IsInfinity(x1) || double.IsInfinity(y1) || double.IsInfinity(x2) || double.IsInfinity(y2))
				throw new ArgumentException("x1,y1,x2,y2 must be finite");

			// Wobble no more than 2% of the line length
			double factor = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)) * 0.02;

			// Distance along line where the control points are
			// Clamp between 20% and 80% so arrow heads aren't angle too much
			double r1 = Clamp(m_random.NextDouble(), 0.2, 0.8);
			double r2 = Clamp(m_random.NextDouble(), 0.2, 0.8);

			double xFactor = m_random.NextDouble() > 0.5 ? factor : -factor;
			double yFactor = m_random.NextDouble() > 0.5 ? factor : -factor;

			var startControlPoint = new Point(
				(x2 - x1) * r1 + x1 + xFactor,
				(y2 - y1) * r1 + y1 + yFactor
			);

			var endControlPoint = new Point(
				(x2 - x1) * r2 + x1 + xFactor,
				(y2 - y1) * r2 + y1 + yFactor
			);

			path.
				Append('C').Append(startControlPoint.X).Append(',').Append(startControlPoint.Y).
				Append(' ').Append(endControlPoint.X).Append(',').Append(endControlPoint.Y).
				Append(' ').Append(x2).Append(',').Append(y2);
		}

		private static double Clamp(double value, double min, double max)
		{
			if (value < min)
				return min;
			if (value > max)
				return max;
			return value;
		}
	}
}