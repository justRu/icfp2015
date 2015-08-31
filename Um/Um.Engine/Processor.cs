using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Um.Engine
{
    public class Processor
    {
        public uint[] Registers = new uint[8];
        public uint[] Program = { };
        public uint Index;
        public MemoryManager Memory = new MemoryManager();
        public Action<uint> Output;
        public Func<uint> Input;

        public void LoadStreams(IEnumerable<Stream> streams)
        {
            Program = streams.SelectMany(stream => EnumerateStream(stream)).ToArray();
            foreach (var stream in streams)
            {
                stream.Close();
            }
            Memory[0] = Program;
        }

        private IEnumerable<uint> EnumerateStream(Stream stream)
        {
            var buf = new byte[4];
            while (stream.Read(buf, 0, 4) == 4)
            {
                yield return
                    ((uint)buf[0] << 24) | ((uint)buf[1]) << 16 | ((uint)buf[2]) << 8 | (buf[3]);
            }
            //int b;
            //while((b = stream.ReadByte()) >= 0)
            //yield return
            //    ((uint) b << 24) | ((uint) stream.ReadByte()) << 16 | ((uint) stream.ReadByte()) << 8 |
            //    ((uint) stream.ReadByte());
        }
        public void Pulse()
        {
            if (Index > Program.Length)
                throw new InvalidOperationException("Index is outside program range");
            var current = Program[Index];
            switch ((Commands)(current >> 28))
            {
                case Commands.ConditionalMove:
                    if (Registers [GetRegisterC(current)] != 0)
                        Registers[GetRegisterA(current)] = Registers[GetRegisterB(current)];
                    break;
                case Commands.ArrayIndex:
                    Registers[GetRegisterA(current)] = Memory[Registers[GetRegisterB(current)]][Registers[GetRegisterC(current)]];
                    break;
                case Commands.ArrayWrite:
                    Memory[Registers[GetRegisterA(current)]][Registers [GetRegisterB(current)]] = Registers[GetRegisterC(current)];
                    break;
                case Commands.Addition:
                    unchecked
                    {
                        Registers[GetRegisterA(current)] = Registers[GetRegisterB(current)] +
                                                           Registers[GetRegisterC(current)];
                    }

                    break;
                case Commands.Multiplication:
                    unchecked
                    {
                        Registers[GetRegisterA(current)] = Registers[GetRegisterB(current)] *
                                                           Registers[GetRegisterC(current)];
                    }

                    break;
                case Commands.Division:
                    Registers[GetRegisterA(current)] = Registers[GetRegisterB(current)] /
                                                       Registers[GetRegisterC(current)];
                    break;
                case Commands.NAnd:
                    Registers[GetRegisterA(current)] = ~(Registers[GetRegisterB(current)] &
                                                       Registers[GetRegisterC(current)]);
                    break;
                case Commands.Halt:
                    throw new Exception("Halt!");
                case Commands.Alloc:
                    Registers[GetRegisterB(current)] = Memory.Allocate(Registers[GetRegisterC(current)]);
                    break;
                case Commands.Free:
                    Memory.Free(Registers[GetRegisterC(current)]);
                    break;
                case Commands.Output:
                    Output(Registers[GetRegisterC(current)]);
                    break;
                case Commands.Input:
                    Registers[GetRegisterC(current)] = Input();
                    break;
                case Commands.Load:
                    Index = Registers[GetRegisterC(current)] - 1;
                    var regb = Registers[GetRegisterB(current)];
                    if (regb == 0)
                        break;
                    Program = (uint[])Memory[regb].Clone();
                    Memory[0] = Program;
                    break;
                case Commands.Set:
                    Registers[(current >> 25) & 7u] = current & 0x1FFFFFFu;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Invalid UM operator");
            }
            Index++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetRegisterA(uint current)
        {
            return (current >> 6) & 7u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetRegisterB(uint current)
        {
            return (current >> 3) & 7u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetRegisterC(uint current)
        {
            return current & 7u;
        }

    }

}
