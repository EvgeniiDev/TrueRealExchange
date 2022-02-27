﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    class FuturesOrderLong : BaseOrder
    {
        private decimal Leverage { get; set; }
        private const decimal feeFactor = 1.002m;
        private decimal liquidationPrice;

        public override void UpdateStatusOfOrder(decimal price)
        {
            if (Status == Status.Close || lastPrice == 0)
                return;
            if (liquidationPrice != 0 && liquidationPrice <= price)
            {
                //Ликвидация позиции
                Status = Status.Close;
                //TODO т.к позиция полностью ликвидирована её бы перенести куда-то в другое место мб...
            }
            else
            {
                UpdateStatusOfDeals(EntryDeals, price);
                UpdateStatusOfDeals(TakeDeals, price);
                UpdateStatusOfDeals(StopDeals, price);
            }
            lastPrice = price;
        }

        public void UpdateStatusOfDeals(List<Deal> deals, decimal price)
        {
            foreach (var deal in deals.Where(x => x.Status == Status.Open)
                                    .Where(x => IsPriceCrossedLevel(x, price)))
            {
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
                deal.Status = Status.Close;
                if (deals.All(x => x.Status == Status.Close))
                    Status = Status.Close;
            }
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

            EntryDeals.AddRange(prices.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (takes != null)
                TakeDeals.AddRange(takes.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (stops != null)
                StopDeals.AddRange(stops.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));
        }
    }
}
