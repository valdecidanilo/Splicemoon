using System;
using System.Collections;
using Fusion;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

namespace Networking
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
    {
        public GameObject playerPrefab;
        public void PlayerJoined(PlayerRef player)
        {
            Runner.Spawn(playerPrefab, Vector2.zero);
            Logger.Log("Instanciou o jogador");
        }
    }
}
