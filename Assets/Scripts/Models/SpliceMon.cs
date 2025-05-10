using System.Collections.Generic;
using System.Linq;
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

            
            stats.movesAttack = new List<MoveDetails>();

            stats.hpStats.baseStat = Maths.CalculateHp(stats.hpStats.baseStat, stats.hpStats.iv, stats.hpStats.effort, level);
            stats.hpStats.currentStat = stats.hpStats.baseStat;

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
    }
}