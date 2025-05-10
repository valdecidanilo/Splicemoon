using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class SplicemonStats
    {
        public Schemas.Stats hpStats;
        public Schemas.Stats attackStats;
        public Schemas.Stats defenseStats;
        public Schemas.Stats specialAttackStats;
        public Schemas.Stats specialDefenseStats;
        public Schemas.Stats speedStats;
        [Header("Movement Stats")] 
        public List<string> possiblesMoveAttack = new();
        public List<MoveDetails> movesAttack = new();
    }
}