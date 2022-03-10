
using System;
using System.Collections.Generic;
using System.Linq;
using TrueRealExchange.Orders;

namespace TrueRealExchange
{
    public class Account
    {
        public string Name { get; private set; }
        readonly string DefaultCurrency;
        public decimal Amount { get; private set; }
        public List<decimal> BalanceHistory { get; private set; } = new List<decimal>();
        public Dictionary<Guid, BaseOrder> Orders { get; private set; } = new Dictionary<Guid, BaseOrder>();

        public Account(string name, string defaultCurrency, decimal startBalance)
        {
            Name = name;
            Amount = startBalance;
            DefaultCurrency = defaultCurrency;
            BalanceHistory.Add(startBalance);
        }

        public Guid PostMarketOrder(OrderType orderType, string pair, List<Deal> prices,
            List<Deal> takes = null, List<Deal> stops = null)
        {
            var order = new MarketOrder(orderType, this, pair, prices, takes, stops);
            var guid = new Guid();
            Orders.Add(guid, order);
            return guid;
        }

        public Guid PostFuturesOrder(OrderType orderType, string pair, int leverage, List<Deal> prices,
                    List<Deal> takes = null, List<Deal> stops = null)
        {
            var order = new BaseOrder();
            if(orderType == OrderType.Long)
                order = new FuturesOrderLong(this, pair, prices, leverage, takes, stops);
            else
                order = new FuturesOrderShort(this, pair, prices, leverage, takes, stops);
            var guid = Guid.NewGuid();
            Orders.Add(guid, order);
            return guid;
        }

        public void RemoveMoney(decimal v)
        {
            Amount -= v;
            BalanceHistory.Add(Amount);
        }

        public void AddMoney(decimal v)
        {
            Amount += v;
            BalanceHistory.Add(Amount);
        }

        public void SellCoins(Guid orderID)
        {
            throw new NotImplementedException();
        }

        public void CancelOrder(Guid orderID)
        {
            var order = Orders[orderID];
            order.Status = Status.Close;
            //order.deals.
            throw new NotImplementedException();
        }

        internal void DataReceiver(Dictionary<string, decimal> prices)
        {
            foreach (var price in prices)
                foreach (var order in Orders.Values.Where(x => x.Status == Status.Open)
                                                    .Where(x => x.Pair == price.Key))
                    order.UpdateStatusOfOrder(price.Value);
        }
    }
}
