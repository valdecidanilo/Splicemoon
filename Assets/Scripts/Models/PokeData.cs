using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

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
    public struct MoveDetails
    {
        [JsonProperty("accuracy")] [CanBeNull] public int? accuracy { get; set; }
        [JsonProperty("name")] public string nameMove { get; set; }
        [JsonProperty("power")] [CanBeNull] public int? powerAttack { get; set; }
        [JsonProperty("pp")] public int ppMax { get; set; }
        [JsonProperty("type")] public TypeMove typeMove { get; set; }
    }
    public struct TypeMove
    {
        [JsonProperty("name")] public string typeAttack { get; set; }
        [JsonProperty("url")] public string url { get; set; }
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
    }

    public struct Type
    {
        [JsonProperty("name")] public string nameType { get; set; }
        [JsonProperty("url")] public string url { get; set; }
    }
    
    //==== Stats

    public partial struct Stats
    {
        [JsonProperty("base_stat")] public int baseStatus { get; set; }
        [JsonProperty("effort")] public int effort { get; set; }
        [JsonProperty("stat")] public Stat status { get; set; }
    }

    public struct Stat
    {
        [JsonProperty("name")] public string nameStat { get; set; }
        [JsonProperty("url")] public string url { get; set; }
    }
    //====
}

