using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using PushMeNow.AspNet5.Models;

namespace PushMeNow.AspNet5
{
    internal class NotificationHubApiClient
    {
        #region Private Static Fields and Constants

        private const string ApiVersion = "?api-version=" + XMsVersion;
        private const string AtomSchema = "http://www.w3.org/2005/Atom";
        private const string ConnectSchema = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect";
        private const string XMsContinuationToken = "X-MS-ContinuationToken";
        private const string XMsVersion = "2015-01";
        private static readonly XName XNameContent = XName.Get("content", AtomSchema);
        private static readonly XName XNameTags = XName.Get("Tags", ConnectSchema);

        #endregion

        #region Constructors

        public NotificationHubApiClient(string hubName, string connectionString)
        {
            HubName = hubName;
            ConnectionString = connectionString;
            ParseConnectionInfo();
        }

        #endregion

        #region Private Properties

        private static string ConnectionString { get; set; }
        private static string Endpoint { get; set; }

        private static string HubName { get; set; }
        private static string MessagesUri => Endpoint + HubName + "/messages/" + ApiVersion;

        private static string RegistrationsUri => Endpoint + HubName + "/registrations/" + ApiVersion;
        private static string SasKeyName { get; set; }
        private static string SasKeyValue { get; set; }

        #endregion

        #region Public Methods

        public async Task<IEnumerable<Registration>> GetRegistrationsAsync()
        {
            var uri = RegistrationsUri;
            var registrations = new List<Registration>();
            for (;;)
            {
                var responseMessage = await GetAsync(uri, null);
                var content = await responseMessage.Content.ReadAsStringAsync();
                registrations.AddRange(DeserializeRegistrations(content));
                if (!responseMessage.Headers.Contains(XMsContinuationToken))
                {
                    break;
                }

                uri = RegistrationsUri + "?ContinuationToken=" +
                      responseMessage.Headers.GetValues(XMsContinuationToken).First();
            }

            return registrations;
        }

        public async Task SendNotificationAsync(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            RegistrationType registrationType;
            if (!Enum.TryParse(message.RegistrationType, out registrationType))
            {
                throw new InvalidOperationException("Unsupported Registration Type");
            }

            switch (registrationType)
            {
                case RegistrationType.Windows:
                    await SendWindowsNativeNotificationToastAsync(message.Text, message.Tags);
                    break;
                case RegistrationType.Gcm:
                    await SendGcmNativeNotificationAsync(message.Text, message.Tags);
                    break;
                case RegistrationType.Apple:
                    await SendAppleNativeNotificationAsync(message.Text, message.Tags);
                    break;
            }
        }

        private async Task<HttpResponseMessage> SendWindowsNativeNotificationToastAsync(string text, string tag)
        {
            var payload =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?><toast><visual lang=""en-US""><binding template=""ToastText01""><text id=""1"">" +
                WebUtility.HtmlEncode(text) + "</text></binding></visual></toast>";

            return await
                PostAsync(MessagesUri, new StringContent(payload, Encoding.UTF8, "application/xml"),
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"ServiceBusNotification-Format", new[] {"windows"}},
                        {"ServiceBusNotification-Tags", new[] {tag}},
                        {"X-WNS-Type", new[] {"wns/toast"}}
                    });
        }

        public async Task<HttpResponseMessage> SendAppleNativeNotificationAsync(string text, string tag)
        {
            var payload = "{\"aps\":{\"alert\":" + JsonConvert.ToString(text) + "}}";

            return await
                PostAsync(MessagesUri, new StringContent(payload, Encoding.UTF8, "application/json"),
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"ServiceBusNotification-Format", new[] {"apple"}},
                        {"ServiceBusNotification-Tags", new[] {tag}},
                        {"ServiceBusNotification-Apns-Expiry", new[] {DateTime.Now.AddHours(1).ToString("u")}},
                    });
        }

        public async Task<HttpResponseMessage> SendGcmNativeNotificationAsync(string text, string tag)
        {
            var payload = "{ \"data\" : {\"message\":" + JsonConvert.ToString(text) + "}}";

            return await
                PostAsync(MessagesUri, new StringContent(payload, Encoding.UTF8, "application/json"),
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"ServiceBusNotification-Format", new[] {"gcm"}},
                        {"ServiceBusNotification-Tags", new[] {tag}},
                    });
        }

        #endregion

        #region Private Methods

        private IEnumerable<Registration> DeserializeRegistrations(string content)
        {
            var document = XDocument.Parse(content);
            var contents = document.Descendants(XNameContent);
            var registrations =
                contents.Select(c =>
                    new Registration
                    {
                        RegistrationType =
                            (c.FirstNode as XElement)?.Name?.LocalName?.Replace("RegistrationDescription", ""),
                        Tags = c.Descendants(XNameTags)?.FirstOrDefault()?.Value
                    }).ToArray();
            return registrations;
        }

        private static string GenerateSaSToken(string uri)
        {
            var targetUri = WebUtility.UrlEncode(uri.ToLower())?.ToLower();

            var expiresOnDate = Convert.ToInt64(DateTime.UtcNow.Subtract
                (new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds) + 60*60;
            var toSign = targetUri + "\n" + expiresOnDate;

            var keyBytes = Encoding.UTF8.GetBytes(SasKeyValue);
            var mac = new HMACSHA256(keyBytes);
            mac.Initialize();
            var rawHmac = mac.ComputeHash(Encoding.UTF8.GetBytes(toSign));
            var signature = WebUtility.UrlEncode(Convert.ToBase64String(rawHmac));

            var token = "SharedAccessSignature sr=" + targetUri + "&sig="
                        + signature + "&se=" + expiresOnDate + "&skn=" + SasKeyName;
            return token;
        }

        private static async Task<HttpResponseMessage> GetAsync(string uri,
            IDictionary<string, IEnumerable<string>> headers)
        {
            var httpClient = CreateClient(uri, headers);
            var responseMessage = await httpClient.GetAsync(uri);
            responseMessage.EnsureSuccessStatusCode();
            return responseMessage;
        }

        private static HttpClient CreateClient(string uri, IDictionary<string, IEnumerable<string>> headers)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(GenerateSaSToken(uri));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-ms-version", XMsVersion);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return httpClient;
        }

        // From http://msdn.microsoft.com/en-us/library/dn495627.aspx 
        private static void ParseConnectionInfo()
        {
            if (string.IsNullOrWhiteSpace(HubName))
            {
                throw new InvalidOperationException("Hub name is empty");
            }

            var parts = ConnectionString.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
            {
                throw new InvalidOperationException("Error parsing connection string: " + ConnectionString);
            }

            foreach (var part in parts)
            {
                if (part.StartsWith("Endpoint"))
                {
                    Endpoint = "https" + part.Substring(11);
                }
                else if (part.StartsWith("SharedAccessKeyName"))
                {
                    SasKeyName = part.Substring(20);
                }
                else if (part.StartsWith("SharedAccessKey"))
                {
                    SasKeyValue = part.Substring(16);
                }
            }
        }

        private static async Task<HttpResponseMessage> PostAsync(string uri, HttpContent content,
            IDictionary<string, IEnumerable<string>> headers)
        {
            var httpClient = CreateClient(uri, headers);
            var responseMessage = await httpClient.PostAsync(uri, content ?? new StringContent(""));
            responseMessage.EnsureSuccessStatusCode();
            return responseMessage;
        }

        #endregion
    }
}