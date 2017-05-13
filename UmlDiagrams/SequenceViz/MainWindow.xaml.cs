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
		public MainWindow()
		{
			InitializeComponent();
		}

		private void SetStyle_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			((MainWindowViewModel)DataContext).SequenceTheme = (SequenceThemes)e.Parameter;
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
	}
}