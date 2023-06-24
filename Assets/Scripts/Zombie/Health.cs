using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private UnityEvent onDie;
    [SerializeField] private UnityEvent<int, int> onHealthChange;
    [SerializeField] private UnityEvent OnTakeDamage;
    public int maxHP;
    private int healthPoint;
    public int HealthPoint
    {
        get => healthPoint;
        set
        {
            healthPoint = value;
            onHealthChange.Invoke(healthPoint, maxHP);
        }
    }

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

        HealthPoint -= damage;
        OnTakeDamage.Invoke();

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
