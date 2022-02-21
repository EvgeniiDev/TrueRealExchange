using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange
{
    internal class ClassicOrder : Order
    {
        public List<Deal> Deals { get; private set; } = new List<Deal>();

        public override void Update(decimal price)
        {
            foreach (var deal in Deals)
            {
                switch (deal.OrderType)
                {
                    case OrderType.Buy when deal.Price >= price:
                        deal.Status = Status.Close;
                        Amount += deal.Amount;
                        break;
                    case OrderType.Sell when deal.Price <= price:
                        deal.Status = Status.Close;
                        Amount -= deal.Amount < Amount ? Amount : deal.Amount;
                        break;
                }
            }
        }

        public ClassicOrder(Account owner, string pair, Dictionary<decimal, decimal> prices,
            Dictionary<decimal, decimal> takes = null, Dictionary<decimal, decimal> stops = null)
        {
            this.owner = owner;
            Pair = pair;

            foreach (var (key, value) in prices)
            {
                Deals.Add(new Deal(key, value, OrderType.Buy));
            }

            if (takes != null)
            {
                foreach (var (key, value) in takes)
                {
                    Deals.Add(new Deal(key, value, OrderType.Sell));
                }
            }

            if (stops != null)
            {
                foreach (var (key, value) in stops)
                {
                    Deals.Add(new Deal(key, value, OrderType.Sell));
                }
            }
        }

    }
}
