using System;

namespace Schemas
{
    [Serializable]
    public struct Stats
    {
        public string name;
        public int baseStat;
        public int currentStat;
        public int effort;
        public int iv;
        
        public Stats(string name, int baseStat, int currentStat, int effort)
        {
            this.name = name;
            this.baseStat = baseStat;
            this.currentStat = currentStat;
            this.effort = effort;
            iv = 0;
        }
        public static int CalculateHp(int baseStat, int iv, int ev, int level)
        {
            var hp = (2 * baseStat + iv + ev / 4) * level / 100 + level + 10;
            return hp;
        }
        public static int CalculateOtherStat(int baseStat, int iv, int ev, int level, double natureMultiplier)
        {
            var stat = (int)(((2 * baseStat + iv + ev / 4) * level / 100 + 5) * natureMultiplier);
            return stat;
        }
    }
}