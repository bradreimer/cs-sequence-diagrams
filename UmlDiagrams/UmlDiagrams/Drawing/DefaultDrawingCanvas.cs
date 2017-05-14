using System.Windows;
using System.Windows.Media;

namespace UmlDiagrams
{
	/// <summary>
	/// The default drawing canvas uses basic shapes.
	/// </summary>
	class DefaultDrawingCanvas : IDrawingCanvas
	{
		private const double LineThickness = 1;
		private static readonly Size s_arrowHeadSize = new Size(6, 3);
		private DrawingContext m_drawingContext;
		private Brush m_arrowBrush;
		private Pen m_solidLinePen;
		private Pen m_dashedLinePen;
		private bool m_isDisposed;

		public DefaultDrawingCanvas(DrawingContext drawingContext, Brush lineBrush)
		{
			m_drawingContext = drawingContext;
			m_arrowBrush = lineBrush;
			m_solidLinePen = new Pen(lineBrush, LineThickness);
			m_dashedLinePen = new Pen(lineBrush, LineThickness) { DashStyle = DashStyles.Dash };
		}

		public virtual void DrawText(double x, double y, FormattedText formattedText)
		{
			Point origin = new Point(x, y);
			DrawText(m_drawingContext, formattedText, origin);
		}

		public void DrawLine(double x1, double y1, double x2, double y2, SequenceLineType lineType, SequenceArrowType arrowHead)
		{
			Pen pen = lineType == SequenceLineType.Solid ? m_solidLinePen : m_dashedLinePen;
			double halfPenWidth = m_solidLinePen.Thickness / 2;

			GuidelineSet guidelines = new GuidelineSet();
			guidelines.GuidelinesX.Add(x1 + halfPenWidth);
			guidelines.GuidelinesX.Add(x2 + halfPenWidth);
			guidelines.GuidelinesY.Add(y1 + halfPenWidth);
			guidelines.GuidelinesY.Add(y2 + halfPenWidth);

			m_drawingContext.PushGuidelineSet(guidelines);

			// Draw the main line segment
			Point start = new Point(x1, y1);
			Point end = new Point(x2, y2);
			DrawLine(m_drawingContext, pen, start, end);

			// Draw the arrowhead
			double direction = (x1 > x2) ? 1 : -1;
			Point arrow0 = new Point(end.X + direction * LineThickness, end.Y);
			Point arrow1 = new Point(x2 + direction * s_arrowHeadSize.Width, y2 - s_arrowHeadSize.Height);
			Point arrow2 = new Point(x2 + direction * s_arrowHeadSize.Width, y2 + s_arrowHeadSize.Height);
			switch (arrowHead)
			{
				case SequenceArrowType.Filled:
					{
						var streamGeometry = new StreamGeometry();
						using (StreamGeometryContext geometryContext = streamGeometry.Open())
						{
							geometryContext.BeginFigure(arrow0, true, true);
							PointCollection points = new PointCollection { arrow1, arrow2 };
							geometryContext.PolyLineTo(points, true, true);
						}

						streamGeometry.Freeze();
						m_drawingContext.DrawGeometry(m_arrowBrush, m_solidLinePen, streamGeometry);
						break;
					}
				case SequenceArrowType.Open:
					{
						m_drawingContext.DrawLine(m_solidLinePen, arrow0, arrow1);
						m_drawingContext.DrawLine(m_solidLinePen, arrow0, arrow2);
						break;
					}
			}

			m_drawingContext.Pop();
		}

		public void DrawRectangle(Brush brush, Pen pen, Rect rect)
		{
			double halfPenWidth = m_solidLinePen.Thickness / 2;

			GuidelineSet guidelines = new GuidelineSet();
			guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
			guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
			guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
			guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);

			m_drawingContext.PushGuidelineSet(guidelines);
			DrawRectangle(m_drawingContext, brush, pen, rect);
			m_drawingContext.Pop();
		}

		protected virtual void DrawText(DrawingContext drawingContext, FormattedText formattedText, Point origin)
		{
			drawingContext.DrawText(formattedText, origin);
		}

		protected virtual void DrawLine(DrawingContext drawingContext, Pen pen, Point start, Point end)
		{
			m_drawingContext.DrawLine(pen, start, end);
		}

		protected virtual void DrawRectangle(DrawingContext drawingContext, Brush brush, Pen pen, Rect rect)
		{
			m_drawingContext.DrawRectangle(brush, pen, rect);
		}

		#region IDisposable Methods

		protected virtual void Dispose(bool disposing)
		{
			if (!m_isDisposed)
			{
				if (disposing)
				{
					//
				}

				m_isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}