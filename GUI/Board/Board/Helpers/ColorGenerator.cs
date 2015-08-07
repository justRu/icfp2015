﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Board.Helpers
{
	internal static class ColorGenerator
	{
		private static readonly Dictionary<string, Color> _colorsCache = new Dictionary<string, Color>();

		internal static Color GetColor(string key)
		{
			Color result;
			if (_colorsCache.TryGetValue(key, out result))
			{
				return result;
			}

			var hash = key.GetHashCode();
			result = HsvToRgb(hash, 0.2, 0.9);
			result.A = 255;
			_colorsCache.Add(key, result);
			return result;
		}

		private static Color HsvToRgb(double h, double S, double V)
		{
			// ######################################################################
			// T. Nathan Mundhenk
			// mundhenk@usc.edu
			// C/C++ Macro HSV to RGB

			double H = h;
			while (H < 0) { H += 360; };
			while (H >= 360) { H -= 360; };
			double R, G, B;
			if (V <= 0)
			{ R = G = B = 0; }
			else if (S <= 0)
			{
				R = G = B = V;
			}
			else
			{
				double hf = H / 60.0;
				int i = (int)Math.Floor(hf);
				double f = hf - i;
				double pv = V * (1 - S);
				double qv = V * (1 - S * f);
				double tv = V * (1 - S * (1 - f));
				switch (i)
				{

					// Red is the dominant color

					case 0:
						R = V;
						G = tv;
						B = pv;
						break;

					// Green is the dominant color

					case 1:
						R = qv;
						G = V;
						B = pv;
						break;
					case 2:
						R = pv;
						G = V;
						B = tv;
						break;

					// Blue is the dominant color

					case 3:
						R = pv;
						G = qv;
						B = V;
						break;
					case 4:
						R = tv;
						G = pv;
						B = V;
						break;

					// Red is the dominant color

					case 5:
						R = V;
						G = pv;
						B = qv;
						break;

					// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

					case 6:
						R = V;
						G = tv;
						B = pv;
						break;
					case -1:
						R = V;
						G = pv;
						B = qv;
						break;

					// The color is not defined, we should throw an error.

					default:
						//LFATAL("i Value error in Pixel conversion, Value is %d", i);
						R = G = B = V; // Just pretend its black/white
						break;
				}
			}
			var r = Clamp((int)(R * 255.0));
			var g = Clamp((int)(G * 255.0));
			var b = Clamp((int)(B * 255.0));
			return Color.FromRgb(r, g, b);
		}

		/// <summary>
		/// Clamp a value to 0-255
		/// </summary>
		private static byte Clamp(int i)
		{
			if (i < 0) return 0;
			if (i > 255) return 255;
			return (byte)i;
		}
	}
}
