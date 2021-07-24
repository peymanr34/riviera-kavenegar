namespace Riviera.Kavenegar.Models
{
    /// <summary>
    /// Defines the type of the message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Flash message.
        /// </summary>
        Flash = 0,

        /// <summary>
        /// Regular message.
        /// </summary>
        MobileMemory = 1,

        /// <summary>
        /// Message will be stored in sim-card memory.
        /// </summary>
        SimMemory = 2,

        /// <summary>
        /// Message will be stored in custom application memory.
        /// </summary>
        AppMemory = 3,
    }
}
