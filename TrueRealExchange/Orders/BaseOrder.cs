using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public class BaseOrder
    {
        protected Account owner;
        public decimal Amount;
        protected decimal AveragePrice;
        public string Pair;
        public List<Deal> TakeDeals = new();
        public List<Deal> StopDeals = new();
        public List<Deal> EntryDeals = new();
        public decimal lastPrice;
        public Status Status;
        protected decimal LiquidationPrice;
        public decimal Balance;

        public virtual void UpdateStatusOfOrder(decimal price)
        {
            if (lastPrice == 0)
            {
                UpdateLastPrice(price);
                return;
            }
            UpdateAllDeals(price);

            if (Amount == 0 && EntryDeals.All(x => x.Status == Status.Close))
                Status = Status.Close;

            if (LiquidationPrice >= price)
            {
                //Liquidation
                Status = Status.Close;
                Amount = 0;
            }
            UpdateLastPrice(price);
        }

        public virtual void UpdateAllDeals(decimal price)
        {
            UpdateStatusOfDeals(EntryDeals, price);
            UpdateStatusOfDeals(TakeDeals, price);
            UpdateStatusOfDeals(StopDeals, price);
        }
        public void CloseOrder()
        {
            if(Balance>=0)
                owner.AddMoney(Balance);
            else
                owner.RemoveMoney(Balance);
            Status = Status.Close;
        }
        protected virtual void UpdateStatusOfDeals(List<Deal> deals, decimal price)
        {
            throw new NotImplementedException();
        }

        protected bool IsPriceCrossedLevel(Deal deal, decimal price)
        {
            return lastPrice >= price && deal.Price <= lastPrice && deal.Price >= price
                   || lastPrice <= price && deal.Price >= lastPrice && deal.Price <= price;
        }

        protected static bool IsPositive(List<Deal> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            return dictionary.All(x => x.Amount >= 0)
                   && dictionary.All(x => x.Price >= 0);
        }

        public void UpdateLastPrice(decimal price)
        {
            lastPrice = price;
        }

        public virtual void Buy(Deal deal)
        {
            throw new NotImplementedException();
        }

        public virtual void Sell(Deal deal)
        {
            throw new NotImplementedException();
        }
    }
}