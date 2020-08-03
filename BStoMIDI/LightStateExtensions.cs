using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightOut
{
    public static class LightStateExtensions
    {
        public static string AsNumberString(this LightState state)
        {
            return ((int)state).ToString();
        }
    }
}
