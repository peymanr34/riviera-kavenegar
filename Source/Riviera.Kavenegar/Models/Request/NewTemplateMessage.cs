namespace Riviera.Kavenegar.Models
{
    using System.Collections.Generic;

    public class NewTemplateMessage
    {
        public NewTemplateMessage()
        {
            Recipient = string.Empty;
            TemplateName = string.Empty;
            Tokens = new List<string>();
        }

        public string Recipient { get; set; }

        public string TemplateName { get; set; }

        public IList<string> Tokens { get; private set; }

        public TemplateMessageType? Type { get; set; }
    }
}
