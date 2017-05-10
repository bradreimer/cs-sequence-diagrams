using System;
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
			// Render sequence diagram to an image
			//int width = (int)Math.Ceiling(m_sequenceDiagram.DesiredSize.Width);
			//int height = (int)Math.Ceiling(m_sequenceDiagram.DesiredSize.Height);
			int width = (int)Math.Ceiling(m_sequenceDiagram.ActualWidth);
			int height = (int)Math.Ceiling(m_sequenceDiagram.ActualHeight);
			double dpi = 96;
			var rtb = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
			rtb.Render(m_sequenceDiagram);

			// Copy to clipboard
			Clipboard.SetImage(rtb);
		}
	}
}
