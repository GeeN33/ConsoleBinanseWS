using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBinanseWS.lib
{
    public class BarCoin
    {
        public DateTime Datetime { get; set; }= DateTime.Now;
        public double Open { get; set; } = 0;
        public double High { get; set; }= 0;
        public double Low { get; set; } = 0;
        public double Close { get; set; } = 0;
        public double Delta_buy { get; set; } = 0;
        public double Delta_sell { get; set; } = 0;

        public void SortedBarCoin(double last, double buy, double sell)
        {
            Close = last;

            if (last > High)
            {
                High= last;
            }

            if (last < Low)
            {
                Low = last;
            }
            Delta_buy = buy;
            Delta_sell = sell;
        }


        public void ZeroBarCoin(double last)
        {
            Open = last;

            Close = last;

            High = last;

            Low = last;
            
            Delta_buy = 0;

            Delta_sell = 0;
        }
    }

    public class BarCoinLopped
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public bool IsSpot { get; set; }
        public int Per { get; set; }
        public DateTime Datetime { get; set; }
        public int Index { get; set; }
    }
}
