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
                TranslationRecordMap = AstmRecordMap.ReadAstmTranslationRecordMap(translationMapPathname);
            }
            
            List<string> lines = File.ReadAllLines(pathanme).ToList();

            var messageContent = ParseMessage(lines);

            return messageContent;
        }

        /// <summary>
        /// ParseMessage
        /// </summary>
        /// <param name="message">List of text records (message)</param>
        /// <returns>A list of dictionaries with the extracted message content. One dictionary per record</returns>
        public List<Dictionary<string, string>> ParseMessage(List<string> message)
        {
            // list of dictionaries with the extracted message content, one for each record
            List<Dictionary<string, string>> mappedMessage = new List<Dictionary<string, string>>();

            string delimiters = @"|\^"; // default
            string escape = @"&"; // default

            // TODO return a list of disctionaries

            foreach (var line in message)
            {
                string text = line;

                if (text[0] == 'H')
                {
                    delimiters = text.Substring(1, 3);
                    escape = text.Substring(4, 1);
                    text = text.Substring(5);  // remove the delimitors
                    text = "H|" + text;
                }
                Record record = new Record() { RecordType = text.Substring(0, 1), Delimiters = delimiters, Escape = escape, Text = text };

                Dictionary<string, string> fields = record.GetItems(); // Extracts all field information from record 

                // Translate the record
                Dictionary<string, string> mapped = AstmRecordMap.RemapRecord(fields, TranslationRecordMap, OnlyMapped);

                mappedMessage.Add(mapped);
            } 

            return mappedMessage;
        }
    }
}
