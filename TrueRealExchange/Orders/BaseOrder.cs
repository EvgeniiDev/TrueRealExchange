using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public abstract class BaseOrder
    {
        public decimal Amount;
        protected decimal AveragePrice;
        public Status Status;
        protected decimal lastPrice;
        protected decimal Balance;
        protected decimal StartBalance = 0;

        public List<Deal> TakeDeals = new();
        public List<Deal> StopDeals = new();
        public List<Deal> EntryDeals = new();

        public int Leverage { get; set; } = 1;
        protected decimal LiquidationPrice;
        protected Account owner { get; }
        public string Pair { get; }

        public virtual void UpdateStatusOfOrder(decimal price)
        {
            if (lastPrice == 0)
            {
                UpdateLastPrice(price);
                return;
            }

            UpdateAllDeals(price);

            if (Math.Abs(Amount) < 10e-4m && EntryDeals.All(x => x.Status == Status.Close))
            {
                CloseOrder();
                return;
            }

            if (LiquidationPrice >= price)
            {
                //Liquidation
                Status = Status.Close;
                Amount = 0;
                return;
            }
            UpdateLastPrice(price);
        }

        protected virtual void UpdateAllDeals(decimal price)
        {
            UpdateStatusOfDeals(EntryDeals, price);
            UpdateStatusOfDeals(TakeDeals, price);
            UpdateStatusOfDeals(StopDeals, price);
        }
        public void CloseOrder()
        {
            var balance = Balance - StartBalance + StartBalance / Leverage;
            if (balance >= 0)
                owner.AddMoney(balance);
            else
                owner.RemoveMoney(balance);
            Status = Status.Close;
        }

        protected virtual void UpdateStatusOfDeals(List<Deal> deals, decimal price)
        {
            foreach (var deal in deals.Where(x => x.Status == Status.Open
                                                    && IsPriceCrossedLevel(x.Price, price)))
            {
                switch (deal.OrderType)
                {
                    case OrderType.Buy:
                        Buy(deal);
                        break;

                    case OrderType.Sell:
                        Sell(deal);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                if (deal.Amount == 0)
                    deal.Status = Status.Close;
            }
        }
        public BaseOrder(Account owner, string pair, List<Deal> entry,
                                List<Deal> takes = null, List<Deal> stops = null)
        {
            if (!IsPositive(entry) || takes != null && !IsPositive(takes)
                                   || stops != null && !IsPositive(stops))
                throw new Exception("Incorrect input");

            this.owner = owner;
            Pair = pair;
            Status = Status.Open;
        }

        protected bool IsPriceCrossedLevel(decimal level, decimal price)
        {
            return lastPrice >= price && level <= lastPrice && level >= price
                   || lastPrice <= price && level >= lastPrice && level <= price;
        }

        protected static bool IsPositive(List<Deal> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return dictionary.All(x => x.Amount >= 0)
                   && dictionary.All(x => x.Price >= 0);
        }

        protected void UpdateLastPrice(decimal price)
        {
            lastPrice = price;
        }

        protected abstract void Buy(Deal deal);

        protected abstract void Sell(Deal deal);
    }
}