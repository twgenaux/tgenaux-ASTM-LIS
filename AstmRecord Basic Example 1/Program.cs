using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tgenaux.astm;

namespace AstmRecord_Basic_Example_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AstmRecordCreateOrderMessage();
            AstmRecordCreateQuryMessage();
        }

        #region AstmRecordCreateOrderMessageContent
        static string[] AstmRecordCreateOrderMessageContent =
            {
            "newH",          // newH identifies creating a new H-Record
            "H.5:Mini LIS",
            "H.12:P",
            "H.13:LIS2-A",
            "H.14:20210309155210",

            "newP",          // newP identifies creating a new P-Record
            "P.3:PID123456",
            "P.6.1.1:Brown",
            "P.6.1.2:Bobby",
            "P.6.1.3:B",
            "P.7:White",
            "P.8:196501020304",
            "P.9:M",
            "P.10.1.1:PHY1001",
            "P.10.1.2:Brewster",
            "P.10.1.3:Katherine",
            "P.10.2.1:PHY1002",
            "P.10.2.2:McCoy",
            "P.10.2.3:Leonard",
            "P.10.2.4:H",

            "newO",          // newO identifies creating a new O-Record
            "O.3:SID304",
            "O.5:Type & Screen", // The escape character will be escaped in the record
            "O.6:N",
            "O.7:20210309155210",
            "O.12:N",
            "O.16:CENTBLOOD",

            "newO",          // newO identifies creating a new O-Record
            "O.3:SID305",
            "O.5:Pheno",
            "O.6:N",
            "O.7:20210309155210",
            "O.12:N",
            "O.16:CENTBLOOD",

            "newP",          // newP identifies creating a new P-Record
            "P.3:PID789012",
            "P.6:Forbin^Charles",
            "P.6.1.1:Forbin",
            "P.6.1.2:Charles",
            "P.7:Fisher",
            "P.8:19410403",
            "P.9:M",
            "P.10.1.1:PHY1001",
            "P.10.1.2:Brewster",
            "P.10.1.3:Katherine",
            "P.10.2.1:PHY1002",
            "P.10.2.2:McCoy",
            "P.10.2.3:Leonard",
            "P.10.2.4:H",

            "newO",          // newO identifies creating a new O-Record
            "O.3:SID306",
            @"O.5:ABO\ABScr", // The repeat separator will be escaped in the record
            "O.6:N",
            "O.7:20210309155210",
            "O.12:N",
            "O.16:CENTBLOOD",

            "newO",          // newO identifies creating a new O-Record
            "O.3:SID307",
            "O.5:Pheno",
            "O.6:N",
            "O.7:20210309155210",
            "O.12:N",
            "O.16:CENTBLOOD",

            "newL",          // newL identifies creating a new L-Record
            "L.3:N",
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

            foreach (var item in AstmRecordCreateOrderMessageContent)
            {
                if (item.Contains("new") && astmRecord.Text.Length > 0)
                {
                    astmMessage.Add(astmRecord.Text);
                }

                switch (item) 
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
                        char[] seps = { ':' };
                        List<string> parts = item.Split(seps).ToList();

                        astmRecord.Set(parts[0], parts[1]);
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
        static void AstmRecordCreateQuryMessage()
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
            astmRecord.Set("L.2", "");
            astmRecord.Set("L.3", "");
            astmMessage.Add(astmRecord.Text);

            Console.WriteLine(string.Join("\r\n", astmMessage));
            Console.WriteLine();

            // Output:
            // 
            // AstmRecord - Create Query Message
            // H|\^&|||Mini LIS|||||||P|LIS2-A|20210309155210
            // Q|1|^=W13131200096100||||||||||O
            // L||
        }
    }
}
