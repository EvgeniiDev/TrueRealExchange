using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueRealExchange
{
    class Order
    {
        public decimal Amount;
        public decimal Price;
        public Account Owner;
        private List<Deal> deals = new List<Deal>();
        public void Update(decimal price)
        {
            throw new NotImplementedException();
        }


        Order(Account owner, decimal amount, decimal price )
        {
            Amount = amount;
            Price = price;
            Owner = owner;
        }
    }
}
