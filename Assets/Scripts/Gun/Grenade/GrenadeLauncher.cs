using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : Shooting
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPos;
    [SerializeField] private AudioSource shootingSound;
    [SerializeField] private Animator anim;
    [SerializeField] private GunAmmo gunAmmo;


    public float bulletSpeed;
    private const int LeftMouseButton = 0;
    private void Update()
    {
        if (Input.GetMouseButtonDown(LeftMouseButton)) 
        {
            ShootBullet();
           // AddProjectile();
        }
    }

    private void ShootBullet()
    {
        anim.SetTrigger("Shoot");
        gunAmmo.SingleFireAmmoCounter();
    }

    public void PlayFireSound()
    { 
        shootingSound.Play();
    }

    public void AddProjectile()
    {
        Debug.Log("======== AddProjectile");
        GameObject bullet = Instantiate(bulletPrefab, firingPos.position, firingPos.rotation);
        bullet.GetComponent<Rigidbody>().velocity = -firingPos.forward * bulletSpeed;
    }
}
