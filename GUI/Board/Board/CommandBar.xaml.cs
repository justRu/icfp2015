using System;
using System.Windows.Controls;
using System.Windows.Input;
using Board.Helpers;
using Solver;

namespace Board
{
	/// <summary>
	/// Interaction logic for CommandBar.xaml
	/// </summary>
	public partial class CommandBar : UserControl
	{
		public event EventHandler SpawnEvent;
		public event EventHandler<MoveDirection> Move;
		public event EventHandler StartSolver;

		public CommandBar()
		{
			InitializeComponent();
			DataContext = new Controller(this);
		}

		internal void InvokeCommand(string cmd)
		{
			if (cmd == "spawn")
			{
				SpawnEvent(this, null);
				return;
			}
			if (cmd == "StartSolver")
			{
#warning One-time debugger, yeah.
				btnSolverStart.IsEnabled = false;
				StartSolver(this, null);
				return;
			}
			MoveDirection move;
			if (!Enum.TryParse(cmd, true, out move))
			{
				throw new InvalidOperationException("Bad command: " + cmd);
			}
			Move(this, move);
		}
	}

	public class Controller
	{
		public ICommand ClickCommand { get; private set; }

		public Controller(CommandBar host)
		{
			ClickCommand = new DelegateCommand(arg => host.InvokeCommand((string)arg), _ => true);
		}
	}
}
