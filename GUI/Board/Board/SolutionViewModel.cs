using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Board.Helpers;
using Solver;

namespace Board
{
	public sealed class SolutionViewModel : ViewModelBase
	{
		private int _currentIndex;
		private Snapshot _currentSnapshot;

		public SolutionViewModel(ExecutionResult result, Snapshot initialSnapshot)
		{
			InitialSnapshot = initialSnapshot;
			CurrentSnapshot = initialSnapshot;
			CurrentIndex = 0;
			Commands = result.Commands;
			Estimate = result.Estimate;

			MoveNextCommand = new DelegateCommand(MoveNext, () => CurrentIndex < Commands.Length);
			ResetCommand = new DelegateCommand(Reset);
			PlayCommand = new DelegateCommand(Play);
			MoveToFinalCommand = new DelegateCommand(MoveToFinal);
		}

		public event Action<Snapshot> SnapshotChanged = delegate {};

		private void Reset()
		{
			CurrentSnapshot = InitialSnapshot;
			CurrentIndex = 0;
			SnapshotChanged(CurrentSnapshot);
		}

		private void MoveNext()
		{
			MoveNextInternal();
			SnapshotChanged(CurrentSnapshot);
		}

		private void MoveNextInternal()
		{
			CurrentSnapshot = Game.MakeMove(CurrentSnapshot, Commands[CurrentIndex]);
			CurrentIndex++;
		}

		public void MoveToIndex()
		{
			CurrentSnapshot = InitialSnapshot;
			int index = CurrentIndex;
			CurrentIndex = 0;
			for (int i = 0; i < index; i++)
			{
				MoveNextInternal();
			}
			SnapshotChanged(CurrentSnapshot);
		}

		private void MoveToFinal()
		{
			CurrentIndex = Commands.Length - 1;
			MoveToIndex();
		}

		private async void Play()
		{
			while (CurrentIndex < Commands.Length)
			{
				MoveNext();
				await Task.Delay(20);
			}
		}

		public ICommand MoveNextCommand { get; set; }

		public ICommand ResetCommand { get; set; }

		public ICommand PlayCommand { get; set; }

		public ICommand MoveToFinalCommand { get; set; }

		public int CurrentIndex
		{
			get { return _currentIndex; }
			set { this.SetAndRaisePropertyChanged(ref _currentIndex, value); }
		}

		public Snapshot InitialSnapshot { get; set; }

		public Snapshot CurrentSnapshot
		{
			get { return _currentSnapshot; }
			set { SetAndRaisePropertyChanged(ref _currentSnapshot, value); }
		}

		public MoveDirection[] Commands { get; set; }

		public double Estimate { get; set; }
	}
}
