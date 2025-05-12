using System.Collections.Generic;
using Models;
using SQLite;

namespace DB.Data
{
    [Table("SplicemonData")]
    public class SplicemonData
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public int UserId { get; set; }

        // Identidade
        public string Name { get; set; }
        public bool IsFemale { get; set; }
        public string Nature { get; set; }
        public int Level { get; set; }
        public int BaseExperience { get; set; }
        public int Experience { get; set; }
        public int ExperienceMax { get; set; }
        public string GrowthRate { get; set; }

        // Visuais e áudio
        public string FrontSprite { get; set; }
        public string BackSprite { get; set; }
        public string CrySound { get; set; }

        // Stats calculadas
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpAttack { get; set; }
        public int SpDefense { get; set; }
        public int Speed { get; set; }

        // IVs (opcional)
        public int IvHp { get; set; }
        public int IvAttack { get; set; }
        public int IvDefense { get; set; }
        public int IvSpAttack { get; set; }
        public int IvSpDefense { get; set; }
        public int IvSpeed { get; set; }
        
        // Effort (opcional)
        public int EffortHp { get; set; }
        public int EffortAttack { get; set; }
        public int EffortDefense { get; set; }
        public int EffortSpAttack { get; set; }
        public int EffortSpDefense { get; set; }
        public int EffortSpeed { get; set; }
        
        // Lista de moves e possíveis moves
        public string MovesJson { get; set; }
        public string PossibleMovesJson { get; set; }
    }
    [System.Serializable]
    public class MoveListWrapper
    {
        public List<MoveDetails> moves;
    }

    [System.Serializable]
    public class StringListWrapper
    {
        public List<string> items;
    }
}