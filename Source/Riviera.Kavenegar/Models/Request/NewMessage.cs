namespace Riviera.Kavenegar.Models
{
    using System;
    using System.Collections.Generic;

    public class NewMessage
    {
        public NewMessage()
        {
            Message = string.Empty;
            Recipients = new List<string>();
            LocalIds = new List<string>();
        }

        public string? Sender { get; set; }

        public string Message { get; set; }

        public MessageType? Type { get; set; }

        public DateTimeOffset? DeliverDate { get; set; }

        public IList<string> Recipients { get; private set; }

        public IList<string>? LocalIds { get; private set; }
    }
}
