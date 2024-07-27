using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBinanseWS.lib
{
    public class CommandFromClient
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public Log Logg { get; set; }
        public BarCoin Barcoin { get; set; }
    }
}
