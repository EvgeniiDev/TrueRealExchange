using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
