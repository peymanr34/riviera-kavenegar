namespace Riviera.Kavenegar.Models
{
    using System.Text.Json.Serialization;

    public class StatusLocal
    {
        [JsonPropertyName("messageid")]
        public long Id { get; set; }

        [JsonPropertyName("localid")]
        public long LocalId { get; set; }

        [JsonPropertyName("status")]
        public MessageStatus Status { get; set; }

        [JsonPropertyName("statustext")]
        public string? StatusText { get; set; }
    }
}
