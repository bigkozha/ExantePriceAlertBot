using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExantePriceAlertBot.Entities
{
    public class Instrument
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("symbolId")]
        public string SymbolId { get; set; }

        [JsonPropertyName("bid")]
        public List<PriceData> Bid { get; set; }

        [JsonPropertyName("ask")]
        public List<PriceData> Ask { get; set; }

        public class PriceData
        {
            [JsonPropertyName("value")]
            public decimal Value { get; set; }

            [JsonPropertyName("size")]
            public decimal Size { get; set; }
        }
    }
}
