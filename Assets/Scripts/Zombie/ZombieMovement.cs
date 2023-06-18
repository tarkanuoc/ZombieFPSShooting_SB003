using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using DG;
using DG.Tweening;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] private Transform playerFoot;
    [SerializeField] private Animator anim;
    [SerializeField] NavMeshAgent agent;
    public float reachingRadius;
    public float jumpHeight = 2f;
    private bool isJumping = false;
    void OnDrawGizmosSelected()
    {
        var color = Color.red;
        color.a = 0.2f;
        Gizmos.color = color;

        Gizmos.DrawSphere(transform.position, reachingRadius);
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, playerFoot.position);
        if (distance > reachingRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(playerFoot.position);
            anim.SetBool("IsWalking", true);
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
        }

        if (agent.isOnOffMeshLink && !isJumping)
        {
            // StartCoroutine(Jump(agent));
            JumpDoTween(agent);
            isJumping = true;
        }

    }

    IEnumerator Jump(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;
        float duration = 1.0f; 

        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = jumpHeight * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        agent.CompleteOffMeshLink();
        isJumping = false;
    }

    private void JumpDoTween(NavMeshAgent agent)
    {
        Debug.Log("=========== Jump DOTween");
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;
        float duration = 2.0f;

        agent.transform.DOJump(endPos, jumpHeight, 1, duration, true).OnComplete(() => {
            agent.CompleteOffMeshLink();
            isJumping = false;
        });
    }
}
