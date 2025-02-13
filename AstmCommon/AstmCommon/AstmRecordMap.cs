/// MIT License
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
    /// AstmRecordMap 
    /// 
    /// A ASTM Record Map is a container of key-value pairs used to hold 
    /// the content of ASTM Messages.
    /// 
    /// This is also used to create a map between record field addresses 
    /// and descriptive tokens. 
    /// </summary>
    public class AstmRecordMap
    {
        /// <summary>
        /// The ASTM Record Map
        /// </summary>
        public Dictionary<string, string> Map { get; set; }

        public AstmRecordMap()
        {
            Map = new Dictionary<string, string>();
        }

        public AstmRecordMap(List<KeyValuePair<string, string>> keyValuePairs)
        {
            Map = new Dictionary<string, string>();

            foreach (var kvp in keyValuePairs)
            {
                if (!Map.Keys.Contains(kvp.Key))
                {
                    Map.Add(kvp.Key, kvp.Value);
                }
            }

        }

        public List<string> ToList()
        {
            List<string> list = new List<string>();

            foreach (string key in Map.Keys) 
            {
                list.Add($"{key}:{Map[key]}");
            }

            return list;
        }

        /// <summary>
        /// RemapMessageContent 
        /// </summary>
        /// <param name="sourceRecordItems">The source record map</param>
        /// <param name="translationMap">The map used to translate the source</param>
        /// <param name="onlyMapped">When true, filters out non-mapped fields, otherwise unmapped fields are included in the optput.</param>
        /// <returns>Returns the translated record map</returns>
        public static List<KeyValuePair<string, string>> RemapRecordContent
            (
            List<KeyValuePair<string, string>> sourceRecordItems,
            AstmRecordMap translationMap,
            bool onlyMapped = false)
        {
            List<KeyValuePair<string, string>> translatedRecordMap = new List<KeyValuePair<string, string>> ();

            foreach (var kvp in sourceRecordItems)
            {
                if (translationMap.Map.ContainsKey(kvp.Key))
                {
                    // Translate
                    KeyValuePair<string, string> map = new KeyValuePair<string, string>(translationMap.Map[kvp.Key], kvp.Value);
                    translatedRecordMap.Add(map);
                }
                else if (!onlyMapped)
                {
                    // Copy the unmapped field
                    translatedRecordMap.Add(kvp);
                }
            }
            return translatedRecordMap;
        }



        public static List<List<KeyValuePair<string, string>>> RemapMessageContent
            (
            List<List<KeyValuePair<string, string>>> sourceMessage,
            AstmRecordMap translationMap,
            bool onlyMapped = false)
        {
            List<List<KeyValuePair<string, string>>> transMessage = new List<List<KeyValuePair<string, string>>> ();

            foreach (var map in sourceMessage) 
            {
                var translated = RemapRecordContent(map, translationMap, onlyMapped);

                transMessage.Add(translated);
            }
            return transMessage;
        }

        /// <summary>
        /// Reads a translation record map
        /// </summary>
        /// <param name="mapFile">The text based record map file path-name</param>
        /// <returns>Returns a AstmRecordMap with the file contents</returns>
        public static AstmRecordMap ReadAstmTranslationRecordMap(FileInfo mapFile)
        {
            AstmRecordMap map = new AstmRecordMap();

            if (mapFile.Exists)
            {
                string[] lines = File.ReadAllLines(mapFile.FullName);
                map = ReadAstmTranslationRecordMap(lines);
            }
            return map;
        }

        /// <summary>
        /// Reads a translation record map
        /// </summary>
        /// <param name="lines">Message nap lines</param>
        /// <returns></returns>
        public static AstmRecordMap ReadAstmTranslationRecordMap(string[] lines)
        {
            AstmRecordMap map = new AstmRecordMap();

            int lineNumber = 0;
            foreach (string line in lines)
            {
                lineNumber++;
                string text = line;

                // Remove comment
                if (text.Contains('#'))
                {
                    text = text.Substring(0, text.IndexOf("#"));
                }

                // Remove leading and trailing whitespace
                text = text.Trim();

                // If not a comment line or a blank line, then parse it
                if (text.Length > 0)
                {
                    var parts = text.Split(':');
                    parts[0].Trim();

                    // Bi-directional map
                    // Both values are add as key/value
                    map.Map[parts[0]] = parts[1];
                    map.Map[parts[1]] = parts[0];
                }
            }
            return map;
        }


        /// <summary>
        /// Read a simple text file record map
        /// </summary>
        /// <param name="mapFile">The text based record map file path-name</param>
        /// <returns>Returns a AstmRecordMap with the file contents</returns>
        public static AstmRecordMap ReadAstmRecordMap(string mapFile)
        {
            AstmRecordMap map = new AstmRecordMap();

            if (File.Exists(mapFile))
            {
                int lineNumber = 0;
                string line;
                try
                {
                    using (var sr = new StreamReader(mapFile))
                    {
                        while (null != (line = sr.ReadLine()))
                        {
                            //Console.WriteLine(line);
                            lineNumber++;

                            // Remove comment
                            if (line.Contains('#'))
                            {
                                line = line.Substring(0, line.IndexOf("#"));
                            }

                            // Remove leading and trailing whitespace
                            line = line.Trim();

                            // If not a comment line or a blank line, then parse it
                            if (line.Length > 0)
                            {
                                var parts = line.Split(':');
                                parts[0].Trim();

                                // Key:Value from map
                                map.Map[parts[0]] = parts[1];
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() );
                }
            }
            return map;
        }

        /// <summary>
        /// SaveRecordMap
        /// </summary>
        /// <param name="map">The AstmRecordMap</param>
        /// <param name="mapFile">The target map file path-name</param>
        public static void SaveRecordMap(AstmRecordMap map, string mapFile)
        {
            using (var sw = new StreamWriter(mapFile))
            {
                foreach (var key in map.Map.Keys)
                {
                    sw.WriteLine($"{key}:{map.Map[key]}");
                }
            }
        }
    }
}
