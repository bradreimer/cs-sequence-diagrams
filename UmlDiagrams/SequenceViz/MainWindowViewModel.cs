using SequenceViz.Properties;
using System.Text;

namespace SequenceViz
{
	class MainWindowViewModel : ObservableObject
	{
		private string m_sequenceText;

		internal MainWindowViewModel()
		{
			m_sequenceText = Resources.Default;
		}

		public string SequenceText
		{
			get { return m_sequenceText; }
			set { Set(ref m_sequenceText, value); }
		}
	}
}
