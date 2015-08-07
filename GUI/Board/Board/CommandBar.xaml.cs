using Board.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Board
{
	/// <summary>
	/// Interaction logic for CommandBar.xaml
	/// </summary>
	public partial class CommandBar : UserControl
	{
		public event EventHandler SpawnEvent;
		public event EventHandler<MoveDirection> Move;
		public event EventHandler<double> Rotate;

		public CommandBar()
		{
			InitializeComponent();
			DataContext = new Controller(this);
		}

		internal void InvokeCommand(string cmd)
		{
			switch (cmd)
			{
				case "spawn":
					SpawnEvent(this, null);
					break;
				case "moveE":
					Move(this, MoveDirection.E);
					break;
				case "moveW":
					Move(this, MoveDirection.W);
					break;
				case "moveSE":
					Move(this, MoveDirection.SE);
					break;
				case "moveSW":
					Move(this, MoveDirection.SW);
					break;
				case "rotateClock":
					Rotate(this, 60);
					break;
				case "rotateCounterClock":
					Rotate(this, -60);
					break;
			}
		}
	}

	public class Controller
	{
		private readonly CommandBar _host;

		private ICommand _clickCommand;
		public ICommand ClickCommand
		{
			get
			{
				return _clickCommand;
			}
		}

		public Controller(CommandBar host)
		{
			_clickCommand = new CommandHandler(host);
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
