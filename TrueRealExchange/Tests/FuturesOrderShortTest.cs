using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TrueRealExchange.Orders;

namespace TrueRealExchange
{
    [TestFixture]
    public class FuturesOrderShortTest
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
        public void JustShortSomeCoinsAndCloseByTake()
        {
            var tickerName = "шоколадные монетки";
            var startBalance = 2000;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var entry = new List<Deal>() { new Deal(10, 100) };
            var take = new List<Deal>() { new Deal(8, 100) };
            var order = account.PostFuturesOrder(OrderType.Short, tickerName, 1, entry, take);
            var priceGoals = new List<decimal>() { 9, 10, 8 };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(true, account.Orders[order].EntryDeals.All(x => x.Status == Status.Close));
            Assert.AreEqual(true, account.Orders[order].TakeDeals.All(x => x.Status == Status.Close));
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance + 2 * 100, account.Amount);
        }

        [Test]
        public void BuySomeCoinsWhenNotEnoughMoneys()
        {
            var startBalance = 20m;
            var tickerName = "шоколадные монетки";
            var buy = new List<Deal>() { new Deal(10, 100) };
            Assert.Catch(
                delegate
                {
                    var account = exchange.CreateAccount("юджин", tickerName, startBalance);
                    var order = account.PostFuturesOrder(OrderType.Short, "шоколадные монетки", 1, buy);
                });
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
        }

        [Test]
        public void ShortSomeCoinsWhenPriceHasNotReached()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var order = account.PostFuturesOrder(OrderType.Short, "шоколадные монетки", 1, buy);
            var priceGoals = new List<decimal>() { 9m, 9.9999m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Open, account.Orders[order].Status);
            Assert.AreEqual(true, account.Orders[order].EntryDeals.All(x => x.Status == Status.Open));
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance-10*100, account.Amount);
            //Assert.AreEqual(10 * 100, account.Orders[order].Balance);
        }

        [Test]
        public void ShortSomeCoinsAndCloseAllByTakes()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var entry = new List<Deal>() { new Deal(15, 100) };
            var take = new List<Deal>() { new Deal(12, 25), new Deal(10, 75), };
            var order = account.PostFuturesOrder(OrderType.Short, tickerName, leverage, entry, take);
            var priceGoals = new List<decimal>() { 13, 15, 11, 9 };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance + 3 * 25 + 5 * 75, account.Amount);
        }

        [Test]
        public void ShortSomeCoinsAndCloseAllByStop()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var stop = new List<Deal>() { new Deal(11, 100) };
            var order = account.PostFuturesOrder(OrderType.Short, tickerName, leverage, buy, null, stop);
            var priceGoals = new List<decimal>() { 9m, 10m, 11m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 100, account.Amount);
        }

        [Test]
        public void ShortSomeCoinsAndClosePartAndPartByTakeByStop()
        {
            var startBalance = 2000m;
            var tickerName = "шоколадные монетки";
            var leverage = 1;
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            var entry = new List<Deal>() { new Deal(10, 100) };
            var stop = new List<Deal>() { new Deal(11, 50) };
            var take = new List<Deal>() { new Deal(7, 50) };
            var order = account.PostFuturesOrder(OrderType.Short, tickerName, leverage, entry, take, stop);
            var priceGoals = new List<decimal>() { 9m, 10.5m, 11m, 7m, 6m, 3m, 1m };
            MovePrice(priceGoals, tickerName);
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0, account.Orders[order].EntryDeals.Select(x => x.Amount).Sum());
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 50 + 3 * 50, account.Amount);
            //Assert.AreEqual(0, account.Orders[order].Balance);
        }

        [Test]
        public void BuySomeCoinsWhenNotEnoughMoneysWithLeverage()
        {
            var startBalance = 20m;
            var tickerName = "шоколадные монетки";
            var buy = new List<Deal>() { new Deal(10, 100) };
            Assert.Catch(
                delegate
                {
                    var account = exchange.CreateAccount("юджин", tickerName, startBalance);
                    var order = account.PostFuturesOrder(OrderType.Short, "шоколадные монетки", 10, buy);
                });
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
        }

        public void BuySomeCoinsWithLeverageLessWhen1()
        {
            var startBalance = 20m;
            var tickerName = "шоколадные монетки";
            var buy = new List<Deal>() { new Deal(10, 100) };
            var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            Assert.Catch(
                delegate
                {
                    var order = account.PostFuturesOrder(OrderType.Short, "шоколадные монетки", 0, buy);
                });
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
        }

        [Test]
        public void ShortSomeCoinsWhenPriceHasNotReachedWithLeverage()
        {
            //for (int i = 1; i < 10; i++)
            //{
            //    var startBalance = 200*(11 - i);
            //    var tickerName = "шоколадные монетки";
            //    var account = exchange.CreateAccount("юджин", tickerName, startBalance);
            //    var buy = new List<Deal>() { new Deal(10, 100) };
            //    var order = account.PostFuturesOrder(OrderType.Short, "шоколадные монетки", i, buy);
            //    Assert.AreEqual(startBalance - (10 * 100)*i, account.Amount);
            //    var priceGoals = new List<decimal>() { 9m, 9.9999m };
            //    MovePrice(priceGoals, tickerName);
            //    Assert.AreEqual(Status.Open, account.Orders[order].Status);
            //    Assert.AreEqual(true, account.Orders[order].EntryDeals.All(x => x.Status == Status.Open));
            //    Assert.AreEqual(0, account.Orders[order].Amount);
            //    Assert.AreEqual(startBalance - 10 * 100, account.Amount);
            //    //Assert.AreEqual(10 * 100, account.Orders[order].Balance);
            //}
        }

        [Test]
        public void ShortSomeCoinsAndCloseAllByTakesWithLeverage()
        {
            for (int i = 1; i < 10; i++)
            {
                var startBalance = 150*(11-i);
                var tickerName = "шоколадные монетки";
                var account = exchange.CreateAccount("юджин", tickerName, startBalance);
                var entry = new List<Deal>() { new Deal(15, 100) };
                var take = new List<Deal>() { new Deal(12, 25), new Deal(10, 75), };
                var order = account.PostFuturesOrder(OrderType.Short, tickerName, i, entry, take);
                Assert.AreEqual((double)startBalance - (15 * 100) / i, (double)account.Amount, (double)1);
                var priceGoals = new List<decimal>() { 13, 15, 11, 9 };
                MovePrice(priceGoals, tickerName);
                Assert.AreEqual(Status.Close, account.Orders[order].Status);
                Assert.AreEqual(0, account.Orders[order].Amount);
                Assert.AreEqual(startBalance + 3 * 25 + 5 * 75, account.Amount);
            }
        }

        [Test]
        public void ShortSomeCoinsAndCloseAllByStopWithLeverage()
        {
            for (int i = 1; i < 10; i++)
            {
                var startBalance = 2000m;
                var tickerName = "шоколадные монетки";
                var account = exchange.CreateAccount("юджин", tickerName, startBalance);
                var buy = new List<Deal>() { new Deal(10, 100) };
                var stop = new List<Deal>() { new Deal(11, 100) };
                var order = account.PostFuturesOrder(OrderType.Short, tickerName, i, buy, null, stop);
                Assert.AreEqual((double)startBalance - (10 * 100) / i, (double)account.Amount,1);
                var priceGoals = new List<decimal>() { 9m, 10m, 11m };
                MovePrice(priceGoals, tickerName);
                Assert.AreEqual(Status.Close, account.Orders[order].Status);
                Assert.AreEqual(0, account.Orders[order].Amount);
                Assert.AreEqual(startBalance - 100, account.Amount);
            }
        }

        [Test]
        public void ShortSomeCoinsAndClosePartAndPartByTakeByStopWithLeverage()
        {
            for (int i = 1; i < 10; i++)
            {
                var startBalance = 2000m;
                var tickerName = "шоколадные монетки";
                var account = exchange.CreateAccount("юджин", tickerName, startBalance);
                var entry = new List<Deal>() { new Deal(10, 100) };
                var stop = new List<Deal>() { new Deal(11, 50) };
                var take = new List<Deal>() { new Deal(7, 50) };
                var order = account.PostFuturesOrder(OrderType.Short, tickerName, i, entry, take, stop);
                Assert.AreEqual((double)startBalance - (10 * 100) / i, (double)account.Amount, 1);
                var priceGoals = new List<decimal>() { 9m, 10.5m, 11m, 7m, 6m, 3m, 1m,1000m };
                MovePrice(priceGoals, tickerName);
                Assert.AreEqual(Status.Close, account.Orders[order].Status);
                Assert.AreEqual(0, account.Orders[order].EntryDeals.Select(x => x.Amount).Sum());
                Assert.AreEqual(0, account.Orders[order].Amount);
                Assert.AreEqual(startBalance - 50 + 3 * 50, account.Amount);
                //Assert.AreEqual(0, account.Orders[order].Balance);
            }
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