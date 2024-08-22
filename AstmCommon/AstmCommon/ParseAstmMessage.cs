using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tgenaux.astm
{
    public class ParseAstmMessage
    {
        public static List<List<KeyValuePair<string, string>>> ExtractMessageContent(List<string> message)
        {
            List<List<KeyValuePair<string, string>>> messageContent = new List<List<KeyValuePair<string, string>>>();
            AstmRecord astmRecord = new AstmRecord();

            foreach (var record in message)
            {
                astmRecord = new AstmRecord();
                astmRecord.Text = record;
                var recordContent = astmRecord.GetAll();
                messageContent.Add(recordContent);
            }

            return messageContent;
        }

        /// <summary>
        /// Low level message creation
        /// The level above this would create the message content from 
        /// data objects associated with the order.
        ///  - Patient demographics
        ///  - Requested tests
        ///  - Histroical results
        /// </summary>
        /// <param name="messageContent"></param>
        /// <returns></returns>
        public static List<string> CreateMessage(List<List<KeyValuePair<string, string>>> messageContent, string delimiters= @"|\^&")
        {
            List<string> astmMessage = new List<string>();

            AstmRecord astmRecord = new AstmRecord();
            astmRecord.Delimiters = delimiters;

            int patSequence = 0;
            int orderSequence = 0;
            foreach (var record in messageContent)
            {
                foreach (var kvp in record)
                {
                    astmRecord.Set(kvp.Key, kvp.Value);
                }
                if (astmRecord.Text.Length > 0)
                {
                    astmMessage.Add(astmRecord.Text);
                    astmRecord.Text = "";
                }
            }

            return astmMessage;
        }

    }
}
