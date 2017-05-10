namespace SequenceViz
{
	class MainWindowViewModel : ObservableObject
	{
		private string m_sequenceText;

		public string SequenceText
		{
			get { return m_sequenceText; }
			set { Set(ref m_sequenceText, value); }
		}
	}
}
