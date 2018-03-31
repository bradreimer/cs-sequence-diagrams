using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace SequenceViz
{
	class MainWindowViewModel : ObservableObject
	{
		#region Members

		private readonly CommandBindingCollection m_commandBindings;
		private SequenceThemes m_sequenceTheme;
		private string m_sequenceText;
		private string m_activeFileName;
		private bool m_isDirty;

		#endregion

		#region Constructor

		public MainWindowViewModel()
		{
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
			set
			{
				if (Set(ref m_sequenceText, value))
					IsDirty = true;
			}
		}

		public string ActiveFileName
		{
			get { return m_activeFileName; }
			set { Set(ref m_activeFileName, value); }
		}

		public bool IsDirty
		{
			get { return m_isDirty; }
			set { Set(ref m_isDirty, value); }
		}

		#endregion

		#region Commands

		private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!ValidateCanEdit())
				return;
			NewUI();
		}

		private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (!ValidateCanEdit())
				return;
			OpenUI();
		}

		private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SaveUI();
		}

		private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SaveAsUI();
		}

		private void SetStyleExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SequenceTheme = (SequenceThemes)e.Parameter;
		}

		#endregion

		#region Methods

		private bool ValidateCanEdit()
		{
			bool canEdit = true;
			if (IsDirty)
			{
				MessageBoxResult result = MessageBox.Show(
					"Save changes to this sequence diagram?",
					"Unsaved Changes Detected",
					MessageBoxButton.YesNoCancel,
					MessageBoxImage.Question);

				switch (result)
				{
					case MessageBoxResult.Yes:
						SaveUI();
						break;
					case MessageBoxResult.Cancel:
						canEdit = false;
						break;
				}
			}
			return canEdit;
		}

		private void NewUI()
		{
			SequenceText = string.Empty;
			ActiveFileName = null;
		}

		private void OpenUI()
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

		private void SaveUI()
		{
			if (ActiveFileName == null)
				SaveAsUI();
			else
				Save(ActiveFileName);
		}

		private void SaveAsUI()
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

		private void Open(string fileName)
		{
			if (!File.Exists(fileName))
				return; // error?

			ActiveFileName = fileName;
			SequenceText = File.ReadAllText(fileName);
			IsDirty = false;
		}

		private void Save(string fileName)
		{
			ActiveFileName = fileName;
			File.WriteAllText(fileName, SequenceText);
			IsDirty = false;
		}

		#endregion
	}
}
