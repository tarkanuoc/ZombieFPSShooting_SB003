using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByKey : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject flashlight;
    public float movingSpeed;
    public float speed = 6.0f;
    public float jumpSpeed = 20.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;


    private void Update()
    {
        if (!GameManager.Instance.IsGameReady)
        {
            return;
        }
        // characterController.SimpleMove(direction * movingSpeed);

        if (Input.GetKeyDown(KeyCode.F))
        { 
            flashlight.SetActive(!flashlight.activeSelf);
        }

        if (characterController.isGrounded)
        {
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");

            moveDirection = transform.right * hInput + transform.forward * vInput;
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

}
