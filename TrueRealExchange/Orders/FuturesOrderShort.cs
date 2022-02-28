using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public class FuturesOrderShort : BaseOrder
    {
        private decimal Leverage { get; set; }
        private const decimal feeFactor = 1.002m;

        public override void UpdateStatusOfOrder(decimal price)
        {
            if (lastPrice == 0)
            {
                UpdateLastPrice(price);
                return;
            }
            UpdateAllDeals(price);

            if (Amount == 0 && EntryDeals.All(x => x.Status == Status.Close))
                CloseOrder();

            if (LiquidationPrice <= price)
            {
                //Liquidation
                CloseOrder();
                Amount = 0;
            }
            UpdateLastPrice(price);
        }

        public override void UpdateAllDeals(decimal price)
        {

            UpdateStatusOfDeals(TakeDeals, price);
            UpdateStatusOfDeals(StopDeals, price);
        }

        protected override void UpdateStatusOfDeals(List<Deal> deals, decimal price)
        {
            foreach (var deal in deals.Where(x => x.Status == Status.Open)
                         .Where(x => IsPriceCrossedLevel(x, price)))
            {
                switch (deal.OrderType)
                {
                    case OrderType.Buy:
                        {
                            Buy(deal);
                            foreach (var _deal in EntryDeals.Where(x => x.Status == Status.Open))
                            {
                                Sell(_deal);
                                if(_deal.Amount==0)
                                    _deal.Status = Status.Close;
                            }
                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }
                deal.Status = Status.Close;
            }
        }

        public override void Buy(Deal deal)
        {
                AveragePrice = (AveragePrice * Amount + deal.Amount * deal.Price) / (Amount + deal.Amount);
                Amount += deal.Amount;
                Balance -= deal.Amount * deal.Price / Leverage;
        }

        public override void Sell(Deal deal)
        {
            var totalSpend = AveragePrice * Amount;
            var amount = Math.Min(deal.Amount, Amount);
            deal.Amount -= amount;
            Amount -= amount;
            var priceOfSell = amount * deal.Price;
            var priceOfBuy = amount * AveragePrice;
            var delta = priceOfSell - priceOfBuy;
            //LiquidationPrice = (totalSpend - totalSpend / Leverage) / Amount * feeFactor;
            Balance += AveragePrice * amount / Leverage + delta;
        }

        public FuturesOrderShort(Account owner, string pair, List<Deal> prices, decimal leverage,
            List<Deal> takes = null, List<Deal> stops = null)
        {
            if (owner.Amount * leverage < prices.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("No money");

            if (!IsPositive(prices) || takes != null && !IsPositive(takes)
                                    || stops != null && !IsPositive(stops))
                throw new Exception("Not correct input");

            this.owner = owner;
            Pair = pair;
            Leverage = leverage;
            Status = Status.Open;
            LiquidationPrice = decimal.MaxValue;
            var balance = prices.Select(x => x.Price * x.Amount).Sum();
            Balance = balance;
            owner.RemoveMoney(balance);
            EntryDeals.AddRange(prices.Select(x => new Deal(x.Price, x.Amount, OrderType.Sell)));

            if (takes != null)
                TakeDeals.AddRange(takes.Select(x => new Deal(x.Price, x.Amount, OrderType.Buy)));

            if (stops != null)
                StopDeals.AddRange(stops.Select(x => new Deal(x.Price, x.Amount, OrderType.Buy)));
        }
    }
}