using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SequenceViz
{
	class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void Set<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(target, value))
			{
				target = value;
				RaisePropertyChanged(propertyName);
			}
		}

		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
