using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using static UmlDiagrams.SequenceParser;

namespace UmlDiagrams
{
	public static class SequenceGrammar
	{
		/// <summary>
		/// Parse the given input into a description of a sequence diagram.
		/// </summary>
		/// <param name="input">The input string</param>
		/// <returns></returns>
		public static (SequenceDiagramViewModel SequenceDiagram, string Error) Parse(string input)
		{
			var inputStream = new CaseInsensitiveInputStream(input);
			var lexer = new SequenceLexer(inputStream);
			var tokenStream = new CommonTokenStream(lexer);
			var parser = new SequenceParser(tokenStream);

			// Report errors to the console (for now)
			var error = new StringBuilder();
			var errorListener = new StringErrorListener(error);
			lexer.AddErrorListener(errorListener);
			parser.AddErrorListener(errorListener);

			// Generate SequenceDiagramViewModel out of parsed result
			StartContext startContext = parser.start();
			SequenceVisitor visitor = new SequenceVisitor(errorListener);
			visitor.Visit(startContext);

			return (visitor.SequenceDiagram, error.ToString());
		}

		/// <summary>
		/// Ensures that our lexer produces case-insensitive matches.
		/// </summary>
		private sealed class CaseInsensitiveInputStream : AntlrInputStream
		{
			public CaseInsensitiveInputStream(string input) : base(input) { }

			public override int La(int i)
			{
				int value = base.La(i);
				if (value == (value & 0xFF))
				{
					char c = (char)value;
					if (char.IsLetter(c))
						return char.ToLowerInvariant(c);
				}
				return value;
			}
		}

		/// <summary>
		/// This class transforms parsed data into a SequenceDiagramViewModel.
		/// </summary>
		private sealed class SequenceVisitor : SequenceBaseVisitor<SequenceDiagramViewModel>
		{
			public readonly SequenceDiagramViewModel SequenceDiagram = new SequenceDiagramViewModel();
			private readonly StringErrorListener m_errorListener;

			public SequenceVisitor(StringErrorListener errorListener)
			{
				m_errorListener = errorListener;
			}

			public override SequenceDiagramViewModel VisitParticipant([NotNull] ParticipantContext context)
			{
				string alias = context.alias?.Text;
				string name = context.name?.Text;
				SequenceDiagram.GetOrCreateActor(alias, name);
				return SequenceDiagram;
			}

			public override SequenceDiagramViewModel VisitNote([NotNull] NoteContext context)
			{
				// Extract actors
				ActorViewModel[] actors = GetActors(context.actor(), context.actorPair());
				if (actors == null || actors.Any(a => a == null))
				{
					m_errorListener.GrammarError("Notes must be declared relative to an actor. e.g. Note left of A: Message");
					return null;
				}

				// Extract placement of note
				SequenceNotePlacement placement = GetPlacement(context.placement());

				// Extract the note's message
				string message = GetMessage(context.message());

				// Combine into a note
				SequenceDiagram.AddNote(actors, placement, message);
				return SequenceDiagram;
			}

			public override SequenceDiagramViewModel VisitTitle([NotNull] TitleContext context)
			{
				string title = GetMessage(context.message());

				// Update title (last instance wins)
				SequenceDiagram.Title = title;
				return SequenceDiagram;
			}

			public override SequenceDiagramViewModel VisitSignal([NotNull] SignalContext context)
			{
				var actorA = GetActor(context.actor(0));
				var actorB = GetActor(context.actor(1));

				if (actorA == null || actorB == null)
				{
					m_errorListener.GrammarError("Signals must occur between two actors. e.g. a->b: pew pew");
					return SequenceDiagram;
				}

				var signalType = GetSignalType(context.signalType());
				var message = GetMessage(context.message());
				SequenceDiagram.AddSignal(actorA, signalType.Item1, signalType.Item2, actorB, message);
				return SequenceDiagram;
			}

			private static Tuple<SequenceLineType, SequenceArrowType> GetSignalType(SignalTypeContext context)
			{
				LineTypeContext lineTypeContext = context.lineType();
				SequenceLineType lineType = lineTypeContext?.DOTLINE() != null ? SequenceLineType.Dotted : SequenceLineType.Solid;

				ArrowTypeContext arrowTypeContext = context.arrowType();
				SequenceArrowType arrowType = arrowTypeContext?.OPENARROW() != null ? SequenceArrowType.Open : SequenceArrowType.Filled;

				return Tuple.Create(lineType, arrowType);
			}

			private static string GetMessage(MessageContext context)
			{
				string text = context.GetText();

				// Trim initial colon and whitespace
				text = text.Substring(1).Trim();

				// Expand escape sequences
				return Regex.Unescape(text);
			}

			private ActorViewModel GetActor(ActorContext actor)
			{
				var alias = actor.GetText();
				if (string.IsNullOrEmpty(alias))
					return null;
				return SequenceDiagram.GetOrCreateActor(alias, null);
			}

			private ActorViewModel[] GetActors(ActorContext actor, ActorPairContext actorPair)
			{
				if (actor != null)
					return new[] { GetActor(actor) };
				else if (actorPair != null)
					return (from item in actorPair.actor() select GetActor(item)).ToArray();
				else
					return null;
			}

			private static SequenceNotePlacement GetPlacement(PlacementContext context)
			{
				if (context == null)
					return SequenceNotePlacement.Over;

				if (context.LEFT_OF() != null)
					return SequenceNotePlacement.LeftOf;

				if (context.RIGHT_OF() != null)
					return SequenceNotePlacement.RightOf;

				throw new NotSupportedException();
			}
		}
	}
}
