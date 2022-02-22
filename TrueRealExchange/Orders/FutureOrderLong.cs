using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueRealExchange.Orders
{
    class FutureOrderLong : Order
    {
        public decimal Leverage { get; set; }
        public List<Deal> Deals { get; private set; } = new List<Deal>();

        decimal lastPrice = 0;
        public override void Update(decimal price)
        {
            foreach (var deal in Deals)
            {
                if ((lastPrice >= price && deal.Price <= lastPrice && deal.Price >= price)
                    || (lastPrice <= price && deal.Price >= lastPrice && deal.Price <= price))
                {
                    if (deal.OrderType == OrderType.Buy)
                    {
                        deal.Status = Status.Close;
                        Amount += deal.Amount;
                        owner.RemoveMoney(deal.Amount * deal.Price / Leverage);
                    }
                    else if (deal.OrderType == OrderType.Sell)
                    {
                        deal.Status = Status.Close;
                        Amount -= deal.Amount < Amount ? deal.Amount : Amount;
                        owner.AddMoney(deal.Amount * deal.Price / Leverage);
                    }
                }
            }
            lastPrice = price;
        }

        public FuturesOrderLong(Account owner, string pair, Dictionary<decimal, decimal> prices, decimal leverage,
            Dictionary<decimal, decimal> takes = null, Dictionary<decimal, decimal> stops = null)
        {
            if (owner.Amount * leverage < prices.Select(x => x.Value * x.Key).Sum())
            {
                throw new Exception("No money");
            }

            if (!IsPositive(prices) || (takes != null && !IsPositive(takes)) || (stops != null && !IsPositive(stops)))
            {
                throw new Exception("Not correct input");
            }

            this.owner = owner;
            Pair = pair;
            Leverage = leverage;
         
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

        private bool IsPositive(Dictionary<decimal, decimal> dictionary)
        {
            return prices.Keys.All(x => x >= 0) && prices.Values.All(x => x >= 0);
        }
    }
}
