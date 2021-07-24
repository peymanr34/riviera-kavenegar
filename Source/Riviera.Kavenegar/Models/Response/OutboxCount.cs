namespace Riviera.Kavenegar.Models
{
    using System;
    using System.Text.Json.Serialization;
    using Riviera.Kavenegar;

    public class OutboxCount
    {
        [JsonPropertyName("startdate")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("enddate")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTimeOffset EndDate { get; set; }

        [JsonPropertyName("sumpart")]
        public long TotalPages { get; set; }

        [JsonPropertyName("sumcount")]
        public long TotalCount { get; set; }

        [JsonPropertyName("cost")]
        public long Cost { get; set; }
    }
}
