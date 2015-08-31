using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Um.Engine
{

    public enum Commands : uint
    {
        /// <summary>
        /// Operator #0. Conditional Move.
        /// The register A receives the value in register B,
        /// unless the register C contains 0.
        /// </summary>

        ConditionalMove = 0,
        /// <summary>
        /// #1. Array Index. 
        /// The register A receives the value stored at offset
        /// in register C in the array identified by B.
        /// </summary>
        ArrayIndex,
        
        /// <summary>
        ///         #2. Array Amendment.
        ///                The array identified by A is amended at the offset
        ///                in register B to store the value in register C.
        /// </summary>
        ArrayWrite,
        /// <summary>
        ///         #3. Addition.
        ///                The register A receives the value in register B plus 
        ///                the value in register C, modulo 2^32.
        /// </summary>
        Addition,
        /// <summary>
        ///         #4. Multiplication.
        ///                The register A receives the value in register B times
        ///                the value in register C, modulo 2^32.
        /// </summary>
        Multiplication,
        /// <summary>
        ///         #5. Division.
        ///                The register A receives the value in register B
        ///                divided by the value in register C, if any, where
        ///                each quantity is treated treated as an unsigned 32
        ///                bit number.
        /// </summary>
        Division,
        /// <summary>
        ///         #6. Not-And.
        ///                Each bit in the register A receives the 1 bit if
        ///               either register B or register C has a 0 bit in that
        ///                position.  Otherwise the bit in register A receives
        ///                the 0 bit.
        /// </summary>
        NAnd,
        /// <summary>
        ///         #7. Halt.
        ///               The universal machine stops computation.
        /// </summary>
        Halt,
        /// <summary>
        ///         #8. Allocation.
        ///                A new array is created with a capacity of platters
        ///                commensurate to the value in the register C. This
        ///                new array is initialized entirely with platters
        ///                holding the value 0. A bit pattern not consisting of
        ///                exclusively the 0 bit, and that identifies no other
        ///                active allocated array, is placed in the B register.
        /// </summary>
        Alloc,
        /// <summary>
        ///         #9. Abandonment.
        ///                The array identified by the register C is abandoned.
        ///                Future allocations may then reuse that identifier.
        /// </summary>
        Free,
        /// <summary>
        ///        #10. Output.
        ///                The value in the register C is displayed on the console
        ///                immediately. Only values between and including 0 and 255
        ///                are allowed.
        /// </summary>
        Output,
        /// <summary>
        ///        #11. Input.
        ///                The universal machine waits for input on the console.
        ///                When input arrives, the register C is loaded with the
        ///                input, which must be between and including 0 and 255.
        ///                If the end of input has been signaled, then the 
        ///                register C is endowed with a uniform value pattern
        ///                where every place is pregnant with the 1 bit.
        /// </summary>
        Input,
        /// <summary>
        ///        #12. Load Program.
        ///                The array identified by the B register is duplicated
        ///                and the duplicate shall replace the '0' array,
        ///                regardless of size. The execution finger is placed
        ///                to indicate the platter of this array that is
        ///                described by the offset given in C, where the value
        ///                0 denotes the first platter, 1 the second, et
        ///                cetera.
        ///                The '0' array shall be the most sublime choice for
        ///                loading, and shall be handled with the utmost
        ///                velocity.
        /// </summary>
        Load,
        /// <summary>
        /// #13. Orthography.
        /// The value indicated is loaded into the register A
        /// forthwith.
        /// </summary>
        Set
    }
}
