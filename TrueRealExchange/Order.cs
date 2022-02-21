using System;

namespace TrueRealExchange
{
    public abstract class Order
    {
        public Account owner;
        public decimal Amount;
        public string Pair;

        public  virtual void Update(decimal price)
        {
            throw new NotImplementedException();
        }
    }
}