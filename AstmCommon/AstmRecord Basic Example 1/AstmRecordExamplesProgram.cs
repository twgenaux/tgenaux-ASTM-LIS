using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tgenaux.astm;
using Newtonsoft.Json;

using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using System.Security.Policy;

namespace AstmRecord_Basic_Example_1
{
    internal class AstmRecordExamplesProgram
    {
        static void Main(string[] args)
        {
            // AstmConveertRecordToJson();
            // AstmRecordCreateQueryMessage();
            AstmRecordParseQueryMessage();
            AstmRecord5600Message();
            // AstmObfuscateMessage();
        }


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
        static void AstmRecord5600Message()
        {
            Console.WriteLine();
            Console.WriteLine("AstmRecord - Parse Query Message");

            // AstmRecord - Parse Query Message
            string[] refMessage =
            {
                @"H|\^&|||LITT||||||||LIS2-A|20061214093913",
                @"P|1|U000856|||ORR^ABIGAIL^G||19780407|F||843 TALL OAKS DR^HAILVILLE, MD 45831|||RASHAMDRA^SANJAY^V|S|||||||||||U7",
                @"C|1|N|Patient is complaining of shortness of breath and chest pain.|G",
                @"O|1|S4331009704||^^^1.0+300+1.0\300+2.0\001+1.0|R||20061214093913||||N||||5||||||||||O",
                @"O|2|U4331009704||^^^1.0+301+1.0|R||20061214093913||||N||||3||||||||||O",
                @"L|1|N",
            };

            Console.WriteLine(string.Join("\n", refMessage));
            Console.WriteLine();

            List<List<KeyValuePair<string, string>>> messageContent = new List<List<KeyValuePair<string, string>>>();
            List<AstmRecordMap> messageMap = new List<AstmRecordMap>();

            AstmRecord astmRecord = new AstmRecord();
            foreach (var record in refMessage)
            {
                Console.WriteLine(record);

                astmRecord.Text = record;
                var content = astmRecord.GetAll();
                AstmRecordMap newMap = new AstmRecordMap(content);
                messageMap.Add(newMap);


                var table = AstmRecordUtilitiescs.RecordContentToMarkdownTable(content);
                Console.WriteLine(string.Join("\n", table));

                messageContent.Add(content);
               
            }
            AstmRecordMap.SaveMessageMap(messageMap, @"E:\5600RecordMap.txt");

            AstmRecordMap transMap = AstmRecordMap.ReadAstmTranslationRecordMap(new FileInfo("5600TransMap.txt"));

             var mappedContent = AstmRecordMap.RemapMessageContent(messageContent, transMap);
          
            Console.WriteLine();

            Console.WriteLine();
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
        static void AstmConveertRecordToJson()
        {
            Console.WriteLine();
            Console.WriteLine("Extracting Astm Record Contnet");
            Console.WriteLine();

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

            Console.WriteLine("1. Extract all content from the record as Position/Value pairs.");
            Console.WriteLine(astmRecord.Text);
            Console.WriteLine();

            astmRecord.SupressTopLevel = false;
            var recordContent = astmRecord.GetAll();
            var table = AstmRecordUtilitiescs.RecordContentToMarkdownTable(recordContent);
            Console.WriteLine(string.Join("\n", table));

            Console.WriteLine("2.Translate all content from Position/Value to Token/Value pairs as shown in this table");
            AstmRecordMap transMap = AstmRecordMap.ReadAstmTranslationRecordMap(new FileInfo("Vision.TransMap.txt"));

            var recordTable = AstmRecordUtilitiescs.MappedRecordToMarkdown(recordContent, transMap);
            Console.WriteLine(string.Join ("\n", recordTable));
            Console.WriteLine();

            Console.WriteLine("3. After translating, we have all the original content as Token/Value pairs");
            List<KeyValuePair<string, string>> mappedContent = AstmRecordMap.RemapRecordContent(recordContent, transMap);
            recordTable = AstmRecordUtilitiescs.RecordContentToMarkdownTable(mappedContent);
            Console.WriteLine(string.Join("\n", recordTable));
            Console.WriteLine();


            //  Extract all message content from each record as Position/Value pairs
            List<List<KeyValuePair<string, string>>> messageContent = new List<List<KeyValuePair<string, string>>>();

            messageContent = ParseAstmMessage.ExtractMessageContent(sourceMessage.ToList());
            var messageTables = AstmRecordUtilitiescs.MessageContentToMarkdownTable(messageContent);

            // TODO save as a Json file?
            var jsonArray = new JArray();
            foreach (var list in messageContent)
            {
                var jsonObject2 = new JObject();
                foreach (var pair in list)
                {
                    jsonObject2[pair.Key] = pair.Value;
                }
                jsonArray.Add(jsonObject2);
            }

            astmRecord = new AstmRecord();
            astmRecord.SupressTopLevel = false;

            List<string> patientContent = new List<string>();
            patientContent = File.ReadAllLines("PatientTransMap.txt").ToList();
            
            foreach (var line in patientContent)
            {
                string[] parts = line.Split(':');
                astmRecord.Set(parts[0], parts[1]);
            }


            // Translate all message content from Position/Value to Token/Value pairs
            //messageContent = AstmRecordMap.RemapMessageContent(messageContent, transMap);

            // TODO
            // Iterate over the sourceMessage
            // Convert to a Json message, such that each patient is one object with all orders nested within it
            // Components should be written as a new named objects
            // Timestamps should be converted to DatTime before adding them?

            Dictionary<string, string> recordTypes = new Dictionary<string, string>();
            recordTypes["H"] = "Header";
            recordTypes["P"] = "Patient";
            recordTypes["O"] = "Order";
            recordTypes["L"] = "TerminationCode";

            AstmRecordMap patientMap = AstmRecordMap.ReadAstmTranslationRecordMap(new FileInfo("PatientTransMap.txt"));

            // TODO Replace this with a file that includes
            //string patientMap = @"P|SeqNumber|PatientID|PatientIDLab|PatientID3" + 
            //    @"|LastName^FirstName^MI|MothersMaiden|BirthDate|Sex||||" +
            //    @"|Attending-ID^Attending-LastName^Attending-FirstName^Attending-MI" +
            //    @"\Attending-ID2^Attending-LastName2^Attending-FirstName2^Attending-MI2";

            //astmRecord = new AstmRecord();
            //astmRecord.SupressTopLevel = false;
            //astmRecord.Text = patientMap;
            //var temp = astmRecord.GetAll();
            //AstmRecordMap patientTransMap = new AstmRecordMap(temp);


            var jsaonMessage = jsonArray.ToString();

            var jsonObject = new JObject();
            foreach (var record in sourceMessage)
            {
                astmRecord = new AstmRecord();
                astmRecord.SupressTopLevel = false;
                astmRecord.Text = record;
                var content = astmRecord.GetAll();
                //var mapped = AstmRecordMap.RemapRecordContent(content, transMap);

                var recordType = astmRecord.RecordType;
                // If Headder, add header object with sender and reciever information plus timestamp
                // If L-record, capture code if available

                if (recordType == "P")
                {
                    var mapped = AstmRecordMap.RemapRecordContent(content, patientMap);

                    // if patient, iterate through subrecords under the patient, adding as new objects
                    var listName = $"{recordTypes[astmRecord.RecordType]}";
                    var jObject = new JObject();
                    foreach (var pair in mapped)
                    {
                        // FIlter out record type ID
                        // Extract two Attending and parse them into a list of mapped types
                        jObject[pair.Key] = pair.Value;
                    }
                    jsonObject[listName] = jObject;
                }
            }
            var jsaonMessage2 = jsonObject.ToString();

            // Something to think about for converting to Json
            //
            // List<int> Position = 0;   ? update each loop
            // JsonMessage
            //  var msgjObject = new JObject();
            //
            // foreach AstmRecord Record in Message
            //     AstmRecord astmRecord
            //     
            //     foreach AstmRecord Field in Record
            //         AstmRecord field;
            //         
            //         foreach Repeat in field
            //             AstmRecord repeat;
            // 
            //             foreach Componet
            //                 AstmRecord componet;
            // 

            messageTables = AstmRecordUtilitiescs.MessageContentToMarkdownTable(messageContent);

            // Translate all message content from Token/Value to Position/Value pairs
            messageContent = AstmRecordMap.RemapMessageContent(messageContent, transMap);
            messageTables = AstmRecordUtilitiescs.MessageContentToMarkdownTable(messageContent);

            // Create ASTM message from message content Position/Value pairs
            var newMessage1 = ParseAstmMessage.CreateMessage(messageContent);
            Console.WriteLine(string.Join("\n", newMessage1));
            Console.WriteLine();
            var newMessage2 = ParseAstmMessage.CreateMessage(messageContent, @"%\^&");
            Console.WriteLine(string.Join("\n", newMessage2));


            Console.WriteLine();

        }
    }
    
}
