using System;

namespace Schemas
{
    [Serializable]
    public struct Stats
    {
        public string name;
        public int iv;
        
        public int baseStat;
        public int effort;
        public int currentStat;
        
        public Stats(string name, int iv, int baseStat, int effort)
        {
            this.name = name;
            this.iv = iv;
            this.baseStat = baseStat;
            this.effort = effort;
            currentStat = baseStat;
        }
        
        public static Stats FromApiModel(Models.Stats apiStats)
        {
            return new Stats
            {
                name = apiStats.status.nameStat,
                iv = apiStats.iv,
                effort = apiStats.effort,
                baseStat = apiStats.baseStatus,
                currentStat = apiStats.baseStatus
            };
        }
    }
}