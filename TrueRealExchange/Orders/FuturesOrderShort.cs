using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public class FuturesOrderShort : BaseOrder
    {
        private const decimal feeFactor = 1.002m;

        public override void UpdateStatusOfOrder(decimal price)
        {
            if (lastPrice == 0)
            {
                UpdateLastPrice(price);
                return;
            }
            UpdateAllDeals(price);

            if (Math.Abs(Amount) < 10e-3m && EntryDeals.All(x => x.Status == Status.Close))
            {
                CloseOrder();
                return;
            }
            //if(EntryDeals.All(x => x.Status == Status.Close)
            //    && TakeDeals.All(x => x.Status == Status.Close)
            //    && StopDeals.All(x => x.Status == Status.Close))
            //    CloseOrder();
            if (LiquidationPrice <= price)
            {
                //Liquidation
                //CloseOrder();
                Status = Status.Close;
                return;
               // Amount = 0;
            }
            UpdateLastPrice(price);
        }

        public override void UpdateAllDeals(decimal price)
        {
            UpdateStatusOfDeals(EntryDeals, price);
            UpdateStatusOfDeals(TakeDeals, price);
            UpdateStatusOfDeals(StopDeals, price);
        }

       
        public override void Buy(Deal deal)
        {
            var amount = deal.Amount;
            deal.Amount -= amount;
            Amount += amount;
            var basePrice = AveragePrice * amount;
            Balance += basePrice - amount * deal.Price;
        }

        public override void Sell(Deal deal)
        {
            AveragePrice = (AveragePrice * Amount + deal.Amount * deal.Price) /
                                                        (Amount + deal.Amount);
            var amount = deal.Amount;
            deal.Amount -= amount;
            Amount -= amount;
            var priceOfSell = amount * deal.Price;

            var totalSpend = Math.Abs(AveragePrice * Amount);
            if (Math.Abs(Amount) < 0.00001m)
                LiquidationPrice = decimal.MaxValue;
            else
                LiquidationPrice = (totalSpend + totalSpend / Leverage) / Math.Abs(Amount) * feeFactor;
            Balance += priceOfSell;
        }

        public FuturesOrderShort(Account owner, string pair, List<Deal> entry, int leverage,
            List<Deal> takes = null, List<Deal> stops = null) 
            : base(owner, pair, entry, takes, stops)
        {
            if (leverage < 1)
                throw new Exception("Leverage should be more when 1");
            if (owner.Amount * leverage < entry.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("No money");

            Leverage = leverage;
            LiquidationPrice = decimal.MaxValue;

            var totalSum = entry.Sum(x => x.Price * x.Amount);
            StartBalance = totalSum;
            owner.RemoveMoney(totalSum/leverage);

            EntryDeals.AddRange(entry.Select(x => 
                        new Deal(x.Price, x.Amount, OrderType.Sell)));
            if (takes != null)
                TakeDeals.AddRange(takes.Select(x => 
                            new Deal(x.Price, x.Amount, OrderType.Buy)));
            if (stops != null)
                StopDeals.AddRange(stops.Select(x => 
                            new Deal(x.Price, x.Amount, OrderType.Buy)));
        }
    }
}