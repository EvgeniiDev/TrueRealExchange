
using System;
using System.Collections.Generic;

namespace TrueRealExchange
{
    public class Account
    {
        public string Name { get; private set; }
        readonly string DefaultCurrency;
        public decimal Amount { get; private set; }
        public List<decimal> BalanceHistory { get; private set; } = new List<decimal>();
        public Dictionary<Guid, Order> Orders { get; private set; } = new Dictionary<Guid, Order>();

        public Account(string name, string defaultCurrency, decimal startBalance)
        {
            Name = name;
            Amount = startBalance;
            DefaultCurrency = defaultCurrency;
            BalanceHistory.Add(startBalance);
        }

        public Guid CreateOrder(string pair, Dictionary<decimal, decimal> prices,
            Dictionary<decimal, decimal> takes = null, Dictionary<decimal, decimal> stops = null)
        {
            var order = new ClassicOrder(this, pair, prices, takes, stops);
            var guid = new Guid();
            Orders.Add(guid, order);
            return guid;
        }

        public void SellCoins(Guid orderID)
        {
            throw new NotImplementedException();
        }

        public void CancelOrder(Guid orderID)
        {
            var order = Orders[orderID];
            //order.deals.
            throw new NotImplementedException();
        }

        internal void DataReceiver(Dictionary<string, decimal> prices)
        {
            foreach (var price in prices)
            {
                foreach (var order in Orders.Values)
                {
                    if (order.Pair == price.Key)
                        order.Update(price.Value);
                }
            }
        }
    }
}
