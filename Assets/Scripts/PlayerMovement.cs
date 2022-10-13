using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 5f;
    public float jumpPower = 10f;
    private Vector2 horizontalInput;

    private CharacterController controller;

    Vector3 velocity;
    Vector3 moveVector;

    [HideInInspector] public bool flying;

    private float currentEnergy;
    private float maxEnergy = 200f;
    public float energyUsage = 1f;
    public Slider energyBar;

    private FMOD.Studio.EventInstance HoverEngine;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentEnergy = maxEnergy;

        //Audio
        HoverEngine = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/EngineState");
        HoverEngine.start();
        HoverEngine.setParameterByName("RPM", 0.8f);
    }

    private void FixedUpdate()
    {
        Vector3 gravity = Physics.gravity;
        Vector3 gravityDir = gravity.normalized;

        if(flying && currentEnergy > 0f){
            velocity = new Vector3(velocity.x, jumpPower, velocity.z);
            currentEnergy -= energyUsage * Time.fixedDeltaTime; //lose energy whenever you fly upwards
        }else{
            velocity = gravity; //constant gravity to act like gliding
        }

        moveVector = new Vector3(horizontalInput.x * playerSpeed * Time.fixedDeltaTime, 0f, 0f);

        energyBar.value = currentEnergy / maxEnergy;

        if(horizontalInput.x != 0f){
            HoverEngine.setParameterByName("RPM", 1f);
        }else{
            HoverEngine.setParameterByName("RPM", 0.8f);
        }
    }

    private void Update()
    {
        controller.Move(velocity * Time.deltaTime + moveVector); //this is here for smoother movement
    }



    //get horizontal input
    public void OnMove(InputAction.CallbackContext input){
        horizontalInput = input.ReadValue<Vector2>();
    }



    public void OnJump(InputAction.CallbackContext input){
        if(input.started || input.canceled){
            flying = !flying;
        }
    }



    //gain energy in light
    void OnTriggerStay(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Light")){ //if inside light
            Vector3 lightSourcePos = other.transform.parent.transform.position; //get position of light source
            Vector3 dir = transform.position - lightSourcePos; //get vector from lightsource to player

            RaycastHit hit;
            if(Physics.Raycast(lightSourcePos, dir, out hit)){ //shoot a ray towards the player
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Player")){ //check if nothing is obstructing the space between the lightsource and player, meaning nothing is creating shade
                    currentEnergy += energyUsage * Time.fixedDeltaTime;
                    currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy); //clamp currentEnergy so it doesn't go above max
                }
            }


        }
    }

}
