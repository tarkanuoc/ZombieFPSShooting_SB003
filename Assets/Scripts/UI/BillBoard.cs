using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        LookTowardCamera();
    }

    private void LookTowardCamera()
    { 
        transform.forward = -mainCamera.transform.forward;
    }
}
