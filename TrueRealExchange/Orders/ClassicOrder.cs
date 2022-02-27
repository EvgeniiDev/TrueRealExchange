using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange
{
    public class ClassicOrder : BaseOrder
    {
        public override void UpdateStatusOfOrder(decimal price)
        {
            if (Status == Status.Close)
                //return;
                if (lastPrice == 0)
                {
                    lastPrice = price;
                    return;
                }

            UpdateStatusOfDeals(EntryDeals, price);
            if (TakeDeals.Count == 0 && StopDeals.Count == 0 && EntryDeals.All(x => x.Status == Status.Close))
                Status = Status.Close;

            UpdateStatusOfDeals(TakeDeals, price);
            if (TakeDeals.Count > 0 && TakeDeals.All(x => x.Status == Status.Close))
                Status = Status.Close;

            UpdateStatusOfDeals(StopDeals, price);
            if (StopDeals.Count > 0 && StopDeals.All(x => x.Status == Status.Close))
                Status = Status.Close;

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
                        Amount += deal.Amount;
                        owner.RemoveMoney(deal.Amount * deal.Price);
                        break;
                    case OrderType.Sell:
                        if (Amount == 0)
                        {
                            //throw new Exception("Странное поведение попытка продать что-то, когда еще ничего не купил");
                        }
                        var amount = deal.Amount <= Amount ? deal.Amount : Amount;
                        Amount -= amount;
                        //Amount -= deal.Amount;
                        owner.AddMoney(amount * deal.Price);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                deal.Status = Status.Close;
            }

        }

        public ClassicOrder(OrderType orderType, Account owner, string pair, List<Deal> prices,
                            List<Deal> takes = null, List<Deal> stops = null)
        {
            if (owner.Amount < prices.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("No money");

            if (!IsPositive(prices) || takes != null && !IsPositive(takes)
                                    || stops != null && !IsPositive(stops))
                throw new Exception("Not correct input");

            this.owner = owner;
            Pair = pair;
            Status = Status.Open;

            if (orderType == OrderType.Buy)
                EntryDeals.AddRange(prices.Select(x => new Deal(x.Price, x.Amount, OrderType.Buy)));
            else
                EntryDeals.AddRange(prices.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (takes != null)
                TakeDeals.AddRange(takes.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (stops != null)
                StopDeals.AddRange(stops.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));
        }

    }
}
