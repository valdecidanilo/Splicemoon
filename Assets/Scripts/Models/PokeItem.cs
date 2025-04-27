using System;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = LenixSO.Logger.Logger;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewSplicemon", menuName = "Splicemon/SplicemonItem")]
public class PokeItem : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string nameSpliceMon;
    public bool isFemale;
    public int level;
    public int levelMax = 100;
    public int baseExperience;
    public int experience;

    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite backSprite;

    [Header("Audio")]
    public AudioClip crySound;

    [Header("Battle Stats")]
    public Stats hpStats;
    public Stats attackStats;
    public Stats defenseStats;
    public Stats specialAttackStats;
    public Stats specialDefenseStats;
    public Stats speedStats;
    
    private static readonly string[] AllNatures =
    {
        "hardy", "bold", "modest", "calm", "timid",
        "lonely", "docile", "mild", "gentle", "hasty",
        "adamant", "impish", "bashful", "careful", "rash",
        "jolly", "naughty", "lax", "quirky", "naive",
        "serious", "relaxed", "quiet", "sassy", "brave"
    };

    public List<ItemMoveAttack> moveAttack = new();

    private void Awake()
    {
        Initialize();
    }

    public void Initialize(string natureIncreasedStat = "", string natureDecreasedStat = "")
    {
        hpStats.iv = Random.Range(0, 32);
        attackStats.iv = Random.Range(0, 32);
        defenseStats.iv = Random.Range(0, 32);
        specialAttackStats.iv = Random.Range(0, 32);
        specialDefenseStats.iv = Random.Range(0, 32);
        speedStats.iv = Random.Range(0, 32);

        var randomNature = GetRandomNature();
        
        hpStats.currentStat = CalculateHp(hpStats.baseStat, hpStats.iv, hpStats.effort, level);

        RecalculateStats(natureIncreasedStat, natureDecreasedStat);
    }
    public void GainExperience(int expGained, string natureIncreasedStat = "", string natureDecreasedStat = "")
    {
        experience += expGained;

        while (experience >= GetExperienceForLevel(level + 1) && level < levelMax)
        {
            LevelUp(1, natureIncreasedStat, natureDecreasedStat);

            Logger.Log($"{nameSpliceMon} subiu para o nível {level}!");
        }
    }

    private static int GetExperienceForLevel(int targetLevel) => targetLevel * targetLevel * targetLevel;
    public void LevelUp(int levelsGained = 1, string natureIncreasedStat = "", string natureDecreasedStat = "")
    {
        level += levelsGained;

        if (level > levelMax)
            level = levelMax;
        hpStats.currentStat = CalculateHp(hpStats.baseStat, hpStats.iv, hpStats.effort, level);
        RecalculateStats(natureIncreasedStat, natureDecreasedStat);
    }

    public string GetRandomNature()
    {
        var randomIndex = Random.Range(0, AllNatures.Length);
        return AllNatures[randomIndex];
    }
    private void RecalculateStats(string natureIncreasedStat = "", string natureDecreasedStat = "")
    {
        attackStats.name = "attack";
        defenseStats.name = "defense";
        specialAttackStats.name = "special-attacks";
        specialDefenseStats.name = "special-defenses";
        speedStats.name = "speed";
        
        attackStats.currentStat = CalculateOtherStat(attackStats.baseStat, attackStats.iv, attackStats.effort, level, GetNatureMultiplier(attackStats.name, natureIncreasedStat, natureDecreasedStat));
        defenseStats.currentStat = CalculateOtherStat(defenseStats.baseStat, defenseStats.iv, defenseStats.effort, level, GetNatureMultiplier(defenseStats.name, natureIncreasedStat, natureDecreasedStat));
        specialAttackStats.currentStat = CalculateOtherStat(specialAttackStats.baseStat, specialAttackStats.iv, specialAttackStats.effort, level, GetNatureMultiplier(specialAttackStats.name, natureIncreasedStat, natureDecreasedStat));
        specialDefenseStats.currentStat = CalculateOtherStat(specialDefenseStats.baseStat, specialDefenseStats.iv, specialDefenseStats.effort, level, GetNatureMultiplier(specialDefenseStats.name, natureIncreasedStat, natureDecreasedStat));
        speedStats.currentStat = CalculateOtherStat(speedStats.baseStat, speedStats.iv, speedStats.effort, level, GetNatureMultiplier(speedStats.name, natureIncreasedStat, natureDecreasedStat));
    }

    private static int CalculateHp(int baseStat, int iv, int ev, int level)
    {
        var hp = (2 * baseStat + iv + ev / 4) * level / 100 + level + 10;
        return hp;
    }

    private static int CalculateOtherStat(int baseStat, int iv, int ev, int level, double natureMultiplier)
    {
        var stat = (int)(((2 * baseStat + iv + ev / 4) * level / 100 + 5) * natureMultiplier);
        return stat;
    }

    private static double GetNatureMultiplier(string statName, string increasedStat, string decreasedStat)
    {
        if (statName == increasedStat)
            return 1.1;
        else if (statName == decreasedStat)
            return 0.9;
        else
            return 1.0;
    }
}

[System.Serializable]
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
}

[System.Serializable]
public struct ItemMoveAttack
{
    public string nameMove;
    public int accuracy;
    public int powerAttack;
    public int pp;
    public int ppMax;
}
