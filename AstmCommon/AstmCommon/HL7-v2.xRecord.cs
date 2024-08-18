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
using System.Linq;

namespace tgenaux.astm
{
    /// <summary>
    /// 
    /// *********** Rewrite for HL7***************
    /// 
    /// The HL7-v2.xRecord class is a container for parsing and creating an
    /// ASTM E1394 and LIS02-A2 record
    /// 
    /// An ASTM record is a list of delimited fields. Every record begins with 
    /// a Record Type ID, like O, for an Order record, which indicates the type
    /// of data contained in that record.
    /// 
    /// Fields can be furtheer partitioned into Repeat Fields and Components.
    /// 
    /// For more information ASTM E1394 and LIS02-A2 records, see:
    /// "Introduction to ASTM E1394 and LIS02-A2 Message Formats"
    /// https://twgenaux.github.io/MessageFormats/MessageFormats
    /// 
    /// </summary>
    public class HL7-v2.xRecord
    {
        // Delimiter Indexes
        const int FieldDelimiterIndex = 0;
        const int RepeatDelimiterIndex = 1;
        const int ComponentDelimiterIndex = 2;
        const int EscapeDelimiterIndex = 3;

        // Separator Indexes
        const int FieldSeparatorIndex = 0;
        const int RepeatSeparatorIndex = 1;
        const int ComponentSeparatorIndex = 2;

        /// <summary>
        /// Separators - an ordered set of characters for partitioning 
        /// the data elements within the record. 
        /// </summary>
        public string Separators { get; set; } = @"|\^";

        /// <summary>
        /// The escape delimiter. Used to escape seporators/seperators in text fields
        /// </summary>
        public string Escape { get; set; } = @"&";

        /// <summary>
        /// Delimiters are an ordered list of characters as defined by standard. 
        /// </summary>
        public string Delimiters 
        { 
            get {  return Separators + Escape; }
            set 
            { 
                Separators = value.Substring(0,3);
                Escape = value.Substring(3,1);
            } 
        } 

        /// <summary>
        /// The delimited line of text
        /// </summary>
        public string Text
        {
            get { return GetText(); }
            set { ParseFields(value); }
        }


        public List<string> Fields { get; set; } = new List<string>();

        /// <summary>
        /// Number of fields
        /// </summary>
        public int Count { get { return Fields.Count; } }


        bool SupressTopLevel { get; set; } = true;

        public HL7-v2.xRecord()
        {
            
        }

        /// <summary>
        /// Parses the fields of in the text
        /// </summary>
        /// <param name="text">The delimited line of text</param>
        private void ParseFields(string text)
        {
            if ((text.Length > 0) && (text[0] == 'H'))
            {
                Delimiters = GetAstmDelimiters(text);

                string[] fields = text.Split(Separators[FieldSeparatorIndex]);
                Fields = fields.ToList();

                // Clear H.2 to prevent issues with parsing
                // SetDelimitersInHRecord can be called later to refill when reading the H-record
                if ( Delimiters.Length > 0 ) 
                {
                    Fields[1] = "";
                }
            }
        }


        /// <summary>
        /// The record type ID (first filed in a record)
        /// </summary>
        public string RecordType
        {
            get
            {
                if (Fields.Count > 0)
                {
                    return Fields[0];
                }
                return "";
            }
        }


        private static string GetAstmDelimiters(string text)
        {
            string delimiters = text.Substring(1, 4);
            return delimiters;
        }

        public void SetDelimitersInHRecord()
        {
            if (RecordType == "H")
            {
                Set(2, Delimiters.Substring(1,3));
            }
        }


        /// <summary>
        /// Converts the Fields to a delimited line of text
        /// </summary>
        /// <returns>The Fields as a delimited line of text</returns>
        private string GetText()
        {
            string fields = string.Join(Separators[0].ToString(), Fields);
            return fields;
        }

        /// <summary>
        /// Sets a feild with the given
        /// </summary>
        /// <param name="column">The field index</param>
        /// <param name="value">The field value</param>
        private void Set(int column, string value)
        {
            if (column < 1)
            {
                throw new ArgumentOutOfRangeException("column");
            }

            while (Fields.Count() < column)
            {
                Fields.Add("");
            }
            Fields[column - 1] = value; // TODO esacpe delimerters in the vale
        }

        /// <summary>
        /// Returns true if a field exists
        /// </summary>
        /// <param name="column">The field index</param>
        /// <returns>Returns the field value</returns>
        public bool Exist(int column)
        {
            return !(Fields.Count() < column);
        }

        /// <summary>
        /// Returns a field value
        /// </summary>
        /// <param name="column">The field index</param>
        /// <returns>Returns the field value</returns>
        private string Get(int column)
        {
            while (Fields.Count() < column)
            {
                Fields.Add("");
            }
            return Fields[column - 1];
        }

        /// <summary>
        /// Sets the value of a field
        /// </summary>
        /// <param name="_address">The field address</param>
        /// <param name="value">The feild value</param>
        public void Set(string _address, string value)
        {
            char[] seps = { '.' };
            List<string> address = _address.Split(seps).ToList();

            // Remove leading Record Type
            if (address.Count() > 0 && Char.IsLetter(address[0][0]))
            {
                address.RemoveAt(0);
            }

            string escaped = value.Replace(Escape, $"{Escape}E{Escape}");
            escaped = escaped.Replace(Separators[FieldDelimiterIndex].ToString(), $"{Escape}F{Escape}");
            escaped = escaped.Replace(Separators[RepeatDelimiterIndex].ToString(), $"{Escape}R{Escape}");
            escaped = escaped.Replace(Separators[ComponentDelimiterIndex].ToString(), $"{Escape}S{Escape}");



            int level = -1;
            Set(level, this.Separators, address, escaped);
        }

