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
            var account = new Account("�����", "���������� �������", startBalance);
            fakePrice.prices["���������� �������"] = 9m;
            exchange.UpdateStates();
            var buy = new Dictionary<decimal, decimal>() { { 10, 10 } };
            var order = account.CreateOrder("���������� �������", buy);
            fakePrice.prices["���������� �������"] = 10m;
            exchange.UpdateStates();
            fakePrice.prices["���������� �������"] = 11m;
            exchange.UpdateStates();
            Assert.AreEqual(account.Orders[order].Amount, 10);
        }

    }
}
