using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;
   
    public int damage;

    public void StartAttack()
    {
        anim.SetBool("IsAttacking", true);
    }

    public void StopAttack()
    {
        anim.SetBool("IsAttacking", false);
    }

    public void OnAttack(int index)
    {
        var playerHealth = Player.Instance.PlayerHealth;

        if (playerHealth == null) 
        {
            return;
        }

        playerHealth.TakeDamage(damage);

        if (index == 1)
        {
            Debug.Log("====== show right scratch");
            Player.Instance.PlayerUI.ShowRightScratch();
        }
        else
        {
            Debug.Log("====== show left scratch");
            Player.Instance.PlayerUI.ShowLeftScratch();
        }

    }
}
