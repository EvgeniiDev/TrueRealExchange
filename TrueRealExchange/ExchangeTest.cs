using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TrueRealExchange
{
    [TestFixture]
    public class ExchangeTest
    {
        private FakePriceGenerator fakePrice;
        private Exchange exchange;
        private Account acc1;

        [SetUp]
        public void SetUp()
        {
            fakePrice = new FakePriceGenerator();
            exchange = new Exchange(fakePrice);
            fakePrice.prices = new Dictionary<string, decimal> { { "BTCUSDT", 10 }, { "ETHUSDT", 10 } };
            acc1 = exchange.CreateAccount("Aboba", "USD", 10000);
        }
        [Test]
        public void BuySomeBTC(decimal startBalance)
        {
            var account = new Account("юджин", "шоколадные монетки", startBalance);
            fakePrice.prices["шоколадные монетки"] = 9m;
            exchange.UpdateStates();
            var buy = new Dictionary<decimal, decimal>() { { 10, 10 } };
            var order = account.CreateOrder("шоколадные монетки", buy);
            fakePrice.prices["шоколадные монетки"] = 10m;
            exchange.UpdateStates();
            fakePrice.prices["шоколадные монетки"] = 11m;
            exchange.UpdateStates();
            Assert.AreEqual(account.Orders[order].Amount, 10);
        }

    }
}
