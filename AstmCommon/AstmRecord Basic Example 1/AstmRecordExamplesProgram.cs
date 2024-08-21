using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tgenaux.astm;

using static System.Net.Mime.MediaTypeNames;

namespace AstmRecord_Basic_Example_1
{
    internal class AstmRecordExamplesProgram
    {
        static void Main(string[] args)
        {
            AstmMappingMessage();
            AstmRecordCreateOrderMessage();
            AstmRecordCreateQueryMessage();
            AstmRecordParseQueryMessage();
            AstmObfuscateMessage();
        }


        #region AstmRecordCreateOrderMessage

        #region AstmRecordCreateOrderMessageContent

        static KeyValuePair<string, string>[] AstmRecordCreateOrderMessageContent = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("newH", "newH"), // create a new H-Record
            new KeyValuePair<string, string>("H.5", "Mini LIS"),

            new KeyValuePair<string, string>("H.12", "P"),
            new KeyValuePair<string, string>("H.13", "LIS2-A"),
            new KeyValuePair<string, string>("H.14", "20210309155210"),

            new KeyValuePair<string, string>("newP", "newP"), // create a new P-Record
            new KeyValuePair<string, string>("P.3", "PID123456"),
            new KeyValuePair<string, string>("P.6.1.1", "Brown"),
            new KeyValuePair<string, string>("P.6.1.2", "Bobby"),
            new KeyValuePair<string, string>("P.6.1.3", "B"),
            new KeyValuePair<string, string>("P.7", "White"),
            new KeyValuePair<string, string>("P.8", "196501020304"),
            new KeyValuePair<string, string>("P.9", "M"),
            new KeyValuePair<string, string>("P.10.1.1", "PHY1001"),
            new KeyValuePair<string, string>("P.10.1.2", "Brewster"),
            new KeyValuePair<string, string>("P.10.1.3", "Katherine"),
            new KeyValuePair<string, string>("P.10.2.1", "PHY1002"),
            new KeyValuePair<string, string>("P.10.2.2", "McCoy"),
            new KeyValuePair<string, string>("P.10.2.3", "Leonard"),
            new KeyValuePair<string, string>("P.10.2.4", "H"),

            new KeyValuePair<string, string>("newO", "newO"), // create a new O-Record
            new KeyValuePair<string, string>("O.3", "SID304"),
            new KeyValuePair<string, string>("O.5", "Type & Screen"), // The escape character will be escaped in the record),
            new KeyValuePair<string, string>("O.6", "N"),
            new KeyValuePair<string, string>("O.7", "20210309155210"),
            new KeyValuePair<string, string>("O.12", "N"),
            new KeyValuePair<string, string>("O.16", "CENTBLOOD"),

            new KeyValuePair<string, string>("newO", "newO"), // create a new O-Record
            new KeyValuePair<string, string>("O.3", "SID305"),
            new KeyValuePair<string, string>("O.5", "Pheno"),
            new KeyValuePair<string, string>("O.6", "N"),
            new KeyValuePair<string, string>("O.7", "20210309155210"),
            new KeyValuePair<string, string>("O.12", "N"),
            new KeyValuePair<string, string>("O.16", "CENTBLOOD"),

            new KeyValuePair<string, string>("newP", "newP"), // create a new P-Record
            new KeyValuePair<string, string>("P.3", "PID789012"),
            new KeyValuePair<string, string>("P.6", "Forbin^Charles"),
            new KeyValuePair<string, string>("P.6.1.1", "Forbin"),
            new KeyValuePair<string, string>("P.6.1.2", "Charles"),
            new KeyValuePair<string, string>("P.7", "Fisher"),
            new KeyValuePair<string, string>("P.8", "19410403"),
            new KeyValuePair<string, string>("P.9", "M"),
            new KeyValuePair<string, string>("P.10.1.1", "PHY1001"),
            new KeyValuePair<string, string>("P.10.1.2", "Brewster"),
            new KeyValuePair<string, string>("P.10.1.3", "Katherine"),
            new KeyValuePair<string, string>("P.10.2.1", "PHY1002"),
            new KeyValuePair<string, string>("P.10.2.2", "McCoy"),
            new KeyValuePair<string, string>("P.10.2.3", "Leonard"),
            new KeyValuePair<string, string>("P.10.2.4", "H"),

            new KeyValuePair<string, string>("newO", "newO"), // create a new O-Record
            new KeyValuePair<string, string>("O.3", "SID306"),
            new KeyValuePair<string, string>("O.5", @"ABO\ABScr"), // The repeat separator will be escaped in the record),
            new KeyValuePair<string, string>("O.6", "N"),
            new KeyValuePair<string, string>("O.7", "20210309155210"),
            new KeyValuePair<string, string>("O.12", "N"),
            new KeyValuePair<string, string>("O.16", "CENTBLOOD"),

