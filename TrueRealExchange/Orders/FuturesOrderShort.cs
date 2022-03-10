using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueRealExchange.Orders
{
    public class FuturesOrderShort : BaseOrder
    {
        private const decimal feeFactor = 1.002m;
        private decimal AveragePriceBuy = 0;
        private decimal AveragePriceSell = 0;

        public override void UpdateStatusOfOrder(decimal price)
        {
            if (lastPrice == 0)
            {
                UpdateLastPrice(price);
                return;
            }
            UpdateAllDeals(price);

            if (Math.Abs(Amount)<10e-3m && EntryDeals.All(x => x.Status == Status.Close))
                CloseOrder();
            //if(EntryDeals.All(x => x.Status == Status.Close)
            //    && TakeDeals.All(x => x.Status == Status.Close)
            //    && StopDeals.All(x => x.Status == Status.Close))
            //    CloseOrder();
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
            UpdateStatusOfDeals(EntryDeals, price);
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
                            if(Amount<0)
                                Buy(deal);
                            if (deal.Amount == 0)
                                deal.Status = Status.Close;
                            break;
                        }
                    case OrderType.Sell:
                        {
                            Sell(deal);
                            if (deal.Amount == 0)
                                deal.Status = Status.Close;
                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public override void Buy(Deal deal)
        {
            //AveragePriceBuy = (AveragePrice * Amount + deal.Amount * deal.Price) / (Amount + deal.Amount);
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
            //LiquidationPrice = (totalSpend - totalSpend / Leverage) / Amount * feeFactor;
            Balance += priceOfSell;
        }

        public FuturesOrderShort(Account owner, string pair, List<Deal> entry, int leverage,
            List<Deal> takes = null, List<Deal> stops = null)
        {
            if (leverage < 1)
                throw new Exception("Leverage should be more when 1");
            if (owner.Amount * leverage < entry.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("No money");

            if (!IsPositive(entry) || takes != null && !IsPositive(takes)
                                    || stops != null && !IsPositive(stops))
                throw new Exception("Not correct input");
            this.owner = owner;
            Pair = pair;
            Leverage = leverage;
            Status = Status.Open;
            LiquidationPrice = decimal.MaxValue;

            var totalSum = entry.Sum(x => x.Price * x.Amount);
            //Amount -= prices.Sum(x => x.Amount);
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