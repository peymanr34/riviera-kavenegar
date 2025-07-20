namespace Riviera.Kavenegar
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Riviera.Kavenegar.Models;

    /// <summary>
    /// The <c>KavenegarService</c> class.
    /// </summary>
    public class KavenegarService
    {
        private readonly HttpClient _httpClient;
        private readonly KavenegarOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="KavenegarService"/> class.
        /// </summary>
        /// <param name="httpClient">An instance of <see cref="HttpClient"/>.</param>
        /// <param name="options">An instance of <see cref="KavenegarOptions"/>.</param>
        public KavenegarService(HttpClient httpClient, IOptions<KavenegarOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new ArgumentException($"'{nameof(_options.ApiKey)}' cannot be null or whitespace.", nameof(options));
            }

            _jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }

        /// <summary>
        /// Sends a text message.
        /// </summary>
        /// <param name="recipient">Recipient number.</param>
        /// <param name="message">Message text.</param>
        /// <param name="sender">Message sender number.</param>
        /// <remarks>Apparently the optional <paramref name="sender"/> is only available for pro tier subscriptions.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<Message>> SendMessageAsync(string recipient, string message, string? sender = null)
        {
            var newMessage = new NewMessage
            {
                Message = message,
                Sender = sender,
            };

            newMessage.Recipients.Add(recipient);

            var result = await SendMessageAsync(newMessage)
                .ConfigureAwait(false);

            return new Response<Message>
            {
                Return = result.Return,
                Entry = result.Entry?.FirstOrDefault(),
            };
        }

        /// <summary>
        /// Sends a text message.
        /// </summary>
        /// <param name="newMessage">A <see cref="NewMessage"/> instance describing the new meesage to send.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Message>>> SendMessageAsync(NewMessage newMessage)
        {
            if (newMessage is null)
            {
                throw new ArgumentNullException(nameof(newMessage));
            }

            if (newMessage.Recipients?.Any() != true)
            {
                throw new ArgumentException($"'{nameof(newMessage.Recipients)}' cannot be null or empty", nameof(newMessage));
            }

            if (string.IsNullOrWhiteSpace(newMessage.Message))
            {
                throw new ArgumentException($"'{nameof(newMessage.Message)}' cannot be null or whitespace", nameof(newMessage));
            }

            var parameters = new Dictionary<string, string>
            {
                ["receptor"] = string.Join(",", newMessage.Recipients),
                ["message"] = newMessage.Message,
            };

            // Sender is optional, ...sort of.
            if (!string.IsNullOrWhiteSpace(newMessage.Sender))
            {
                parameters.Add("sender", newMessage.Sender);
            }

            // DeliverDate is optional.
            if (newMessage.DeliverDate.HasValue)
            {
                parameters.Add("date", newMessage.DeliverDate.Value.ToUnixTimeSecondsString(CultureInfo.InvariantCulture));
            }

            // MessageType is optional.
            if (newMessage.Type.HasValue)
            {
                parameters.Add("type", newMessage.Type.Value.ToString("d"));
            }

            // LocalId is optional.
            if (newMessage.LocalIds?.Any() == true)
            {
                parameters.Add("localid", string.Join(",", newMessage.LocalIds));
            }

            return await GetJsonAsync<Response<List<Message>>>("sms/send", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends multiple messages at once.
        /// </summary>
        /// <param name="message">A <see cref="NewMessageArray"/> instance describing the new meesages to send.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Message>>> SendArrayAsync(NewMessageArray message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var senders = JsonSerializer.Serialize(message.Senders);
            var recipients = JsonSerializer.Serialize(message.Recipients);
            var messages = JsonSerializer.Serialize(message.Messages);
            var types = JsonSerializer.Serialize(message.MessageTypes);

            var parameters = new Dictionary<string, string>
            {
                ["message"] = messages,
                ["sender"] = senders,
                ["receptor"] = recipients,
                ["type"] = types,
            };

            // LocalId is optional.
            if (message.LocalIds?.Any() == true)
            {
                parameters.Add("localmessageids", JsonSerializer.Serialize(message.LocalIds));
            }

            // DeliverDate is optional.
            if (message.DeliverDate.HasValue)
            {
                parameters.Add("date", message.DeliverDate.Value.ToUnixTimeSecondsString(CultureInfo.InvariantCulture));
            }

            // IsPrivate is optional.
            if (message.IsPrivate.HasValue)
            {
                parameters.Add("hide", message.IsPrivate.Value ? "1" : "0");
            }

            return await GetJsonAsync<Response<List<Message>>>("sms/sendarray", parameters)
            .ConfigureAwait(false);
        }

        /// <summary>
        /// Get status of a message.
        /// </summary>
        /// <param name="messageId">Id of the message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<Status>> GetStatusAsync(long messageId)
        {
            var result = await GetStatusAsync(new[] { messageId })
                .ConfigureAwait(false);

            return new Response<Status>
            {
                Return = result.Return,
                Entry = result.Entry?.FirstOrDefault(),
            };
        }

        /// <summary>
        /// Get status of messages.
        /// </summary>
        /// <param name="messageIds">Message ids.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Status>>> GetStatusAsync(IList<long> messageIds)
        {
            if (messageIds is null)
            {
                throw new ArgumentNullException(nameof(messageIds));
            }

            var parameters = new Dictionary<string, string>
            {
                ["messageid"] = string.Join(",", messageIds),
            };

            return await GetJsonAsync<Response<List<Status>>>("sms/status", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get message status by your local id.
        /// </summary>
        /// <param name="messageId">Id of the message in your database.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<StatusLocal>> GetStatusByLocalIdAsync(string messageId)
        {
            var result = await GetStatusByLocalIdAsync(new[] { messageId })
                .ConfigureAwait(false);

            return new Response<StatusLocal>
            {
                Return = result.Return,
                Entry = result.Entry?.FirstOrDefault(),
            };
        }

        /// <summary>
        /// Get messages status by your local id.
        /// </summary>
        /// <param name="messageIds">Id of the messages in your database.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<StatusLocal>>> GetStatusByLocalIdAsync(IList<string> messageIds)
        {
            if (messageIds is null)
            {
                throw new ArgumentNullException(nameof(messageIds));
            }

            var parameters = new Dictionary<string, string>
            {
                ["localid"] = string.Join(",", messageIds),
            };

            return await GetJsonAsync<Response<List<StatusLocal>>>("sms/statuslocalmessageid", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get detailed information about a message.
        /// </summary>
        /// <remarks>
        /// Consider using <seealso cref="GetStatusAsync(long)"/> for message delivery status.
        /// </remarks>
        /// <param name="messageId">Id of the message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<Message>> GetMessageAsync(long messageId)
        {
            var result = await GetMessageAsync(new[] { messageId })
                .ConfigureAwait(false);

            return new Response<Message>
            {
                Return = result.Return,
                Entry = result.Entry?.FirstOrDefault(),
            };
        }

        /// <summary>
        /// Get detailed information about messages.
        /// </summary>
        /// <remarks>
        /// Consider using <seealso cref="GetStatusAsync(IList{long})"/> messages for delivery status.
        /// </remarks>
        /// <param name="messageIds">Id of the messages.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Message>>> GetMessageAsync(IList<long> messageIds)
        {
            if (messageIds is null)
            {
                throw new ArgumentNullException(nameof(messageIds));
            }

            var parameters = new Dictionary<string, string>
            {
                ["messageid"] = string.Join(",", messageIds),
            };

            return await GetJsonAsync<Response<List<Message>>>("sms/select", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get list of sent messages in a given date.
        /// </summary>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <param name="sender">Sender number.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Message>>> GetOutboxAsync(DateTimeOffset startDate, DateTimeOffset? endDate = null, string? sender = null)
        {
            var parameters = new Dictionary<string, string>
            {
                ["startdate"] = startDate.ToUnixTimeSecondsString(CultureInfo.InvariantCulture),
            };

            // EndDate is optional.
            if (endDate.HasValue)
            {
                parameters.Add("enddate", endDate.Value.ToUnixTimeSecondsString(CultureInfo.InvariantCulture));
            }

            // Sender is optional.
            if (!string.IsNullOrWhiteSpace(sender))
            {
                parameters.Add("sender", sender);
            }

            return await GetJsonAsync<Response<List<Message>>>("sms/selectoutbox", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get latest sent messages.
        /// </summary>
        /// <param name="pagesize">Total amount of items.</param>
        /// <param name="sender">Sender number.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Message>>> GetLatestOutboxAsync(long? pagesize = null, string? sender = null)
        {
            var parameters = new Dictionary<string, string>();

            // Page size is optional.
            if (pagesize.HasValue)
            {
                parameters.Add("pagesize", pagesize.Value.ToString(CultureInfo.InvariantCulture));
            }

            // Sender is optional.
            if (!string.IsNullOrWhiteSpace(sender))
            {
                parameters.Add("sender", sender);
            }

            return await GetJsonAsync<Response<List<Message>>>("sms/latestoutbox", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get sent messages count in a given date.
        /// </summary>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <param name="status">Message status.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<OutboxCount>>> GetOutboxCountAsync(DateTimeOffset startDate, DateTimeOffset? endDate = null, MessageStatus? status = null)
        {
            var parameters = new Dictionary<string, string>
            {
                ["startdate"] = startDate.ToUnixTimeSecondsString(CultureInfo.InvariantCulture),
            };

            // EndDate is optional.
            if (endDate.HasValue)
            {
                parameters.Add("enddate", endDate.Value.ToUnixTimeSecondsString(CultureInfo.InvariantCulture));
            }

            // Status is optional.
            if (status.HasValue)
            {
                parameters.Add("status", status.Value.ToString("d"));
            }

            return await GetJsonAsync<Response<List<OutboxCount>>>("sms/countoutbox", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel a scheduled message.
        /// </summary>
        /// <param name="messageIds">Id of the messages.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Status>>> CancelMessageAsync(IList<long> messageIds)
        {
            if (messageIds is null)
            {
                throw new ArgumentNullException(nameof(messageIds));
            }

            var parameters = new Dictionary<string, string>
            {
                ["messageid"] = string.Join(",", messageIds),
            };

            return await GetJsonAsync<Response<List<Status>>>("sms/cancel", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get received (inbox) messages.
        /// </summary>
        /// <param name="lineNumber">Line number.</param>
        /// <param name="onlyOldMessages">Return already read messages only.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Receive>>> GetInboxAsync(string lineNumber, bool onlyOldMessages = false)
        {
            if (string.IsNullOrWhiteSpace(lineNumber))
            {
                throw new ArgumentException($"'{nameof(lineNumber)}' cannot be null or whitespace", nameof(lineNumber));
            }

            var parameters = new Dictionary<string, string>
            {
                ["linenumber"] = lineNumber,
                ["isread"] = onlyOldMessages ? "1" : "0",
            };

            return await GetJsonAsync<Response<List<Receive>>>("sms/receive", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get inbox messages count.
        /// </summary>
        /// <param name="inboxCount">A <see cref="NewInboxCount"/> instance.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<InboxCount>>> GetInboxCountAsync(NewInboxCount inboxCount)
        {
            if (inboxCount is null)
            {
                throw new ArgumentNullException(nameof(inboxCount));
            }

            var parameters = new Dictionary<string, string>
            {
                ["startdate"] = inboxCount.StartDate.ToUnixTimeSecondsString(CultureInfo.InvariantCulture),
            };

            // EndDate is optional.
            if (inboxCount.EndDate.HasValue)
            {
                parameters.Add("enddate", inboxCount.EndDate.Value.ToUnixTimeSecondsString(CultureInfo.InvariantCulture));
            }

            // LineNumber is optional.
            if (!string.IsNullOrWhiteSpace(inboxCount.LineNumber))
            {
                parameters.Add("linenumber", inboxCount.LineNumber);
            }

            if (inboxCount.IncludeOldMessagesOnly.HasValue)
            {
                parameters.Add("isread", inboxCount.IncludeOldMessagesOnly.Value ? "1" : "0");
            }

            return await GetJsonAsync<Response<List<InboxCount>>>("sms/countinbox", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Send a pre-defined (template) text message.
        /// </summary>
        /// <param name="templateMessage">A <see cref="NewTemplateMessage"/> instance describing the new meesage to send.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Message>>> SendTemplateMessageAsync(NewTemplateMessage templateMessage)
        {
            if (templateMessage is null)
            {
                throw new ArgumentNullException(nameof(templateMessage));
            }

            if (string.IsNullOrWhiteSpace(templateMessage.Recipient))
            {
                throw new ArgumentException($"'{nameof(templateMessage.Recipient)}' cannot be null or whitespace", nameof(templateMessage));
            }

            if (string.IsNullOrWhiteSpace(templateMessage.TemplateName))
            {
                throw new ArgumentException($"'{nameof(templateMessage.TemplateName)}' cannot be null or whitespace", nameof(templateMessage));
            }

            if (templateMessage.Tokens?.Any() != true)
            {
                throw new ArgumentException($"'{nameof(templateMessage.Tokens)}' cannot be null or empty.", nameof(templateMessage));
            }

            var parameters = new Dictionary<string, string>
            {
                ["receptor"] = templateMessage.Recipient,
                ["template"] = templateMessage.TemplateName,
                ["token"] = templateMessage.Tokens.First(),
            };

            // Type is optional.
            if (templateMessage.Type.HasValue)
            {
                parameters.Add("type", templateMessage.Type.Value.ToString("d"));
            }

            int index = 1;
            foreach (string item in templateMessage.Tokens.Skip(1))
            {
                index += 1;
                parameters.Add($"token{index}", item);
            }

            return await GetJsonAsync<Response<List<Message>>>("verify/lookup", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Send a text to speach call.
        /// </summary>
        /// <param name="textToSpeechCall">A <see cref="NewTextToSpeechCall"/> instance describing the new meesage to send.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<List<Message>>> SendTextToSpeechCallAsync(NewTextToSpeechCall textToSpeechCall)
        {
            if (textToSpeechCall is null)
            {
                throw new ArgumentNullException(nameof(textToSpeechCall));
            }

            var parameters = new Dictionary<string, string>
            {
                ["receptor"] = string.Join(",", textToSpeechCall.Recipients),
                ["message"] = textToSpeechCall.Message,
            };

            if (textToSpeechCall.DeliverDate.HasValue)
            {
                parameters.Add("date", textToSpeechCall.DeliverDate.Value.ToUnixTimeSecondsString(CultureInfo.InvariantCulture));
            }

            if (textToSpeechCall.LocalIds?.Any() == true)
            {
                parameters.Add("localid", string.Join(",", textToSpeechCall.LocalIds));
            }

            if (textToSpeechCall.RepeatCount.HasValue)
            {
                parameters.Add("repeat", textToSpeechCall.RepeatCount.Value.ToString(CultureInfo.InvariantCulture));
            }

            return await GetJsonAsync<Response<List<Message>>>("call/maketts", parameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get account information.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<AccountInfo>> GetAccountInfoAsync()
        {
            return await GetJsonAsync<Response<AccountInfo>>("account/info")
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get account configuration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<AccountConfig>> GetAccountConfigAsync()
        {
            return await GetJsonAsync<Response<AccountConfig>>("account/config")
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Set account configuration.
        /// </summary>
        /// <param name="accountConfig">A <see cref="AccountConfig"/> instance describing the new configuration.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Response<AccountConfig>> SetAccountConfigAsync(AccountConfig accountConfig)
        {
            if (accountConfig is null)
            {
                throw new ArgumentNullException(nameof(accountConfig));
            }

            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(accountConfig.ApiLogs))
            {
                parameters.Add("apilogs", accountConfig.ApiLogs);
            }

            if (!string.IsNullOrWhiteSpace(accountConfig.DailyReport))
            {
                parameters.Add("dailyreport", accountConfig.DailyReport);
            }

            if (!string.IsNullOrWhiteSpace(accountConfig.DebugMode))
            {
                parameters.Add("debugmode", accountConfig.DebugMode);
            }

            if (!string.IsNullOrWhiteSpace(accountConfig.DefaultSender))
            {
                parameters.Add("defaultsender", accountConfig.DefaultSender);
            }

            if (accountConfig.MinCreditAlarm.HasValue)
            {
                parameters.Add("mincreditalarm", accountConfig.MinCreditAlarm.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrWhiteSpace(accountConfig.ResendFailed))
            {
                parameters.Add("resendfailed", accountConfig.ResendFailed);
            }

            return await GetJsonAsync<Response<AccountConfig>>("account/config", parameters)
                .ConfigureAwait(false);
        }

        private async Task<T> GetJsonAsync<T>(string requestUrl, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            string url = $"https://api.kavenegar.com/v1/{_options.ApiKey}/{requestUrl}.json";

            using var content = new FormUrlEncodedContent(parameters!);

            using var response = await _httpClient
                .PostAsync(new Uri(url), content, cancellationToken)
                .ConfigureAwait(false);

            string json = await response.Content
#if NET5_0_OR_GREATER
                .ReadAsStringAsync(cancellationToken)
#else
                .ReadAsStringAsync()
#endif
                .ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        private async Task<T> GetJsonAsync<T>(string requestUrl, CancellationToken cancellationToken = default)
        {
            string url = $"https://api.kavenegar.com/v1/{_options.ApiKey}/{requestUrl}.json";

            using var response = await _httpClient
                .GetAsync(new Uri(url), cancellationToken)
                .ConfigureAwait(false);

            string json = await response.Content
#if NET5_0_OR_GREATER
                .ReadAsStringAsync(cancellationToken)
#else
                .ReadAsStringAsync()
#endif
                .ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
    }
}
