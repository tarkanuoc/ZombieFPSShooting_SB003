using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public void OnGameOver()
    { 
        Time.timeScale = 0f;
    }
}
