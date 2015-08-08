using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Board.Helpers
{
	/// <summary>
	/// Represents a base class for a view model.
	/// </summary>
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		protected ViewModelBase()
		{
			Dispatcher = Dispatcher.CurrentDispatcher;
		}

		public Dispatcher Dispatcher { get; private set; }

		/// <summary>
		/// Verify if the current thread is the same thread which created this object.
		/// </summary>
		public void VerifyAccess()
		{
			Dispatcher.VerifyAccess();
		}

		/// <summary>
		/// Verifies current thread and runs <paramref name="action"/> either in current thread
		/// (if it's a UI thread) or in the original thread via <see cref="Dispatcher"/>.
		/// </summary>
		/// <param name="action">Action to execute.</param>
		public void CheckAccessExecute(Action action)
		{
			if (Dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				Dispatcher.Invoke(action);
			}
		}

		/// <summary>
		/// Verifies current thread and runs <paramref name="func"/> either in current thread
		/// (if it's a UI thread) or in the original thread via <see cref="Dispatcher"/>.
		/// </summary>
		/// <param name="func">Function to get value from.</param>
		public T CheckAccessExecute<T>(Func<T> func)
		{
			if (Dispatcher.CheckAccess())
			{
				return func();
			}
			return (T)Dispatcher.Invoke(func);
		}

		/// <summary>
		/// Sets the field <paramref name="field"/> to the new value <paramref name="value"/> (if value is different)
		/// and raises <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <typeparam name="T">Field type.</typeparam>
		/// <param name="field">Referemce to field.</param>
		/// <param name="value">New value.</param>
		/// <param name="propertyName">Property name (do not specify manually, let the compiler do it).</param>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SetAndRaisePropertyChanged<T>(
			ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(field, value))
				return;
			field = value;
			RaisePropertyChanged(propertyName);
		}

		/// <summary>
		/// Raises an event about property value has been changed.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <remarks>We recommend you to use <see cref="DependOnAttribute"/> for other options
		/// instead of calling this method if possible.</remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
					"propertyName name is empty, either you pass null or using old compiler");
			}
			VerifyAccess();
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}