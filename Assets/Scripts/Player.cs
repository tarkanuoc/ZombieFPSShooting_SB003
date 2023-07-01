using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private GunSwitcher gunSwitcher;
    [SerializeField] private Transform playerFoot;
    [SerializeField] private Health playerHealth;

    public PlayerUI PlayerUI { get => playerUI;}
    public Transform PlayerFoot { get => playerFoot; }
    public Health PlayerHealth { get => playerHealth; }

    public void InitPlayer()
    {
        gunSwitcher.SetActiveGun(0);
    }
}
