namespace UmlDiagrams
{
	/// <summary>
	/// A signal describes a message that is sent between two participating actors.
	/// </summary>
	public sealed class SignalViewModel : AnnotationViewModelBase
	{
		internal SignalViewModel(
			ActorViewModel actorA, SequenceLineType lineType, SequenceArrowType arrowType,
			ActorViewModel actorB, string message)
			: base(message)
		{
			ActorA = actorA;
			ActorB = actorB;
			LineType = lineType;
			ArrowType = arrowType;
		}

		public ActorViewModel ActorA { get; }
		public ActorViewModel ActorB { get; }
		public SequenceLineType LineType { get; }
		public SequenceArrowType ArrowType { get; }

		public bool IsSelf()
		{
			return ActorA.Index == ActorB.Index;
		}
	}
}