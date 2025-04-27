using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "New Item Object", menuName = "Inventory System/Item/Item")]
    public class ItemObject : DefaultObject {
        public bool IAP;
        public void Awake(){
            type = ItemType.Item;
        }
    }
}