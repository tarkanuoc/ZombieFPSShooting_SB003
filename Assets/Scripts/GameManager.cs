using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool IsGameReady;

    public void OnGameOver()
    { 
        Time.timeScale = 0f;
    }

    public void InitGame()
    { 
        
    }
}
