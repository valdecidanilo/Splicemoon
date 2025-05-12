using System.Collections.Generic;
using System.Linq;
using Inventory;
using Models;
using UnityEngine;

public class PlayerBag : MonoBehaviour
{
    public SpliceMon currentSplicemon;
    public List<SpliceMon> splicemons = new ();
    public bool bagInitialized = false;
}
