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
        private static readonly string _botToken = Environment.GetEnvironmentVariable("BotToken");
        private static readonly string[] _chatIds = Environment.GetEnvironmentVariable("ChatIds").Split(" ");
        private static readonly string[] _symbolIds = Environment.GetEnvironmentVariable("SymbolsToCheck").Split(" ");

        [FunctionName("CheckPrice")]
        public static void Run([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var symbolTreshHoldPriceDict = new Dictionary<string, decimal>();

            for (int i = 0; i < _symbolIds.Length-1; i+=2)
            {
                symbolTreshHoldPriceDict.Add(_symbolIds[i], Convert.ToDecimal(_symbolIds[i + 1]));
            }

            try
            {
                foreach (var symbolId in symbolTreshHoldPriceDict)
                {
                    var currentPrice = ExanteApiClient.GetCurrentPrice(symbolId.Key).Result;

                    if (currentPrice <= symbolId.Value)
                    {
                        foreach (var chatId in _chatIds)
                        {
                            RestClient restClient = new RestClient($"https://api.telegram.org/bot{_botToken}/sendMessage?" +
                                $"chat_id={chatId}" +
                                $"&text={symbolId.Key}%20price%20ALLERT%20{currentPrice}");

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
