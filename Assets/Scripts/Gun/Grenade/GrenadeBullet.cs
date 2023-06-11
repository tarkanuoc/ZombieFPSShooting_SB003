using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    public float explosionRadius;
    public float explosionForce;
    public int damage;
    private List<Health> oldVictims = new List<Health>();

    private void OnCollisionEnter(Collision collision)
    {
        oldVictims.Clear();
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
            DeliverDamage(affectedObjects[i]);
            AddForceToObject(affectedObjects[i]);
        }

    }

    private void DeliverDamage(Collider victim) 
    {
        Health health = victim.GetComponentInParent<Health>();
        if (health != null && !oldVictims.Contains(health)) 
        {
            Debug.Log("======== Grenade DeliverDamage " + victim.name);
            health.TakeDamage(damage);
            oldVictims.Add(health);
        }
    }

    private void AddForceToObject(Collider affectedObject)
    {
        Rigidbody rigidbody = affectedObject.attachedRigidbody;

        if (rigidbody != null)
        {
            rigidbody.AddExplosionForce(explosionForce, transform.position,
                explosionRadius, 1, ForceMode.Impulse);
        }
    }
}
