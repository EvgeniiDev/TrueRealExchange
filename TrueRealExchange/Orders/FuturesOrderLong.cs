using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    class FuturesOrderLong : BaseOrder
    {
        private decimal Leverage { get; set; }
        private const decimal feeFactor = 1.002m;
        private decimal liquidationPrice;

        public override void Update(decimal price)
        {
            if (Status == Status.Close)
                return;
            if (liquidationPrice != 0 && liquidationPrice <= price)
            {
                //Ликвидация позиции
                Status = Status.Close;
                //TODO т.к позиция полностью ликвидирована её бы перенести куда-то в другое место мб...
            }
            else
            {
                foreach (var deal in Deals)
                {
                    if (deal.Status == Status.Close)
                        continue;
                    if (lastPrice >= price && deal.Price <= lastPrice && deal.Price >= price
                        || lastPrice <= price && deal.Price >= lastPrice && deal.Price <= price)
                    {
                        deal.Status = Status.Close;
                        switch (deal.OrderType)
                        {
                            case OrderType.Buy:
                            {
                                Amount += deal.Amount;
                                owner.RemoveMoney(deal.Amount * deal.Price / Leverage);
                                TotalSpend += deal.Amount * deal.Price;
                                liquidationPrice = (TotalSpend - TotalSpend / Leverage) / Amount * feeFactor;
                                break;
                            }
                            case OrderType.Sell:
                            {
                                var priceOfSell = deal.Amount * deal.Price;
                                var priceOfBuy = deal.Amount * TotalSpend / Amount;
                                var delta = priceOfSell - priceOfBuy;
                                owner.AddMoney(TotalSpend / Amount / Leverage);
                                if (delta > 0)
                                    owner.AddMoney(delta);
                                else
                                    owner.RemoveMoney(delta);
                                Amount -= deal.Amount;
                                break;
                            }
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
            lastPrice = price;
        }

        FuturesOrderLong(Account owner, string pair, List<Deal> prices, decimal leverage,
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

            Deals.AddRange(prices.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (takes != null)
                Deals.AddRange(takes.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (stops != null)
                Deals.AddRange(stops.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));
        }

        private static bool IsPositive(List<Deal> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            return dictionary.All(x => x.Amount >= 0)
                   && dictionary.All(x => x.Price >= 0);
        }
    }
}
