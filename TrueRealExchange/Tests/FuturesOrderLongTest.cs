using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TrueRealExchange.Orders;

namespace TrueRealExchange.Tests
{
    [TestFixture]
    public class FuturesOrderLongTest
    {
        private FakePriceGenerator fakePrice;
        private Exchange exchange;
        //private Account acc1;

        [SetUp]
        public void SetUp()
        {
            fakePrice = new FakePriceGenerator();
            exchange = new Exchange(fakePrice);
            fakePrice.prices = new Dictionary<string, decimal>();
        }

        [Test]
        public void JustBuySomeCoins()
        {
            var account = exchange.CreateAccount("юджин", "шоколадные монетки", 2000);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var order = account.PostFuturesOrder(OrderType.Long, "шоколадные монетки", 1, buy);
            fakePrice.prices["шоколадные монетки"] = 9m;
            exchange.UpdateStates();
            fakePrice.prices["шоколадные монетки"] = 10m;
            exchange.UpdateStates();
            fakePrice.prices["шоколадные монетки"] = 11m;
            exchange.UpdateStates();
            //Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(true, account.Orders[order].EntryDeals.All(x => x.Status == Status.Close));
            Assert.AreEqual(100, account.Orders[order].Amount);
            Assert.AreEqual(1000m, account.Amount);
        }

        [Test]
        public void BuySomeCoinsWhenNotEnoughMoneys()
        {
            var startBalance = 200m;
            var tickerName = "шоколадные монетки";
            var buy = new List<Deal>() { new Deal(10, 100) };
            Assert.Catch(
                delegate
                {
                    var account = exchange.CreateAccount("юджин", tickerName, startBalance);
                    var order = account.PostFuturesOrder(OrderType.Long, "шоколадные монетки", 1, buy);
                });
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
        }

        [Test]
        public void BuySomeCoinsWhenPriceHasNotReached()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var order = account.PostFuturesOrder(OrderType.Long, "шоколадные монетки", 1, buy);
            var priceGoals = new List<decimal>() { 9m, 9.9999m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Open, account.Orders[order].Status);
            Assert.AreEqual(true, account.Orders[order].EntryDeals.All(x => x.Status == Status.Open));
            Assert.AreEqual(0, account.Orders[order].Amount);
            //Assert.AreEqual(startBalance, account.Amount);
        }

        [Test]
        public void BuySomeCoinsAndSellAllByTakes()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var take = new List<Deal>() { new Deal(12, 25), new Deal(15, 75), };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, take);
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 20 };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100 + 12 * 25 + 15 * 75, account.Amount);
        }

        [Test]
        public void BuySomeCoinsAndSellPartByTakes()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var take = new List<Deal>() { new Deal(11, 25) };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, take);
            var priceGoals = new List<decimal>() { 9m, 10m, 11m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Open, account.Orders[order].Status);
            Assert.AreEqual(75m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100, account.Amount);
        }
        [Test]
        public void BuySomeCoinsByFourEntryAndSellByThree()
        {
            var startBalance = 3000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(9, 50), new Deal(10, 25), new Deal(11, 100), new Deal(11.5m, 100) };
            var take = new List<Deal>() { new Deal(13, 125), new Deal(15, 150), };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, take);
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 14m, 19m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 2950 + 3875, account.Amount);
        }
        [Test]
        public void BuySomeCoinsAndSellPartByStop()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var stop = new List<Deal>() { new Deal(9, 25) };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, null, stop);
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Open, account.Orders[order].Status);
            Assert.AreEqual(75m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100, account.Amount);
        }

        [Test]
        public void BuySomeCoinsAndSellAllByStop()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var stop = new List<Deal>() { new Deal(9, 25), new Deal(8.5m, 75) };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, null, stop);
            var priceGoals = new List<decimal>() { 9.6m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100 + 9 * 25 + 8.5m * 75, account.Amount);
        }
        [Test]
        public void BuySomeCoinsAndSellAllByTakeAndStop()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var take = new List<Deal>() { new Deal(15, 20), new Deal(17, 30), new Deal(18, 50) };
            var stop = new List<Deal>() { new Deal(9, 25) };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, take, stop);
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8.6m, 17, 9, 20 };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100 + 9 * 25 + 15 * 20 + 17 * 30 + 25 * 18, account.Amount);
        }
        [Test]
        public void BuySomeCoinsWhenHasMoneyOnlyOnPartOfCoins()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var buy = new List<Deal>() { new Deal(10, 100), new Deal(5, 1000) };
            Assert.Catch(
                delegate
                {
                    var account = exchange.CreateAccount("юджин", tickerName, startBalance);
                    var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy);
                });
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
        }

        [Test]
        public void BuyCoinsAndGetLiquidation()
        {
            var tickerName = "шоколадные монетки";
            var account = exchange.CreateAccount("юджин", tickerName, 2000);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var order = account.PostFuturesOrder(OrderType.Long, "шоколадные монетки", 10, buy, null, null);
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 7m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(true, account.Orders[order].EntryDeals.All(x => x.Status == Status.Close));
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(1900m, account.Amount);
        }

        [Test]
        public void BuySomeCoinsByLeverageAndSellAllByStop()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 10;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var stop = new List<Deal>() { new Deal(9.5m, 100) };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, null, stop);
            var priceGoals = new List<decimal>() { 9.6m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100 + 9.5m * 100, account.Amount);
        }

        [Test]
        public void BuySomeCoinsByLeverageAndSellAllByTakes()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 10;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var take = new List<Deal>() { new Deal(12, 25), new Deal(15, 75), };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, take);
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 20 };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance + 2 * 25 + 5 * 75, account.Amount);
        }

        [Test]
        public void BuySomeCoinsByLeverageAndMultiplyOrderAndSellAllByTakes()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 10;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 50), new Deal(11, 50) };
            var take = new List<Deal>() { new Deal(12m, 25), new Deal(15, 75), };
            var order = account.PostFuturesOrder(OrderType.Long, tickerName, leverage, buy, take);
            var priceGoals = new List<decimal>() { 9, 10, 11, 20 };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance + 1.5m * 25 + 4.5m * 75, account.Amount);
        }

        private void MovePrice(List<decimal> priceGoals, string tickerName)
        {
            foreach (var price in priceGoals)
            {
                fakePrice.prices[tickerName] = price;
                exchange.UpdateStates();
            }
        }
    }
}