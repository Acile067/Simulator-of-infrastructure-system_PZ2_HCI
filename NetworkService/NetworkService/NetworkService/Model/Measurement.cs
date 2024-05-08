using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class Measurement
    {
        public string Time { get; set; }
        public int Value { get; set; }
        public bool IsValid { get; set; }
        public Brush Color { get; set; }

        public Measurement(string time, int value, bool isValid)
        {
            Time = time;
            Value = value;
            IsValid = isValid;
            Color = isValid ? Brushes.Green : Brushes.Red;
        }
    }
}
