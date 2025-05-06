using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class GrassManager : MonoBehaviour
{
    private static readonly List<GrassOverlayLayer> AllGrass = new ();
    
    public static void Register(GrassOverlayLayer grass)
    {
        AllGrass.Add(grass);
    }
    public static void UpdateNearbyGrass(Vector2 playerPos, float radius, float radiusGrass)
    {
        GrassOverlayLayer closestGrass = null;
        var closestDistance = float.MaxValue;
        
        foreach (var grass in AllGrass.Where(grass => Vector2.Distance(playerPos, grass.transform.position) <= radiusGrass))
        {
            if (Vector2.Distance(playerPos, grass.transform.position) <= radius)
            {
                var yDist = Mathf.Abs(playerPos.y - grass.transform.position.y);
                if (yDist < closestDistance)
                {
                    closestDistance = yDist;
                    closestGrass = grass;
                }
            }
            grass.UpdateVisibility(playerPos.y);

            if (closestGrass) 
                closestGrass.PlayParticles();
        }
    }
}