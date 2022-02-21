using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueRealExchange
{
    class Account
    {
        public string Name;
        private decimal BTCAmount;
        private decimal usdAmount;
        private List<decimal> balanceHistory = new List<decimal>();
        private List<Order> orders = new List<Order>();

        public Account(string name, decimal startBalance)
        {
            Name = name;
            usdAmount = startBalance;
            balanceHistory.Add(startBalance);
        }

        public void CreateOrder()
        {
            var order = new Order(this, "Buy", 10000, 0.5);

            orders.Add(order);
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
