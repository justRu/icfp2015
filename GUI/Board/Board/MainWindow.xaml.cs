﻿using Board.Entities;
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
			_currentUnit = Translate(_currentUnit, e);
			background.DrawUnit(_input, _currentUnit);
		}

		private void commandBarRotate(object sender, double e)
		{
			_currentUnit = Rotate(_currentUnit, e);
			background.DrawUnit(_input, _currentUnit);
		}

		private Unit Translate(Unit unit, MoveDirection direction)
		{
			switch (direction)
			{
				case MoveDirection.SE:
					return Translate(unit, (unit.Pivot.Y % 2) == 0 ? 0 : 1, 1);
				case MoveDirection.SW:
					return Translate(unit, (unit.Pivot.Y % 2) == 0 ? -1 : 0, 1);
				case MoveDirection.E:
					return Translate(unit, 1, 0);
				case MoveDirection.W:
					return Translate(unit, -1, 0);
				default:
					return unit;
			}
		}

		private Unit Translate(Unit unit, int dx, int dy)
		{
			var pivot = new Position
			{
				X = unit.Pivot.X + dx,
				Y = unit.Pivot.Y + dy
			};
			return new Unit
			{
				Pivot = pivot,
				Members = unit.Members.Select(member =>
				new Position
					{
						X = member.X + dx,
						Y = member.Y + dy
					}
				).ToArray()
			};
		}

		private Unit Rotate(Unit unit, double degrees)
		{
			return new Unit
			{
				Pivot = unit.Pivot,
				Members = unit.Members.Select(member => Rotate(member, unit.Pivot, degrees)).ToArray()
			};
		}

		private Position Rotate(Position point, Position pivot, double degrees)
		{
#warning I've been looking you all of my life
			var radians = degrees * Math.PI / 180;
			var relX = point.X - pivot.X;
			var relY = point.Y - pivot.Y;

			var nrelx = relX * Math.Cos(radians) - relY * Math.Sin(radians);
			var nrely = relX * Math.Sin(radians) + relY * Math.Cos(radians);

			var len = Math.Sqrt(Math.Pow(relX, 2) + Math.Pow(relY, 2));

			var absX = (pivot.X + nrelx) * len;
			var absY = (pivot.Y + nrely) * len;
			return new Position { X = (int)absX, Y = (int)absY };
		}

		private void Update()
		{
			background.Draw(_input);
		}
	}
}
