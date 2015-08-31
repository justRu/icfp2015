using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Um.Engine
{
    public class MemoryManager
    {
        private Dictionary<uint, uint[]> _memory = new Dictionary<uint, uint[]>() {{0u, new uint[] {}}};
        private uint _new = 0;

        public uint Allocate(uint length)
        {
            while (_memory.ContainsKey(_new))
                unchecked
                {
                    _new++;
                }
            _memory.Add(_new, new uint[length]);
            return _new;
        }

        public void Free(uint key)
        {
            if (!_memory.ContainsKey(key))
                throw new InvalidOperationException(string.Format("Failed to delete array with key {0} - not exists.",
                    key));
            _memory.Remove(key);
        }

        public uint[] this[uint key]
        {
            get
            {
                if (!_memory.ContainsKey(key))
                    throw new InvalidOperationException(string.Format("Failed to get array with key {0} - not exists.",
                        key));
                return _memory[key];
            }
            set { _memory[key] = value; }
        }
    }
}
