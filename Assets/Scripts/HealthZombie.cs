using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthZombie : Health
{
    [SerializeField] private NavMeshAgent agent;
    public override void Die()
    {
        base.Die();
        GameHelper.Instance.MissionManager.OnZombieKilled();
        agent.isStopped = true;
        Destroy(gameObject, 3f);
    }
}
