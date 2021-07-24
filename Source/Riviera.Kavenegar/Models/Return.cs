namespace Riviera.Kavenegar.Models
{
    using System.Text.Json.Serialization;

    public class Return
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
