using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
   private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        LookTowardCamera();
    }

    private void LookTowardCamera()
    { 
        transform.forward = -mainCamera.transform.forward;
    }
}