            new KeyValuePair<string, string>("newO", "newO"), // create a new O-Record
            new KeyValuePair<string, string>("O.3", "SID307"),
            new KeyValuePair<string, string>("O.5", "Pheno"),
            new KeyValuePair<string, string>("O.6", "N"),
            new KeyValuePair<string, string>("O.7", "20210309155210"),
            new KeyValuePair<string, string>("O.12", "N"),
            new KeyValuePair<string, string>("O.16", "CENTBLOOD"),

            new KeyValuePair<string, string>("newL", "newL"), // create a new L-Record
            new KeyValuePair<string, string>("L.3", "N"),
        };


        #endregion

        /// <summary>
        /// AstmRecord Example for creating an Order message
        /// 
        /// AstmRecord—Create Order Message creates an Order message from a list
        /// of Key: value pairs. Each key is the location in the record where the
        /// value is written or read.
        /// 
        /// Keys are written in an ASTM Record Notation. In the Key:Value pair of
        /// P.3:PID123456, the record location is P.3 and the value is PID123456. 
        /// 
        /// The Key:Value of P.3:PID123456 indicates that the value should be 
        /// written in the Patient record field P.3. 
        /// 
        /// The ASTM Record Notation format is Record-ID.Field.Repeat.Component. 
        /// 
        /// The notation can be abbreviated as follows:
        /// 
        /// - P.3:PID789012 - Patient record Field 3.
        /// - O.16.2:PLASMA - Order record Field 16, Repeat Field 2 
        /// - P.10.2.3:McCoy - Patient record Field 10, Repeat Field 2, and Component 3. 
        /// </summary>
        static void AstmRecordCreateOrderMessage()
        {
            // TODO: Create message from an order queue.
            // A queue will contain two patient IDs
            // for each PID
            // Retrieve all orders
            // Create the Patient and Order records
            // Send the message with Header and Trailer

            Console.WriteLine();
            Console.WriteLine("AstmRecord - Create Order Message");

            // AstmRecord Create Order Message
            // H|\^&|||Mini LIS||||||||LIS2-A|20210309155210
            // P|1|PID123456|||Brown^Bobby^B|White|196501020304|M|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H
            // O|1|SID304||Type &E& Screen|N|20210309155210|||||N||||CENTBLOOD
            // O|2|SID305||Pheno|N|20210309155210|||||N||||CENTBLOOD
            // P|2|PID789012|||Forbin^Charles|Fisher|19410403|M|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H
            // O|1|SID306||ABO&R&ABScr|N|20210309155210|||||N||||CENTBLOOD
            // O|2|SID307||Pheno|N|20210309155210|||||N||||CENTBLOOD
            // L|1|N

            List<string> astmMessage = new List<string>();

            AstmRecord astmRecord = new AstmRecord();
            int patSequence = 0;
            int orderSequence = 0;

            foreach (var kvp in AstmRecordCreateOrderMessageContent)
            {
                if (kvp.Key.Contains("new") && astmRecord.Text.Length > 0)
                {
                    astmMessage.Add(astmRecord.Text);
                }

                switch (kvp.Key) 
                {
                    case "newH":
                        astmRecord = new AstmRecord();
                        astmRecord.Set("H.1", "H");
                        astmRecord.SetDelimitersInHRecord();
                        patSequence = 0;
                        orderSequence = 0;
                        break;

                    case "newP":
                        orderSequence = 0;

                        astmRecord = new AstmRecord();
                        astmRecord.Set("P.1", "P");
                        astmRecord.Set("P.2", $"{++patSequence}");
                        break;

                    case "newO":
                        astmRecord = new AstmRecord();
                        astmRecord.Set("O.1", "O");
                        astmRecord.Set("O.2", $"{++orderSequence}");
                        break;

                    case "newL":
                        astmRecord = new AstmRecord();
                        astmRecord.Set("L.1", "L");
                        astmRecord.Set("L.2", "1");
                        break;

                    default:
                        astmRecord.Set(kvp.Key, kvp.Value);
                        break;
                }
            }
            if (astmRecord.Text.Length > 0)
            {
                astmMessage.Add(astmRecord.Text);
            }

            Console.WriteLine(string.Join("\r\n", astmMessage));
            Console.WriteLine();

            // Output:
            // 
            // AstmRecord - Create Order Message
            // H|\^&|||Mini LIS|||||||P|LIS2-A|20210309155210
            // P|1|PID123456|||Brown^Bobby^B|White|196501020304|M|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H
            // O|1|SID304||Type &E& Screen|N|20210309155210|||||N||||CENTBLOOD
            // O|2|SID305||Pheno|N|20210309155210|||||N||||CENTBLOOD
            // P|2|PID789012|||Forbin^Charles|Fisher|19410403|M|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H
            // O|1|SID306||ABO&R&ABScr|N|20210309155210|||||N||||CENTBLOOD
            // O|2|SID307||Pheno|N|20210309155210|||||N||||CENTBLOOD
            // L|1|N
        }
        #endregion


