using System;

namespace Solver
{
	public struct Unit
	{
		public Position[] Members;

		public Position Pivot;

		public int GetMaxX()
		{
			int res = Members[0].X;
			for (int i = 1; i < Members.Length; i++)
			{
				if (Members[i].X > res)
				{
					res = Members[i].X;
				}
			}
			return res;
		}

		public int GetMinX()
		{
			int res = Members[0].X;
			for (int i = 1; i < Members.Length; i++)
			{
				if (Members[i].X < res)
				{
					res = Members[i].X;
				}
			}
			return res;
		}

		public int GetMaxY()
		{
			int res = Members[0].Y;
			for (int i = 1; i < Members.Length; i++)
			{
				if (Members[i].Y > res)
				{
					res = Members[i].Y;
				}
			}
			return res;
		}

		public int GetMinY()
		{
			int res = Members[0].Y;
			for (int i = 1; i < Members.Length; i++)
			{
				if (Members[i].Y < res)
				{
					res = Members[i].Y;
				}
			}
			return res;
		}

		public int GetWidth()
		{
			return GetMaxX() - GetMinX();
		}

		public int GetHeight()
		{
			return GetMaxY() - GetMinY();
		}
	}
}
