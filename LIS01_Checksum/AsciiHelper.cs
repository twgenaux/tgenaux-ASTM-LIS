// This software is provided “AS IS” (see below).
// 
// Copyright @2023 Theron W. Genaux
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the “Software”),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
//  - The above copyright notice and this permission notice is required
//  to be included in all copies or substantial portions of the Software. 
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//  CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace AsciiHelper
{
    /// <summary>
    /// ASCII control characters as bytes
    /// </summary>

    public class B
    {
        public const byte NUL = 0;
        public const byte ENQ = 5;
        public const byte ACK = 6;
        public const byte NAK = 21;
        public const byte EOT = 4;
        public const byte ETX = 3;
        public const byte ETB = 23;
        public const byte STX = 2;
        public const byte CR = 13;
        public const byte LF = 10;
    }

    /// <summary>
    /// ASCII control characters
    /// </summary>
    public class C
    {
        public const char NUL = (char)B.NUL;
        public const char ENQ = (char)B.ACK;
        public const char ACK = (char)B.ACK;
        public const char NAK = (char)B.NAK;
        public const char EOT = (char)B.EOT;
        public const char ETX = (char)B.ETX;
        public const char ETB = (char)B.ETB;
        public const char STX = (char)B.STX;
        public const char CR = (char)B.CR;
        public const char LF = (char)B.LF;
    }
}
