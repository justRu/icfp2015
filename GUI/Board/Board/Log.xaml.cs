using Solver;
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
	/// Interaction logic for Log.xaml
	/// </summary>
	public partial class Log : UserControl
	{
		public Log()
		{
			InitializeComponent();
		}

		public void LogMessage(string message)
		{
			log.AppendText(message + Environment.NewLine);
		}

		public void LogMessage(MoveDirection[] moves)
		{
			log.AppendText((moves == null || moves.Length == 0 ? "empty" : string.Join(", ", moves)) + Environment.NewLine);
		}
	}
}
