namespace Riviera.Kavenegar.Models
{
    /// <summary>
    /// Status of the message.
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// Message has been queued for send.
        /// </summary>
        Queued = 1,

        /// <summary>
        /// Message is in the schedule.
        /// </summary>
        Scheduled = 2,

        /// <summary>
        /// Message has been sent to the carrier.
        /// </summary>
        SentToCarrier = 4,

        /// <summary>
        /// Message has been sent to the carrier.
        /// </summary>
        SentToCarrier2 = 5,

        /// <summary>
        /// Message has not been sent.
        /// </summary>
        Failed = 6,

        /// <summary>
        /// Message has been delivered.
        /// </summary>
        Delivered = 10,

        /// <summary>
        /// Message has not been delivered.
        /// </summary>
        NotDelivered = 11,

        /// <summary>
        /// Message has been cancelled or failed to sent.
        /// </summary>
        Cancelled = 13,

        /// <summary>
        /// Message has been blocked due to restrictions on the recipient number.
        /// </summary>
        Blocked = 14,

        /// <summary>
        /// Incorrect message id.
        /// </summary>
        Incorrect = 100,
    }
}
