using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button join;
    private void Start()
    {
        AudioManager.Instance.Play("Lobby");
        join.onClick.AddListener(StartJoin);
    }

    private void Update()
    {

    }
    private static void StartJoin() => AudioManager.Instance.Play("ClickUI");
}
