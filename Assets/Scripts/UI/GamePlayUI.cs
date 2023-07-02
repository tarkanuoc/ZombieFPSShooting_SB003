using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayUI : MonoBehaviour
{
    [SerializeField] private GameObject popUpGameOver;
    [SerializeField] private GameObject popUpGameWin;

    public void OnGameOver()
    {
        if (popUpGameOver != null)
        {
           
            popUpGameOver.SetActive(true);
        }
    }

    public void OnGameWin()
    {
        if (popUpGameWin != null)
        {
            popUpGameWin.SetActive(true);
        }

    }

    public void OnMissionCompleted()
    {
        OnGameWin();
        StopGame();
    }

    public void StopGame() 
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
