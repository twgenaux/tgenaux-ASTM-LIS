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
        #region Usage
        static void Usage()
        {
            // Show usage
            string programName = Environment.GetCommandLineArgs()[0];
            FileInfo fi = new FileInfo(programName);

            Console.WriteLine();
            Console.WriteLine($"{fi.Name} - ASTM Message Parser");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine($"  {fi.Name} Pathname [/TransMap:Pathname] [/onlyMapped]");
            Console.WriteLine();
            Console.WriteLine("   where: ");
            Console.WriteLine("         pathname - is one or more ASTM message file pathnames.");
            Console.WriteLine("");
            Console.WriteLine("         /TransMap:Pathname - Pathname to a translation map file.");
            Console.WriteLine("         /onlyMapped - only exports mapped fields");
            Console.WriteLine("");
            Console.WriteLine("Outputs Key:Value pairs to the console.");
            Console.WriteLine("        Where the key is the field name or field address and the field value.");
            Console.WriteLine("");
            Console.WriteLine("PatientRecord:P");
            Console.WriteLine("PatSeqNumber:1");
            Console.WriteLine("PatPracticePID:PID123456");
            Console.WriteLine("PatPID3:NID123456^MID123456^OID123456");
            Console.WriteLine("PatNatPID3:NID123456");
            Console.WriteLine("PatMedPID3:MID123456");
            Console.WriteLine("PatOtherPID3:OID123456");
            Console.WriteLine("PatFullName:Brown^Bobby^B");
            Console.WriteLine("P.6.1.1:Brown");
            Console.WriteLine("P.6.1.2:Bobby");
            Console.WriteLine("P.6.1.3:B");
            Console.WriteLine("");
            Console.WriteLine("Field Address of a patient first name is defined as:P.6.1.2");
            Console.WriteLine("   Where P is the patient record type");
            Console.WriteLine("         6 is the field index");
            Console.WriteLine("         1 is the repeat field index");
            Console.WriteLine("         2 is the componet index.");
            Console.WriteLine("");
        }
        #endregion


        static void Main(string[] args)
        {
            if (args.Count() > 0)
            {
                ParseFiles(args);
            }
            else
            {
                AstmRecordExamle();
                
                BriefExample();

                DetailedExample();
            }
        }

        #region AstmRecord Example
        static void AstmRecordExamle()
        {
            Console.WriteLine();
            Console.WriteLine("AstmRecord Example");

            // Test records
            string text = "";
            //text = @"H|\^&|||Phadia.Prime^1.2.0.12371^4.0^^^^|||||^127.0.0.1||P|1|20120522101251";
            //text = @"H|\^&|||Phadia.Prime^1.2.0.12371^4.0|||||^127.0.0.1||P|1|20120522101251";
            text = @"O|1|SID102\SID103||Field-&F& Repeat-&R& Componet-&S& Escape-&E&||^^^|\|||^O.11.2|||||PACKEDCELLS\PLASMA|^^^\PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H\^^^";
            //text = @"R|^^^|1^^^|^^2^|^^^3"; 
            Console.WriteLine(text);
            Console.WriteLine();

            Console.WriteLine("Extract all data from Record");
            AstmRecord astmRecord = new AstmRecord();
            astmRecord.Text = text;
            List<string> astmList = astmRecord.GetAll();

            foreach (var item in astmList)
            {
                Console.WriteLine($"{item}");
            }

            Console.WriteLine();
            Console.WriteLine("Extract each data item by its address");

            foreach (var item in astmList)
            {
                var parts = item.Split(':');
                string value = astmRecord.Get(parts[0]);

                Console.WriteLine($"{parts[0]}:{value}");
            }


            // Recreate the record
            Console.WriteLine();
            Console.WriteLine("Recreate a record equivalent to the original record");

            AstmRecord astmRecord2 = new AstmRecord();

            foreach (var item in astmList)
            {
                var parts = item.Split(':');
                astmRecord2.Set(parts[0], parts[1]);
            }
            Console.WriteLine(astmRecord2.Text);
        }
        #endregion

        #region Detailed Example
        static void DetailedExample()
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
        #endregion

        #region Brief Example
        static void BriefExample()
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
        #endregion


        #region ParseFiles
        static void ParseFiles(string[] args)
        {
            if (!ParseArgs(args))
            {
                Usage();
                Environment.Exit(-1);
            }

            foreach (string path in paths)
            {
                ////// Create default (empty) translation map
                AstmRecordMap transMap = new AstmRecordMap();

                ////// Load the bi-directional translation map
                if (File.Exists(translationMapPathname))
                {
                    transMap = AstmRecordMap.ReadAstmTranslationRecordMap(new FileInfo(translationMapPathname));
                }

                //////////////////////////////////////// Extract the message content

                ////// Parse the message
                string[] msg = File.ReadAllLines(path);
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

            }
            Console.WriteLine();

        }
        #endregion


        #region Parse Args

        static List<string> paths = new List<string>();

        // The translation map pathname
        static string translationMapPathname = "";

        /// <summary>
        /// If true, only exports mapped fields
        /// </summary>
        static bool OnlyMapped = false;


        /// <summary>
        /// Parse command line args
        /// </summary>
        /// <param name="args">command line args</param>
        /// <returns>Returs false if usage should be displayed</returns>
        static bool ParseArgs(string[] args)
        {
            foreach (var arg in args)
            {
                if ((arg.IndexOf("help", StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                    (arg.IndexOf("?", StringComparison.CurrentCultureIgnoreCase) >= 0))
                {
                    return false; // display usage
                }
                // Translation map pathname
                else if (arg.IndexOf("/TransMap:", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    string text = arg.Remove(0, "/TransMap:".Length);
                    char[] trim = { '"' };
                    text.Trim(trim);
                    if (File.Exists(text))
                    {
                        translationMapPathname = text;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Translatiion map file does not exist: {arg}");
                        return false;
                    }
                }
                // Report onlly translation mapped fields
                else if (arg.IndexOf("/onlyMapped", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    OnlyMapped = true;
                }
                else
                {
                    if (File.Exists(arg))
                    {
                        paths.Add(arg);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"File does not exist: {arg}");
                        return false;
                    }
                }
            }

            // Must have at least one ASTM message file
            if (paths.Count <= 0)
            {
                return false;
            }

            return true;
        }
#endregion


        static void DumpMappedMessage(List<Dictionary<string, string>> mappedMessage)
        {
            foreach (var record in mappedMessage)
            {
                if (record.ContainsKey("_Delimiters") && record.ContainsKey("_Type") && record["_Type"] == "H")
                {
                    Console.WriteLine($"Delimiters:{record["_Delimiters"]}");
                }

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
