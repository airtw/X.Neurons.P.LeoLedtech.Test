using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.Models.Jig
{
    public class Message
    {
        public string Version { get; set; }
        public List<Channel> Channels { get; set; }
        public bool Button { get; set; }
    }
}
