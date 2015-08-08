using System;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Solver;

namespace Board
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Input _input;
		private readonly ExecutionRequest _execution;
		private readonly ISolver _solver;

		private Unit _currentUnit;

		public MainWindow()
		{
			InitializeComponent();

			_input = JsonConvert.DeserializeObject<Input>(File.ReadAllText("input.json"));

			_execution = new ExecutionRequest
			{
				Snapshot = new Snapshot(_input, _input.SourceSeeds.First()),
				Options = new ExecutionOptions
				{
					MaxWidth = 3,
					MaxHeight = 8,
					MinEstimation = double.MinValue
				}
			};

			_solver = new IterativeSearchSolver(1200); //new TraverseSolver();

			SizeChanged += (s, e) => Update();

			commandBar.SpawnEvent += commandBarSpawnEvent;
			commandBar.Move += commandBarMove;
			commandBar.StartSolver += commandBarStartSolver;
		}

		private void commandBarStartSolver(object sender, EventArgs e)
		{
			var results = _solver.Solve(_execution).First();
			var model = new SolutionViewModel(results, _execution.Snapshot);
			solution.DataContext = model;
			model.SnapshotChanged += ShowSnapshot;
			ShowSnapshot(_execution.Snapshot);
		}

		private void commandBarSpawnEvent(object sender, EventArgs e)
		{
			_currentUnit = Moving.Spawn(_input, _input.Units.First());
			background.DrawUnit(_execution.Snapshot.Field, _currentUnit, null);
		}

		private void commandBarMove(object sender, MoveDirection e)
		{
			_currentUnit = Moving.Translate(_currentUnit, e);
			background.DrawUnit(_execution.Snapshot.Field, _currentUnit, null);
		}

		private void Update()
		{
			background.DrawUnit(_execution.Snapshot.Field, _currentUnit, null);
		}

		private void ShowSnapshot(Snapshot snapshot)
		{
			background.DrawUnit(snapshot.Field, snapshot.CurrentUnit, snapshot.UnitIndex);
		}
	}
}
