using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private PlayerUI playerUI;

    public PlayerUI PlayerUI { get => playerUI;}
}
