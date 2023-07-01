using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayUI : Singleton<GamePlayUI>
{
    [SerializeField] private GameObject popUpGameOver;
    [SerializeField] private GameObject popUpGameWin;

    public void OnGameOver()
    {
        if (popUpGameOver != null)
        {
            Time.timeScale = 0f;
            popUpGameOver.SetActive(true);
        }
    }

    public void OnGameWin()
    {
        if (popUpGameWin != null)
        {
            Time.timeScale = 1f;
            popUpGameWin.SetActive(true);
        }

    }
}
