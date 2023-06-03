using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    public float explosionRadius;
    public float explosionForce;


    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        BlowObjects();
        Destroy(gameObject);
    }

    private void BlowObjects()
    {
        //List<Collider> affectedObjects = new List<Collider>();    
        Collider[] affectedObjects = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            Rigidbody rigidbody = affectedObjects[i].attachedRigidbody;

            if (rigidbody != null) 
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position,
                    explosionRadius, 1 , ForceMode.Impulse);
            }

        }

    }
}
