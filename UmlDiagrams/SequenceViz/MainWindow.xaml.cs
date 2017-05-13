using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
			Foreground = fg;
			Background = bg;

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
			Foreground = fg;
			Background = bg;

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
			Foreground = Brushes.Black;
			Background = Brushes.White;

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
			Foreground = Brushes.Black;
			Background = Brushes.White;

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
			Foreground = Brushes.Black;
			Background = Brushes.White;

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
			var seq = m_sequenceDiagram;

			var rect = new Rect(seq.DesiredSize);
			seq.Measure(rect.Size);
			seq.Arrange(rect);

			Size renderSize = seq.RenderSize;

			var drawingVisual = new DrawingVisual();
			var drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawRectangle(Background, null, new Rect(renderSize));
			drawingContext.Close();

			int width = (int)renderSize.Width, height = (int)renderSize.Height;
			RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

			bmp.Render(drawingVisual);
			bmp.Render(seq);

			// Copy to clipboard
			Clipboard.SetImage(bmp);
		}
	}
}
