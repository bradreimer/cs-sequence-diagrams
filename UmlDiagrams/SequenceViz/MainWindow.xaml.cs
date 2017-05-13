using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UmlDiagrams;

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
		private CustomStyle m_currentStyle;

		private sealed class CustomStyle
		{
			public Brush Foreground;
			public Brush Background;
			public Brush ActorBackground;
			public Brush ActorForeground;
			public Brush ActorBorder;
			public Brush NoteBackground;
			public Brush NoteForeground;
			public Brush NoteBorder;
			public Brush SignalForeground;
		}

		public MainWindow()
		{
			InitializeComponent();

			// Extract brush resoureces
			m_officeOrange = (Brush)Resources["OfficeOrange"];
			m_officeBlue = (Brush)Resources["OfficeBlue"];
			m_officeGreen = (Brush)Resources["OfficeGreen"];
			m_officeGray = (Brush)Resources["OfficeGray"];
			m_officeBlack = (Brush)Resources["OfficeBlack"];

			SetCustomStyle1();
		}

		private void SetStyle1_Execute(object sender, ExecutedRoutedEventArgs e) => SetCustomStyle1();
		private void SetStyle2_Execute(object sender, ExecutedRoutedEventArgs e) => SetCustomStyle2();
		private void SetStyle3_Execute(object sender, ExecutedRoutedEventArgs e) => SetCustomStyle3();
		private void SetStyle4_Execute(object sender, ExecutedRoutedEventArgs e) => SetCustomStyle4();
		private void SetStyle5_Execute(object sender, ExecutedRoutedEventArgs e) => SetCustomStyle5();

		private void SetCustomStyle1()
		{
			var fg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A8A8"));
			var bg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF272822"));
			SetCustomStyle(new CustomStyle
			{
				Foreground = fg,
				Background = bg,
				ActorBackground = m_officeGreen,
				ActorForeground = Brushes.White,
				ActorBorder = m_officeBlack,
				NoteBackground = m_officeBlue,
				NoteForeground = Brushes.White,
				NoteBorder = m_officeBlack,
				SignalForeground = fg,
			});
		}

		private void SetCustomStyle2()
		{
			var fg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA8A8A8"));
			var bg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF272822"));
			SetCustomStyle(new CustomStyle
			{
				Foreground = fg,
				Background = bg,
				ActorBackground = m_officeOrange,
				ActorForeground = Brushes.White,
				ActorBorder = m_officeBlack,
				NoteBackground = m_officeBlue,
				NoteForeground = Brushes.White,
				NoteBorder = m_officeBlack,
				SignalForeground = fg,
			});
		}

		private void SetCustomStyle3()
		{
			SetCustomStyle(new CustomStyle
			{
				Foreground = Brushes.Black,
				Background = Brushes.White,
				ActorBackground = m_officeOrange,
				ActorForeground = Brushes.White,
				ActorBorder = m_officeBlack,
				NoteBackground = m_officeBlue,
				NoteForeground = Brushes.White,
				NoteBorder = m_officeBlack,
				SignalForeground = Brushes.Black,
			});
		}

		private void SetCustomStyle4()
		{
			SetCustomStyle(new CustomStyle
			{
				Foreground = Brushes.Black,
				Background = Brushes.White,
				ActorBackground = m_officeBlue,
				ActorForeground = Brushes.White,
				ActorBorder = m_officeBlack,
				NoteBackground = m_officeGreen,
				NoteForeground = Brushes.White,
				NoteBorder = m_officeBlack,
				SignalForeground = Brushes.Black,
			});
		}

		private void SetCustomStyle5()
		{
			SetCustomStyle(new CustomStyle
			{
				Foreground = Brushes.Black,
				Background = Brushes.White,
				ActorBackground = Brushes.White,
				ActorForeground = Brushes.Black,
				ActorBorder = Brushes.Black,
				NoteBackground = Brushes.White,
				NoteForeground = Brushes.Black,
				NoteBorder = Brushes.Black,
				SignalForeground = Brushes.Black,
			});
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
			// Clone this sequence diagram
			SequenceDiagram seq = m_sequenceDiagram;

			// Measure and re-arrange
			Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
			seq.Measure(size);

			Rect rect = new Rect(seq.DesiredSize);
			seq.Arrange(rect);

			// Render the sequence to a bitmap
			var drawingVisual = new DrawingVisual();
			var drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawRectangle(Background, null, rect);
			drawingContext.Close();

			int width = (int)rect.Width, height = (int)rect.Height;
			RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

			bmp.Render(drawingVisual);
			bmp.Render(seq);

			// Copy to clipboard
			Clipboard.SetImage(bmp);

			// Restore the previous layout by re-arranging its container
			m_sequenceDiagramContainer.InvalidateArrange();
		}

		private void SetCustomStyle(CustomStyle currentStyle)
		{
			m_currentStyle = currentStyle;
			Foreground = currentStyle.Foreground;
			Background = currentStyle.Background;

			m_sequenceDiagram.ActorBackground = currentStyle.ActorBackground;
			m_sequenceDiagram.ActorForeground = currentStyle.ActorForeground;
			m_sequenceDiagram.ActorBorder = currentStyle.ActorBorder;
			m_sequenceDiagram.NoteBackground = currentStyle.NoteBackground;
			m_sequenceDiagram.NoteForeground = currentStyle.NoteForeground;
			m_sequenceDiagram.NoteBorder = currentStyle.NoteBorder;
			m_sequenceDiagram.SignalForeground = currentStyle.SignalForeground;
		}
	}
}