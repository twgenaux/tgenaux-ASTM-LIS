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

namespace tgenaux.astm
{
    internal class Program
    {

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

        static void Main(string[] args)
        {
            if (!ParseArgs(args))
            {
                Usage();
                Environment.Exit(-1);
            }

            foreach (string path in paths)
            {
                MessageParsing messageParsing = new MessageParsing();

                messageParsing.OnlyMapped = OnlyMapped;

                if (File.Exists(translationMapPathname))
                {
                    messageParsing.TranslationRecordMap = AstmRecordMap.ReadAstmTranslationRecordMap(translationMapPathname);
                }

                List<Dictionary<string, string>> mappedMessage = messageParsing.ParseMessage(path);

                foreach (var record in mappedMessage)
                {
                    foreach (var key in record.Keys)
                    {
                        Console.WriteLine($"{key}:{record[key]}");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static List<string> paths = new List<string>();

        // The translation map pathname
        static string translationMapPathname = "";

        static bool OnlyMapped = false;

        static  bool ParseArgs(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.IndexOf("help", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return false;
                }
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

            if (paths.Count <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
