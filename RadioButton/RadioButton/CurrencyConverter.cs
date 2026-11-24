using Microsoft.Maui.Storage;
using Org.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace RadioButton
{
    public class CurrencyConverter
    {
        private readonly HttpClient _httpClient;

        public CurrencyRates ratesUSD;
        public string USDtoEUR;
        public string USDtoBRL;
        public string USDtoARS;

        public CurrencyRates ratesBRL;
        public string BRLtoUSD;
        public string BRLtoEUR;
        public string BRLtoARS;

        public CurrencyRates ratesEUR;
        public string EURtoUSD;
        public string EURtoBRL;
        public string EURtoARS;

        public CurrencyRates ratesARS;
        public string ARStoUSD;
        public string ARStoEUR;
        public string ARStoBRL;

        public string cacheDir = FileSystem.Current.CacheDirectory;

        public CurrencyConverter()
        {
            _httpClient = new HttpClient();

            GetAllCurrencyRates();
        }

        public async Task<CurrencyRates> GetRates(string base_code)
        {
            string url = $"https://open.er-api.com/v6/latest/{base_code}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            string tempRatesFilePath = Path.Combine(cacheDir, $"temp{base_code}rates.txt");
            if (response.IsSuccessStatusCode)
            {

                string JsonString = await response.Content.ReadAsStringAsync();

                await File.WriteAllTextAsync(tempRatesFilePath, JsonString);

                return JsonSerializer.Deserialize<CurrencyRates>(JsonString);
            }
                string content = await File.ReadAllTextAsync(tempRatesFilePath);

                return JsonSerializer.Deserialize<CurrencyRates>(content);
        }

        private async void GetAllCurrencyRates()
        {
            ratesUSD = await GetRates("USD");
            ratesBRL = await GetRates("BRL");
            ratesEUR = await GetRates("EUR");
            ratesARS = await GetRates("ARS");
        }

        public decimal ConvertValue(decimal amount, string from, string to)
        {
            if (from == "USD")
            {
                return amount * ratesUSD.rates[to];
            }
            else if (from == "BRL")
            {
                return amount * ratesBRL.rates[to];
            }
            else if (from == "EUR")
            {
                return amount * ratesEUR.rates[to];
            }
            return 0;
        }
    }
}
