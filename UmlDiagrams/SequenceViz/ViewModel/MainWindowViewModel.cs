using System;
using System.Windows.Input;

namespace SequenceViz
{
	class MainWindowViewModel : ObservableObject
	{
		#region Members

		private readonly CommandBindingCollection m_commandBindings;
		private SequenceThemes m_sequenceTheme;
		private string m_sequenceText;

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

		#endregion

		#region Commands

		private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
		}

		private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
