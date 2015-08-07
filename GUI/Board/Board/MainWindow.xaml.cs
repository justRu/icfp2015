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

		public MainWindow()
		{
			InitializeComponent();

			_input = JsonConvert.DeserializeObject<Input>(File.ReadAllText("input.json"));

			Loaded += (s, e) => Update();
			SizeChanged += (s, e) => Update();
		}

		private void Update()
		{
			background.Draw(_input);
		}
	}
}
