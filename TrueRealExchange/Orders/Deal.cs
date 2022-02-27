
namespace TrueRealExchange
{
    public class Deal
    {
        public decimal Price;
        public decimal Amount;
        public OrderType OrderType;
        public Status Status;


        public Deal(decimal price, decimal amount)
        {
            Price = price;
            Amount = amount;
        }

        public Deal(decimal price, decimal amount, OrderType orderType)
        {
            Price = price;
            Amount = amount;
            OrderType = orderType;
        }

        public override string ToString()
        {
            return $"{OrderType} {Amount} coins for {Price} every";
        }
    }

    public enum OrderType
    {
        Buy,
        Sell,
        LiqLong,
        LiqShort,
    }

    public enum Status
    {
        Open,
        Close,
    }

}
