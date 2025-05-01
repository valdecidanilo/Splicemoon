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

        [Header("Battle Stats")] public GameStats hpStats;
        public GameStats attackStats;
        public GameStats defenseStats;
        public GameStats specialAttackStats;
        public GameStats specialDefenseStats;
        public GameStats speedStats;

        public List<string> moveAttack = new();
        public List<MoveDetails> itensMove = new();

        public void Initialize(PokeData PokeData, bool isFemale = false)
        {
            hpStats.name = "hp";
            attackStats.name = "attack";
            defenseStats.name = "defense";
            specialAttackStats.name = "special-attacks";
            specialDefenseStats.name = "special-defenses";
            speedStats.name = "speed";

            hpStats.iv = Random.Range(0, 32);
            attackStats.iv = Random.Range(0, 32);
            defenseStats.iv = Random.Range(0, 32);
            specialAttackStats.iv = Random.Range(0, 32);
            specialDefenseStats.iv = Random.Range(0, 32);
            speedStats.iv = Random.Range(0, 32);

            nature = Natures.GetRandomNature();
            var effects = Natures.NatureEffects[nature];

            hpStats.baseStat = Maths.CalculateHp(hpStats.baseStat, hpStats.iv, hpStats.effort, level);
            hpStats.currentStat = hpStats.baseStat;

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
                moveAttack.Add(moves);
            
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
            hpStats.currentStat = Maths.CalculateHp(hpStats.baseStat, hpStats.iv, hpStats.effort, level);
            RecalculateStats(natureIncreasedStat, natureDecreasedStat);
        }


        private void RecalculateStats(string natureIncreasedStat = "", string natureDecreasedStat = "")
        {
            attackStats.currentStat = Maths.CalculateOtherStat(
                attackStats.baseStat, attackStats.iv, attackStats.effort, level,
                Natures.GetNatureMultiplier(
                    attackStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            defenseStats.currentStat = Maths.CalculateOtherStat(
                defenseStats.baseStat, defenseStats.iv, defenseStats.effort, level,
                Natures.GetNatureMultiplier(
                    defenseStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            specialAttackStats.currentStat = Maths.CalculateOtherStat(
                specialAttackStats.baseStat, specialAttackStats.iv, specialAttackStats.effort, level,
                Natures.GetNatureMultiplier(
                    specialAttackStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            specialDefenseStats.currentStat = Maths.CalculateOtherStat(
                specialDefenseStats.baseStat, specialDefenseStats.iv, specialDefenseStats.effort, level,
                Natures.GetNatureMultiplier(
                    specialDefenseStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            speedStats.currentStat = Maths.CalculateOtherStat(
                speedStats.baseStat, speedStats.iv, speedStats.effort, level,
                Natures.GetNatureMultiplier(
                    speedStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
        }
    }
}