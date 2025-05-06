using System;
using Player;
using UnityEngine;

namespace Utils
{
    public class GrassOverlayLayer : MonoBehaviour
    {
        public SpriteRenderer grassRenderer;
        public float yThreshold = 0.16f;
        public ParticleSystem grassParticles;
        private void Awake()
        {
            GrassManager.Register(this);
        }
        
        public void UpdateVisibility(float playerY)
        {
            var grassY = transform.position.y;
            grassRenderer.enabled = grassY < playerY;
            
        }
        public void PlayParticles()
        {
            if(!grassParticles.isPlaying)
                grassParticles.Play();
        }
    }
}
