using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SequenceViz
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Brush m_officeOrange;
		private readonly Brush m_officeBlue;
		private readonly Brush m_officeGreen;
		private readonly Brush m_officeGray;
		private readonly Brush m_officeBlack;

		public MainWindow()
		{
			InitializeComponent();

			// Extract brush resoureces
			m_officeOrange = (Brush)Resources["OfficeOrange"];
			m_officeBlue = (Brush)Resources["OfficeBlue"];
			m_officeGreen = (Brush)Resources["OfficeGreen"];
			m_officeGray = (Brush)Resources["OfficeGray"];
			m_officeBlack = (Brush)Resources["OfficeBlack"];
		}

		private void SetStyle1_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			// Update style
			var fg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A8A8"));
			var bg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF272822"));
			m_scrollViewer.Foreground = fg;
			m_scrollViewer.Background = bg;

			m_sequenceDiagram.ActorBackground = m_officeGreen;
			m_sequenceDiagram.ActorForeground = Brushes.White;
			m_sequenceDiagram.ActorBorder = m_officeBlack;
			m_sequenceDiagram.NoteBackground = m_officeBlue;
			m_sequenceDiagram.NoteForeground = Brushes.White;
			m_sequenceDiagram.NoteBorder = m_officeBlack;
			m_sequenceDiagram.SignalForeground = fg;
		}

		private void SetStyle2_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			// Update style
			var fg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A8A8"));
			var bg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF272822"));
			m_scrollViewer.Foreground = fg;
			m_scrollViewer.Background = bg;

			m_sequenceDiagram.ActorBackground = m_officeOrange;
			m_sequenceDiagram.ActorForeground = Brushes.White;
			m_sequenceDiagram.ActorBorder = m_officeBlack;
			m_sequenceDiagram.NoteBackground = m_officeBlue;
			m_sequenceDiagram.NoteForeground = Brushes.White;
			m_sequenceDiagram.NoteBorder = m_officeBlack;
			m_sequenceDiagram.SignalForeground = fg;
		}

		private void SetStyle3_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			// Update style
			m_scrollViewer.Foreground = Brushes.Black;
			m_scrollViewer.Background = Brushes.White;

			m_sequenceDiagram.ActorBackground = m_officeOrange;
			m_sequenceDiagram.ActorForeground = Brushes.White;
			m_sequenceDiagram.ActorBorder = m_officeBlack;
			m_sequenceDiagram.NoteBackground = m_officeBlue;
			m_sequenceDiagram.NoteForeground = Brushes.White;
			m_sequenceDiagram.NoteBorder = m_officeBlack;
			m_sequenceDiagram.SignalForeground = Brushes.Black;
		}

		private void SetStyle4_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			// Update style
			m_scrollViewer.Foreground = Brushes.Black;
			m_scrollViewer.Background = Brushes.White;

			m_sequenceDiagram.ActorBackground = m_officeBlue;
			m_sequenceDiagram.ActorForeground = Brushes.White;
			m_sequenceDiagram.ActorBorder = m_officeBlack;
			m_sequenceDiagram.NoteBackground = m_officeGreen;
			m_sequenceDiagram.NoteForeground = Brushes.White;
			m_sequenceDiagram.NoteBorder = m_officeBlack;
			m_sequenceDiagram.SignalForeground = Brushes.Black;
		}

		private void SetStyle5_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			// Update style
			m_scrollViewer.Foreground = Brushes.Black;
			m_scrollViewer.Background = Brushes.White;

			m_sequenceDiagram.ActorBackground = Brushes.White;
			m_sequenceDiagram.ActorForeground = Brushes.Black;
			m_sequenceDiagram.ActorBorder = Brushes.Black;
			m_sequenceDiagram.NoteBackground = Brushes.White;
			m_sequenceDiagram.NoteForeground = Brushes.Black;
			m_sequenceDiagram.NoteBorder = Brushes.Black;
			m_sequenceDiagram.SignalForeground = Brushes.Black;
		}

		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void CopyToClipboard_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			// Prepare a render target using the render size of our sequence
			int width = (int)Math.Ceiling(m_sequenceDiagram.RenderSize.Width);
			int height = (int)Math.Ceiling(m_sequenceDiagram.RenderSize.Height);
			double dpi = 96;

			var rtb = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);

			// Draw the sequence diagram onto our render target
			var brush = new VisualBrush(m_sequenceDiagram);
			var visual = new DrawingVisual();

			var drawingContext = visual.RenderOpen();
			drawingContext.DrawRectangle(brush, null, new Rect(new Point(0, 0), new Point(width, height)));
			drawingContext.Close();

			rtb.Render(visual);

			// Copy to clipboard
			Clipboard.SetImage(rtb);
		}
	}
}
