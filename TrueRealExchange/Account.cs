
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
            BaseOrder order = orderType == OrderType.Long ?
                                new FuturesOrderLong(this, pair, prices, leverage, takes, stops)
                                : new FuturesOrderShort(this, pair, prices, leverage, takes, stops);
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

        public void SellCoins(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public void ChangeStops(Guid orderId, List<Deal> stops)
        {
            if (!Orders.ContainsKey(orderId))
                throw new Exception("Unknown orderId");

            Orders[orderId].StopDeals = stops;
        }

        public void ChangeEntryes(Guid orderId, List<Deal> entryes)
        {
            if (!Orders.ContainsKey(orderId))
                throw new Exception("Unknown orderId");

            Orders[orderId].EntryDeals = entryes;
        }

        public void ChangeTakes(Guid orderId, List<Deal> takes)
        {
            if (!Orders.ContainsKey(orderId))
                throw new Exception("Unknown orderId");

            Orders[orderId].TakeDeals = takes;
        }

        public void CancelOrder(Guid OrderId)
        {
            var order = Orders[OrderId];
            order.Status = Status.Close;
            order.CloseOrder();
            //order.
            throw new NotImplementedException();
        }

        internal void DataReceiver(Dictionary<string, decimal> prices)
        {
            var openedOrders = Orders.Values.Where(x => x.Status == Status.Open);

            foreach (var price in prices)
                foreach (var order in openedOrders.Where(x => x.Pair == price.Key))
                    order.UpdateStatusOfOrder(price.Value);
        }
    }
}
