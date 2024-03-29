using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public class MarketOrder : BaseOrder
    {
        protected override void Buy(Deal deal)
        {
            Amount += deal.Amount;
            owner.RemoveMoney(deal.Amount * deal.Price);
            deal.Amount = 0;
        }

        protected override void Sell(Deal deal)
        {
            var amount = deal.Amount <= Amount ? deal.Amount : Amount;
            Amount -= amount;
            deal.Amount -= amount;

            owner.AddMoney(amount * deal.Price);
        }

        public MarketOrder(OrderType orderType, Account owner, string pair, List<Deal> entry,
                     List<Deal> takes = null, List<Deal> stops = null) : base(owner, pair, entry, takes, stops)
        {
            if (owner.Amount < entry.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("No money");

            if (orderType == OrderType.Buy)
                EntryDeals.AddRange(entry.Select(x => new Deal(x.Price, x.Amount, OrderType.Buy)));
            else
                EntryDeals.AddRange(entry.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (takes != null)
                TakeDeals.AddRange(takes.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (stops != null)
                StopDeals.AddRange(stops.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));
        }
    }
}
