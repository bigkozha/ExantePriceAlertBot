using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExantePriceAlertBot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace ExantePriceAlertBot
{
    public static class CheckPrice
    {
        [FunctionName("CheckPrice")]
        public static void Run([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var symbolIds = Environment.GetEnvironmentVariable("SymbolsToCheck").Split(" ");
            var symbolTreshHoldPriceDict = new Dictionary<string, decimal>();

            for (int i = 0; i < symbolIds.Length-1; i+=2)
            {
                symbolTreshHoldPriceDict.Add(symbolIds[i], Convert.ToDecimal(symbolIds[i + 1]));
            }

            try
            {
                foreach (var symbolId in symbolTreshHoldPriceDict)
                {
                    var currentPrice = ExanteApiClient.GetCurrentPrice(symbolId.Key).Result;

                    if (currentPrice <= symbolId.Value)
                    {
                        var botToken = Environment.GetEnvironmentVariable("BotToken");
                        var chatIds = Environment.GetEnvironmentVariable("ChatIds").Split(" ");

                        foreach (var chatId in chatIds)
                        {
                            RestClient restClient = new RestClient($"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={symbolId.Key}%20price%20ALLERT%20{currentPrice}");
                            restClient.Execute(new RestRequest(Method.GET));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
