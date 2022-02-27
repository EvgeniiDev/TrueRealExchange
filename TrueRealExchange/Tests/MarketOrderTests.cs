using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TrueRealExchange.Tests
{
    [TestFixture]
    public class MarketOrderTests
    {
        private FakePriceGenerator _fakePrice;
        private Exchange _exchange;
        //private Account acc1;

        [SetUp]
        public void SetUp()
        {
            _fakePrice = new FakePriceGenerator();
            _exchange = new Exchange(_fakePrice);
            _fakePrice.prices = new Dictionary<string, decimal>();
        }
        [Test]
        public void JustBuySomeCoins()
        {
            //ToDo починить списывание деняк
            var account = _exchange.CreateAccount("юджин", "шоколадные монетки", 2000);
            var buy = new List<Deal>() { new Deal(10,100) };
            var order = account.CreateOrder(OrderType.Buy, "шоколадные монетки", buy);
            _fakePrice.prices["шоколадные монетки"] = 9m;
            _exchange.UpdateStates();
            _fakePrice.prices["шоколадные монетки"] = 10m;
            _exchange.UpdateStates();
            _fakePrice.prices["шоколадные монетки"] = 11m;
            _exchange.UpdateStates();
            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(true, account.Orders[order].EntryDeals.All(x=> x.Status == Status.Close));
            Assert.AreEqual(100, account.Orders[order].Amount);
            Assert.AreEqual(1000m, account.Amount);
        }

        [Test]
        public void BuySomeCoinsWhenNotEnoughMoneys()
        {
            //ToDo починить списывание деняк
            const decimal startBalance = 200m;
            const string tickerName = "шоколадные монетки";
            var buy = new List<Deal>() { new Deal(10, 100) };


            Assert.Catch(
                delegate {
                    var account = _exchange.CreateAccount("юджин", tickerName, startBalance);
                    var order = account.CreateOrder(OrderType.Buy, tickerName, buy);
                });
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);

        }


        [Test]
        public void BuySomeCoinsWhenPriceHasNotReached()
        {
            //ToDo починить списывание деняк
            const decimal startBalance = 2000m;
            const string tickerName = "шоколадные монетки";
            var account = _exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var order = account.CreateOrder(OrderType.Buy, tickerName, buy);

            var priceGoals = new List<decimal>() { 9m, 9.9999m };
            MovePrice(priceGoals, tickerName);

            Assert.AreEqual(Status.Open, account.Orders[order].Status);
            Assert.AreEqual(true, account.Orders[order].EntryDeals.All(x => x.Status == Status.Open));
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance, account.Amount);
        }
        [Test]
        public void BuySomeCoinsAndSellAllByTakes()
        {
            //ToDo починить списывание деняк
            const decimal startBalance = 2000m;
            const string tickerName = "шоколадные монетки";
            var account = _exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var take = new List<Deal>() { new Deal(12, 25), new Deal(15, 75), };
            var order = account.CreateOrder(OrderType.Buy, tickerName, buy,take);

            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 20 };
            MovePrice(priceGoals, tickerName);

            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0, account.Orders[order].Amount);
            Assert.AreEqual(startBalance-10*100 + 12*25 + 15*75, account.Amount);
        }
        [Test]
        public void BuySomeCoinsAndSellPartByTakes()
        {
            //ToDo починить списывание деняк
            const decimal startBalance = 2000m;
            const string tickerName = "шоколадные монетки";
            var account = _exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var take = new List<Deal>() { new Deal(11, 25) };
            var order = account.CreateOrder(OrderType.Buy, tickerName, buy, take);

            var priceGoals = new List<decimal>() { 9m, 10m, 11m };
            MovePrice(priceGoals, tickerName);
            //TODO по каким критериям определяется выполненный ордер?
            //Assert.AreEqual(Status.Open, account.Orders[order].Status);
            Assert.AreEqual(75m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100 + 11 * 25 , account.Amount);
        }

        [Test]
        public void BuySomeCoinsAndSellPartByStop()
        {
            //ToDo починить списывание деняк
            const decimal startBalance = 2000m;
            const string tickerName = "шоколадные монетки";
            var account = _exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var stop = new List<Deal>() { new Deal(9, 25) };
            var order = account.CreateOrder(OrderType.Buy, tickerName, buy, null, stop);

            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);

            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(75m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100 + 9 * 25, account.Amount);
        }

        [Test]
        public void BuySomeCoinsAndSellAllByStop()
        {
            //ToDo починить списывание деняк
            const decimal startBalance = 2000m;
            const string tickerName = "шоколадные монетки";
            var account = _exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var stop = new List<Deal>() { new Deal(9, 25), new Deal(8.5m, 75) };
            var order = account.CreateOrder(OrderType.Buy, tickerName, buy, null, stop);

            var priceGoals = new List<decimal>() { 9.6m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);

            Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100 + 9 * 25 + 8.5m*75, account.Amount);
        }
        [Test]
        public void BuySomeCoinsAndSellAllByTakeAndStop()
        {
            //ToDo починить списывание деняк
            const decimal startBalance = 2000m;
            const string tickerName = "шоколадные монетки";
            var account = _exchange.CreateAccount("юджин", tickerName, startBalance);
            var buy = new List<Deal>() { new Deal(10, 100) };
            var take = new List<Deal>() { new Deal(15, 20), new Deal(17, 30), new Deal(18, 50) };
            var stop = new List<Deal>() { new Deal(9, 25)};
            var order = account.CreateOrder(OrderType.Buy, tickerName, buy, take, stop);

            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8.6m, 17, 9, 20 };
            MovePrice(priceGoals, tickerName);

            //Assert.AreEqual(Status.Close, account.Orders[order].Status);
            Assert.AreEqual(0m, account.Orders[order].Amount);
            Assert.AreEqual(startBalance - 10 * 100 + 9 * 25 + 15 * 20 + 17*30 + 25 * 18, account.Amount);
        }
        [Test]
        public void BuySomeCoinsWhenHasMoneyOnlyOnPartOfCoins()
        {
            //ToDo починить списывание деняк
            const decimal startBalance = 2000m;
            const string tickerName = "шоколадные монетки";

            var buy = new List<Deal>() { new Deal(10, 100), new Deal(5, 1000) };

            Assert.Catch(
                delegate {
                    var account = _exchange.CreateAccount("юджин", tickerName, startBalance);
                    var order = account.CreateOrder(OrderType.Buy, tickerName, buy);
            });
            var priceGoals = new List<decimal>() { 9m, 10m, 11m, 8m };
            MovePrice(priceGoals, tickerName);
        }

        private void MovePrice(List<decimal> priceGoals, string tickerName)
        {
            foreach (var price in priceGoals)
            {
                _fakePrice.prices[tickerName] = price;
                _exchange.UpdateStates();
            }
        }
    }
}