        #region AstmRecordCreateQueryMessage
        /// <summary>
        /// AstmRecord Example for creating a Qury message
        /// 
        /// Creates a Query message by using AstmRecord to set the record fields
        /// to their value.
        /// 
        /// For each value, the AstmRecord.Set(string _address, string value) is 
        /// called with the field location and value. 
        /// 
        /// Locations are written in an ASTM Record Notation. A location of P.3 
        /// indicates that the value should be written in Field 3 of the record.
        /// 
        /// The P is the record ID and is optional.
        /// 
        /// The ASTM Record Notation format is: Record-ID.Field.Repeat.Component
        /// 
        /// The notation can be abbreviated as follows:
        /// 
        /// - P.3 - Patient record Field 3.
        /// - O.16.2 - Order record Field 16, Repeat Field 2 
        /// - P.10.2.3 - Patient record Field 10, Repeat Field 2, and Component 3. 
        /// </summary>
        static void AstmRecordCreateQueryMessage()
        {
            Console.WriteLine();
            Console.WriteLine("AstmRecord - Create Query Message");

            // AstmRecord Create Query Message
            // H|\^&|||Mini LIS|||||||P|LIS2-A|20210309140523
            // Q|1|^=W13131200096100||||||||||O
            // L||

            List<string> astmMessage = new List<string>();

            AstmRecord astmRecord = new AstmRecord();
            astmRecord = new AstmRecord();
            astmRecord.Set("H.1", "H");
            astmRecord.Set("H.5", "Mini LIS");
            astmRecord.Set("H.12", "P");
            astmRecord.Set("H.13", "LIS2-A");
            astmRecord.Set("H.14", "20210309155210");
            astmRecord.SetDelimitersInHRecord();
            astmMessage.Add(astmRecord.Text);

            astmRecord = new AstmRecord();
            astmRecord.Set("Q.1", "Q");
            astmRecord.Set("Q.2", "1");
            astmRecord.Set("Q.3.1.2", "=W13131200096100");
            astmRecord.Set("Q.13", "O");
            astmMessage.Add(astmRecord.Text);

            astmRecord = new AstmRecord();
            astmRecord.Set("L.1", "L");
            astmRecord.Set("L.2", "1");
            astmRecord.Set("L.3", "N");
            astmMessage.Add(astmRecord.Text);

            Console.WriteLine(string.Join("\r\n", astmMessage));
            Console.WriteLine();

            // Output:
            // 
            // AstmRecord - Create Query Message
            // H|\^&|||Mini LIS|||||||P|LIS2-A|20210309155210
            // Q|1|^=W13131200096100||||||||||O
            // L|1|N
        }
        #endregion

        static void AstmRecordParseQueryMessage()
        {
            Console.WriteLine();
            Console.WriteLine("AstmRecord - Parse Query Message");

            // AstmRecord - Parse Query Message
            string[] queryMessage =
            {
                @"H|\^&|||Mini LIS|||||||P|LIS2-A|20210309140523",
                @"Q|1|^=W13131200096100||||||||||O",
                @"L|1|N",
            };

            Console.WriteLine(string.Join("\n", queryMessage));
            Console.WriteLine();

            List<List<KeyValuePair<string, string>>> messageContent = new List<List<KeyValuePair<string, string>>>();

            AstmRecord astmRecord = new AstmRecord();
            foreach (var record in queryMessage)
            {
                Console.WriteLine(record);

                astmRecord.Text = record;
                var content = astmRecord.GetAll();

                var table = AstmRecordUtilitiescs.RecordContentToMarkdownTable(content);
                Console.WriteLine(string.Join("\n", table));

                messageContent.Add(content);
            }
            Console.WriteLine();

            Console.WriteLine();

            // Output:
            // 
            // AstmRecord - Parse Query Message
            // H*\^&***Mini LIS*******P*LIS2-A*2021030914052
            // Q*1*^=W13131200096100**********O
            // L*1*N
            // 
            // H.1 : H
            // H.5 : Mini LIS
            // H.12 : P
            // H.13 : LIS2-A
            // H.14 : 2021030914052
            // 
            // Q.1 : Q
            // Q.2 : 1
            // Q.3.1.2 : =W13131200096100
            // Q.13 : O
            // 
            // L.1 : L
            // L.2 : 1
            // L.3 : N
        }

