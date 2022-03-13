using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public class FuturesOrderLong : BaseOrder
    {
        private const decimal feeFactor = 1.002m;

       

        public override void Buy(Deal deal)
        {
            var amount = deal.Amount;

            deal.Amount -= amount;
            AveragePrice = (AveragePrice * Amount + amount * deal.Price) / (Amount + amount);
            Amount += amount;
            Balance -= amount * deal.Price;
            Console.WriteLine(Balance);
            var totalSpend = Math.Abs(AveragePrice * Amount);
            if (Math.Abs(Amount) < 0.00001m)
                LiquidationPrice = decimal.MinValue;
            else
                LiquidationPrice = (totalSpend - totalSpend / Leverage) / Math.Abs(Amount) * feeFactor;
        }

        public override void Sell(Deal deal)
        {
            if (Amount <= 0) return;
            var amount = deal.Amount <= Amount ? deal.Amount : Amount;
            deal.Amount -= amount;
            Amount -= amount;
            Balance += deal.Price * amount;
        }

        public FuturesOrderLong(Account owner, string pair, List<Deal> entry, int leverage,
            List<Deal> takes = null, List<Deal> stops = null)
            : base(owner,pair,entry,takes,stops)
        {
            if (leverage < 1)
                throw new Exception("Leverage should be more when 1");
            if (owner.Amount * leverage < entry.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("Not enough money");

            Leverage = leverage;

            var totalSum = entry.Sum(x => x.Price * x.Amount);
            StartBalance = totalSum;
            Balance = totalSum;
            owner.RemoveMoney(totalSum / leverage);

            EntryDeals.AddRange(entry.Select(x => new Deal(x.Price, x.Amount, OrderType.Buy)));
            if (takes != null)
                TakeDeals.AddRange(takes.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (stops != null)
                StopDeals.AddRange(stops.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));
        }
    }
}