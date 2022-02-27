using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public class FuturesOrderLong : BaseOrder
    {
        private decimal Leverage { get; set; }
        private const decimal feeFactor = 1.002m;

        public override void UpdateStatusOfDeals(List<Deal> deals, decimal price)
        {
            foreach (var deal in deals.Where(x => x.Status == Status.Open)
                                    .Where(x => IsPriceCrossedLevel(x, price)))
            {
                switch (deal.OrderType)
                {
                    case OrderType.Buy:
                        {
                            ;
                            AveragePrice += (AveragePrice * Amount + deal.Amount * deal.Price) / (Amount + deal.Amount);
                            Amount += deal.Amount;
                            owner.RemoveMoney(deal.Amount * deal.Price / Leverage);
                            ;
                            var TotalSpend = AveragePrice * Amount;
                            liquidationPrice = (TotalSpend - TotalSpend / Leverage) / Amount * feeFactor;
                            break;
                        }
                    case OrderType.Sell:
                        {
                            if (Amount > 0)
                            {
                                var TotalSpend = AveragePrice * Amount;
                                var priceOfSell = deal.Amount * deal.Price;
                                var priceOfBuy = deal.Amount * TotalSpend / Amount;
                                var delta = priceOfSell - priceOfBuy;
                                if (delta > 0)
                                    owner.AddMoney(TotalSpend / Amount / Leverage + delta);
                                else
                                    owner.RemoveMoney(TotalSpend / Amount / Leverage - delta);
                                Amount -= deal.Amount;
                            }
                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }
                deal.Status = Status.Close;
            }
        }


        public FuturesOrderLong(Account owner, string pair, List<Deal> prices, decimal leverage,
                        List<Deal> takes = null, List<Deal> stops = null)
        {
            if (owner.Amount * leverage < prices.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("No money");

            if (!IsPositive(prices) || takes != null && !IsPositive(takes)
                                    || stops != null && !IsPositive(stops))
                throw new Exception("Not correct input");

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
