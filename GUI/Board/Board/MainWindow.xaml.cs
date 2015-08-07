using Board.Engine;
using Board.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Board.Entities;
using Newtonsoft.Json;

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

		private void Update()
		{
			background.Draw(_input);
		}
	}
}
