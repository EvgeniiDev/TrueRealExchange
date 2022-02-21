using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueRealExchange
{
    internal class Deal
    {
        public decimal Price;
        public decimal Amount;
        public OrderType OrderType;
        public Status Status;

        public Deal(decimal price, decimal amount, OrderType orderType)
        {
            Price = price;
            Amount = amount;
            OrderType = orderType;
        }
    }

    public enum OrderType
    {
        Buy,
        Sell,
    }

    public enum Status
    {
        Open,
        Close,
    }

}
