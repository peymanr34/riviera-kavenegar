namespace Riviera.Kavenegar.Models
{
    using System;
    using System.Text.Json.Serialization;
    using Riviera.Kavenegar;

    public class Message
    {
        [JsonPropertyName("messageid")]
        public long Id { get; set; }

        [JsonPropertyName("message")]
        public string? Text { get; set; }

        [JsonPropertyName("status")]
        public MessageStatus Status { get; set; }

        [JsonPropertyName("statustext")]
        public string? StatusText { get; set; }

        [JsonPropertyName("sender")]
        public string? Sender { get; set; }

        [JsonPropertyName("receptor")]
        public string? Recipient { get; set; }

        [JsonPropertyName("cost")]
        public int Cost { get; set; }

        [JsonPropertyName("date")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTimeOffset Date { get; set; }
    }
}
