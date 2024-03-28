// MIT License
// 
// Copyright (c) 2024 twgenaux
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using System;
using System.IO;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace tgenaux.astm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EampleBrief();

            EampleDetailed();
        }

        static void EampleDetailed()
        {
            Console.WriteLine();
            Console.WriteLine("*** Detailed, step-by-step Example ***");
            Console.WriteLine();

            ////// Load the bi-directional translation map
            AstmRecordMap transMap = AstmRecordMap.ReadAstmTranslationRecordMap(new FileInfo("Vision.TransMap.txt"));

            //////////////////////////////////////// Extract the message content

            ////// Parse the message
            string[] msg = File.ReadAllLines("Vision.txt");
            List<Dictionary<string, string>> mappedMessage1 = MessageParsing.ParseMessage(msg.ToList(), false, new AstmRecordMap());

            // The message content has been extracted into a list of key-value pairs, 
            // where the key is the location of the value in each record.
            // O.1:O
            // O.2:1
            // O.3:SID101
            // O.5:ABO-D
            // O.6:N
            // O.7:20240307151207
            // O.16:CENTBLOOD
            // O.23:20240307151237
            // O.26:F
            DumpMappedMessage(mappedMessage1);
            Console.WriteLine();

            ////// Translate the message, replacing the location of each value with a token.
            List<Dictionary<string, string>> mappedMessage2 = AstmRecordMap.RemapRecord(mappedMessage1, transMap, true);
            DumpMappedMessage(mappedMessage2);
            Console.WriteLine();

            // The message content is now a list of key-value pairs, 
            // where the key is token for the value in each record.
            // O-RecordType:O
            // O-SeqNumber:1
            // O-SampleIDs:SID101
            // O-Profiles:ABO-D
            // O-Priority:N
            // O-RequestedTimeStamp:20240307151207
            // O-SampleTypes:CENTBLOOD
            // O-ReportedTime:20240307151237
            // O-ReportType:F


            //////////////////////////////////////// Create a new message with the extracted content of the original message

            ////// Translate the message, replacing the token of each value 
            ////// with the location of the value in each record (O.16:CENTBLOOD).
            List<Dictionary<string, string>> mappedMessage3 = AstmRecordMap.RemapRecord(mappedMessage2, transMap, true);
            DumpMappedMessage(mappedMessage3);
            Console.WriteLine();

            // The message content is now a list of key-value pairs, 
            // where the key is the location of the value in each record.
            // O.1:O
            // O.2:1
            // O.3:SID101
            // O.5:ABO-D
            // O.6:N
            // O.7:20240307151207
            // O.16:CENTBLOOD
            // O.23:20240307151237
            // O.26:F

            ////// The delimiters to use for this message
            string delimiters = @"|\^&";
            string septerators = @"|\^";
            string escape = @"&";

            ///// Create the message.
            List<string> roundTripMsg = MessageParsing.CreateMessge(septerators, escape, delimiters, mappedMessage3, false, new AstmRecordMap());

            ////// Display the original message
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"{string.Join("\r\n", msg)}");
            Console.WriteLine();

            ////// Display the new message
            Console.WriteLine();
            Console.WriteLine($"{string.Join("\r\n", roundTripMsg)}");
            Console.WriteLine();


            // # Original message
            // H|\^&|||OCD^VISION^5.10.0.46252^JNumber|||||||P|LIS2-A|20240307151237
            // P|1|PID123456||NID123456^MID123456^OID123456|Brown^Bobby^B|White|19650102030400|U||||||||||||||||||||||||||
            // O|1|SID101||ABO-D|N|20240307151207|||||||||CENTBLOOD|||||||20240307151237|||F|||||
            // L||

            // # New message, notice that there are no trailing delimiters in this message
            // H|\^&|||OCD^VISION^5.10.0.46252^JNumber|||||||P|LIS2-A|20240307151237
            // P|1|PID123456||NID123456^MID123456^OID123456|Brown^Bobby^B|White|19650102030400|U
            // O|1|SID101||ABO-D|N|20240307151207|||||||||CENTBLOOD|||||||20240307151237|||F
            // L

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        static void EampleBrief()
        {
            Console.WriteLine();
            Console.WriteLine("*** Brief Example ***");
            Console.WriteLine();

            ////// Load the bi-directional translation map
            AstmRecordMap transMap = AstmRecordMap.ReadAstmTranslationRecordMap(new FileInfo("Vision.TransMap.txt"));

            //////////////////////////////////////// Extract the message content

            ////// Parse the message
            string[] msg = File.ReadAllLines("Vision.txt");
            List<Dictionary<string, string>> mappedMessage = MessageParsing.ParseMessage(msg.ToList(), OnlyMapped:true, transMap);

            // The message content is first extracted into a list of key-value pairs, 
            // where the key is the location of the value in each record.
            // O.1:O
            // O.2:1
            // O.3:SID101
            // O.5:ABO-D
            // O.6:N
            // O.7:20240307151207
            // O.16:CENTBLOOD
            // O.23:20240307151237
            // O.26:F

            // Then the message content translated (transMap) into a list of key-value pairs, 
            // where each key is a token for the value in each record.
            // O-RecordType:O
            // O-SeqNumber:1
            // O-SampleIDs:SID101
            // O-Profiles:ABO-D
            // O-Priority:N
            // O-RequestedTimeStamp:20240307151207
            // O-SampleTypes:CENTBLOOD
            // O-ReportedTime:20240307151237
            // O-ReportType:F

            // When OnlyMapped is true, it will filter out any message content that is not mapped. 
            Console.WriteLine("*** Message Content is now a list of key-value pairs, ");
            Console.WriteLine("*** where each key is the a token of the value in the record (O-SampleTypes:CENTBLOOD).");
            DumpMappedMessage(mappedMessage);
            Console.WriteLine();


            //////////////////////////////////////// Create a new message with the extracted content of the original message

            ////// The delimiters to use for this message
            string delimiters = @"|\^&";
            string septerators = @"|\^";
            string escape = @"&";

            ///// Create the message.
            List<string> roundTripMsg = MessageParsing.CreateMessge(septerators, escape, delimiters, mappedMessage, onlyMapped:true, transMap);

            ////// Display the original message
            Console.WriteLine();
            Console.WriteLine("Original Message");
            Console.WriteLine($"{string.Join("\r\n", msg)}");
            Console.WriteLine();

            ////// Display the new message
            Console.WriteLine();
            Console.WriteLine("New Message");
            Console.WriteLine($"{string.Join("\r\n", roundTripMsg)}");
            Console.WriteLine();


            // # Original message
            // H|\^&|||OCD^VISION^5.10.0.46252^JNumber|||||||P|LIS2-A|20240307151237
            // P|1|PID123456||NID123456^MID123456^OID123456|Brown^Bobby^B|White|19650102030400|U||||||||||||||||||||||||||
            // O|1|SID101||ABO-D|N|20240307151207|||||||||CENTBLOOD|||||||20240307151237|||F|||||
            // L||

            // # New message, notice that there are no trailing delimiters in this message
            // H|\^&|||OCD^VISION^5.10.0.46252^JNumber|||||||P|LIS2-A|20240307151237
            // P|1|PID123456||NID123456^MID123456^OID123456|Brown^Bobby^B|White|19650102030400|U
            // O|1|SID101||ABO-D|N|20240307151207|||||||||CENTBLOOD|||||||20240307151237|||F
            // L

            Console.WriteLine();
            Console.WriteLine();
        }

        static void DumpMappedMessage(List<Dictionary<string, string>> mappedMessage)
        {
            foreach (var record in mappedMessage)
            {
                foreach (var key in record.Keys)
                {
                    if (key[0] != '_')
                    {
                        Console.WriteLine($"{key}:{record[key]}");
                    }
                }
            }
        }
    }
}
