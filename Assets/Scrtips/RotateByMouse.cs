using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByMouse : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder;
    public float _anglePerSecondYaw;
    public float _anglePerSecondPitch;
    public float _minPitch;
    public float _maxPitch;

    private float _pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        UpdateYaw();
        UpdatePitch();
    }

    private void UpdateYaw()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float yaw = mouseX * _anglePerSecondYaw;
        transform.Rotate(0, yaw, 0);
    }

    private void UpdatePitch()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        float deltaPitch = -mouseY * _anglePerSecondPitch;
        _pitch = Mathf.Clamp(_pitch + deltaPitch, _minPitch, _maxPitch);
        cameraHolder.localEulerAngles = new Vector3(_pitch, 0, 0);
    }

}
