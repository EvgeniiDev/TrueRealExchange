using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueRealExchange
{
    internal class Exchange
    {
        private List<Account> accounts = new List<Account>();
        readonly IExchange exchange;

        Exchange(IExchange exchange)
        {
            this.exchange = exchange;
        }

        private void UpdateStates()
        {
            var prices = exchange.GetPrices();
            foreach (var acc in accounts)
                acc.DataReceiver(prices);
        }

        public Account CreateAccount(string name, decimal startBalance)
        {
            var account = new Account(name, startBalance);
            accounts.Add(account);
            return account;
        }
    }
}
