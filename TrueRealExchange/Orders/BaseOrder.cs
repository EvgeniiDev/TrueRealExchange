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
        protected List<Deal> TakeDeals = new();
        protected List<Deal> StopDeals = new();
        public List<Deal> EntryDeals = new();
        private decimal lastPrice;
        public Status Status;

        protected decimal LiquidationPrice;

        public void UpdateStatusOfOrder(decimal price)
        {
            if (lastPrice == 0)
            {
                UpdateLastPrice(price);
                return;
            }

            if (Status == Status.Close)
                return;
            
            if (LiquidationPrice != 0 && LiquidationPrice <= price)
                CloseOrder();

            UpdateAllDeals(price);

            if (Amount == 0 && EntryDeals.All(x => x.Status == Status.Close))
                Status = Status.Close;

            UpdateLastPrice(price);
        }

        private void UpdateAllDeals(decimal price)
        {
            UpdateStatusOfDeals(EntryDeals, price);
            UpdateStatusOfDeals(TakeDeals, price);
            UpdateStatusOfDeals(StopDeals, price);
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

        private void UpdateLastPrice(decimal price)
        {
            lastPrice = price;
        }

        private void CloseOrder()
        {
            Status = Status.Close;
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