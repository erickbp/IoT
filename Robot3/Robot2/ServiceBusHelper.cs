using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Robot2
{
    public class ServiceBusHelper
    {
        private const string BaseAddress = "https://RobotSpeech.servicebus.windows.net/";
        private const string QueueName = "robotcar1";
        private const string SasKeyName = "ConsumersSharedAccessKey";
        private const string SasKeyValue = "5hKuqKFPckVoz9mtCBHXuKbIZ14GjOV5ky0sdgAnDy8=";

        static string _token;

        public static async Task<string> ReceiveAndDeleteMessage()
        {
            if (_token == null)
                _token = GetSasToken();

            HttpResponseMessage response = null;
            try
            {
                var httpClient = new HttpClient();
                var fullAddress = BaseAddress + QueueName + "/messages/head" + "?timeout=3600000";

                httpClient.DefaultRequestHeaders.Add("Authorization", _token);
                response = await httpClient.DeleteAsync(fullAddress);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                if (response == null || response.StatusCode != HttpStatusCode.Unauthorized)
                    return null;

                _token = GetSasToken();
                return await ReceiveAndDeleteMessage();
            }
        }

        private static string GetSasToken()
        {

            var fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + 20 * 60);

            var stringToSign = WebUtility.UrlEncode(BaseAddress) + "\n" + expiry;

            var signature = GetSha256Key(SasKeyValue, stringToSign);

            var sasToken = string.Format(CultureInfo.InvariantCulture,
                "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                WebUtility.UrlEncode(BaseAddress), WebUtility.UrlEncode(signature), expiry, SasKeyName);
            return sasToken;
        }

        private static string GetSha256Key(string hashKey, string stringToSign)
        {
            var macAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA256");
            var encoding = BinaryStringEncoding.Utf8;
            var messageBuffer = CryptographicBuffer.ConvertStringToBinary(stringToSign, encoding);
            var keyBuffer = CryptographicBuffer.ConvertStringToBinary(hashKey, encoding);
            var hmacKey = macAlgorithmProvider.CreateKey(keyBuffer);
            var signedMessage = CryptographicEngine.Sign(hmacKey, messageBuffer);
            return CryptographicBuffer.EncodeToBase64String(signedMessage);
        }

    }
}
