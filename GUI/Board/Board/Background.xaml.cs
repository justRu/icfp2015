using Board.Entities;
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

		public void Draw(int w, int h, Unit[] units)
		{
			host.Children.Clear();
			var controlWidht = ActualWidth - 50;
			var controlHeight = ActualHeight - 50;

			var nodeWidth = controlWidht / w;
			var nodeHeight = controlHeight / w;

			var padded = false;

			for (var topIdx = 0; topIdx < h; topIdx++)
			{
				for (var leftIdx = 0; leftIdx < w; leftIdx++)
				{
					DrawHex((leftIdx * nodeWidth) + (padded ? nodeWidth / 2 : 0), topIdx * nodeHeight, nodeWidth, nodeHeight, 4);
				}
				padded = !padded;
			}

			foreach (var unit in units)
			{
				foreach (var memberPos in unit.Members)
				{
					DrawMember(memberPos, nodeWidth, nodeHeight, 16, GenerateColor(unit));
				}
				DrawMember(unit.Pivot, nodeWidth, nodeHeight, 24, Colors.Black);
			}
		}

		private void DrawHex(double xs, double ys, double w, double h, double p)
		{
			var poly = new Polygon();
			
			poly.Points.Add(new Point(w / 2, p));
			poly.Points.Add(new Point(w - p, h / 3));
			poly.Points.Add(new Point(w - p, h / 3 * 2));
			poly.Points.Add(new Point(w / 2, h - p));

			poly.Points.Add(new Point(p, h / 3 * 2));
			poly.Points.Add(new Point(p, h / 3));
			poly.Points.Add(new Point(w / 2, p));

			poly.Fill = Brushes.LightBlue;
			
			Canvas.SetLeft(poly, xs);
			Canvas.SetTop(poly, ys);

			host.Children.Add(poly);
		}

		private void DrawMember(Position pos, double nodeW, double nodeH, double padding, Color color)
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

			host.Children.Add(poly);
		}

		private static Color GenerateColor(Unit unit)
		{
			var key = string.Join("_", unit.Members.Concat(new [] {unit.Pivot}).Select(m => string.Format("{0}.{1}", m.X, m.Y)));
			return ColorGenerator.GetColor(key);
		}
	}
}
