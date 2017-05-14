using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UmlDiagrams;
using UmlDiagramsTest.Properties;

namespace UmlDiagramsTest
{
	[TestClass]
	public class SequenceGrammarTest
	{
		[TestMethod]
		public void SequenceGrammarEmpty()
		{
			(var seq, string error) = SequenceGrammar.Parse("");
			Assert.IsNotNull(seq);
			Assert.AreEqual(0, seq.Actors.Count);
			Assert.IsTrue(string.IsNullOrEmpty(seq.Title));
		}

		[TestMethod]
		public void SequenceGrammarValidateExamples()
		{
			Assert.IsNotNull(SequenceGrammar.Parse(Resources.Demo).SequenceDiagram);
			Assert.IsNotNull(SequenceGrammar.Parse(Resources.Example1).SequenceDiagram);
			Assert.IsNotNull(SequenceGrammar.Parse(Resources.Example2).SequenceDiagram);
			Assert.IsNotNull(SequenceGrammar.Parse(Resources.Example3).SequenceDiagram);
		}

		[TestMethod]
		public void SequenceGrammarParseTitle()
		{
			const string ExpectedTitle = "My title goes here!";
			(var seq, string error) = SequenceGrammar.Parse($"title: {ExpectedTitle}");
			Assert.AreEqual(ExpectedTitle, seq.Title);
		}

		[TestMethod]
		public void SequenceGrammarParseMultipleTitles()
		{
			var input = new StringBuilder().
				AppendLine("title: First title").
				AppendLine("title: Last title wins");
			(var seq, string error) = SequenceGrammar.Parse(input.ToString());
			Assert.AreEqual("Last title wins", seq.Title);
		}

		[TestMethod]
		public void SequenceGrammarParseTitleMultipleLines()
		{
			(var seq, string error) = SequenceGrammar.Parse(@"title: Line One\nLine Two");
			Assert.AreEqual("Line One\nLine Two", seq.Title, "Escape sequences are automatically expanded");
		}

		[TestMethod]
		public void SequenceGrammarParseTitleWhitespace()
		{
			(var seq, string error) = SequenceGrammar.Parse("title:    \tWhite  Space!   ");
			Assert.AreEqual("White  Space!", seq.Title);
		}

		[TestMethod]
		public void SequenceGrammarParseParticipant()
		{
			var participant1 = GetActorHelper("participant apple");
			Assert.AreEqual("apple", participant1.Alias);
			Assert.AreEqual("apple", participant1.Name);

			var participant2 = GetActorHelper("participant macintosh as apple");
			Assert.AreEqual("apple", participant2.Alias);
			Assert.AreEqual("macintosh", participant2.Name);

			var participant3 = GetActorHelper("participant \"two words\"");
			Assert.AreEqual("two words", participant3.Alias);
			Assert.AreEqual("two words", participant3.Name);

			var participant4 = GetActorHelper("participant \"two words\" as one");
			Assert.AreEqual("one", participant4.Alias);
			Assert.AreEqual("two words", participant4.Name);

			var participant5 = GetActorHelper("participant \"real name\" as \"alias name\"");
			Assert.AreEqual("alias name", participant5.Alias);
			Assert.AreEqual("real name", participant5.Name);
		}

