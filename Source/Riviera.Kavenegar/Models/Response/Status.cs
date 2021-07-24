namespace Riviera.Kavenegar.Models
{
    using System.Text.Json.Serialization;

    public class Status
    {
        [JsonPropertyName("messageid")]
        public long Id { get; set; }

        [JsonPropertyName("status")]
        public MessageStatus MessageStatus { get; set; }

        [JsonPropertyName("statustext")]
        public string? StatusText { get; set; }
    }
}
