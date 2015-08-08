using Board.Helpers;
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
using Solver;

namespace Board
{
	/// <summary>
	/// Interaction logic for Background.xaml
	/// </summary>
	public partial class Background : UserControl
	{
		public Background()
		{
			InitializeComponent();
		}

		public void DrawUnit(Field field, Unit unit, int? unitIndex)
		{
			var nodeWidth = GetNodeWidth(field.Width);
			var nodeHeight = GetNodeHeight(field.Height);
			Draw(field, nodeWidth, nodeHeight);

			if (unit.Members != null)
			{
				var color = HasCollision(field, unit) ? Colors.Red : Colors.Green;

				// Render unit
				foreach (var memberPos in unit.Members)
				{
					DrawMember(memberPos, nodeWidth, nodeHeight, 16, color, null);
				}
				DrawMember(unit.Pivot, nodeWidth, nodeHeight, 24, Colors.Black, unitIndex.HasValue ? unitIndex.Value.ToString() : null);
			}
		}

		private void Draw(Field field, double nodeWidth, double nodeHeight)
		{
			host.Children.Clear();

			var padded = false;

			var filledHash = GetFilledCells(field);

			// Render hexs
			for (var topIdx = 0; topIdx < field.Height; topIdx++)
			{
				for (var leftIdx = 0; leftIdx < field.Width; leftIdx++)
				{
					DrawHex((leftIdx * nodeWidth) + (padded ? nodeWidth / 2 : 0), topIdx * nodeHeight, nodeWidth, nodeHeight, 4,
						filledHash.Contains(new Position { X = leftIdx, Y = topIdx}) ? Colors.Blue : Colors.LightBlue);
				}
				padded = !padded;
			}
		}

		/*public void DrawUnits(Input input)
		{
			var nodeWidth = GetNodeWidth(input.Width);
			var nodeHeight = GetNodeHeight(input.Height);

			// Render units
			foreach (var unit in input.Units)
			{
				foreach (var memberPos in unit.Members)
				{
					DrawMember(memberPos, nodeWidth, nodeHeight, 16, GenerateColor(unit));
				}
				DrawMember(unit.Pivot, nodeWidth, nodeHeight, 24, Colors.Black);
			}
		}*/

		private void DrawHex(double xs, double ys, double w, double h, double p, Color color)
		{
			var poly = new Polygon();
			
			poly.Points.Add(new Point(w / 2, p));
			poly.Points.Add(new Point(w - p, h / 3));
			poly.Points.Add(new Point(w - p, h / 3 * 2));
			poly.Points.Add(new Point(w / 2, h - p));

			poly.Points.Add(new Point(p, h / 3 * 2));
			poly.Points.Add(new Point(p, h / 3));
			poly.Points.Add(new Point(w / 2, p));

			poly.Fill = new SolidColorBrush(color);
			
			Canvas.SetLeft(poly, xs);
			Canvas.SetTop(poly, ys);

			host.Children.Add(poly);
		}

		private void DrawMember(Position pos, double nodeW, double nodeH, double padding, Color color, string title)
		{
			var poly = new Polygon();

			poly.Points.Add(new Point(padding, padding));
			poly.Points.Add(new Point(nodeW - padding, padding));
			poly.Points.Add(new Point(nodeW - padding, nodeH - padding));
			poly.Points.Add(new Point(padding, nodeH - padding));
			poly.Points.Add(new Point(padding, padding));

			poly.Fill = new SolidColorBrush(color);

			var xs = pos.X * nodeW + (pos.Y % 2 == 0 ? 0.0 :  nodeW / 2);
			var ys = pos.Y * nodeH;

			Canvas.SetLeft(poly, xs);
			Canvas.SetTop(poly, ys);


			if (title != null)
			{
				var titleBlock = new TextBlock();
				titleBlock.Text = title;
				Canvas.SetLeft(titleBlock, xs);
				Canvas.SetTop(titleBlock, ys);
				host.Children.Add(titleBlock);
			}

			host.Children.Add(poly);
		}

		private double GetNodeWidth(double w)
		{
			var controlWidht = ActualWidth - 50;
			return controlWidht / w;
		}

		private double GetNodeHeight(double h)
		{
			var controlHeight = ActualHeight - 50;
			return controlHeight / h;
		}

		private static Color GenerateColor(Unit unit)
		{
			var key = string.Join("_", unit.Members.Concat(new [] {unit.Pivot}).Select(m => string.Format("{0}.{1}", m.X, m.Y)));
			return ColorGenerator.GetColor(key);
		}

		private static HashSet<Position> GetFilledCells(Field field)
		{
			var result = new HashSet<Position>();
			for (var x = 0; x < field.Width; x++)
			{
				for (var y = 0; y < field.Height; y++)
				{
					if (field[x, y])
					{
						result.Add(new Position(x, y));
					}
				}
			}
			return result;
		}

		private static bool HasCollision(Field field, Unit unit)
		{
			var filled = GetFilledCells(field);
			foreach (var member in unit.Members)
			{
				if (member.X < 0 || member.Y < 0 || member.X >= field.Width || member.Y >= field.Height)
				{
					return true;
				}
				if (filled.Contains(member))
				{
					return true;
				}
			}
			return false;
		}
	}
}
