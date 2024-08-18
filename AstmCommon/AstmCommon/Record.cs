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
    /// The Record class is a container for storing and parsing a delimited 
    /// line of text.
    /// 
    /// A record can be partitioned into fields. A mnemonic for a field is 
    /// constructed from the record ID and the field's numeric position in the
    /// record. A record holding the demographics of a person could have a 
    /// record ID of P. If the person's name is stored in position 7, the 
    /// mnemonic is P-7. If the person's name is further partitioned into
    /// it's components of last, first, and middle; then the mnemonic for the
    /// last name is P-7.1.
    /// 
    /// The address of the last name in P-7 is 7.1.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Separators - an ordered set of characters for partitioning 
        /// the data elements within the record. 
        /// </summary>
        public string Separators { get; set; }

        /// <summary>
        /// The escape delimiter. Used to escape delimiters/seperators in text fields
        /// </summary>
        public string Escape { get; set; }

        /// <summary>
        /// Delimiters are an ordered list of characters as defined by standard. 
        /// </summary>
        public string Delimiters { get; set; }

        /// <summary>
        /// The record type ID (first filed in a record)
        /// </summary>
        public string RecordType { get; set; }

        /// <summary>
        /// The delimited line of text
        /// </summary>
        public string Text
        {
            get { return GetText(); }
            set { ParseFields(value); }
        }
        public List<string> Fields; // the text separated into fields

        /// <summary>
        /// Number of fields
        /// </summary>
        public int Count { get { return Fields.Count; } }


        public Record()
        {
            Separators = "";
            Fields = new List<string>();
        }

        /// <summary>
        /// Parses the fields of in the text
        /// </summary>
        /// <param name="text">The delimited line of text</param>
        private void ParseFields(string text)
        {
            string[] fields = text.Split(Separators[0]);
            Fields = fields.ToList();
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
        public void Set(int column, string value)
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
        /// Returns a field value
        /// </summary>
        /// <param name="_address">The field address</param>
        /// <returns>Returns the field value</returns>
        public string Get(string _address)
        {
            string value = "";

            char[] seps = { '.' };
            List<string> address = _address.Split(seps).ToList();

            int level = -1;
            value = Get(level, this.Separators, address);

            return value;
        }

        /// <summary>
        /// Given an address, returns a field value
        /// </summary>
        /// <param name="level">The level of recusiond</param>
        /// <param name="delimiters">The delimiters for this address level</param>
        /// <param name="address">The field address</param>
        /// <returns>Returns a field value</returns>
        private string Get(int level, string delimiters, List<string> address)
        {
            string value = "";
            level++;

            // TODO This is cryptic, needs an description

            int column = Convert.ToInt16(address[level]);
            if (this.Exist(column))
            {
                if (level + 1 == address.Count())
                {
                    value = this.Get(column);
                }
                else
                {
                    Record record = new Record();
                    record.Separators = this.Separators.Substring(1);
                    record.Text = this.Get(column);
                    value = record.Get(level, record.Separators, address);
                }
            }
            return value;
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

            int level = -1;
            Set(level, this.Separators, address, value);
        }

        /// <summary>
        /// Sets the value of a field
        /// </summary>
        /// <param name="level">The level of recusion</param>
        /// <param name="delimiters">the delimiters for this level</param>
        /// <param name="address">The field address</param>
        /// <param name="value">The feild valu</param>
        private void Set(int level, string delimiters, List<string> address, string value)
        {
            level++;

            int column = Convert.ToInt16(address[level]);
            if (level + 1 == address.Count())
            {
                this.Set(column, value);
            }
            else
            {
                Record record = new Record();
                record.Separators = this.Separators.Substring(1);
                record.Text = this.Get(column);
                record.Set(level, record.Separators, address, value);
                this.Set(column, record.Text);
            }
        }

        /// <summary>
        /// Gets the content of a record as a field address and value dictionary
        /// </summary>
        /// <returns>the content of a record as a field address and value dictionary</returns>
        public Dictionary<string, string> GetItems()
        {
            Dictionary<string, string> items =  GetItems(this.RecordType, this.Separators, this.Escape, this.Text);

            return items;
        }

        /// <summary>
        /// Gets the content of a record as a field address and value dictionary
        /// 
        /// This initializes parameters and calls the recursive GetItems method.
        /// </summary>
        /// <param name="recordType">The record type</param>
        /// <param name="delimiters">The delimiters </param>
        /// <param name="escape">The escape character</param>
        /// <param name="segment">The record text</param>
        /// <returns>the content of a record as a field address and value dictionary</returns>
        static Dictionary<string, string> GetItems(string recordType, string delimiters, string escape, string segment)
        {
            Dictionary<string, string> items = new Dictionary<string, string>();

            List<string> address = new List<string>();

            GetItems(recordType, delimiters, escape, segment, address, items);

            return items;
        }

        /// <summary>
        /// Gets the content of a record as a field address and value dictionary
        /// </summary>
        /// <param name="recordType">The record type</param>
        /// <param name="delimiters">The delimiters </param>
        /// <param name="escape">The escape character</param>
        /// <param name="segment">The record text</param>
        /// <param name="address">The field address</param>
        /// <param name="items">the content of a record</param>
        static void GetItems(string recordType, string delimiters, string escape, string segment, List<string> address, Dictionary<string, string> items)
        {
            // End the recursion?
            if (delimiters.Length <= 0)
                return;

            Record record = new Record() { Separators = delimiters, Text = segment };

            int pos = 0;
            foreach (var s in record.Fields)
            {
                pos++;

                // filter out duplicate addresses
                bool matchesParent = MatchesParent(s, recordType, address, items);

                address.Add(pos.ToString()); // push

                // don't add empty fields
                if (s != "")
                {
                    string addressStr = String.Join(".", address);
                    if (!matchesParent)
                    {
                        items[$"{recordType}.{addressStr}"] = s;
                    }

                    // delimiters are reduced for every subsegment
                    GetItems(recordType, delimiters.Substring(1, delimiters.Length - 1), escape, s, address, items);
                }
                address.RemoveAt(address.Count - 1); // pop
            }
        }

        /// <summary>
        /// Dectects duplicate addresses
        /// </summary>
        /// <param name="segment">The record text</param>
        /// <param name="recordType">The record type</param>
        /// <param name="address">The field address</param>
        /// <param name="items">the content of a record</param>
        /// <returns></returns>
        static bool MatchesParent(string segment, string recordType, List<string> address, Dictionary<string, string> items)
        {
            bool matchesParent = false;
            List<string> parentsAdd = new List<string>(address);
            while (!matchesParent && (parentsAdd.Count > 0))
            {
                string parentAdd = String.Join(".", parentsAdd);
                parentAdd = $"{recordType}.{parentAdd}";
                if (items.Keys.Contains(parentAdd))
                {
                    matchesParent = (segment == items[parentAdd]);
                }
                parentsAdd = parentsAdd.GetRange(0, parentsAdd.Count - 1);
            }
            return matchesParent;
        }
    }
}
