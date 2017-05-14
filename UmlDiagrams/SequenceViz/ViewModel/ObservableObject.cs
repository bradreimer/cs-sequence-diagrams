using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SequenceViz
{
	class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected bool Set<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(target, value))
				return false;

			target = value;
			RaisePropertyChanged(propertyName);
			return true;
		}

		protected bool Set<T>(ref T target, T value, params string[] propertyNames)
		{
			if (!EqualityComparer<T>.Default.Equals(target, value))
				return false;

			target = value;
			foreach (string propertyName in propertyNames)
				RaisePropertyChanged(propertyName);
			return true;
		}

		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
