using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange
{
    public class BaseOrder
    {
        public Account owner;
        public decimal Amount;
        public decimal AveragePrice;
        public string Pair;
        public List<Deal> TakeDeals = new();
        public List<Deal> StopDeals = new();
        public List<Deal> EntryDeals = new();
        public decimal lastPrice;
        public Status Status;
        public decimal liquidationPrice;
        //TODO у нас все ордера хранятся в одном листе, и открытые и уже закрытые... и когда приходит новая инфа по цене метод апдейт для всех вызывается...

        public void UpdateStatusOfOrder(decimal price)
        {
            if (Status == Status.Close)
                return;
            if (liquidationPrice != 0 && liquidationPrice <= price)
            {
                //Ликвидация позиции
                Status = Status.Close;
                //TODO т.к позиция полностью ликвидирована её бы перенести куда-то в другое место мб...
            }
            if (lastPrice == 0)
            {
                lastPrice = price;
                return;
            }

            UpdateStatusOfDeals(EntryDeals, price);
            if (TakeDeals.Count == 0 && StopDeals.Count == 0 && EntryDeals.All(x => x.Status == Status.Close))
                Status = Status.Close;

            UpdateStatusOfDeals(TakeDeals, price);
            if (Amount == 0 && TakeDeals.Count > 0 && TakeDeals.All(x => x.Status == Status.Close))
                Status = Status.Close;

            UpdateStatusOfDeals(StopDeals, price);
            if (Amount == 0 && StopDeals.Count > 0 &&
                (StopDeals.All(x => x.Status == Status.Close)
                || price <= StopDeals.Select(x => x.Price).Min()))
                Status = Status.Close;

            lastPrice = price;
        }
        public virtual void UpdateStatusOfDeals(List<Deal> deals, decimal price)
        {
            throw new NotImplementedException();
        }
        public bool IsPriceCrossedLevel(Deal deal, decimal price)
        {
            return lastPrice >= price && deal.Price <= lastPrice && deal.Price >= price
                    || lastPrice <= price && deal.Price >= lastPrice && deal.Price <= price;
        }

        public static bool IsPositive(List<Deal> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            return dictionary.All(x => x.Amount >= 0)
                   && dictionary.All(x => x.Price >= 0);
        }
    }
}