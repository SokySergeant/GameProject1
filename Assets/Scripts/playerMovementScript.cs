using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovementScript : MonoBehaviour
{
    public float playerSpeed = 5f;
    private Vector2 horizontalInput;

    private CharacterController controller;

    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }



    void FixedUpdate()
    {
        Vector3 gravity = Physics.gravity;
        Vector3 gravityDir = gravity.normalized;

        if (controller.isGrounded)
            velocity = Vector3.ProjectOnPlane(velocity, gravityDir); // The sign of the normal doesn't matter.

        velocity += gravity * Time.fixedDeltaTime;

        Vector3 moveVector = new Vector3(horizontalInput.x * playerSpeed * Time.fixedDeltaTime, 0f, 0f);
        controller.Move(velocity * Time.fixedDeltaTime + moveVector);
    }



    //get horizontal input
    public void OnMove(InputValue input){
        horizontalInput = input.Get<Vector2>();
    }
}
