using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange
{
    public class BaseOrder
    {
        public Account owner;
        public decimal Amount;
        public decimal TotalSpend;
        public string Pair;
        public List<Deal> Deals = new();
        public decimal lastPrice;
        public Status Status;
        //TODO у нас все ордера хранятся в одном листе, и открытые и уже закрытые... и когда приходит новая инфа по цене метод апдейт для всех вызывается...
        public virtual void Update(decimal price)
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