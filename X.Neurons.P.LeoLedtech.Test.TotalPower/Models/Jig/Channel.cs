using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Neurons.P.LeoLedtech.Test.TotalPower.Models.Jig
{
    public class Channel
    {
        public int ID { get; set; }
        public double Vbus { get; set; }
        public double Vload { get; set; }
        public double I { get; set; }
        public double P { get; set; }
    }
}
