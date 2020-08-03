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
