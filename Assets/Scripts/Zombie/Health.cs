using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private UnityEvent onDie;
    public int maxHP;
    private int healthPoint;

    // Start is called before the first frame update
    void Start()
    {
        healthPoint = maxHP;
    }
    private bool IsDead()
    {
        return healthPoint <= 0;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead())
        {
            return;
        }

        healthPoint -= damage;

        if (IsDead())
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log("============= Dieeeeeee");
        onDie?.Invoke();
    }
}
