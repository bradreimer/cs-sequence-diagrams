namespace UmlDiagrams
{
	public abstract class AnnotationViewModelBase
	{
		public string Message { get; }

		public AnnotationViewModelBase(string message)
		{
			Message = message;
		}
	}
}