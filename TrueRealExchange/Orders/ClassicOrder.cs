using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange
{
    public class ClassicOrder : BaseOrder
    {
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
                        owner.RemoveMoney(deal.Amount * deal.Price);
                    }
                    else if (deal.OrderType == OrderType.Sell)
                    {
                        Amount -= deal.Amount < Amount ? deal.Amount : Amount;
                        owner.AddMoney(deal.Amount * deal.Price);
                    }
                }
            }
            lastPrice = price;
        }

        public ClassicOrder(OrderType orderType, Account owner, string pair, List<Deal> prices,
            List<Deal> takes = null, List<Deal> stops = null)
        {
            this.owner = owner;
            Pair = pair;

            foreach (var item in prices)
            {
                if (orderType == OrderType.Buy)
                    Deals.Add(new Deal(item.Amount, item.Price, OrderType.Buy));
                else
                    Deals.Add(new Deal(item.Amount, item.Price, OrderType.Buy));
            }

            if (takes != null)
                foreach (var item in takes)
                {
                    Deals.Add(new Deal(item.Amount, item.Price, OrderType.Sell));
                }

            if (stops != null)
                foreach (var item in stops)
                {
                    Deals.Add(new Deal(item.Amount, item.Price, OrderType.Sell));
                }
        }
    }
}
