using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;

namespace SequenceViz
{
	class MainWindowViewModel : ObservableObject
	{
		#region Members

		private readonly DialogService m_dialogService;
		private readonly CommandBindingCollection m_commandBindings;
		private SequenceThemes m_sequenceTheme;
		private string m_sequenceText;
		private string m_activeFileName;

		#endregion

		#region Constructor

		public MainWindowViewModel(DialogService dialogService)
		{
			m_dialogService = dialogService;
			m_commandBindings = new CommandBindingCollection();
			InitializeCommandBindings();
		}

		private void InitializeCommandBindings()
		{
			m_commandBindings.Add(new CommandBinding(ApplicationCommands.New, NewExecuted));
			m_commandBindings.Add(new CommandBinding(ApplicationCommands.Open, OpenExecuted));
			m_commandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveExecuted));
			m_commandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, SaveAsExecuted));
			m_commandBindings.Add(new CommandBinding(SequenceDiagramStylesCommands.SetStyle, SetStyleExecuted));
		}

		#endregion

		#region Properties

		public CommandBindingCollection CommandBindings
		{
			get { return m_commandBindings; }
		}

		public SequenceThemes SequenceTheme
		{
			get { return m_sequenceTheme; }
			set { Set(ref m_sequenceTheme, value); }
		}

		public string SequenceText
		{
			get { return m_sequenceText; }
			set { Set(ref m_sequenceText, value); }
		}

		public string ActiveFileName
		{
			get { return m_activeFileName; }
			set { Set(ref m_activeFileName, value); }
		}

		#endregion

		#region Commands

		private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SequenceText = string.Empty;
			ActiveFileName = null;
		}

		private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var dlg = new OpenFileDialog
			{
				Title = "Open a Sequence Diagram",
				DefaultExt = ".txt",
				Filter = "Sequence Diagram|*.txt",
			};

			bool? result = dlg.ShowDialog();

			if (result == true && !string.IsNullOrEmpty(dlg.FileName))
			{
				Open(dlg.FileName);
			}
		}

		private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (ActiveFileName == null)
				SaveAsExecuted(sender, e);
			else
				Save(ActiveFileName);
		}

		private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var dlg = new SaveFileDialog
			{
				Title = "Save a Sequence Diagram",
				FileName = ActiveFileName ?? "NewSequence.txt",
				DefaultExt = ".txt",
				Filter = "Sequence Diagram|*.txt",
			};

			bool? result = dlg.ShowDialog();

			if (result == true && !string.IsNullOrEmpty(dlg.FileName))
			{
				Save(dlg.FileName);
			}
		}

		private void SetStyleExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SequenceTheme = (SequenceThemes)e.Parameter;
		}

		#endregion

		#region Methods

		private void Open(string fileName)
		{
			if (!File.Exists(fileName))
				return; // error?

			ActiveFileName = fileName;
			SequenceText = File.ReadAllText(fileName);
		}

		private void Save(string fileName)
		{
			ActiveFileName = fileName;
			File.WriteAllText(fileName, SequenceText);
		}

		#endregion
	}
}
