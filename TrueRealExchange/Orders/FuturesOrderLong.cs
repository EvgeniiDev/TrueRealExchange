using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public class FuturesOrderLong : BaseOrder
    {
        private decimal Leverage { get; set; }
        private const decimal feeFactor = 1.002m;

        protected override void UpdateStatusOfDeals(List<Deal> deals, decimal price)
        {
            foreach (var deal in deals.Where(x => x.Status == Status.Open)
                         .Where(x => IsPriceCrossedLevel(x, price)))
            {
                switch (deal.OrderType)
                {
                    case OrderType.Buy:
                    {
                        Buy(deal);
                        break;
                    }
                    case OrderType.Sell:
                    {
                        Sell(deal);
                        break;
                    }
                    default:
                        throw new NotImplementedException();
                }
                deal.Status = Status.Close;
            }
        }

        public override void Buy(Deal deal)
        {
            AveragePrice = (AveragePrice * Amount + deal.Amount * deal.Price) / (Amount + deal.Amount);
            Amount += deal.Amount;
            owner.RemoveMoney(deal.Amount * deal.Price / Leverage);
            var totalSpend = AveragePrice * Amount;
            LiquidationPrice = (totalSpend - totalSpend / Leverage) / Amount * feeFactor;
        }

        public override void Sell(Deal deal)
        {
            if (Amount <= 0) return;
            var amount = deal.Amount <= Amount ? deal.Amount : Amount;
            var priceOfSell = amount * deal.Price;
            var priceOfBuy = amount * AveragePrice;
            var delta = priceOfSell - priceOfBuy;
            Amount -= amount;
            owner.AddMoney(AveragePrice * amount / Leverage);
            if (delta > 0)
                owner.AddMoney(delta);
            else
                owner.RemoveMoney(-delta);
        }

        public FuturesOrderLong(Account owner, string pair, List<Deal> prices, decimal leverage,
            List<Deal> takes = null, List<Deal> stops = null)
        {
            if (owner.Amount * leverage < prices.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("Not enough money");

            if (!IsPositive(prices) || takes != null && !IsPositive(takes)
                                    || stops != null && !IsPositive(stops))
                throw new Exception("Incorrect input");

            this.owner = owner;
            Pair = pair;
            Leverage = leverage;
            Status = Status.Open;
            //TODO сразу можно цену ликвидации посчитать

            EntryDeals.AddRange(prices.Select(x => new Deal(x.Price, x.Amount, OrderType.Buy)));

            if (takes != null)
                TakeDeals.AddRange(takes.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (stops != null)
                StopDeals.AddRange(stops.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));
        }
    }
}