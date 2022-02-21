using System;
using System.Collections.Generic;

namespace TrueRealExchange
{
    public class Account
    {
        public string Name;
        private decimal BTCAmount;
        private string defaultCurrency;
        private decimal usdAmount;
        private List<decimal> balanceHistory = new List<decimal>();
        private Dictionary<Guid, Order> orders = new Dictionary<Guid, Order>();

        public Account(string name, string defaultCurrency, decimal startBalance)
        {
            Name = name;
            usdAmount = startBalance;
            balanceHistory.Add(startBalance);
        }

        public void CreateOrder(string pair, Dictionary<decimal, decimal> prices,
            Dictionary<decimal, decimal> takes = null, Dictionary<decimal, decimal> stops = null)
        {
            var order = new ClassicOrder(this, pair, prices, takes, stops);
            var guid = new Guid();
            orders.Add(guid, order);
            throw new NotImplementedException();
        }

        public void SellCoins(Guid orderID)
        {
            throw new NotImplementedException();
        }

        public void CancelOrder()
        {
            throw new NotImplementedException();
        }

        internal void DataReceiver(Dictionary<string, decimal> prices)
        {
            foreach (var price in prices)
            {
                foreach (var order in orders.Values)
                {
                    if(order.Pair == price.Key)
                        order.Update(price.Value);
                }
            }
            throw new NotImplementedException();
        }
    }
}
