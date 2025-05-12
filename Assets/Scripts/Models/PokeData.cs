using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Models
{
    public class PokeData
    {
        [JsonProperty("id")] public int id { get; set; }
        [JsonProperty("name")] public string NameSpliceMoon { get; set; }
        [JsonProperty("abilities")] public List<AbilityData> abilities;
        [JsonProperty("base_experience")] public int baseExperience;
        [JsonProperty("cries")] public SoundUrl soundUrl;
        [JsonProperty("moves")] public List<MovesAttack> movesAttack;
        [JsonProperty("sprites")] public Sprites sprites;
        [JsonProperty("types")] public List<PokeType> types;
        [JsonProperty("stats")] public List<Stats> status;
    }
    //==== Skills
    public struct AbilityData
    {
        public Ability ability { get; set; }
        public bool is_hidden { get; set; }
        public int slot { get; set; }
    }
    public struct Ability
    {
        [JsonProperty("name")] public string name { get; set; }
        [JsonProperty("url")] public string url { get; set; }
    }
    //====

    //==== Move Attack
    public struct MovesAttack
    {
        [JsonProperty("move")] public Move move { get; set; }
    }
    public struct Move
    {
        [JsonProperty("name")] public string nameAttack { get; set; }
        [JsonProperty("url")] public string url { get; set; }
    }
    [Serializable]
    public class MoveDetails
    {
        [JsonProperty("accuracy")]
        [SerializeField] public int? accuracy;

        [JsonProperty("name")]
        [SerializeField] public string nameMove;

        [JsonProperty("power")]
        [SerializeField] public int? powerAttack;

        [JsonProperty("pp")]
        [SerializeField] public int ppMax;

        [JsonProperty("type")]
        [SerializeField] public TypeMove typeMove;

        [SerializeField] public int ppCurrent;
        public int? Accuracy => accuracy;
        public string NameMove => nameMove;
        public int? PowerAttack => powerAttack;
        public int PpMax => ppMax;
        public TypeMove TypeMove => typeMove;
        public int PpCurrent => ppCurrent;
    }
    [Serializable]
    public class TypeMove
    {
        [JsonProperty("name")]
        [SerializeField] private string typeAttack;

        [JsonProperty("url")]
        [SerializeField] private string url;

        public string TypeAttack => typeAttack;
        public string Url => url;
    }
    //====
    
    //==== Sprites
    public struct Sprites
    {
        [JsonProperty("front_default")] public string frontDefault { get; set; }
        [JsonProperty("front_female")] public string frontFemale { get; set; }
        [JsonProperty("front_shiny")] public string frontShiny { get; set; }
        [JsonProperty("front_shiny_female")] public string frontShinyFemale { get; set; }
        
        [JsonProperty("back_default")] public string backDefault { get; set; }
        [JsonProperty("back_female")] public string backFemale { get; set; }
        [JsonProperty("back_shiny")] public string backShiny { get; set; }
        [JsonProperty("back_shiny_female")] public string backShinyFemale { get; set; }
    }
    //====
    public struct SoundUrl
    {
        public string latest { get; set; }
        public string legacy { get; set; }
    }
    //====

    public struct PokeType
    {
        [JsonProperty("slot")] public int slot { get; set; }
        [JsonProperty("name")] public string name { get; set; }
        [JsonProperty("type")] public Type type { get; set; }
        public int Slot => slot;
        public string Name => name;
        public Type Type => type;
    }

    public struct Type
    {
        [JsonProperty("name")] public string nameType { get; set; }
        [JsonProperty("url")] public string url { get; set; }
        public string NameType => nameType;
        public string Url => url;
    }
    
    //==== Stats

    public struct Stats
    {
        [JsonProperty("base_stat")] public int baseStatus { get; set; }
        [JsonProperty("effort")] public int effort { get; set; }
        [JsonProperty("stat")] public Stat status { get; set; }
        public string name { get; set; }
        public int iv { get; set; }
    }

    public struct Stat
    {
        [JsonProperty("name")] public string nameStat { get; set; }
        [JsonProperty("url")] public string url { get; set; }
    }
    //====
}

