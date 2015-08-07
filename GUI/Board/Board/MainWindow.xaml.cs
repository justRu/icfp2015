using Board.Engine;
using Board.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Input _input;

		private Unit _currentUnit;

		public MainWindow()
		{
			InitializeComponent();

			_input = JsonConvert.DeserializeObject<Input>(File.ReadAllText("input.json"));

			Loaded += (s, e) => Update();
			SizeChanged += (s, e) => Update();

			commandBar.SpawnEvent += commandBarSpawnEvent;
			commandBar.Move += commandBarMove;
			commandBar.Rotate += commandBarRotate;
		}

		private void commandBarSpawnEvent(object sender, EventArgs e)
		{
			_currentUnit = _input.Units.First();
			background.DrawUnit(_input, _currentUnit);
		}

		private void commandBarMove(object sender, MoveDirection e)
		{
			_currentUnit = Moving.Translate(_currentUnit, e);
			background.DrawUnit(_input, _currentUnit);
		}

		private void commandBarRotate(object sender, double e)
		{
			_currentUnit = Moving.Rotate(_currentUnit, e);
			background.DrawUnit(_input, _currentUnit);
		}

		private void Update()
		{
			background.Draw(_input);
		}
	}
}
