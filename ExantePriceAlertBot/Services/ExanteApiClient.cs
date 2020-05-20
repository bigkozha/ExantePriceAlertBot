
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExantePriceAlertBot.Entities;
using RestSharp;
using RestSharp.Authenticators;

namespace ExantePriceAlertBot.Services
{
    public static class ExanteApiClient
    {
        private static readonly string _accessKey = Environment.GetEnvironmentVariable("AccessKey");
        private static readonly string _applicationId = Environment.GetEnvironmentVariable("ApplicationId");
        private static readonly RestClient _restClient =  new RestClient(Environment.GetEnvironmentVariable("ExanteUrl"));
        
        public static async Task<decimal> GetCurrentPrice(string symbolId)
        {
            try
            {
                _restClient.Authenticator = new HttpBasicAuthenticator(_applicationId, _accessKey); 
                var request = new RestRequest($"md/2.0/feed/{symbolId}/last");
                var instruments = await _restClient.GetAsync<List<Instrument>>(request);

                var result = instruments[0].Ask[0].Value;
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}