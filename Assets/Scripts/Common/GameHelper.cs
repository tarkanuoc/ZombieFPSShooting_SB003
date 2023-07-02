using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelper : Singleton<GameHelper> 
{
    [SerializeField] private HitEffectManager hitEffectManager;
    [SerializeField] private GamePlayUI gamePlayUI;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private Player player;

    public HitEffectManager HitEffectManager { get => hitEffectManager; }
    public GamePlayUI GamePlayUI { get => gamePlayUI;}
    public MissionManager MissionManager { get => missionManager; }
    public Player Player { get => player; }
}
