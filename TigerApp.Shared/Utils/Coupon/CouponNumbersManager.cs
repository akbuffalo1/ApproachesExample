using System;
using TigerApp.Shared.Utils.Coupon;

namespace TigerApp.Shared.Utils
{
    public class CouponNumbersManager {
        private INumberCalculator _calculator;
        public int Order { get; private set; }

        public CouponNumbersManager(INumberCalculator calculator, int offset = 0) 
        {
            _calculator = calculator;
            if (offset < 0) throw new ArgumentException("Coupon initial offset state can't be less than '0'");
            Order = offset; 
        }

        public int GetCollectedCouponAmount() {
            return _calculator.Calculate(Order);
        }

        public void IncrementOrder() {
            Order++;
        }

        public int GetAndIncrement() {
            var result = GetCollectedCouponAmount();
            IncrementOrder();
            return result;
        }

        public int Get(int order) { 
            return _calculator.Calculate(Order = order);
        }
    }
}

