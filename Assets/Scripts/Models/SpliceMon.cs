using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Schemas;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;

namespace Models
{
    public class SpliceMon : MonoBehaviour
    {
        public PokeData PokeData;

        [Header("Identity")] public int id;
        public string nameSpliceMon;
        public bool isFemale;
        public string nature;
        public int level;
        public int levelMax = 100;
        public int baseExperience;
        public int experience;

        public GrowthRate growthRate;

        [Header("Sprites")] public string frontSprite;
        public string backSprite;

        [Header("Audio")] public string crySound;

        [Header("Battle Stats")] public Schemas.Stats hpStats;
        public Schemas.Stats attackStats;
        public Schemas.Stats defenseStats;
        public Schemas.Stats specialAttackStats;
        public Schemas.Stats specialDefenseStats;
        public Schemas.Stats speedStats;

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

            hpStats.currentStat = Schemas.Stats.CalculateHp(hpStats.baseStat, hpStats.iv, hpStats.effort, level);

            RecalculateStats(effects.increased, effects.decreased);
            UpdateSliceMon(PokeData, isFemale);
        }

        public void UpdateSliceMon(PokeData pokeData, bool isFemale)
        {
            PokeData = pokeData;
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

        public void GainExperience(int defeatedLevel, bool isTrainerPokemon = false, bool hasLuckyEgg = false)
        {
            var modifier = 1.0f;
            if (isTrainerPokemon) modifier *= 1.5f;
            if (hasLuckyEgg) modifier *= 1.5f;

            var expGained = Mathf.FloorToInt((baseExperience * defeatedLevel * modifier) / 7);

            experience += expGained;
            Logger.Log($"{nameSpliceMon} ganhou {expGained} EXP!");

            while (experience >= GetExperienceForLevel(level + 1) && level < levelMax)
            {
                LevelUp();
                Logger.Log($"{nameSpliceMon} subiu para o nível {level}!");
            }
        }

        private static int GetExperienceForLevel(int targetLevel) => targetLevel * targetLevel * targetLevel;

        public void LevelUp(int levelsGained = 1, string natureIncreasedStat = "", string natureDecreasedStat = "")
        {
            level += levelsGained;

            if (level > levelMax)
                level = levelMax;
            hpStats.currentStat = Schemas.Stats.CalculateHp(hpStats.baseStat, hpStats.iv, hpStats.effort, level);
            RecalculateStats(natureIncreasedStat, natureDecreasedStat);
        }


        private void RecalculateStats(string natureIncreasedStat = "", string natureDecreasedStat = "")
        {
            attackStats.currentStat = Schemas.Stats.CalculateOtherStat(
                attackStats.baseStat, attackStats.iv, attackStats.effort, level,
                Natures.GetNatureMultiplier(
                    attackStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            defenseStats.currentStat = Schemas.Stats.CalculateOtherStat(
                defenseStats.baseStat, defenseStats.iv, defenseStats.effort, level,
                Natures.GetNatureMultiplier(
                    defenseStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            specialAttackStats.currentStat = Schemas.Stats.CalculateOtherStat(
                specialAttackStats.baseStat, specialAttackStats.iv, specialAttackStats.effort, level,
                Natures.GetNatureMultiplier(
                    specialAttackStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            specialDefenseStats.currentStat = Schemas.Stats.CalculateOtherStat(
                specialDefenseStats.baseStat, specialDefenseStats.iv, specialDefenseStats.effort, level,
                Natures.GetNatureMultiplier(
                    specialDefenseStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
            speedStats.currentStat = Schemas.Stats.CalculateOtherStat(
                speedStats.baseStat, speedStats.iv, speedStats.effort, level,
                Natures.GetNatureMultiplier(
                    speedStats.name,
                    natureIncreasedStat,
                    natureDecreasedStat)
            );
        }
    }
}