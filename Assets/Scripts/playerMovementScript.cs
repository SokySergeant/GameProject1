using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float playerSpeed = 5f;
    public float jumpPower = 10f;
    private Vector2 horizontalInput;

    private CharacterController controller;

    Vector3 velocity;
    private bool jumping = false;



    void Start()
    {
        controller = GetComponent<CharacterController>();
    }



    void FixedUpdate()
    {
        Vector3 gravity = Physics.gravity;
        Vector3 gravityDir = gravity.normalized;

        if (controller.isGrounded && !jumping){
            velocity = Vector3.ProjectOnPlane(velocity, gravityDir); // The sign of the normal doesn't matter.
        }

        velocity += gravity * Time.fixedDeltaTime;

        Vector3 moveVector = new Vector3(horizontalInput.x * playerSpeed * Time.fixedDeltaTime, 0f, 0f);
        controller.Move(velocity * Time.fixedDeltaTime + moveVector);
    }



    //get horizontal input
    public void OnMove(InputValue input){
        horizontalInput = input.Get<Vector2>();
    }



    public void OnJump(InputValue input){
        if(controller.isGrounded){
            StartCoroutine(Jump());
        }
    }

    private IEnumerator Jump(){
        jumping = true;
        velocity += Vector3.up * jumpPower;
        yield return new WaitForFixedUpdate(); //Waiting for fixed update so that it doesn't reset velocity since the player is still touching ground.
        jumping = false;
    }


}
