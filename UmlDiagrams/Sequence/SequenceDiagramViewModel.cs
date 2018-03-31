using System;
using System.Collections.Generic;
using System.Linq;

namespace UmlDiagrams
{
	/// <summary>
	/// Describes a sequence diagram that can be draw using a <see cref="SequenceDiagram"/> control.
	/// </summary>
	public sealed class SequenceDiagramViewModel
	{
		/// <summary>
		/// Gets or sets title text used to describe this sequence.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets a list of actors that participate this this sequence.
		/// </summary>
		public List<ActorViewModel> Actors { get; } = new List<ActorViewModel>();

		/// <summary>
		/// Gets a list of all annotations (i.e. signals and notes) in their correct order.
		/// </summary>
		public List<AnnotationViewModelBase> Annotations { get; } = new List<AnnotationViewModelBase>();

		/// <summary>
		/// Gets a list of signals (i.e. arrows describing actions).
		/// </summary>
		public List<SignalViewModel> Signals => Annotations.OfType<SignalViewModel>().ToList();
	
		/// <summary>
		/// Gets a list of notes annotating this sequence.
		/// </summary>
		public List<NoteViewModel> Notes => Annotations.OfType<NoteViewModel>().ToList();

		/// <summary>
		/// Gets an existing actor with this alias, or creates a new one with alias and name.
		/// </summary>
		public ActorViewModel GetOrCreateActor(string alias, string name)
		{
			if (string.IsNullOrEmpty(alias))
				throw new ArgumentNullException(nameof(alias));

			// Trim whitespace and quotes from alias and name
			alias = alias.Trim().Trim('\"');
			if (name != null)
				name = name.Trim().Trim('\"');

			foreach (ActorViewModel i in Actors)
			{
				if (string.Equals(i.Alias, alias, StringComparison.OrdinalIgnoreCase))
					return i;
			}

			ActorViewModel actor = new ActorViewModel(alias, name ?? alias, Actors.Count);
			Actors.Add(actor);
			return actor;
		}

		/// <summary>
		/// Adds a signal.
		/// </summary>
		public void AddSignal(ActorViewModel actorA, SequenceLineType lineType, SequenceArrowType arrowType, ActorViewModel actorB, string message)
		{
			Annotations.Add(new SignalViewModel(actorA, lineType, arrowType, actorB, message));
		}

		/// <summary>
		/// Adds a note.
		/// </summary>
		/// <param name="actors"></param>
		/// <param name="placement"></param>
		/// <param name="message"></param>
		public void AddNote(ActorViewModel[] actors, SequenceNotePlacement placement, string message)
		{
			if (actors == null)
				throw new ArgumentNullException(nameof(actors));
			if (message == null)
				throw new ArgumentNullException(nameof(message));
			Annotations.Add(new NoteViewModel(actors, placement, message));
		}
	}
}