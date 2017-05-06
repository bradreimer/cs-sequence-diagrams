using System;
using System.Drawing;
using System.IO;
using System.Windows;
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
			DataContext = new MainWindowViewModel();
		}

		private void CopyToClipboardExecuted(object sender, RoutedEventArgs e)
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