        /// <summary>
        /// Sets the value of a field
        /// </summary>
        /// <param name="level">The level of recusion</param>
        /// <param name="seporators">the seporators for this level</param>
        /// <param name="address">The field address</param>
        /// <param name="value">The feild valu</param>
        private void Set(int level, string separators, List<string> address, string value)
        {
            level++;

            int column = Convert.ToInt16(address[level]);
            if (level + 1 == address.Count())
            {
                this.Set(column, value);
            }
            else
            {
                HL7-v2.xRecord record = new HL7-v2.xRecord();
                string trimmedSeps = separators.Substring(1);
                record.Separators = trimmedSeps;
                string columnValue = this.Get(column);
                record.Text = columnValue;
                record.Set(level, trimmedSeps, address, value);
                this.Set(column, record.Text);
            }
        }

        public List<string> GetAll()
        {
            List<string> list = new List<string>();
            
            GetAll("", this.Separators, Escape, Text, list);

            return list;
        }

        private void GetAll(string prefix, string septerators, string escape, string text, List<string> list)
        {
            if (septerators.Length > 0)
            {
                string trimmed = TrimEmptyCells(septerators, text);

                string[] fields = trimmed.Split(septerators[0]);

                for (int i = 0; i < fields.Count(); i++)
                {
                    string field = TrimEmptyCells(septerators, fields[i]);

                    bool fieldContainsSeperator = false;
                    foreach (var sep in septerators)
                    {
                        fieldContainsSeperator = (field.IndexOf(sep) > -1) || fieldContainsSeperator;

                        if (fieldContainsSeperator)
                        {
                            fieldContainsSeperator = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(field))
                    {
                        string newPrefix = prefix.Length > 0 ? $"{prefix}.{i + 1}" : $"{i + 1}";

                        if (!(SupressTopLevel && fieldContainsSeperator))
                        {
                            // Replace escape sequence for this separator.
                            // &F& Embedded Field separator
                            // &R& Embedded Repeat separator
                            // &S& Embedded Component separator
                            // &E& Embedded Escape delimiter
                            string escaped = field.Replace($"{Escape}F{Escape}", Separators[FieldDelimiterIndex].ToString());
                            escaped = escaped.Replace($"{Escape}R{Escape}", Separators[RepeatDelimiterIndex].ToString());
                            escaped = escaped.Replace($"{Escape}S{Escape}", Separators[ComponentDelimiterIndex].ToString());
                            escaped = escaped.Replace($"{Escape}E{Escape}", Escape);

                            if (list.Count > 0)
                            {
                                // do not add duplicate values to the list
                                string[] parts = list[list.Count - 1].Split(':');
                                if (parts[1] != $"{escaped}")
                                {
                                    list.Add($"{RecordType}.{newPrefix}:{escaped}");
                                }
                            }
                            else
                            {
                                list.Add($"{RecordType}.{newPrefix}:{escaped}");
                            }
                        }
                        GetAll(newPrefix, septerators.Substring(1, septerators.Length - 1), escape, field, list);
                    }
                }
            }
        }

        static public string TrimEmptyCells(string septerators, string text)
        {
            string trimmed = text;

            foreach (var sep in septerators)
            {
                trimmed = trimmed.TrimEnd(sep);
            }

            return trimmed;
        }

        public string Get(string item)
        {
            char[] seps = { '.' };
            List<string> address = item.Split(seps).ToList();

            // Remove leading Record Type
            if (address.Count() > 0 && Char.IsLetter(address[0][0]))
            {
                address.RemoveAt(0);
            }

            string value = Get(string.Join(".", address), "", this.Separators, this.Text);

            // Replace escape sequence for this separator.
            string escaped = value.Replace($"{Escape}F{Escape}", Separators[FieldDelimiterIndex].ToString());
            escaped = escaped.Replace($"{Escape}R{Escape}", Separators[RepeatDelimiterIndex].ToString());
            escaped = escaped.Replace($"{Escape}S{Escape}", Separators[ComponentDelimiterIndex].ToString());
            escaped = escaped.Replace($"{Escape}E{Escape}", Escape);

            return escaped;
        }

        private string Get(string item, string prefix, string septerators, string text)
        {
            string value = "";
            if (septerators.Length > 0)
            {
                string[] fields = text.Split(septerators[0]);

                for (int i = 0; string.IsNullOrEmpty(value) && i < fields.Count(); i++)
                {
                    string pos = prefix.Length > 0 ? $"{prefix}.{i + 1}" : $"{i + 1}"; //$"{prefix}.{i + 1}";
                    if (item == pos)
                    {
                        return fields[i];
                    }
                    else if (!string.IsNullOrEmpty(fields[i]))
                    {
                        value = Get(item, pos, septerators.Substring(1, septerators.Length - 1), fields[i]);
                    }
                }
            }
            return value;
        }
    }
}
