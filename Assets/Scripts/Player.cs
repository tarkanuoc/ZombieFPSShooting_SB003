using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private GunSwitcher gunSwitcher;

    public PlayerUI PlayerUI { get => playerUI;}

    public void InitPlayer()
    {
        gunSwitcher.SetActiveGun(0);
    }
}
