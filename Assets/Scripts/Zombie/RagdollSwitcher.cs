using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RagdollSwitcher : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody[] rigids;
    [ContextMenu("Retrieve Rigidbodies")]
    private void RetrieveRigibodies()
    {
        rigids = GetComponentsInChildren<Rigidbody>();
    }

    [ContextMenu("Clear Ragdoll")]
    private void ClearRagdoll()
    {
        CharacterJoint[] joins = GetComponentsInChildren<CharacterJoint>();
        for (int i = 0; i < joins.Length; i++)
        {
            DestroyImmediate(joins[i]);
        }

        Rigidbody[] rigidList = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigidList.Length; i++)
        {
            DestroyImmediate(rigidList[i]);
        }

        Collider[] colls = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colls.Length; i++)
        {
            DestroyImmediate(colls[i]);
        }

    }

    [ContextMenu("Enable Ragdoll")]
    public void EnableRagdoll()
    {
        SetRagdoll(true);
    }

    [ContextMenu("Disable Ragdoll")]
    public void DisableRagdoll()
    {
        SetRagdoll(false);
    }

    private void SetRagdoll(bool ragdollEnable)
    {
        anim.enabled = !ragdollEnable;
        for (int i = 0; i < rigids.Length; i++)
        {
            rigids[i].isKinematic = !ragdollEnable;
        }
    }

    [ContextMenu("Add Hit Surface")]
    private void AddHitSurface()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (gameObject.GetComponent<HitSurface>() == null)
            { 
                var hitSurface = colliders[i].AddComponent<HitSurface>();
                hitSurface.surfaceType = HitSurfaceType.Blood;
            }
        }
    }
}
