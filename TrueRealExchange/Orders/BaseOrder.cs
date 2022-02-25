using System;
using System.Collections.Generic;

namespace TrueRealExchange
{
    public class BaseOrder
    {
        public Account owner;
        public decimal Amount;
        public decimal TotalSpend;
        public string Pair;
        public List<Deal> Deals = new();
        public decimal lastPrice = 0;
        public Status Status;
        //TODO у нас все ордера хранятся в одном листе, и открытые и уже закрытые... и когда приходит новая инфа по цене метод апдейт для всех вызывается...
        public virtual void Update(decimal price)
        {
            throw new NotImplementedException();
        }
    }
}