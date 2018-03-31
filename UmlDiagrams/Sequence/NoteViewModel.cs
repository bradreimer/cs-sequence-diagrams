using System;

namespace UmlDiagrams
{
	/// <summary>
	/// A note annotates a sequence diagram with useful text.
	/// </summary>
	public sealed class NoteViewModel : AnnotationViewModelBase
	{
		internal NoteViewModel(ActorViewModel[] actors, SequenceNotePlacement placement, string message)
			: base(message)
		{
			Actors = actors;
			Placement = placement;

			if (actors.Length > 1 && actors[0] == actors[1])
				throw new ArgumentException(nameof(actors), "Note should overlap two different actors");
		}

		public ActorViewModel[] Actors { get; }
		public SequenceNotePlacement Placement { get; }
	}
}