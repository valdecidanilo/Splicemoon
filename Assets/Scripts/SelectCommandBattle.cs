using System;
using Models;
using UnityEngine;

public class SelectCommandBattle : MonoBehaviour
{
    public RectTransform select;
    public Vector2[] pos;
    
    public int currentPosition = 0;
    private bool inputReleased = true;


    public void Move(float horizontal, float vertical, Action<int> currentID)
    {
        if (select == null || pos == null || pos.Length != 4)
        {
            Debug.LogError("Configuração incompleta! Verifique os campos no Inspector.");
            return;
        }
        
        if ((Mathf.Abs(horizontal) > 0.5f || Mathf.Abs(vertical) > 0.5f) && inputReleased)
        {
            inputReleased = false;
            
            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                if (horizontal > 0)
                {
                    if (currentPosition == 0) currentPosition = 1;
                    else if (currentPosition == 2) currentPosition = 3;
                }
                else
                {
                    if (currentPosition == 1) currentPosition = 0;
                    else if (currentPosition == 3) currentPosition = 2;
                }
            }
            else
            {
                if (vertical > 0)
                {
                    if (currentPosition == 2) currentPosition = 0;
                    else if (currentPosition == 3) currentPosition = 1;
                }
                else
                {
                    if (currentPosition == 0) currentPosition = 2;
                    else if (currentPosition == 1) currentPosition = 3;
                }
            }
            
            select.anchoredPosition = pos[currentPosition];
        }
        else if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            inputReleased = true;
        }
        currentID?.Invoke(currentPosition);
    }
}
