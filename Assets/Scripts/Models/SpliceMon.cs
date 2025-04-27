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
    [CreateAssetMenu(fileName = "New Splicemon", menuName = "Splicemon/new Splicemon")]
    public class SpliceMon : DefaultObject
    {
        [Header("Identity")]
        public int id;
        public string nameSpliceMon;
        public bool isFemale;
        public string nature;
        public int level;
        public int levelMax = 100;
        public int baseExperience;
        public int experience;
    
        public GrowthRate growthRate;

        [Header("Sprites")]
        public string frontSprite;
        public string backSprite;

        [Header("Audio")]
        public string crySound;

        [Header("Battle Stats")]
        public Schemas.Stats hpStats;
        public Schemas.Stats attackStats;
        public Schemas.Stats defenseStats;
        public Schemas.Stats specialAttackStats;
        public Schemas.Stats specialDefenseStats;
        public Schemas.Stats speedStats;
        
        public List<string> moveAttack = new();

        private void Awake()
        {
            Initialize();
            type = ItemType.Splicemon;
        }
        
        private void Initialize()
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
        public void ApplyTo(SpliceMon target)
        {
            target.id = this.id;
            target.nameSpliceMon = this.nameSpliceMon;
            target.isFemale = this.isFemale;
            target.nature = this.nature;
            target.level = this.level;
            target.levelMax = this.levelMax;
            target.baseExperience = this.baseExperience;
            target.experience = this.experience;
            target.growthRate = this.growthRate;
            
            target.frontSprite = this.frontSprite;
            target.backSprite = this.backSprite;
            target.crySound = this.crySound;
            
            target.moveAttack = new List<string>(this.moveAttack);
            
            target.hpStats = new Schemas.Stats(
                this.hpStats.name,
                this.hpStats.baseStat,
                this.hpStats.currentStat,
                this.hpStats.effort
            );
            target.hpStats.iv = this.hpStats.iv;
    
            target.attackStats = new Schemas.Stats(
                this.attackStats.name,
                this.attackStats.baseStat,
                this.attackStats.currentStat,
                this.attackStats.effort
            );
            target.attackStats.iv = this.attackStats.iv;
    
            target.defenseStats = new Schemas.Stats(
                this.defenseStats.name,
                this.defenseStats.baseStat,
                this.defenseStats.currentStat,
                this.defenseStats.effort
            );
            target.defenseStats.iv = this.defenseStats.iv;
    
            target.specialAttackStats = new Schemas.Stats(
                this.specialAttackStats.name,
                this.specialAttackStats.baseStat,
                this.specialAttackStats.currentStat,
                this.specialAttackStats.effort
            );
            target.specialAttackStats.iv = this.specialAttackStats.iv;
    
            target.specialDefenseStats = new Schemas.Stats(
                this.specialDefenseStats.name,
                this.specialDefenseStats.baseStat,
                this.specialDefenseStats.currentStat,
                this.specialDefenseStats.effort
            );
            target.specialDefenseStats.iv = this.specialDefenseStats.iv;
    
            target.speedStats = new Schemas.Stats(
                this.speedStats.name,
                this.speedStats.baseStat,
                this.speedStats.currentStat,
                this.speedStats.effort
            );
            target.speedStats.iv = this.speedStats.iv;
            
            // Garante que o tipo está correto
            target.type = ItemType.Splicemon;
        }
    }

    [Serializable]
    public struct ItemMoveAttack
    {
        public string nameMove;
        public int accuracy;
        public int powerAttack;
        public int pp;
        public int ppMax;

        public ItemMoveAttack(string nameMove, int accuracy, int powerAttack, int pp, int ppMax)
        {
            this.nameMove = nameMove;
            this.accuracy = accuracy;
            this.powerAttack = powerAttack;
            this.pp = pp;
            this.ppMax = ppMax;
        }
    }
}