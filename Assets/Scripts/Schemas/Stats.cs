using System;
using Models;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

namespace Schemas
{
    [Serializable]
    public struct GameStats
    {
        public string name;
        public int iv;
        
        public int baseStat;
        public int effort;
        public int currentStat;
        
        public static GameStats FromApiModel(Stats apiStats)
        {
            return new GameStats
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