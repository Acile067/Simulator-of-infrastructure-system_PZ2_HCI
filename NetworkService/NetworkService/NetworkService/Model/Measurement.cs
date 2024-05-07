using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Measurement
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
        public bool IsValid { get; set; }
    }
}
