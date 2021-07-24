namespace Riviera.Kavenegar.Models
{
    using System.Text.Json.Serialization;

    public class Response<T>
        where T : class
    {
        [JsonIgnore]
        public bool IsSuccess => Return?.Status == 200;

        [JsonPropertyName("return")]
        public Return? Return { get; set; }

        [JsonPropertyName("entries")]
        public T? Entry { get; set; }
    }
}
