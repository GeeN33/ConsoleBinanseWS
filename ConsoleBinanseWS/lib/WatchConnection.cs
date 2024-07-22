using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBinanseWS.lib
{
    public class WatchConnection
    {
        public bool CurrentUpTik { get; set; } = false;
       
        public int UpTikNot { get; set; } = 0;

        public bool Go { get; set; } = true;

        public List<int> TargetHour = new List<int>() {1, 12};
}
}
