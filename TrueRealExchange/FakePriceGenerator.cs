using System.Collections.Generic;

namespace TrueRealExchange
{
    internal class FakePriceGenerator : IExchange
    {

        public Dictionary<string, decimal> prices = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> GetPrices()
        {
            return prices;
        }
    }
}
