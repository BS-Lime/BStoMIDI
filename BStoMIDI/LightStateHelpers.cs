using BStoMIDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BStoMidi
{
    public static class LightStateHelpers
    {
        public static string AsNumberString(this LightState state)
        {
            return ((int)state).ToString();
        }
        public static int AsInt(this LightElement state)
        {
            return ((int)state);
        }
    }
}
