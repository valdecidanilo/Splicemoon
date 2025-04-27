using UnityEngine;

namespace Inventory
{
    public abstract class DefaultObject : ScriptableObject {
        [Header("Default Item")]
        GameObject prefab;
        public ItemType type;
        public Sprite icon;
        public string nameItem;
        [TextArea]public string description;
        public int amount, price;
    }
}