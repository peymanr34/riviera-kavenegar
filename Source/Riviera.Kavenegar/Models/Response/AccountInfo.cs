namespace Riviera.Kavenegar.Models
{
    using System;
    using System.Text.Json.Serialization;
    using Riviera.Kavenegar;

    public class AccountInfo
    {
        [JsonPropertyName("remaincredit")]
        public long RemainingCredit { get; set; }

        [JsonPropertyName("expiredate")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTimeOffset ExpireDate { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
