using Models;
using Schemas;
using UnityEngine;

namespace Utils
{
    public static class Maths
    {
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

        public static GrowthRate GetRandomGrowthRate()
        {
            var random = Random.Range(0, 6);
            return random switch
            {
                0 => GrowthRate.Erratic,
                1 => GrowthRate.Fast,
                2 => GrowthRate.MediumFast,
                3 => GrowthRate.MediumSlow,
                4 => GrowthRate.Slow,
                _ => GrowthRate.Fluctuating
            };
        }
        public static int CalculateFluctuatingExp(int level)
        {
            if (level <= 15) return Mathf.FloorToInt(Mathf.Pow(level, 3) * (24 + ((level + 1) / 3)) / 50f);
            else if (level <= 36) return Mathf.FloorToInt(Mathf.Pow(level, 3) * (14 + level) / 50f);
            else return Mathf.FloorToInt(Mathf.Pow(level, 3) * (32 + (level / 2)) / 50f);
        }
        public static int GainExperience(int opponentLevel, int baseAllyExperience, int experience, int experienceMax, SplicemonStats stats, bool isTrainerPokemon = false, bool hasLuckyEgg = false)
        {
            var modifier = 1.0f;
            if (isTrainerPokemon) modifier *= 1.5f;
            if (hasLuckyEgg) modifier *= 1.5f;

            var expGained = Mathf.FloorToInt((baseAllyExperience * opponentLevel * modifier) / 7);

            experience += expGained;
            if (experience >= experienceMax)
            {
                //LevelUp();
                //LenixSO.Logger.Logger.Log($"{nameSpliceMon} subiu para o nível {level}!");
            }
            //while (experience >= GetExperienceForLevel(level + 1) && level < levelMax)
            //{
            //    LevelUp();
            //    Logger.Log($"{nameSpliceMon} subiu para o nível {level}!");
            //}
            return expGained;
        }

        public static int GetExperienceForLevel(int targetLevel, GrowthRate currentGrowthRate)
        {
            return currentGrowthRate switch
            {
                GrowthRate.Fast => Mathf.FloorToInt((4f * Mathf.Pow(targetLevel, 3)) / 5f),
                GrowthRate.MediumFast => Mathf.FloorToInt(Mathf.Pow(targetLevel, 3)),
                GrowthRate.MediumSlow => Mathf.FloorToInt((6f / 5f * Mathf.Pow(targetLevel, 3)) -
                    (15f * Mathf.Pow(targetLevel, 2)) + (100f * targetLevel) - 140f),
                GrowthRate.Slow => Mathf.FloorToInt((5f * Mathf.Pow(targetLevel, 3)) / 4f),
                GrowthRate.Erratic => Maths.CalculateErraticExp(targetLevel),
                GrowthRate.Fluctuating => Maths.CalculateFluctuatingExp(targetLevel),
                _ => Mathf.FloorToInt(Mathf.Pow(targetLevel, 3))
            };
        }

        private static void LevelUp(int currentLevel, int levelMax, SplicemonStats stats, int levelsGained = 1, string natureIncreasedStat = "", string natureDecreasedStat = "")
        {
            currentLevel += levelsGained;

            if (currentLevel > levelMax)
                currentLevel = levelMax;
            stats.hpStats.currentStat = CalculateHp(stats.hpStats.baseStat, stats.hpStats.iv, stats.hpStats.effort, currentLevel);
            RecalculateStats(stats, currentLevel, natureIncreasedStat, natureDecreasedStat);
        }
        public static void RecalculateStats(SplicemonStats stats, int level, string natureIncreasedStat = "", string natureDecreasedStat = "")
        {
            stats.attackStats.currentStat = CalculateOtherStat(
                stats.attackStats.baseStat, stats.attackStats.iv, stats.attackStats.effort, level,
                Natures.GetNatureMultiplier(
                    stats.attackStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            stats.defenseStats.currentStat = Maths.CalculateOtherStat(
                stats.defenseStats.baseStat, stats.defenseStats.iv, stats.defenseStats.effort, level,
                Natures.GetNatureMultiplier(
                    stats.defenseStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            stats.specialAttackStats.currentStat = Maths.CalculateOtherStat(
                stats.specialAttackStats.baseStat, stats.specialAttackStats.iv, stats.specialAttackStats.effort, level,
                Natures.GetNatureMultiplier(
                    stats.specialAttackStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            stats.specialDefenseStats.currentStat = Maths.CalculateOtherStat(
                stats.specialDefenseStats.baseStat, stats.specialDefenseStats.iv, stats.specialDefenseStats.effort, level,
                Natures.GetNatureMultiplier(
                    stats.specialDefenseStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            stats.speedStats.currentStat = Maths.CalculateOtherStat(
                stats.speedStats.baseStat, stats.speedStats.iv, stats.speedStats.effort, level,
                Natures.GetNatureMultiplier(
                    stats.speedStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
        }
    }
}