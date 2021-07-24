namespace Riviera.Kavenegar.Models
{
    using System;
    using System.Collections.Generic;

    public class NewTextToSpeechCall
    {
        public NewTextToSpeechCall()
        {
            Message = string.Empty;
            Recipients = new List<string>();
            LocalIds = new List<string>();
        }

        public IList<string> Recipients { get; private set; }

        public string Message { get; set; }

        public DateTimeOffset? DeliverDate { get; set; }

        public IList<string> LocalIds { get; private set; }

        public int? RepeatCount { get; set; }
    }
}
