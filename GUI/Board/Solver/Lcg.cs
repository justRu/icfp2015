using System;
using System.Collections.Generic;
using System.Linq;

namespace Solver
{
    /// <summary>
    /// Linear congruential generator.
    /// </summary>
    internal static class Lcg
    {
        public static T Get<T>(T[] input, uint seed, int index)
        {
			var k = GetSequence(seed).ElementAt(index);
	        return input[k%input.Length];
        }

        private static IEnumerable<uint> GetSequence(uint start)
        {
            return GetRandomSequence(
                start,
                modulus: UInt32.MaxValue,
                multiplier: 1103515245,
                increment: 12345)
                .Select(num => (num >> 16) & 0x00007FFF); // get bits 30..16
        }

        private static IEnumerable<uint> GetRandomSequence(
            uint start, uint modulus, uint multiplier, uint increment)
        {
            uint number = start;
            while (true)
            {
                yield return number;
                number = (multiplier * number + increment) % modulus;
            }
        }
    }
}
