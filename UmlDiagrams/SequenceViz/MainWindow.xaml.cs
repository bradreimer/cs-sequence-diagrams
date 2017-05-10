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
		public MainWindow()
		{
			InitializeComponent();
		}

		private void SetStyleOfficeDark_Execute(object sender,ExecutedRoutedEventArgs e)
		{
			// Extract brush resources
			var officeOrange = (Brush)Resources["OfficeOrange"];
			var officeBlue = (Brush)Resources["OfficeBlue"];
			var officeGreen = (Brush)Resources["OfficeGreen"];
			var officeGray = (Brush)Resources["OfficeGray"];
			var officeBlack = (Brush)Resources["OfficeBlack"];
		}

		private void SetStyleOfficeLight_Execute(object sender, ExecutedRoutedEventArgs e)
		{

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
