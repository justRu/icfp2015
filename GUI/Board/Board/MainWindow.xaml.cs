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

		public MainWindow()
		{
			InitializeComponent();

			_input = JsonConvert.DeserializeObject<Input>(File.ReadAllText("input.json"));

			_execution = new ExecutionRequest
			{
				Snapshot = new Snapshot(_input, _input.SourceSeeds.First()),
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

		private void commandBarStartSolver(object sender, EventArgs e)
		{
			Task.Run(() =>
				{
					var results = _solver.Solve(_execution).ToArray();
					foreach (var result in results)
					{
						
						
						Dispatcher.Invoke(() => {
							log.LogMessage(result.Commands);
							ShowSnapshot(result.Snapshot);
						});
						Thread.Sleep(3000);
					}
				}
				);
			
		}

		private void commandBarSpawnEvent(object sender, EventArgs e)
		{
			_currentUnit = Moving.Spawn(_input, _input.Units.First());

			background.DrawUnit(_execution.Snapshot.Field, _currentUnit);
		}

		private void commandBarMove(object sender, MoveDirection e)
		{
			_currentUnit = Moving.Translate(_currentUnit, e);
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