        static void AstmObfuscateMessage()
        {
            Console.WriteLine();
            Console.WriteLine("Astm Obfuscate Message");

            // Obfuscate Personally Identifiable Information (PII)
            // 
            // The purpose is to allow sending messages for review to troubleshoot
            // issues found in the field.
            // Plan:
            // - Obfuscate PII in the message records will be done simply by replacing PII with 5-x's "xxxxxx".


            // Source Message
            string[] sourceMessage =
            {
                @"H|\^&|||Mini LIS||||||||LIS2-A|20210309155210",
                @"P|1|PID123456|||Brown^Bobby^B|White|196501020304|M|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H",
                @"O|1|SID304||Type &E& Screen|N|20210309155210|||||N||||CENTBLOOD",
                @"O|2|SID305||Pheno|N|20210309155210|||||N||||CENTBLOOD",
                @"P|2|PID789012|||Forbin^Charles|Fisher|19410403|M|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H",
                @"O|1|SID306||ABO&R&ABScr|N|20210309155210|||||N||||CENTBLOOD",
                @"O|2|SID307||Pheno|N|20210309155210|||||N||||CENTBLOOD",
                @"L|1|N",
            };

            Console.WriteLine("Source Message");
            Console.WriteLine();

            List<string> outMessge = new List<string>();

            AstmRecord astmRecord = new AstmRecord();

            foreach (var record in sourceMessage)
            {
                Console.WriteLine(record);

                astmRecord.Text = record;
                switch (astmRecord.RecordType)
                {
                    case "H":
                        astmRecord.SetDelimitersInHRecord();
                        break;

                    case "P":
                        string[] replace =
                        {
                            "P.3", "P.6", "P.7", "P.8", "P.9",
                        };

                        foreach (var item in replace)
                        {
                            if (!string.IsNullOrEmpty(astmRecord.Get(item)))
                            {
                                astmRecord.Set(item, "xxxxx");
                            }
                        }
                        break;

                    case "O":
                        if (!string.IsNullOrEmpty(astmRecord.Get("O.3")))
                        {
                            astmRecord.Set("O.3", "xxxxx");
                        }
                        break;

                    default:
                        break;
                }

                outMessge.Add(astmRecord.Text);
            }
            Console.WriteLine();
            Console.WriteLine();

            string text = string.Join("\n", outMessge);
            Console.WriteLine(text);
            Console.WriteLine();
            Console.WriteLine();

            // Output:
            // 
            // H|\^&|||Mini LIS||||||||LIS2-A|20210309155210
            // P|1|xxxxx|||xxxxx|xxxxx|xxxxx|xxxxx|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H
            // O|1|xxxxx||Type &E& Screen|N|20210309155210|||||N||||CENTBLOOD
            // O|2|xxxxx||Pheno|N|20210309155210|||||N||||CENTBLOOD
            // P|2|xxxxx|||xxxxx|xxxxx|xxxxx|xxxxx|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H
            // O|1|xxxxx||ABO&R&ABScr|N|20210309155210|||||N||||CENTBLOOD
            // O|2|xxxxx||Pheno|N|20210309155210|||||N||||CENTBLOOD
            // L|1|N

        }
        static void AstmMappingMessage()
        {
            Console.WriteLine();
            Console.WriteLine("Astm Mapping Message");

            // Source Message
            string[] sourceMessage =
            {
                @"H|\^&|||Mini LIS||||||||LIS2-A|20210309155210",
                @"P|1|PID123456|||Brown^Bobby^B|White|196501020304|M|||||PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H",
                @"O|1|SID304||Type &E& Screen|N|20210309155210|||||N||||CENTBLOOD",
                @"O|2|SID305||Pheno|N|20210309155210|||||N||||CENTBLOOD",
                @"P|2|PID789012|||Forbin^Charles|Fisher|19410403|M|PHY1001^Brewster^Katherine\PHY1002^McCoy^Leonard^H",
                @"O|1|SID306||ABO&R&ABScr|N|20210309155210|||||N||||CENTBLOOD",
                @"O|2|SID307||Pheno|N|20210309155210|||||N||||CENTBLOOD",
                @"L|1|N",
            };

            AstmRecord astmRecord = new AstmRecord();
            astmRecord.Text = sourceMessage[1]; // P[1]

            Console.WriteLine("Extract each item of information from the meassage (position and value).");
            var recordContent = astmRecord.GetAll();
            var table = AstmRecordUtilitiescs.RecordContentToMarkdownTable(recordContent);
            Console.WriteLine(string.Join("\n", table));

            Console.WriteLine("Translate each item of information (positions and values), into tokens and values..");
            AstmRecordMap transMap = AstmRecordMap.ReadAstmTranslationRecordMap(new FileInfo("Vision.TransMap.txt"));
            var recordTable = AstmRecordUtilitiescs.MappedRecordToMarkdown(recordContent, transMap);
            Console.WriteLine(string.Join ("\n", recordTable));

            //List<Dictionary<string, string>> mappedMessage2 = AstmRecordMap.RemapRecord(mappedMessage, transMap, true);

            Console.WriteLine();
        }
    }
}
