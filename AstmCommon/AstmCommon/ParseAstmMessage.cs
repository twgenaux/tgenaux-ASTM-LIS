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
    }
}
