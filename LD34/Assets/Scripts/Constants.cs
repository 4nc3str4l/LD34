﻿using System;
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

        public static readonly int MONSTER = 11;
        public static readonly int MONSTER_MASK = 1 << MONSTER;
    }

    public static readonly int MAP_MIN_X = -55;
    public static readonly int MAP_MAX_X = 65;
}
