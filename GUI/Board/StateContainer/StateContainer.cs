using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Board.Engine;
using Board.Entities;

namespace StateContainer
{
    public class StateContainer
    {
        private readonly byte[] _cells;
        public uint SizeX { get; private set; }
        public uint SizeY { get; private set; }
        public MoveDirection Command { get; private set; }
        public uint Score { get; set; }
        public Unit Unit { get; private set; }
        public int UnitIndex { get; set; }

        private StateContainer _parent = null;

        public StateContainer(uint sizeX, uint sizeY)
        {
            SizeX = SizeX;
            SizeY = SizeY;
            var length = SizeX * SizeY;
            UnitIndex = -1;
            _cells = new byte[(uint)Math.Ceiling(((double)length) / 8)];
        }

        public StateContainer(StateContainer parent, MoveDirection command, Unit unit, int unitIndex)
        {
            SizeX = parent.SizeX;
            SizeY = parent.SizeY;
            _cells = _cells.ToArray();
            _parent = parent;
            Command = command;
            UnitIndex = unitIndex;
            
            // transform
            Unit = unit.Translate(command);
        }

        public bool this[uint x, uint y]
        {
            get { return this[x + y * SizeY]; }
            set { this[x + y * SizeY] = value; }
        }

        public bool this[uint offset]
        {
            get { return (_cells[offset >> 3] & (1 << (int)(offset & 7))) != 0; }
            set
            {
                if (value)
                    _cells[offset >> 3] |= (byte)(1 << (int)(offset & 7));
                else
                    _cells[offset >> 3] &= (byte)~(1 << (int)(offset & 7));
            }
        }
    }
}
