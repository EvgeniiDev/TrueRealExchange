﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    class FuturesOrderLong : BaseOrder
    {
        public decimal Leverage { get; set; }
        static decimal feeFactor = 1.002m;

        public override void Update(decimal price)
        {
            foreach (var deal in Deals)
            {
                if ((lastPrice >= price && deal.Price <= lastPrice && deal.Price >= price)
                    || (lastPrice <= price && deal.Price >= lastPrice && deal.Price <= price))
                {
                    deal.Status = Status.Close;
                    if (deal.OrderType == OrderType.Buy)
                    {
                        Amount += deal.Amount;
                        owner.RemoveMoney(deal.Amount * deal.Price / Leverage);
                        TotalSpend += deal.Amount * deal.Price;
                        //TODO посчитать цену ликвидации
                        Deals.Where(x => x.OrderType == OrderType.LiqLong);
                        var liqPrice = (TotalSpend - (TotalSpend / Leverage)) / Amount * feeFactor;

                    }
                    else if (deal.OrderType == OrderType.Sell && Amount > 0)
                    {
                        var priceOfSell = deal.Amount * deal.Price;
                        var priceOfBuy = deal.Amount * TotalSpend / Amount;
                        var delta = priceOfSell - priceOfBuy;
                        owner.AddMoney((TotalSpend / Amount)/Leverage);
                        if (delta > 0)
                            owner.AddMoney(delta);
                        else
                            owner.RemoveMoney(delta);
                    }
                }
            }
            lastPrice = price;
        }

        FuturesOrderLong(Account owner, string pair, List<Deal> prices, decimal leverage,
            List<Deal> takes = null, List<Deal> stops = null)
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

            Deals.AddRange(prices.Select(x => new Deal(x.Amount, x.Price, OrderType.Sell)));

            if (takes != null)
                Deals.AddRange(takes.Select(x => new Deal(x.Amount, x.Price, OrderType.Sell)));

            if (stops != null)
                Deals.AddRange(stops.Select(x => new Deal(x.Amount, x.Price, OrderType.Sell)));
        }

        private bool IsPositive(List<Deal> dictionary)
        {
            return dictionary.All(x => x.Amount >= 0)
                && dictionary.All(x => x.Price >= 0);
        }
    }
}
