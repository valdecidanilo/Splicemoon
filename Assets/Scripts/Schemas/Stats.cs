using System;
using Models;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

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
        public static int CalculateDamage(int pp, int level, int attackstats, int defensestats)
        {
            var random = UnityEngine.Random.Range(0.85f, 1.0f);

            var baseDamage = ((2f * level / 5f + 2f) * pp * attackstats / defensestats / 50f + 2f) * random;

            //var modifiers = burn * screen * targets * weather * ff * stockpile * critical * doubleDmg * charge * hh * stab * type1 * type2 * random;

            // Dano final
            //var finalDamage = baseDamage * modifiers;
            return Mathf.FloorToInt(baseDamage);
        }
        public static int CalculateErraticExp(int level)
        {
            if (level <= 50) return Mathf.FloorToInt(Mathf.Pow(level, 3) * (100 - level) / 50f);
            else if (level <= 68) return Mathf.FloorToInt(Mathf.Pow(level, 3) * (150 - level) / 100f);
            else if (level <= 98) return Mathf.FloorToInt(Mathf.Pow(level, 3) * (1911 - 10 * level) / 500f);
            else return Mathf.FloorToInt(Mathf.Pow(level, 3) * (160 - level) / 100f);
        }

        public static int CalculateFluctuatingExp(int level)
        {
            if (level <= 15) return Mathf.FloorToInt(Mathf.Pow(level, 3) * (24 + ((level + 1) / 3)) / 50f);
            else if (level <= 36) return Mathf.FloorToInt(Mathf.Pow(level, 3) * (14 + level) / 50f);
            else return Mathf.FloorToInt(Mathf.Pow(level, 3) * (32 + (level / 2)) / 50f);
        }
    }
}