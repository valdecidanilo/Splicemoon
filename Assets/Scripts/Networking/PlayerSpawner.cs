using Fusion;
using UnityEngine;

namespace Networking
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
    {
        public GameObject playerPrefab;
    
        public void PlayerJoined(PlayerRef player)
        {
            Runner.Spawn(playerPrefab, Vector2.zero);
        }
    }
}
