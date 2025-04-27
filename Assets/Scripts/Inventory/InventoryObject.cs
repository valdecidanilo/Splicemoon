using System.Collections.Generic;
using Inventory;
using UnityEngine;
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory", order = 0)]
public class InventoryObject : ScriptableObject{
    public List<InventorySlot> ListItens = new List<InventorySlot>();
    public void AddItem(DefaultObject _item, int _amount = 1){
        bool hasItem = false;
        for (int i = 0; i < ListItens.Count; i++){
            if(ListItens[i].item == _item){
                ListItens[i].AddAmount(_amount);
                hasItem = true;
                break;
            }
        }
        if(!hasItem){
            ListItens.Add(new InventorySlot(_item, _amount));
        }
        //string json = JsonUtility.ToJson(Inventory);
        //Debug.Log(json);
    }
    public void RemoveItem(DefaultObject _item, int _amount = 1){
        bool hasItem = false;
        int indexItem = -1;
        for (int i = 0; i < ListItens.Count; i++){
            if(ListItens[i].item == _item){
                indexItem = i;
                hasItem = ListItens[i].RemoveAmount(_amount);
            }
        }
        if(!hasItem){
            ListItens.RemoveAt(indexItem);
        }
    }
    public bool CheckItem(DefaultObject _item){
        bool hasItem = false;
        for (int i = 0; i < ListItens.Count; i++){
            if(ListItens[i].item == _item){
                hasItem = true;
                break;
            }
        }
        return hasItem;
    }
}
[System.Serializable]
public class InventorySlot{
    public DefaultObject item;
    public int amount;
    public InventorySlot(DefaultObject _item, int _amount = 1){
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value = 1){
        amount += value;
    }
    public bool RemoveAmount(int value = 1){
        bool hasItem = true;
        if(amount > 0){
            amount -= value;
        }
        if(amount <= 0){
            hasItem = false;
        }
        return hasItem;
    }
}