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


using AsciiHelper;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LIS1Helper
{
    /// <summary>
    /// A helpful collection of methods and constants for the LIS1 protocol
    /// </summary>
    public class LIS1Helper
    {
        /// <summary>
        /// Timestamp format
        /// </summary>
        const string FORMAT = "yyyyMMddHHmmss";

        /// <summary>
        /// Convert ASTM time stamp to DateTime.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns>DateTime.MinValue on error</returns>
        public static DateTime TimestampToDateTime(string timestamp)
        {
            DateTime dt = DateTime.MinValue;

            try
            {
                CultureInfo cultureInfo = CultureInfo.InvariantCulture;
                DateTime ts = DateTime.ParseExact(timestamp, FORMAT, cultureInfo);
                dt = ts;
            }

            catch
            {
                // do nothing
            }

            return dt;
        }

        /// <summary>
        /// Convert DateTime to ASTM time stamp.
        /// </summary>
        /// <param name="DateTime dt"></param>
        /// <returns>00000000000000 on error</returns>
        public static string DateTimeToTimeStamp(DateTime dt)
        {
            string timestamp = "00000000000000"; // default if error

            try
            {
                dt.ToString(FORMAT);
            }
            catch
            {
                // do nothing
            }

            return timestamp;
        }

        /// <summary>
        /// The number of characters added by the protocol
        /// </summary>
        const int FRAME_OVERHEAD = 7; // STX, FN, ETX, C1, C2, CR, LF


        /// <summary>
        /// Calculate ASTM frame checksum
        /// </summary>
        /// <param name="frame"></param>
        /// <returns>Defaults to 00</returns>
        /// The checksum is computed by adding the binary values of the characters, keeping the least significant 
        /// eight bits of the result.
        ///         
        /// LIS1-A, 8.3.3.1. 
        /// The checksum is initialized to zero with the <STX> character.
        /// The first character used in computing the checksum is the frame number. 
        /// Each character in the message text is added to the checksum (modulo 256). 
        /// The computation for the checksum does not include <STX>, the checksum characters or the trailing <CR> and <LF>.
        public static string CalculateFrameChecksum(byte[] frame)
        {
            byte checksum = 0; // sum-of-bytes mod 256
            foreach (byte b in frame)
            {
                if (B.STX == b)
                {
                    checksum = 0; // The checksum is initialized to zero with the <STX> character.
                }
                else if ((B.ETX == b) || (B.ETB == b))
                {
                    checksum += b;
                    break; // Done, exit foreach loop
                }
                else
                {
                    checksum += b;
                }
            }
            
            return checksum.ToString("X2");
        }

        /// <summary>
        /// Convert control characters in frame to readable text
        /// </summary>
        /// <param name="frame"></param>
        /// <returns>beautified frame</returns>
        public static byte[] Beautify(byte[] frame)
        {
            byte[] beautified = new byte[] { };

            foreach (byte b in frame)
            {
                beautified = beautified.Concat(ConvertChar(b)).ToArray();
            }

            return beautified;
        }

        /// <summary>
        /// Convert control character to readable text
        /// </summary>
        /// <param name="c">Character to convert</param>
        /// <returns>readable text</returns>
        public static byte[] ConvertChar(byte c)
        {
            byte[] text = null;
            switch (c)
            {
                case B.NUL:
                    text = Encoding.ASCII.GetBytes("<NUL>");
                    break;
                case B.ACK:
                    text = Encoding.ASCII.GetBytes("<ACK>");
                    break;
                case B.CR:
                    text = Encoding.ASCII.GetBytes("<CR>");
                    break;
                case B.ENQ:
                    text = Encoding.ASCII.GetBytes("<ENQ>");
                    break;
                case B.EOT:
                    text = Encoding.ASCII.GetBytes("<EOT>");
                    break;
                case B.ETB:
                    text = Encoding.ASCII.GetBytes("<ETB>");
                    break;
                case B.ETX:
                    text = Encoding.ASCII.GetBytes("<ETX>");
                    break;
                case B.LF:
                    text = Encoding.ASCII.GetBytes("<LF>");
                    break;
                case B.NAK:
                    text = Encoding.ASCII.GetBytes("<NAK>");
                    break;
                case B.STX:
                    text = Encoding.ASCII.GetBytes("<STX>");
                    break;
                default:
                    text = new byte[] { c };
                    break;
            }

            return text;
        }

        /// <summary>
        /// Convert readable frame to control characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns>frame</returns>
        public static string Unbeautify(string text)
        {
            string processed = text;

            processed = processed.Replace("<NUL>", C.NUL.ToString());
            processed = processed.Replace("<ACK>", C.ACK.ToString());
            processed = processed.Replace("<CR>", C.CR.ToString());
            processed = processed.Replace("<ENQ>", C.ENQ.ToString());
            processed = processed.Replace("<EOT>", C.EOT.ToString());
            processed = processed.Replace("<ETB>", C.ETB.ToString());
            processed = processed.Replace("<ETX>", C.ETX.ToString());
            processed = processed.Replace("<LF>", C.LF.ToString());
            processed = processed.Replace("<NAK>", C.NAK.ToString());
            processed = processed.Replace("<STX>", C.STX.ToString());

            processed = processed.Replace("<nul>", C.NUL.ToString());
            processed = processed.Replace("<ack>", C.ACK.ToString());
            processed = processed.Replace("<cr>", C.CR.ToString());
            processed = processed.Replace("<enq>", C.ENQ.ToString());
            processed = processed.Replace("<eot>", C.EOT.ToString());
            processed = processed.Replace("<etb>", C.ETB.ToString());
            processed = processed.Replace("<etx>", C.ETX.ToString());
            processed = processed.Replace("<lf>", C.LF.ToString());
            processed = processed.Replace("<nak>", C.NAK.ToString());
            processed = processed.Replace("<stx>", C.STX.ToString());

            return processed;
        }
    }
}
