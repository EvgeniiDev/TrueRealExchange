using System;
using System.Collections.Generic;

namespace TrueRealExchange
{
    class Account
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

        public void CreateOrder()
        {
            var order = new Order(this, "Buy", 10000, 0.5);
            orders.Add(order);
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
                foreach (var order in orders)
                {
                    order.Update(prices);
                }
            }
            throw new NotImplementedException();
        }
    }
}
