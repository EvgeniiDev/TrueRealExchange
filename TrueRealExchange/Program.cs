﻿using System;

namespace TrueRealExchange
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var virtualExchange = new Exchange((IExchange) new RealExchange());
            var acc1 = virtualExchange.CreateAccount("Aboba", "USD", 10000);
            var acc2 = virtualExchange.CreateAccount("Aboba", "USD", 10000);
            var acc3 = virtualExchange.CreateAccount("Aboba", "USD", 10000);

            Console.WriteLine("Hello World!");
        }
    }
}
