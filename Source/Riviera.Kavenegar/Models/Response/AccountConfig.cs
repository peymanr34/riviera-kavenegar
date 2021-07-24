namespace Riviera.Kavenegar.Models
{
    using System.Text.Json.Serialization;

    public class AccountConfig
    {
        [JsonPropertyName("apilogs")]
        public string? ApiLogs { get; set; }

        [JsonPropertyName("dailyreport")]
        public string? DailyReport { get; set; }

        [JsonPropertyName("debugmode")]
        public string? DebugMode { get; set; }

        [JsonPropertyName("defaultsender")]
        public string? DefaultSender { get; set; }

        [JsonPropertyName("mincreditalarm")]
        public int? MinCreditAlarm { get; set; }

        [JsonPropertyName("resendfailed")]
        public string? ResendFailed { get; set; }
    }
}
