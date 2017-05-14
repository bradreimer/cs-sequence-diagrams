using System.Windows;
using SequenceViz.Properties;

namespace SequenceViz
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private MainWindowViewModel m_viewModel;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Initialize view model text from last time this
			// application was run. If none exist, use the
			// default text
			var vm = new MainWindowViewModel();

			var text = Settings.Default.LastSequenceText;

			if (string.IsNullOrWhiteSpace(text))
				text = SequenceViz.Properties.Resources.DefaultSequenceText;

			vm.SequenceText = text;
			m_viewModel = vm;

			// Create main window
			var mainWindow = new MainWindow();
			mainWindow.DataContext = vm;

			mainWindow.Show();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			// Backup our sequence diagram text
			Settings.Default.LastSequenceText = m_viewModel.SequenceText;
			Settings.Default.Save();
		}
	}
}
