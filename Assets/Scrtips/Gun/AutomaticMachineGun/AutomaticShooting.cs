using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticShooting : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource shootSound;
    public int rpm;

    private float lastShot;
    private float interval;

    private void Start()
    {
        interval = 60f / (float)rpm;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UpdateFiring();
        }
    }

    private void UpdateFiring()
    {
        // Time.time = 17h08 1s 000ms -  17h08 0s 000ms >= 100ms
        if (Time.time - lastShot >= interval)
        {
            Shoot();
            lastShot = Time.time;
            //lastshot = 17h08 1s 000ms
        }
    }

    private void Shoot()
    {
        anim.Play("Shoot");
        shootSound.Play();
    }
}
