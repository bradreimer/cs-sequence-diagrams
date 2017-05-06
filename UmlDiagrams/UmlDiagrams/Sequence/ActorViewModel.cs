using System;

namespace UmlDiagrams
{
	/// <summary>
	/// An actor participates in a sequence of events.
	/// </summary>
	public sealed class ActorViewModel
	{
		/// <summary>
		/// Initializes a new instance of the ActorViewModel class.
		/// </summary>
		public ActorViewModel(string alias, string name, int index)
		{
			if (string.IsNullOrEmpty(alias))
				throw new ArgumentNullException(nameof(alias));
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			Alias = alias;
			Name = name;
			Index = index;
		}

		/// <summary>
		/// Gets a value indentifying this actor.
		/// </summary>
		public string Alias { get; }

		/// <summary>
		///Gets the full name to use when displaying this actor.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the index of this actor within its parent sequence.
		/// </summary>
		public int Index { get; }
	}
}