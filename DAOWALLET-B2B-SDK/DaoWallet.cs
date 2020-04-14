using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAOWALLET_B2B_SDK
{
    /// <summary>
    /// Dao wallet
    /// </summary>
    public class DaoWallet
    {
        private string _apiKey;
        private string _secretKey;
        private string _url;

        static DaoWallet()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiKey">Api key</param>
        /// <param name="secretKey">Secret key</param>
        /// <param name="url">url (default: https://b2b.daowallet.com)</param>
        public DaoWallet(string apiKey, string secretKey, string url = "https://b2b.daowallet.com")
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
            _url = UrlCombine(url, "api/v2");
        }

        /// <summary>
        /// Take address
        /// </summary>
        /// <param name="foreignId">Foreign id, max lenght: 128</param>
        /// <param name="currency">Currency name</param>
        public ApiResponse AddressesTake(string foreignId, string currency)
        {
            if (foreignId.Length > 128)
            {
                throw new ArgumentException("foreignId is too long (max length = 128)");
            }

            var req = new Dictionary<string, object>();
            req.Add("foreign_id", foreignId);
            req.Add("currency", currency);

            return ApiQueryAsync("/addresses/take", req).Result;
        }

        /// <summary>
        /// Crypto withdrowal
        /// </summary>
        /// <param name="foreignId">Foreign id, max lenght: 128</param>
        /// <param name="amount">Amount</param>
        /// <param name="currency">Currency name</param>
        /// <param name="address">Address</param>
        public ApiResponse CryptoWithdrowal(string foreignId, decimal amount, string currency, string address)
        {
            if (foreignId.Length > 128)
            {
                throw new ArgumentException("foreignId is too long (max length = 128)");
            }

            var req = new Dictionary<string, object>();
            req.Add("foreign_id", foreignId);
            req.Add("amount", amount);
            req.Add("currency", currency);
            req.Add("address", address);

            return ApiQueryAsync("/withdrawal/crypto", req).Result;
        }

        /// <summary>
        /// Query
        /// </summary>
        private async Task<ApiResponse> ApiQueryAsync(string apiName, IDictionary<string, object> req)
        {
            using (var client = new HttpClient())
            {
                var message = DictionaryToJson(req);
                var signature = HMACSHA512(_secretKey, message);
                client.DefaultRequestHeaders.Add("X-Processing-Key", _apiKey);
                client.DefaultRequestHeaders.Add("X-Processing-Signature", signature);

                var data = new StringContent(message, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(UrlCombine(_url, apiName), data);
                    return new ApiResponse(response.StatusCode, response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception ex)
                {
                    return new ApiResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        public static string UrlCombine(string url1, string url2)
        {
            if (url1 == null || url1.Length == 0)
            {
                return url2;
            }

            if (url2 == null || url2.Length == 0)
            {
                return url1;
            }

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return string.Format("{0}/{1}", url1, url2);
        }

        public static string DictionaryToJson(IDictionary<string, object> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\":{2}{1}{2}", d.Key, d.Value, d.Value is string ? "\"" : string.Empty));
            return "{" + string.Join(",", entries) + "}";
        }

        public static string HMACSHA512(string secretKey, string body)
        {
            using (HMACSHA512 hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
                return ByteToString(b);
            }
        }

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary).ToLowerInvariant();
        }
    }
}
