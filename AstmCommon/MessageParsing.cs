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
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace tgenaux.astm
{
    /// <summary>
    /// MessageParsing - an ASTM Message Parser.
    /// 
    /// This can also be used for other delimited message type files (HL7 v2, ...)
    /// </summary>
    public class MessageParsing
    {
        /// <summary>
        /// A Translation Record Map
        /// </summary>
        public AstmRecordMap TranslationRecordMap { get; set; }    

        /// <summary>
        /// Only translate mapped fields
        /// </summary>
        public bool OnlyMapped { get; set; }


        public MessageParsing()
        {
            TranslationRecordMap = new AstmRecordMap();
        }

        /// <summary>
        /// ParseMessage
        /// </summary>
        /// <param name="pathanme">Pathname to the ASTM message file</param>
        /// <param name="translationMapPathname">>Pathname to translation file. Ingnored if empty string</param>
        /// <returns>A list of dictionaries with the extracted message content. One dictionary per record</returns>
        public List<Dictionary<string, string>> ParseMessage(string pathanme, string translationMapPathname="")
        {
            if (File.Exists(translationMapPathname)) 
            {
                TranslationRecordMap = AstmRecordMap.ReadAstmTranslationRecordMap(new FileInfo(translationMapPathname));
            }
            
            List<string> lines = File.ReadAllLines(pathanme).ToList();

            var messageContent = this.ParseMessage(lines);

            return messageContent;
        }

        /// <summary>
        /// ParseMessage
        /// </summary>
        /// <param name="message">List of text records (message)</param>
        /// <returns>A list of dictionaries with the extracted message content. One dictionary per record</returns>
        public List<Dictionary<string, string>> ParseMessage(List<string> message)
        {
            List<Dictionary<string, string>> mappedMessage = MessageParsing.ParseMessage(message, OnlyMapped, TranslationRecordMap);

            return mappedMessage;
        }


        public static List<Dictionary<string, string>> ParseMessage(List<string> message, bool OnlyMapped, AstmRecordMap translationMap)
        {
            // list of dictionaries with the extracted message content, one for each record
            List<Dictionary<string, string>> mappedMessage = new List<Dictionary<string, string>>();

            string msgType = "ASTM";

            string standardDelimiters = "";
            string septerators = @"|\^"; // default
            string escape = @"&"; // default

            foreach (var line in message)
            {
                Record record = new Record() { Separators = septerators, Escape = escape };

                string text = line;

                // Skip empty or comment lines (#)
                // Comment lines must start with #
                // End of line comments are not supported
                if (string.IsNullOrEmpty(line) || line.Trim().StartsWith("#"))
                {
                    continue;
                }

                // ASTM 
                else if (text.Substring(0, 1) == "H")
                {
                    msgType = "ASTM";
                    standardDelimiters = text.Substring(1, 4);
                    record.Delimiters = standardDelimiters;

                    septerators = text.Substring(1, 3);
                    escape = text.Substring(4, 1);

                    text = text.Substring(0, 2) + text.Substring(5);  // remove the delimitors for ease of paring the MSH record
                }

                // HL7
                else if (text.Substring(0, 3) == "MSH")  // MSH|^~\&  - Feild (|), Componet (^), Repetition (~), (&) Escape (\), Subcomponent
                {
                    msgType = "HL7";
                    // TODO: HL7 septerators |^~& Needs to be ordered as Field, Repeat, Composit, Sub-Composit => |~^&
                    // https://www.qvera.com/kb/index.php/440/please-explain-the-use-of-a-tilde-or-squiggly-in-the-hpath
                    // 012345678
                    // MSH|^~\&|
                    standardDelimiters = text.Substring(3, 5);
                    record.Delimiters = standardDelimiters;

                    septerators = text.Substring(3, 1); // Feild (|)
                    septerators += text.Substring(5, 1); // Repetition (~)
                    septerators += text.Substring(4, 1); // Componet (^)
                    septerators += text.Substring(7, 1); // Subcomponent (&)

                    escape = text.Substring(6, 1); // \

                    text = text.Substring(0, 4) + text.Substring(8);  // remove the delimitors for ease of paring the MSH record
                }
                record.Separators = septerators;
                record.Escape = escape;
                record.Text = text;
                record.RecordType = record.Get("1"); // first field contains the Record Type

                Dictionary<string, string> items = record.GetItems(); // Extracts all field information from record

                // Translate the record
                Dictionary<string, string> mapped = AstmRecordMap.RemapRecord(items, translationMap, OnlyMapped);

                mapped["_Type"] = record.RecordType;
                mapped["_Delimiters"] = standardDelimiters;
                mapped["_MsgType"] = msgType;

                mappedMessage.Add(mapped);
            }

            return mappedMessage;
        }



        /// <summary>
        /// CreateMessge - creates a message from a list of mapped records
        /// </summary>
        /// <param name="seperators">Separators - an ordered set of characters for partitioning the 
        /// data elements within the record.</param>
        /// <param name="escape">The escape character is used to escape delimiters/seperators within text fields</param>
        /// <param name="delimitors">Delimiters are an ordered list of characters as defined by standard.</param>
        /// <param name="mappedMessage">A mapped message is a list of key-value parirs</param>
        /// <returns>return a message</returns>
        public List<string> CreateMessge(string seperators, string escape, string delimitors, List<Dictionary<string, string>> mappedMessage)
        {
            List<string> message = MessageParsing.CreateMessge(seperators, escape, delimitors, mappedMessage, OnlyMapped, TranslationRecordMap);

            return message;
        }

        public string CreateRecord(string seperators, string escape, string delimitors, AstmRecordMap recordMap)
        {
            return MessageParsing.CreateRecord(seperators, escape, delimitors, recordMap, OnlyMapped, TranslationRecordMap);
        }

        public static List<string> CreateMessge(string seperators, string escape, string delimitors, List<Dictionary<string, string>> mappedMessage, bool onlyMapped, AstmRecordMap transMap)
        {
            List<string> message = new List<string>();
            foreach (var mappedRecord in mappedMessage)
            {
                AstmRecordMap recMap = new AstmRecordMap() { Map = mappedRecord };
                string text = CreateRecord(seperators, escape, delimitors, recMap, onlyMapped, transMap);

                if (text.Length > 0)
                {
                    message.Add(text);
                }
            }

            return message;
        }
        public static string CreateRecord(string seperators, string escape, string delimitors, AstmRecordMap recordMap, bool onlyMapped, AstmRecordMap transMap)
        {
            Record record = new Record();
            record.Separators = seperators;
            record.Escape = escape;
            record.Delimiters = delimitors;
            if (recordMap.Map.ContainsKey("_Type"))
            {
                record.RecordType = recordMap.Map["_Type"];
            }
            else if(recordMap.Map.ContainsKey("RecordType"))
            {
                record.RecordType = recordMap.Map["RecordType"];
            }

            Dictionary<string, string> mapped = AstmRecordMap.RemapRecord(recordMap.Map, transMap, onlyMapped);

            foreach (var key in mapped.Keys)
            {
                if (key[0] != '_')
                {
                    string address = key;
                    if ((key.Length > 2) && (!Char.IsDigit(key[0])))
                    {
                        address = address.Substring(2);
                    }
                    record.Set(address, mapped[key]);
                }
            }

            if (record.RecordType == "H")
            {
                record.Set(2, delimitors.Substring(1));
            }

            return record.Text;
        }

        public static void GetAstmDelimiters(string text, out string delimiters, out string seperators, out string escape)
        {
            delimiters = text.Substring(1, 4);
            seperators = delimiters.Substring(0, 3);
            escape = delimiters.Substring(3, 1);
        }

        public static void GetHL7Delimiters(string text, out string delimiters, out string seperators, out string escape)
        {
            delimiters = text.Substring(3, 5);
            seperators = delimiters.Substring(0, 3) + text.Substring(4, 1);
            escape = delimiters.Substring(3, 1);
        }

    }
}
