using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Solver;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

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
			var results = _solver.Solve(_execution);

			Snapshot prevSnapshot = _execution.Snapshot;
			ExecutionResult currentResultDisplay = null;
			var enumerator = results.GetEnumerator();
			IEnumerator<MoveDirection> moveEnumerator = null;
			commandBar.NextSolverStep += (s, ee) =>
				{
					if (enumerator.MoveNext())
					{
						if (currentResultDisplay != null)
						{
							prevSnapshot = currentResultDisplay.Snapshot;
						}
						currentResultDisplay = enumerator.Current;
						moveEnumerator = currentResultDisplay.Commands.Cast<MoveDirection>().GetEnumerator();
						log.LogMessage("Got commands:");
						log.LogMessage(currentResultDisplay.Commands);
						
						//ShowSnapshot(currentResultDisplay.Snapshot);
					}
				};

			commandBar.NextMoveStep += (s, ee) =>
				{
					if (moveEnumerator == null)
					{
						log.LogMessage("missing cmds");
						return;
					}

					if (moveEnumerator.MoveNext())
					{
						log.LogMessage("  cmd: " + moveEnumerator.Current);
						var movedSnapshot = Game.MakeMove(currentResultDisplay.Snapshot, moveEnumerator.Current);
						ShowSnapshot(movedSnapshot);
					}
					else
					{
						log.LogMessage(" no more cmds");
					}
				};

			commandBar.ShowInSnapshot += (s, ee) =>
				{
					if (currentResultDisplay != null)
					{
						moveEnumerator = currentResultDisplay.Commands.Cast<MoveDirection>().GetEnumerator();
					}
					ShowSnapshot(prevSnapshot);
				};

			commandBar.ShowOutSnapshot += (s, ee) =>
				{
					if (currentResultDisplay != null)
					{
						ShowSnapshot(currentResultDisplay.Snapshot);
					}
					else
					{
						log.LogMessage(" missing out snapshot");
					}
				};
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