		[TestMethod]
		public void SequenceGrammarParseNote()
		{
			const string ExpectedMessage = "some message";

			// Left placement
			var note1 = GetNoteHelper($"note left of Delta: some message");
			Assert.AreEqual("Delta", note1.Actors[0].Name);
			Assert.AreEqual(SequenceNotePlacement.LeftOf, note1.Placement);
			Assert.AreEqual(ExpectedMessage, note1.Message);

			// Right placement
			var note2 = GetNoteHelper($"note right of Burnaby: some message");
			Assert.AreEqual("Burnaby", note2.Actors[0].Name);
			Assert.AreEqual(SequenceNotePlacement.RightOf, note2.Placement);
			Assert.AreEqual(ExpectedMessage, note2.Message);

			// Over single actor
			var note3 = GetNoteHelper($"note over Delta: some message");
			Assert.AreEqual("Delta", note3.Actors[0].Name);
			Assert.AreEqual(1, note3.Actors.Length);
			Assert.AreEqual(SequenceNotePlacement.Over, note3.Placement);
			Assert.AreEqual(ExpectedMessage, note3.Message);

			// Over pair of actors
			var note4 = GetNoteHelper($"note over Delta, Burnaby: some message");
			Assert.AreEqual("Delta", note4.Actors[0].Name);
			Assert.AreEqual("Burnaby", note4.Actors[1].Name);
			Assert.AreEqual(SequenceNotePlacement.Over, note4.Placement);
			Assert.AreEqual(ExpectedMessage, note4.Message);
		}

		[TestMethod]
		public void SequenceGrammarParseNoteMultipleLines()
		{
			var input = new StringBuilder().
				AppendLine("note left of Delta: some message").
				AppendLine("note right of Delta: another message");
			(var seq, string error) = SequenceGrammar.Parse(input.ToString());
			Assert.AreEqual(2, seq.Notes.Count);
			Assert.AreEqual("some message", seq.Notes[0].Message);
			Assert.AreEqual("another message", seq.Notes[1].Message);
		}

		[TestMethod]
		public void SequenceGrammarParseSignal()
		{
			var signal = GetSignalHelper("foo -> bar: message");
			Assert.AreEqual("foo", signal.ActorA.Name);
			Assert.AreEqual("bar", signal.ActorB.Name);
			Assert.AreEqual(SequenceLineType.Solid, signal.LineType);
			Assert.AreEqual(SequenceArrowType.Filled, signal.ArrowType);
			Assert.AreEqual("message", signal.Message);
		}

		[TestMethod]
		public void SequenceGrammarParseSignalLineTypes()
		{
			var signal1 = GetSignalHelper("foo -> bar: message");
			Assert.AreEqual(SequenceLineType.Solid, signal1.LineType);

			var signal2 = GetSignalHelper("foo --> bar: message");
			Assert.AreEqual(SequenceLineType.Dotted, signal2.LineType);
		}

		[TestMethod]
		public void SequenceGrammarParseSignalArrowTypes()
		{
			var signal1 = GetSignalHelper("foo -> bar: message");
			Assert.AreEqual(SequenceArrowType.Filled, signal1.ArrowType);

			var signal2 = GetSignalHelper("foo ->> bar: message");
			Assert.AreEqual(SequenceArrowType.Open, signal2.ArrowType);

			var signal3 = GetSignalHelper("foo-bar: message");
			Assert.AreEqual(SequenceArrowType.Filled, signal1.ArrowType, "Arrow is filled by default");
		}

		[TestMethod]
		public void SequenceGrammarParseSignalSelfReference()
		{
			var signal1 = GetSignalHelper("foo-bar: message");
			Assert.IsFalse(signal1.IsSelf());

			var signal2 = GetSignalHelper("foo-foo: message");
			Assert.IsTrue(signal2.IsSelf());

			var signal3 = GetSignalHelper("foo-->>foo: message");
			Assert.IsTrue(signal3.IsSelf());
			Assert.AreEqual(SequenceLineType.Dotted, signal3.LineType);
			Assert.AreEqual(SequenceArrowType.Open, signal3.ArrowType);
		}

		private static ActorViewModel GetActorHelper(string input)
		{
			(var seq, string error) = SequenceGrammar.Parse(input);
			return seq.Actors.Single();
		}

		private static NoteViewModel GetNoteHelper(string input)
		{
			(var seq, string error) = SequenceGrammar.Parse(input);
			return seq.Notes.Single();
		}

		private static SignalViewModel GetSignalHelper(string input)
		{
			(var seq, string error) = SequenceGrammar.Parse(input);
			return seq.Signals.Single();
		}
	}
}
