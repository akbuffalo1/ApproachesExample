using System;
using System.Reactive.Subjects;

namespace TigerApp.Droid.UI.Coupons
{
    public class CouponNumberModel
    {
        public ISubject<State> StateChange { get; private set; }

        private Func<int, int> _evaluator;
        private int _value;
        public int[] StateResources { get; set; }

        public CouponNumberModel(int[] stateResources, int value)
        {
            if (stateResources.Length < 3)
                throw new ArgumentException("Coupon number state recources count can't be less than 3");      
            StateResources = stateResources;
            _evaluator = amount => {
                if (amount == _value)
                    return 1;
                if (amount > value)
                    return 2;
                return 0;
            };
            _value = value;
            StateChange = new Subject<State>();
        }

        public void EvaluateSource(int amount) {
            StateChange.OnNext(new State() { IconSrc = StateResources[_evaluator(amount)] });
        }

        public class State { 
            public int IconSrc { get; set; }
        }
    }
}

