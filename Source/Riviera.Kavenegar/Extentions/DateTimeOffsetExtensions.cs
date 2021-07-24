namespace Riviera.Kavenegar
{
    using System;

    /// <summary>
    /// Extensions for <see cref="DateTimeOffset"/>.
    /// </summary>
    internal static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Gets the number of seconds that have elapsed since 1970-01-01T00:00:00Z as string.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> object.</param>
        /// <param name="provider">An System.IFormatProvider that supplies culture-specific formatting information.</param>
        /// <returns>Returns the number of seconds that have elapsed since 1970-01-01T00:00:00Z as string.</returns>
        public static string ToUnixTimeSecondsString(this DateTimeOffset dateTimeOffset, IFormatProvider provider)
        {
            return dateTimeOffset.ToUnixTimeSeconds()
                .ToString(provider);
        }
    }
}
