using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange
{
    public class ClassicOrder : BaseOrder
    {
        public override void Update(decimal price)
        {
            if (Status == Status.Close)
                return;
            foreach (var deal in Deals)
            {
                if (deal.Status == Status.Close)
                    continue;
                if (lastPrice >= price && deal.Price <= lastPrice && deal.Price >= price
                    || lastPrice <= price && deal.Price >= lastPrice && deal.Price <= price)
                {
                    switch (deal.OrderType)
                    {
                        case OrderType.Buy:
                            Amount += deal.Amount;
                            owner.RemoveMoney(deal.Amount * deal.Price);
                            break;
                        case OrderType.Sell:
                            Amount -= deal.Amount < Amount ? deal.Amount : Amount;
                            owner.AddMoney(deal.Amount * deal.Price);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    deal.Status = Status.Close;
                    //if(Deals.All())
                }
            }
            lastPrice = price;
        }

        public ClassicOrder(OrderType orderType, Account owner, string pair, List<Deal> prices,
            List<Deal> takes = null, List<Deal> stops = null)
        {
            this.owner = owner;
            Pair = pair;
            Status = Status.Open;

            if (orderType == OrderType.Buy)
                Deals.AddRange(prices.Select(x => new Deal(x.Price, x.Amount, OrderType.Buy)));
            else
                Deals.AddRange(prices.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (takes != null)
                Deals.AddRange(takes.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (stops != null)
                Deals.AddRange(stops.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));
        }
    }
}
