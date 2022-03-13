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
        public Status Status;
        public List<Deal> TakeDeals = new();
        public List<Deal> StopDeals = new();
        public List<Deal> EntryDeals = new();
        public decimal lastPrice;
        protected decimal LiquidationPrice;
        public decimal Balance;
        public decimal StartBalance = 0;
        public int Leverage = 1;

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

        public virtual void UpdateAllDeals(decimal price)
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

        public virtual void UpdateStatusOfDeals(List<Deal> deals, decimal price)
        {
            foreach (var deal in deals.Where(x => x.Status == Status.Open)
                                        .Where(x => IsPriceCrossedLevel(x, price)))
            {
                switch (deal.OrderType)
                {
                    case OrderType.Buy:
                        Buy(deal);
                        if (deal.Amount == 0)
                            deal.Status = Status.Close;
                        break;
                    case OrderType.Sell:
                        Sell(deal);
                        if (deal.Amount == 0)
                            deal.Status = Status.Close;
                        break;
                    default:
                        throw new NotImplementedException();
                }
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

        public BaseOrder()
        {
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