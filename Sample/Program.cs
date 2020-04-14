using System;
using System.Net;
using Newtonsoft.Json;

using DAOWALLET_B2B_SDK;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            const string apiKey = "sA4kH4BX4IAKBzm5DpOFoHL6XoUNJ0sP";
            const string secretKey = "GAD0DcpFiS2dSAZFucjScSuUhS9yQNEtHT2es4Fq";
            var dw = new DaoWallet(apiKey, secretKey, "https://b2b.test.daowallet.com");

            // Адрес
            var addressResponse = dw.AddressesTake("user-1250", "BTC");
            Console.WriteLine(string.Format("Address:\n\tStatusCode: {0}\n\tResult: {1}\n", 
                (int)addressResponse.StatusCode, 
                addressResponse.Result));

            // Вывод
            var withdrowalResponse = dw.CryptoWithdrowal("user-1250", Convert.ToDecimal(0.01), "BTC", "1MDY9GwakAUYKPXEnFxrTEufzZW3kTE7Rx");
            Console.WriteLine(string.Format("Withdrowal:\n\tStatusCode: {0}\n\tResult: {1}\n",
                (int)withdrowalResponse.StatusCode, 
                withdrowalResponse.Result));

            // Пример парсинга
            if (addressResponse.StatusCode == HttpStatusCode.OK || addressResponse.StatusCode == HttpStatusCode.Created)
            {
                var address = JsonConvert.DeserializeObject<dynamic>(addressResponse.Result).data.address;
                Console.WriteLine(string.Format("Address parsing example:\n\t{0}", 
                    address));
            }

            Console.ReadKey();
        }
    }
}
