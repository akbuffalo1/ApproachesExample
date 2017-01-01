using System;
namespace TigerApp.Shared.Utils.Coupon
{
    public class FibonacciNumberCalculator : INumberCalculator {
        private Func<int, int> _calcFunction;

        public FibonacciNumberCalculator(int offset) 
        {
            _calcFunction = (order) =>
            {   
                if (order == 0)
                    return order;
                if (order == 1)
                    return offset;
                else
                    return _calcFunction(order - 1) + _calcFunction(1) + order;
            };
        }

        public int Calculate(int calcOrder) {
            return _calcFunction(calcOrder);
        }
    }
}

