namespace Riviera.Kavenegar.Models
{
    using System;
    using System.Collections.Generic;

    public class NewMessageArray
    {
        public NewMessageArray()
        {
            Senders = new List<string>();
            Messages = new List<string>();
            LocalIds = new List<string>();
            Recipients = new List<string>();
            MessageTypes = new List<MessageType>();
        }

        public IList<string> Senders { get; private set; }

        public IList<string> Recipients { get; private set; }

        public IList<string> Messages { get; private set; }

        public IList<MessageType> MessageTypes { get; private set; }

        public IList<string> LocalIds { get; private set; }

        public DateTimeOffset? DeliverDate { get; set; }

        public bool? IsPrivate { get; set; }
    }
}
