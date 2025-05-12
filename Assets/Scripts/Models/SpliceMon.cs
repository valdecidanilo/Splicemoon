using System;
using System.Collections.Generic;
using System.Linq;
using DB.Data;
using Newtonsoft.Json;
using Schemas;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Models
{
    public class SpliceMon : MonoBehaviour
    {
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

        [Header("Sprites")] public string frontSprite;
        public string backSprite;

        [Header("Audio")] public string crySound;
        
        [Header("Battle Stats")] 
        public SplicemonStats stats = new ();

        public void LoadMoves()
        {
            stats.movesAttack = GetMoveUsed(stats.possiblesMoveAttack);
        }
        public void InitializeFromData(SplicemonData data)
        {
            id = data.Id;
            nameSpliceMon = data.Name;
            isFemale = data.IsFemale;
            nature = data.Nature;
            level = data.Level;
            baseExperience = data.BaseExperience;
            experience = data.Experience;
            experienceMax = Maths.GetExperienceForLevel(level, growthRate);

            // HP
            stats.hpStats = new Schemas.Stats("hp", data.IvHp, data.Hp, data.EffortHp)
            {
                currentStat = data.Hp
            };

            // Attack
            stats.attackStats = new Schemas.Stats("attack", data.IvAttack, data.Attack, data.EffortAttack)
            {
                currentStat = data.Attack
            };

            // Defense
            stats.defenseStats = new Schemas.Stats("defense", data.IvDefense, data.Defense, data.EffortDefense)
            {
                currentStat = data.Defense
            };

            // Special Attack
            stats.specialAttackStats = new Schemas.Stats("sp_attack", data.IvSpAttack, data.SpAttack, data.EffortSpAttack)
            {
                currentStat = data.SpAttack
            };

            // Special Defense
            stats.specialDefenseStats = new Schemas.Stats("sp_defense", data.IvSpDefense, data.SpDefense, data.EffortSpDefense)
            {
                currentStat = data.SpDefense
            };

            // Speed
            stats.speedStats = new Schemas.Stats("speed", data.IvSpeed, data.Speed, data.EffortSpeed)
            {
                currentStat = data.Speed
            };

            // Growth rate
            if (Enum.TryParse(data.GrowthRate, out GrowthRate parsedGrowthRate))
                growthRate = parsedGrowthRate;
            else
                Debug.LogWarning($"GrowthRate inválido: {data.GrowthRate}");

            frontSprite = data.FrontSprite;
            backSprite = data.BackSprite;
            crySound = data.CrySound;

            // Moves possíveis
            try
            {
                var wrapperPossible = JsonConvert.DeserializeObject<StringListWrapper>(data.PossibleMovesJson);
                stats.possiblesMoveAttack = wrapperPossible?.items ?? new List<string>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro ao desserializar PossibleMovesJson: {ex.Message}");
                stats.possiblesMoveAttack = new List<string>();
            }

            // Moves atuais
            try
            {
                var wrapperMoves = JsonConvert.DeserializeObject<MoveListWrapper>(data.MovesJson);
                stats.movesAttack = wrapperMoves?.moves ?? new List<MoveDetails>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro ao desserializar MovesJson: {ex.Message}");
                stats.movesAttack = new List<MoveDetails>();
            }

            // Natureza: NÃO recalcular os stats — já foram salvos prontos.
            if (!Natures.NatureEffects.ContainsKey(nature))
            {
                Debug.LogWarning($"Natureza inválida ou não registrada: {nature}");
            }
        }

        public void Initialize(PokeData pokeData, bool isCanFemale = false)
        {
            stats.hpStats = new Schemas.Stats("hp", Random.Range(0, 32), 0, 0);
            stats.attackStats = new Schemas.Stats("attack", Random.Range(0, 32), 0, 0);
            stats.defenseStats = new Schemas.Stats("defense", Random.Range(0, 32), 0, 0);
            stats.specialAttackStats = new Schemas.Stats("special-attacks", Random.Range(0, 32), 0, 0);
            stats.specialDefenseStats = new Schemas.Stats("special-defenses", Random.Range(0, 32), 0, 0);
            stats.speedStats = new Schemas.Stats("speed", Random.Range(0, 32), 0, 0);

            growthRate = Maths.GetRandomGrowthRate();

            nature = Natures.GetRandomNature();
            var effects = Natures.NatureEffects[nature];

            stats.hpStats.baseStat = Maths.CalculateHp(stats.hpStats.baseStat, stats.hpStats.iv, stats.hpStats.effort, level);
            stats.hpStats.currentStat = stats.hpStats.baseStat;
            
            stats.attackStats.baseStat = Maths.CalculateOtherStat(stats.attackStats.baseStat, 
                stats.attackStats.iv, stats.attackStats.effort, level, 
                Natures.GetNatureMultiplier("attack", effects.increased, effects.decreased));
            stats.attackStats.currentStat = stats.attackStats.baseStat;

            stats.defenseStats.baseStat = Maths.CalculateOtherStat(stats.defenseStats.baseStat, 
                stats.defenseStats.iv, stats.defenseStats.effort, level, 
                Natures.GetNatureMultiplier("defense", effects.increased, effects.decreased));
            stats.defenseStats.currentStat = stats.defenseStats.baseStat;

            stats.specialAttackStats.baseStat = Maths.CalculateOtherStat(stats.specialAttackStats.baseStat, 
                stats.specialAttackStats.iv, stats.specialAttackStats.effort, level, 
                Natures.GetNatureMultiplier("special-attack", effects.increased, effects.decreased));
            stats.specialAttackStats.currentStat = stats.specialAttackStats.baseStat;

            stats.specialDefenseStats.baseStat = Maths.CalculateOtherStat(stats.specialDefenseStats.baseStat, 
                stats.specialDefenseStats.iv, stats.specialDefenseStats.effort, level, 
                Natures.GetNatureMultiplier("special-defense", effects.increased, effects.decreased));
            stats.specialDefenseStats.currentStat = stats.specialDefenseStats.baseStat;

            stats.speedStats.baseStat = Maths.CalculateOtherStat(stats.speedStats.baseStat, 
                stats.speedStats.iv, stats.speedStats.effort, level, 
                Natures.GetNatureMultiplier("speed", effects.increased, effects.decreased));
            stats.speedStats.currentStat = stats.speedStats.baseStat;
            
            

            experienceMax = Maths.GetExperienceForLevel(level, growthRate);
            Maths.RecalculateStats(stats, level, effects.increased, effects.decreased);
            UpdateSliceMon(pokeData, isCanFemale);
        }

        private void UpdateSliceMon(PokeData pokeData, bool isCanFemale)
        {
            id = pokeData.id;
            nameSpliceMon = pokeData.NameSpliceMoon;
            baseExperience = pokeData.baseExperience;
            var sprite = pokeData.sprites;
            frontSprite = isCanFemale ? sprite.frontFemale : sprite.frontDefault;
            backSprite = isCanFemale ? sprite.backFemale : sprite.backDefault;
            crySound = pokeData.soundUrl.latest;
            var urlList = pokeData.movesAttack.Select(move => move.move.url).ToList();
            foreach (var moves in urlList)
                stats.possiblesMoveAttack.Add(moves);
        }

        private List<MoveDetails> GetMoveUsed(List<string> movesAttack)
        {
            var fourfirst = movesAttack.Take(4).ToList();
            var fourMoves = new List<MoveDetails>();
            StartCoroutine(ApiManager.GetMoveDetails(fourfirst, moveAttackList =>
            {
                for (var i = 0; i < 4; i++)
                {
                    moveAttackList[i].ppCurrent = moveAttackList[i].ppMax;
                    fourMoves.Add(moveAttackList[i]);
                }
            }));
            return fourMoves;
        }
    }
}