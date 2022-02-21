namespace TrueRealExchange
{
    public abstract class Order
    {
        private Account Owner;
        public decimal Amount;
        public string Pair;

        public virtual void Update(decimal price)
        {
            
        }
    }
}