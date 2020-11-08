using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace K.JsonMoveAgain
{
    internal static class Program
    {
        private const int MaxOffersPerFeed = 200;
        
        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            var resultFeed = new OffersFeed() {Offers = new List<Offer>(n * MaxOffersPerFeed)};
            for (var i = 0; i < n; i++)
            {
                var feed = JsonConvert.DeserializeObject<OffersFeed>(Console.ReadLine());
                resultFeed.Offers.AddRange(feed.Offers);
            }
            
            resultFeed.Offers.Sort(new OfferComparer());
            Console.WriteLine(JsonConvert.SerializeObject(resultFeed));
        }
    }

    internal sealed class Offer
    {
        [JsonProperty("offer_id", Order = 1)]
        public string OfferId { get; set; }
        
        [JsonProperty("market_sku", Order = 0)]
        public uint MarketSku { get; set; }
        
        [JsonProperty("price", Order = 2)]
        public uint Price { get; set; }
    }

    internal sealed class OffersFeed
    {
        [JsonProperty("offers")]
        public List<Offer> Offers { get; set; }
    }
    
    internal sealed class OfferComparer : IComparer<Offer>
    {
        private static readonly Offer DefaultOffer = new Offer();
        public int Compare(Offer x, Offer y)
        {
            x ??= DefaultOffer;
            y ??= DefaultOffer;
            return (x.Price, x.OfferId).CompareTo((y.Price, y.OfferId));
        }
    }
}