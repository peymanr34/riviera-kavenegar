namespace Riviera.Kavenegar.Models
{
    using System;

    public class NewInboxCount
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string? LineNumber { get; set; }

        public bool? IncludeOldMessagesOnly { get; set; }
    }
}
