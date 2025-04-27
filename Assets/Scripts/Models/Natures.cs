using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public static class Natures
    {
        private static readonly string[] AllNatures =
        {
            "hardy", "bold", "modest", "calm", "timid",
            "lonely", "docile", "mild", "gentle", "hasty",
            "adamant", "impish", "bashful", "careful", "rash",
            "jolly", "naughty", "lax", "quirky", "naive",
            "serious", "relaxed", "quiet", "sassy", "brave"
        };
        public static readonly Dictionary<string, (string increased, string decreased)> NatureEffects = new()
        {
            {"hardy", ("", "")},
            {"docile", ("", "")},
            {"bashful", ("", "")},
            {"quirky", ("", "")},
            {"serious", ("", "")},
            {"lonely", ("attack", "defense")},
            {"adamant", ("attack", "special-attacks")},
            {"naughty", ("attack", "special-defenses")},
            {"brave", ("attack", "speed")},
            {"bold", ("defense", "attack")},
            {"impish", ("defense", "special-attacks")},
            {"lax", ("defense", "special-defenses")},
            {"relaxed", ("defense", "speed")},
            {"modest", ("special-attacks", "attack")},
            {"mild", ("special-attacks", "defense")},
            {"rash", ("special-attacks", "special-defenses")},
            {"quiet", ("special-attacks", "speed")},
            {"calm", ("special-defenses", "attack")},
            {"gentle", ("special-defenses", "defense")},
            {"careful", ("special-defenses", "special-attacks")},
            {"sassy", ("special-defenses", "speed")},
            {"timid", ("speed", "attack")},
            {"hasty", ("speed", "defense")},
            {"jolly", ("speed", "special-attacks")},
            {"naive", ("speed", "special-defenses")}
        };
        public static string GetRandomNature()
        {
            var randomIndex = Random.Range(0, AllNatures.Length);
            return AllNatures[randomIndex];
        }
        public static double GetNatureMultiplier(string statName, string increasedStat, string decreasedStat)
        {
            if (statName == increasedStat)
                return 1.1;
            else if (statName == decreasedStat)
                return 0.9;
            else
                return 1.0;
        }
    }
}