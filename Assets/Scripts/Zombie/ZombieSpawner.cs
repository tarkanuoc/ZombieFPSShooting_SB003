using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private SphereCollider sphereCollider;
    public float spawnInterval;
    public float radius;
    private bool isActiveSpawner;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = new Color(1, 0, 0, 0.1f);
        Handles.DrawSolidDisc(transform.position, Vector3.up, radius);

    }
#endif
    private void Start()
    {
        sphereCollider.radius = radius;
        isActiveSpawner = false;
        InvokeRepeating("SpawnZombie", 1f, spawnInterval);
    }

    void SpawnZombie()
    {
        if (!isActiveSpawner) 
        {
            return;
        }
        Instantiate(zombiePrefab, transform.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActiveSpawner = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActiveSpawner = false;
        }
    }

}
