using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange
{
    internal class ClassicOrder : Order
    {
        public List<Deal> Deals { get; private set; } = new List<Deal>();
        private decimal lastPrice=0;

        public override void Update(decimal price)
        {
            
            foreach (var deal in Deals)
            {
                //TODO тут нужно запоминать старую цену, чтобы понимать ,проищошел рост цены или падение.
                if (deal.OrderType == OrderType.Buy)
                {
                    //TODO при первом заходе в цикл мы не можем купить шоколадные монетки потому что ластпрайс = 0
                    if (deal.Price <= lastPrice && deal.Price >= price)
                    {
                        deal.Status = Status.Close;
                        Amount += deal.Amount;
                    }
                }
                else if (deal.OrderType == OrderType.Sell)
                {
                    if (deal.Price>=lastPrice && deal.Price <= price)
                    {
                        deal.Status = Status.Close;
                        Amount -= deal.Amount < Amount ? Amount : deal.Amount;
                    }
                }
            }
            lastPrice = price;
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
