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

        /// <summary>
        /// RemapRecord 
        /// </summary>
        /// <param name="sourceRecordMap">The source record map</param>
        /// <param name="translationMap">The map used to translate the source</param>
        /// <param name="onlyMapped">When true, filters out non-mapped fields, otherwise unmapped fields are included in the optput.</param>
        /// <returns>Returns the translated record map</returns>
        public static Dictionary<string, string> RemapRecord
            (
            Dictionary<string, string> sourceRecordMap, 
            AstmRecordMap translationMap, 
            bool onlyMapped=false)
        {
            Dictionary<string, string> translatedRecordMap = new Dictionary<string, string>();

            foreach (var key in sourceRecordMap.Keys)
            {
                if (translationMap.Map.ContainsKey(key))
                {
                    translatedRecordMap[translationMap.Map[key]] = sourceRecordMap[key];
                }
                else if (!onlyMapped)
                {
                    // Copy the unmapped field
                    translatedRecordMap[key] = sourceRecordMap[key];
                }
            }
            return translatedRecordMap;
        }

        /// <summary>
        /// Read a translation record map
        /// </summary>
        /// <param name="mapFile">The text based record map file path-name</param>
        /// <returns>Returns a AstmRecordMap with the file contents</returns>
        public static AstmRecordMap ReadAstmTranslationRecordMap(string mapFile)
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

                                // Bi-directional map
                                // Both values are add as key/value
                                map.Map[parts[0]] = parts[1];
                                map.Map[parts[1]] = parts[0];
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
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
