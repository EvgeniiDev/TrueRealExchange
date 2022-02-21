using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueRealExchange
{
    internal interface IExchange
    {
        public Dictionary<string, decimal> GetPrices();
    }
}
