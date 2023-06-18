using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Health playerHealth;
    public int damage;

    public void StartAttack()
    {
        anim.SetBool("IsAttacking", true);
    }

    public void StopAttack()
    {
        anim.SetBool("IsAttacking", false);
    }

    public void OnAttack()
    {
        playerHealth.TakeDamage(damage);
    }
}
