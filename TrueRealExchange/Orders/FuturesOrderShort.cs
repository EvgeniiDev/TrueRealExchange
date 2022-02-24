using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    class FuturesOrderShort : BaseOrder
    {
        public decimal Leverage { get; set; }

        public override void Update(decimal price)
        {
            foreach (var deal in Deals)
            {
                if ((lastPrice >= price && deal.Price <= lastPrice && deal.Price >= price)
                    || (lastPrice <= price && deal.Price >= lastPrice && deal.Price <= price))
                {
                    deal.Status = Status.Close;
                    //TODO полностью переписать все расчёты под шорт
                    if (deal.OrderType == OrderType.Buy)
                    {
                        Amount += deal.Amount;
                        owner.RemoveMoney(deal.Amount * deal.Price / Leverage);
                    }
                    else if (deal.OrderType == OrderType.Sell)
                    {
                        Amount -= deal.Amount < Amount ? deal.Amount : Amount;
                        //TODO тут скорее всего ошибка в начислении денег на счёт
                        owner.AddMoney(deal.Amount * deal.Price / Leverage);
                    }
                }
            }
            lastPrice = price;
        }

        FuturesOrderShort(Account owner, string pair, Dictionary<decimal, decimal> prices, decimal leverage,
            Dictionary<decimal, decimal> takes = null, Dictionary<decimal, decimal> stops = null)
        {
            if (owner.Amount * leverage < prices.Select(x => x.Value * x.Key).Sum())
            {
                throw new Exception("No money");
            }

            if (!IsPositive(prices)
                || (takes != null && !IsPositive(takes))
                || (stops != null && !IsPositive(stops)))
            {
                throw new Exception("Not correct input");
            }

            this.owner = owner;
            Pair = pair;
            Leverage = leverage;

            Deals.AddRange(prices.Select(x => new Deal(x.Key, x.Value, OrderType.Sell)));

            if (takes != null)
                Deals.AddRange(takes.Select(x => new Deal(x.Key, x.Value, OrderType.Sell)));

            if (stops != null)
                Deals.AddRange(stops.Select(x => new Deal(x.Key, x.Value, OrderType.Sell)));
        }

        private bool IsPositive(Dictionary<decimal, decimal> dictionary)
        {
            return dictionary.Keys.All(x => x >= 0)
                && dictionary.Values.All(x => x >= 0);
        }
    }
}
