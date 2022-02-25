using System;
using System.Collections.Generic;

namespace TrueRealExchange
{
    public class BaseOrder
    {
        public Account owner;
        public decimal Amount;
        public decimal TotalSpend;
        public string Pair;
        public List<Deal> Deals = new();
        public decimal lastPrice = 0;
        public Status Status;
        //TODO � ��� ��� ������ �������� � ����� �����, � �������� � ��� ��������... � ����� �������� ����� ���� �� ���� ����� ������ ��� ���� ����������...
        public virtual void Update(decimal price)
        {
            throw new NotImplementedException();
        }
    }
}