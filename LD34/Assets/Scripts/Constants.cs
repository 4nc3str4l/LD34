using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Constants
{
    public static class Layers
    {
        public static readonly int GROUND = 8;
        public static readonly int GROUND_MASK = 1 << GROUND;

        public static readonly int ABILITIES = 9;
        public static readonly int ABILITIES_MASK = 1 << ABILITIES;

        public static readonly int UNIT = 10;
        public static readonly int UNIT_MASK = 1 << UNIT;
    }
}
