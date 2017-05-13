namespace SequenceViz
{
	class MainWindowViewModel : ObservableObject
	{
		private SequenceThemes m_sequenceTheme;
		private string m_sequenceText;

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
	}
}
