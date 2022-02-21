using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange
{
    class ClassicOrder
    {
        public Account Owner;
        public decimal Amount;
        public string Pair;

        private List<Deal> deals = new List<Deal>();
        
        public void Update(decimal price)
        {
            foreach(var deal in deals)
            {
                if(deal.OrderType == OrderType.Buy)
                {

                }
            }
        }

        ClassicOrder(Account owner, string pair, Dictionary<decimal,decimal> prices,
            Dictionary<decimal, decimal> takes=null, Dictionary<decimal, decimal> stops=null)
        {
            Owner = owner;
            Pair = pair;
            foreach(var buyDeals in prices)
            {
                var deal = new Deal(buyDeals.Key, buyDeals.Value,OrderType.Buy);
                deals.Add(deal);
            }
            foreach (var buyDeals in takes)
            {
                var deal = new Deal(buyDeals.Key, buyDeals.Value, OrderType.Sell);
                deals.Add(deal);
            }
            foreach (var buyDeals in stops)
            {
                var deal = new Deal(buyDeals.Key, buyDeals.Value, OrderType.Sell);
                deals.Add(deal);
            }
        }
    }
}
