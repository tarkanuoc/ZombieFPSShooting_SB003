using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AutomaticShooting : Shooting
{
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private GameObject hitMarkerPrefab;
    [SerializeField] private Camera aimingCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] UnityEvent onShoot;
    
    public int rpm;
    public int damage;

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
        Debug.DrawLine(aimingCamera.transform.position, aimingCamera.transform.forward);
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
        PerformRaycasting();
        onShoot?.Invoke();
        
    }

    private void PerformRaycasting()
    {
        Ray aimingRay = new Ray(aimingCamera.transform.position, aimingCamera.transform.forward);
        if (Physics.Raycast(aimingRay, out RaycastHit hitInfo, 1000f, layerMask)) 
        {
            Quaternion effectRotation = Quaternion.LookRotation(hitInfo.normal);
            Instantiate(hitMarkerPrefab, hitInfo.point, effectRotation);
            DeliverDamage(hitInfo);
        }
    }

    private void DeliverDamage(RaycastHit hitInfo)
    { 
        Health health = hitInfo.collider.GetComponentInParent<Health>();
        if (health != null) 
        {
            health.TakeDamage(damage);
        }
    }
}
