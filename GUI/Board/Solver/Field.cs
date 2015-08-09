using System;
using System.Collections.Generic;
using System.Linq;

namespace Solver
{
    public sealed class Field
    {
        public byte[] Cells { get; set; } // TODO: make em Int32 or Int64

        public int Width { get; set; }

        public int Height { get; set; }

        public Field(Field other)
        {
            Width = other.Width;
            Height = other.Height;
            Cells = other.Cells.ToArray();
        }

        public Field(Input input)
        {
            Width = input.Width;
            Height = input.Height;
            Cells = new byte[(uint)Math.Ceiling(((double)Width * Height) / 8)];
            foreach (var cell in input.Filled)
            {
                this[cell.X, cell.Y] = true;
            }
        }

        public bool this[int x, int y]
        {
            get { return this[x + y * Width]; }
            set { this[x + y * Width] = value; }
        }

	    public bool this[Position p]
	    {
			get { return this[p.X, p.Y]; }
			set { this[p.X, p.Y] = value; }
	    }

        public bool IsLineFull(int y)
        {
            for (int i = 0; i < Width; i++)
            {
                if (!this[i, y])
                    return false;
            }
            return true;
        }

        private bool this[int offset]
        {
            get { return (Cells[offset >> 3] & (1 << (offset & 7))) != 0; }
            set
            {
                if (value)
                    Cells[offset >> 3] |= (byte)(1 << (offset & 7));
                else
                    Cells[offset >> 3] &= (byte)~(1 << (offset & 7));
            }
        }

        const byte One = 1;
        public IEnumerable<bool> GetEnumerable()
        {
            int maxCount = Width * Height;
            foreach (var cell in Cells)
            {
                byte shifted = 1;
                for (int shift = 0; shift < 8; shift++)
                {
                    yield return (cell & shifted) != 0;
                    maxCount--;
                    if (maxCount == 0)
                        yield break;
                    shifted = unchecked((byte)(shifted << One));
                }
            }
        }

        public void SaveEnumerable(IEnumerable<bool> input)
        {
            //clear
            Cells = new byte[Cells.Length];
            var enumerator = input.GetEnumerator();
            for(int i =0; i< Cells.Length;i++)
            {
                byte shifted = 1;
                for (int shift = 0; shift < 8; shift++)
                {
                    if (!enumerator.MoveNext())
                        return;
                    if(enumerator.Current)
                        Cells[i] |= shifted;
                    shifted = unchecked((byte)(shifted << One));
                }
            }
        }


    }
}