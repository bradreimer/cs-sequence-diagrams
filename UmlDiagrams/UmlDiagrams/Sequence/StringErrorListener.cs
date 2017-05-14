using System.Text;
using Antlr4.Runtime;

namespace UmlDiagrams
{
	sealed class StringErrorListener : BaseErrorListener, IAntlrErrorListener<int>
	{
		private readonly StringBuilder m_message;

		public StringErrorListener(StringBuilder message)
		{
			m_message = message;
		}

		public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			if (m_message.Length > 0)
				m_message.AppendLine();
			m_message.Append("Parser error: ").Append(msg).Append(' ').Append(e);
		}

		public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			if (m_message.Length > 0)
				m_message.AppendLine();
			m_message.Append("Lexer error: ").Append(msg).Append(' ').Append(e);
		}

		public void GrammarError(string msg)
		{
			if (m_message.Length > 0)
				m_message.AppendLine();
			m_message.Append("Grammar error: ").Append(msg);
		}
	}
}
