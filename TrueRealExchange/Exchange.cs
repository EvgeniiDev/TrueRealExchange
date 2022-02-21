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

        public Exchange(IExchange exchange)
        {
            this.exchange = exchange;
        }

        private void UpdateStates()
        {
            var prices = exchange.GetPrices();
            foreach (var acc in accounts)
                acc.DataReceiver(prices);
        }

        public Account CreateAccount(string name,string defaultCurrency, decimal startBalance)
        {
            var account = new Account(name, defaultCurrency, startBalance);
            accounts.Add(account);
            return account;
        }
    }
}
