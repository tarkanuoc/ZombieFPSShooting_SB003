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
        var playerHealth = GameHelper.Instance.Player.PlayerHealth;

        if (playerHealth == null) 
        {
            return;
        }

        playerHealth.TakeDamage(damage);

        if (index == 1)
        {
            Debug.Log("====== show right scratch");
            GameHelper.Instance.Player.PlayerUI.ShowRightScratch();
        }
        else
        {
            Debug.Log("====== show left scratch");
            GameHelper.Instance.Player.PlayerUI.ShowLeftScratch();
        }

    }
}
