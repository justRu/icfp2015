using System;
using System.Windows.Controls;
using System.Windows.Input;
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
			ClickCommand = new CommandHandler(host);
		}
	}

	public class CommandHandler : ICommand
	{
		private readonly CommandBar _host;
		public CommandHandler(CommandBar host)
		{
			_host = host;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			var cmd = (string)parameter;
			_host.InvokeCommand(cmd);
		}
	}
}
