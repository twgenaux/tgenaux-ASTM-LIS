using System;
using System.Collections.Generic;
using System.Linq;

namespace tgenaux.astm
{
    public class AstmRecordUtilitiescs
    {
        public static string CenterString(string text, int width)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            int padding = width - text.Length;
            if (padding < 0)
                return text; // Text is wider than the specified width

            int leftPadding = padding / 2 + padding % 2;
            int rightPadding = padding / 2;

            string padded = new string(' ', leftPadding) + text + new string(' ', rightPadding);
            return padded;
        }
        public static string RightString(string text, int width)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            int padding = width - text.Length;
            if (padding <= 0)
                return text; // Text is wider than the specified width

            string padded = text + new string(' ', padding);
            return padded;
        }
        public static List<List<string>> MessageContentToMarkdownTable(List<List<KeyValuePair<string, string>>> messageContent)
        {
            List<List<string>> meassageTables = new List<List<string>>();

            foreach (var record in messageContent)
            {
                List<string> recordTable = RecordContentToMarkdownTable(record);

                Console.WriteLine(string.Join("\n", recordTable));

                meassageTables.Add(recordTable);
            }
            return meassageTables;
        }

        public static List<string> RecordContentToMarkdownTable(List<KeyValuePair<string, string>> recordContent)
        {
            List<string> recordTable = new List<string>();

            // Calculate max column widths
            var maxColumnWidths = new int[2];
            foreach (var kvp in recordContent)
            {
                maxColumnWidths[0] = Math.Max(maxColumnWidths[0], kvp.Key.Trim().Length);
                maxColumnWidths[1] = Math.Max(maxColumnWidths[1], kvp.Value.Trim().Length);
            }

            // Heading
            string position = "Position";
            string value = "Value";

            maxColumnWidths[0] = Math.Max(maxColumnWidths[0], position.Length);
            maxColumnWidths[1] = Math.Max(maxColumnWidths[1], value.Length);

            position = RightString(position, maxColumnWidths[0]);
            value = RightString(value, maxColumnWidths[1]);

            recordTable.Add("");
            recordTable.Add($"| {position} | {value} |");
            recordTable.Add($"| {new string('-', maxColumnWidths[0])} | {new string('-', maxColumnWidths[1])} |");

            foreach (var kvp in recordContent)
            {
                position = RightString(kvp.Key, maxColumnWidths[0]);
                value = RightString(kvp.Value, maxColumnWidths[1]);

                //                |  Position  | Value   |
                recordTable.Add($"| {position} | {value} |");
            }
            recordTable.Add("");

            return recordTable;
        }

        public static List<string> MappedRecordToMarkdown(List<KeyValuePair<string, string>> recordContent, AstmRecordMap recordMap)
        {
            List<string> recordTable = new List<string>();

            // Calculate max column widths
            var maxColumnWidths = new int[3];
            foreach (var kvp in recordContent)
            {
                maxColumnWidths[0] = Math.Max(maxColumnWidths[0], kvp.Key.Trim().Length);
                maxColumnWidths[2] = Math.Max(maxColumnWidths[2], kvp.Value.Trim().Length);
                if (recordMap.Map.Keys.Contains(kvp.Key))
                {
                    maxColumnWidths[1] = Math.Max(maxColumnWidths[1], recordMap.Map[kvp.Key].Length);
                }
            }

            // Heading
            string position = "Position";
            string dataType = "Token";
            string value = "Value";

            maxColumnWidths[0] = Math.Max(maxColumnWidths[0], position.Length);
            maxColumnWidths[1] = Math.Max(maxColumnWidths[1], dataType.Length);
            maxColumnWidths[2] = Math.Max(maxColumnWidths[2], value.Length);

            position = RightString(position, maxColumnWidths[0]);
            dataType = RightString(dataType, maxColumnWidths[1]);
            value = RightString(value, maxColumnWidths[2]);

            recordTable.Add("");
            recordTable.Add($"| {position} | {dataType}  | {value} |");
            recordTable.Add($"| {new string('-', maxColumnWidths[0])} | {new string('-', maxColumnWidths[1])}  | {new string('-', maxColumnWidths[2])} |");

            foreach (var kvp in recordContent)
            {
                position = RightString(kvp.Key, maxColumnWidths[0]);
                if (recordMap.Map.Keys.Contains(kvp.Key))
                {
                    dataType = RightString(recordMap.Map[kvp.Key], maxColumnWidths[1]);
                }
                else
                {
                    dataType = new string(' ', maxColumnWidths[1]);
                }

                value = RightString(kvp.Value, maxColumnWidths[2]);

                //                |  Position  | Data Type  | Value |
                recordTable.Add($"| {position} | {dataType} | {value} |");
            }
            recordTable.Add("");

            return recordTable;
        }

    }
}
