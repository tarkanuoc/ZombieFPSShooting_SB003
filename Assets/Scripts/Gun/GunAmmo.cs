using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunAmmo : MonoBehaviour
{
    [SerializeField] private Shooting gun;
    [SerializeField] private Animator anim;
    [SerializeField] private int _loadedAmmo;
    [SerializeField] private AudioSource[] reloadSound;
    [SerializeField] private UnityEvent loadedAmmoChanged;
    public UnityEvent LoadedAmmoChanged { get => loadedAmmoChanged;}
      
    public int magSize;
        

    public int LoadedAmmo
    {
        get => _loadedAmmo;
        set
        {
            _loadedAmmo = value;

            loadedAmmoChanged?.Invoke();

            if (_loadedAmmo <= 0)
            {
                Reload();
            }
            else
            {
                UnlockShooting();
            }
        }
    }

    private void Start()
    {
        RefillAmmo();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            Reload();
        }
    }

    private void Reload()
    {
        anim.SetTrigger("Reload");
        LockShooting();
    }

    public void SingleFireAmmoCounter()
    {
        Debug.Log("========== tru dan");
        LoadedAmmo--;
    }

    private void LockShooting()
    {
        gun.enabled = false;
    }

    private void UnlockShooting()
    {
        gun.enabled = true;
    }

    private void RefillAmmo()
    {
        LoadedAmmo = magSize;
    }

    public void PlayReloadPart1Sound() 
    {
        reloadSound[0].Play();
    }
    public void PlayReloadPart2Sound() 
    {
        reloadSound[1].Play();
    }
    public void PlayReloadPart3Sound() 
    {
        reloadSound[2].Play();
    }
    public void PlayReloadPart4Sound()
    {
        reloadSound[3].Play();
        ReloadToIdle();
    }
    public void PlayReloadPart5Sound() 
    {
        reloadSound[4].Play();
      
    }

    public void ReloadToIdle()
    {
        RefillAmmo();
        Debug.Log("======== Reload Anim Finish");
    }

    public void OnGunSelected()
    {
        UpdateShootingLock();
    }

    private void UpdateShootingLock()
    {
        gun.enabled = _loadedAmmo > 0;
    }
}
