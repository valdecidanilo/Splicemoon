using System.Collections.Generic;
using System.Linq;
using Models;
using Schemas;
using UnityEngine;
using Utils;
using Stats = Models.Stats;

namespace Inventory
{
    [CreateAssetMenu(fileName = "New Splicemon Object", menuName = "Inventory System/Item/Splicemon")]
    public class SplicemonObject : ItemObject {
        public void Awake(){
            type = ItemType.Splicemon;
        }
        [Header("Identity")] public int id;
        public string nameSpliceMon;
        public bool isFemale;
        public string nature;
        public int level = 1;
        public int levelMax = 100;
        public int baseExperience;
        public int experience;
        public int experienceMax;
        public GrowthRate growthRate;

        [Header("Sprites")] 
        public string frontSprite;
        public string backSprite;

        [Header("Audio")] public string crySound;
        
        public SplicemonStats stats;

        public void Initialize(PokeData PokeData, bool isFemale = false)
        {
            stats.hpStats.name = "hp";
            stats.attackStats.name = "attack";
            stats.defenseStats.name = "defense";
            stats.specialAttackStats.name = "special-attacks";
            stats.specialDefenseStats.name = "special-defenses";
            stats.speedStats.name = "speed";

            stats.hpStats.iv = Random.Range(0, 32);
            stats.attackStats.iv = Random.Range(0, 32);
            stats.defenseStats.iv = Random.Range(0, 32);
            stats.specialAttackStats.iv = Random.Range(0, 32);
            stats.specialDefenseStats.iv = Random.Range(0, 32);
            stats.speedStats.iv = Random.Range(0, 32);

            nature = Natures.GetRandomNature();
            var effects = Natures.NatureEffects[nature];

            stats.hpStats.baseStat = Maths.CalculateHp(stats.hpStats.baseStat, stats.hpStats.iv, stats.hpStats.effort, level);
            stats.hpStats.currentStat = stats.hpStats.baseStat;

            experienceMax = GetExperienceForLevel(level);
            RecalculateStats(effects.increased, effects.decreased);
            UpdateSliceMon(PokeData, isFemale);
        }

        public void UpdateSliceMon(PokeData pokeData, bool isFemale)
        {
            id = pokeData.id;
            nameSpliceMon = pokeData.NameSpliceMoon;
            baseExperience = pokeData.baseExperience;
            var sprite = pokeData.sprites;
            frontSprite = isFemale ? sprite.frontFemale : sprite.frontDefault;
            backSprite = isFemale ? sprite.backFemale : sprite.backDefault;
            crySound = pokeData.soundUrl.latest;
            var urlList = pokeData.movesAttack.Select(move => move.move.url).ToList();
            foreach (var moves in urlList)
                stats.possiblesMoveAttack.Add(moves);
            
        }
        //OLD Gain Expericence
        public int GainExperience(int defeatedLevel, bool isTrainerPokemon = false, bool hasLuckyEgg = false)
        {
            var modifier = 1.0f;
            if (isTrainerPokemon) modifier *= 1.5f;
            if (hasLuckyEgg) modifier *= 1.5f;

            var expGained = Mathf.FloorToInt((baseExperience * defeatedLevel * modifier) / 7);

            experience += expGained;
            if (experience >= experienceMax)
            {
                LevelUp();
                LenixSO.Logger.Logger.Log($"{nameSpliceMon} subiu para o nível {level}!");
            }
            //while (experience >= GetExperienceForLevel(level + 1) && level < levelMax)
            //{
            //    LevelUp();
            //    Logger.Log($"{nameSpliceMon} subiu para o nível {level}!");
            //}

            return expGained;
        }

        public int GetExperienceForLevel(int targetLevel)
        {
            switch (growthRate)
            {
                case GrowthRate.Fast:
                    return Mathf.FloorToInt((4f * Mathf.Pow(targetLevel, 3)) / 5f);
                case GrowthRate.MediumFast:
                    return Mathf.FloorToInt(Mathf.Pow(targetLevel, 3));
                case GrowthRate.MediumSlow:
                    return Mathf.FloorToInt((6f/5f * Mathf.Pow(targetLevel, 3)) - (15f * Mathf.Pow(targetLevel, 2)) + (100f * targetLevel) - 140f);
                case GrowthRate.Slow:
                    return Mathf.FloorToInt((5f * Mathf.Pow(targetLevel, 3)) / 4f);
                case GrowthRate.Erratic:
                    return Maths.CalculateErraticExp(targetLevel);
                case GrowthRate.Fluctuating:
                    return Maths.CalculateFluctuatingExp(targetLevel);
                default:
                    return Mathf.FloorToInt(Mathf.Pow(targetLevel, 3));
            }
        }
        
        public void LevelUp(int levelsGained = 1, string natureIncreasedStat = "", string natureDecreasedStat = "")
        {
            level += levelsGained;

            if (level > levelMax)
                level = levelMax;
            stats.hpStats.currentStat = Maths.CalculateHp(stats.hpStats.baseStat, stats.hpStats.iv, stats.hpStats.effort, level);
            RecalculateStats(natureIncreasedStat, natureDecreasedStat);
        }


        private void RecalculateStats(string natureIncreasedStat = "", string natureDecreasedStat = "")
        {
            stats.attackStats.currentStat = Maths.CalculateOtherStat(
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