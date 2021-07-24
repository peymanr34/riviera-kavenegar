namespace Riviera.Kavenegar
{
    using System;

    /// <summary>
    /// The <c>KavenegarOptions</c> class.
    /// </summary>
    public class KavenegarOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KavenegarOptions"/> class.
        /// </summary>
        public KavenegarOptions()
            : base()
        {
            ApiKey = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KavenegarOptions"/> class.
        /// </summary>
        /// <param name="apiKey">The api key.</param>
        public KavenegarOptions(string apiKey)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        /// <summary>
        /// Gets or sets apiKey.
        /// </summary>
        public string ApiKey { get; set; }
    }
}
