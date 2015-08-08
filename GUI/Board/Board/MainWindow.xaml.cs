using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Solver;
using System.Threading;
using System.Threading.Tasks;

namespace Board
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Input _input;
		private readonly ExecutionRequest _execution;
		private readonly SerialSolver _solver;

		private Unit _currentUnit;
		private Snapshot _initialSnapshot;

		public MainWindow()
		{
			InitializeComponent();

			_input = JsonConvert.DeserializeObject<Input>(File.ReadAllText("input.json"));

			_initialSnapshot = new Snapshot(_input, _input.SourceSeeds.First());

			_execution = new ExecutionRequest
			{
				Snapshot = _initialSnapshot,
				Options = new ExecutionOptions
				{
					MaxWidth = 2,
					MaxHeight = 3,
					MinEstimation = double.MinValue
				}
			};

			_solver = new SerialSolver();

			SizeChanged += (s, e) => Update();

			commandBar.SpawnEvent += commandBarSpawnEvent;
			commandBar.Move += commandBarMove;
			commandBar.StartSolver += commandBarStartSolver;
		}

		private async void commandBarStartSolver(object sender, EventArgs e)
		{
			var results = await Task.Run(() => _solver.Solve(_execution).ToArray());
			foreach (var result in results)
			{
				log.LogMessage("Result: " + result.Estimation);
				log.LogMessage(result.Commands);
				var snapshot = _initialSnapshot;
				ShowSnapshot(snapshot);
				await Task.Delay(1000);
				foreach (var command in result.Commands)
				{
					snapshot = snapshot.New(command);
					ShowSnapshot(snapshot);
					await Task.Delay(1000);
				}
			}
		}

		private void commandBarSpawnEvent(object sender, EventArgs e)
		{
			_currentUnit = Moving.Spawn(_input, _input.Units.First());

			background.DrawUnit(_execution.Snapshot.Field, _currentUnit);
		}

		private void commandBarMove(object sender, MoveDirection e)
		{
			_currentUnit = _currentUnit.Translate(e);
			background.DrawUnit(_execution.Snapshot.Field, _currentUnit);
		}

		private void Update()
		{
			background.DrawUnit(_execution.Snapshot.Field, _currentUnit);
		}

		private void ShowSnapshot(Snapshot snapshot)
		{
			background.DrawUnit(snapshot.Field, snapshot.CurrentUnit);
		}
	}
}
