using System;
using LenixSO.Logger;
using TMPro;
using UnityEngine;
using Logger = LenixSO.Logger.Logger;

public class BattleUIManager : MonoBehaviour
{
    public SelectCommandBattle userSelectInterface;
    public SelectCommandBattle userAttackInterface;
    public int currentIDInterface;

    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        if(currentIDInterface == 0) userSelectInterface.Move(horizontal, vertical, Selected);
        else userAttackInterface.Move(horizontal, vertical, Selected);
    }

    private void Selected(int id)
    {
        if (!Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return)) return;
        if(currentIDInterface == 0) HandleUserInterface(id);
        else HandlerAttackInterface(id);
        
    }

    private void HandleUserInterface(int id)
    {
        switch (id)
        {
            case 0:
                userAttackInterface.gameObject.SetActive(true);
                userSelectInterface.gameObject.SetActive(false);
                currentIDInterface = 1;
                break;
            case 1:
                Logger.Log("Apertou no BAG");
                break;
            case 2:
                Logger.Log("Apertou no POKEMON");
                break;
            case 3:
                Logger.Log("Apertou no RUN");
                break;
        }
    }

    private void HandlerAttackInterface(int id)
    {
        if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
        {
            Logger.Log("Back");
            userAttackInterface.gameObject.SetActive(false);
            userSelectInterface.gameObject.SetActive(true);
            currentIDInterface = 0;
            return;
        }
        switch (id)
        {
            case 0:
                Logger.Log("Apertou no 1");
                break;
            case 1:
                Logger.Log("Apertou no 2");
                break;
            case 2:
                Logger.Log("Apertou no 3");
                break;
            case 3:
                Logger.Log("Apertou no 4");
                break;
        }
    }
}
