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
            foreach (var deal in deals)
            {
                if (deal.OrderType == OrderType.Buy && deal.Price >= price)
                {
                    deal.Status = Status.Close;
                    Amount += deal.Amount;
                }
                else if (deal.OrderType == OrderType.Sell && deal.Price <= price)
                {
                    deal.Status = Status.Close;
                    Amount -= deal.Amount < Amount ? Amount : deal.Amount;
                }
            }
        }

        ClassicOrder(Account owner, string pair, Dictionary<decimal, decimal> prices,
            Dictionary<decimal, decimal> takes = null, Dictionary<decimal, decimal> stops = null)
        {
            Owner = owner;
            Pair = pair;

            foreach (var buyDeals in prices)
            {
                deals.Add(new Deal(buyDeals.Key, buyDeals.Value, OrderType.Buy));
            }

            foreach (var buyDeals in takes)
            {
                deals.Add(new Deal(buyDeals.Key, buyDeals.Value, OrderType.Sell));
            }

            foreach (var buyDeals in stops)
            {
                deals.Add(new Deal(buyDeals.Key, buyDeals.Value, OrderType.Sell));
            }
        }

    }
}
