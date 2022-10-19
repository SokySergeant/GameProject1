using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 15f;
    public float flyingSpeed = 10f;
    public float fallingSpeed = 1f;
    private float currentFallingSpeed;
    public float fallingSpeedOnDownHold = 2f;
    public Vector2 horizontalInput;

    [HideInInspector] public CharacterController controller;

    Vector3 velocity;
    Vector3 moveVector;

    [HideInInspector] public bool flying;

    private float currentEnergy;
    private float maxEnergy = 200f;
    public float energyFlyingUsage = 1f;
    public float energyDepletionRate = 20f;
    public Slider energyBar;

    private FMOD.Studio.EventInstance solarEngine;
    public FMOD.Studio.EventInstance hoverEngine;
    private FMOD.Studio.EventInstance windHorizontalSound;
    private FMOD.Studio.EventInstance chargingSound;
    private FMOD.Studio.EventInstance fallingSound;

    private Animator animator;

    private float tempRpm;
    public float engineRampupSpeed = 0.5f;

    private float tempHorizontalRpm;
    public float engineHorizontalRampupSpeed = 0.5f;

    private float tempWindHorizontal = 0.5f;
    public float windHorizontalRampupSpeed = 0.5f;

    private float tempFallingSoundVol = 0f;
    public float fallingSoundVolRampupSpeed = 0.5f;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        currentEnergy = maxEnergy;
        currentFallingSpeed = fallingSpeed;

        hoverEngine = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/EngineState");
        hoverEngine.start();

        solarEngine = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/SolarPowerFly");

        windHorizontalSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Wind/WindMove");
        windHorizontalSound.setParameterByName("WindLR", 0.5f);

        chargingSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/EngineCharging");

        fallingSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Wind/WindFlying");
        fallingSound.start();
        fallingSound.setParameterByName("WindFlyVol", 0f);

    }

    private void FixedUpdate()
    {
        Vector3 gravity = Physics.gravity;
        Vector3 gravityDir = gravity.normalized;

        //flying
        if(flying && currentEnergy > 0f){
            velocity = new Vector3(velocity.x, flyingSpeed, velocity.z);
            currentEnergy -= energyFlyingUsage * Time.fixedDeltaTime; //lose energy whenever you fly upwards

            //engine sound ramping
            tempRpm += engineRampupSpeed * Time.fixedDeltaTime;
            tempRpm = Mathf.Clamp(tempRpm, 0f, 1f);
            solarEngine.setParameterByName("RPM SP", tempRpm);

            //tempFallingSoundVol += fallingSoundVolRampupSpeed * Time.fixedDeltaTime;
        }
        else{
            velocity = gravity * currentFallingSpeed; //constant gravity to act like gliding
            solarEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if (!controller.isGrounded){
                //tempFallingSoundVol -= fallingSoundVolRampupSpeed * Time.fixedDeltaTime;
            }else{
                //tempFallingSoundVol += fallingSoundVolRampupSpeed * Time.fixedDeltaTime;
            }
                
        }
        //tempFallingSoundVol = Mathf.Clamp(tempFallingSoundVol, 0f, 1f);
        //fallingSound.setParameterByName("WindFlyVol", tempFallingSoundVol);
        float temp = 0f;
        fallingSound.getParameterByName("WindFlyVol", out temp);
        Debug.Log(temp);

        moveVector = new Vector3(horizontalInput.x, 0f, 0f);

        energyBar.value = currentEnergy / maxEnergy;


        if(horizontalInput.x != 0f){
            tempHorizontalRpm += engineHorizontalRampupSpeed * Time.fixedDeltaTime;
        }
        else{
            tempHorizontalRpm -= engineHorizontalRampupSpeed * Time.fixedDeltaTime;
        }
        //horizontal sound
        tempHorizontalRpm = Mathf.Clamp(tempHorizontalRpm, 0.8f, 1f);
        hoverEngine.setParameterByName("RPM", tempHorizontalRpm);

        if (horizontalInput.x > 0f) {
            tempWindHorizontal += windHorizontalRampupSpeed * Time.fixedDeltaTime;
        } else if (horizontalInput.x < 0f) {
            tempWindHorizontal -= windHorizontalRampupSpeed * Time.fixedDeltaTime;
        } else{
            tempWindHorizontal = Mathf.Lerp(tempWindHorizontal, 0.5f, windHorizontalRampupSpeed * 10f * Time.fixedDeltaTime);
        }
        tempWindHorizontal = Mathf.Clamp(tempWindHorizontal, 0f, 1f);
        windHorizontalSound.setParameterByName("WindLR", tempWindHorizontal);

        //constantly deleting energy 
        currentEnergy -= energyDepletionRate * Time.fixedDeltaTime;
        if(currentEnergy <= 0f){
            currentEnergy = 0f;
        }

        //animations parameters
        animator.SetBool("Flying", flying);
        animator.SetFloat("Horizontal", horizontalInput.x);
        animator.SetBool("Grounded", controller.isGrounded);
    }

    private void Update()
    {
        controller.Move((velocity + moveVector * playerSpeed) * Time.deltaTime); //this is here as opposed to in FixedUpdate for smoother movement
    }



    //get horizontal input
    public void OnMove(InputAction.CallbackContext input){
        horizontalInput = input.ReadValue<Vector2>();

        if (input.started){
            windHorizontalSound.start();
            tempWindHorizontal = 0.5f;
            windHorizontalSound.setParameterByName("WindLR", 0.5f);
        }
    }



    public void OnJump(InputAction.CallbackContext input){
        if(input.started || input.canceled){
            flying = !flying;
        }

        if(input.started && currentEnergy > 0f){
            solarEngine.start();
            tempRpm = 0.2f;
        }else if(input.canceled && currentEnergy > 0f){
            solarEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }



    public void OnFall(InputAction.CallbackContext input){
        if(input.started){
            currentFallingSpeed = fallingSpeedOnDownHold;
        }else if(input.canceled){
            currentFallingSpeed = fallingSpeed;
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
                    currentEnergy += energyFlyingUsage * Time.fixedDeltaTime;
                    currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy); //clamp currentEnergy so it doesn't go above max

                    //charging sound
                    FMOD.Studio.PLAYBACK_STATE state;
                    chargingSound.getPlaybackState(out state);
                    if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING){
                        chargingSound.start();
                    }
                }
            }else{
                chargingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            

        }
    }


    private void OnTriggerExit(Collider other){
        if (other.gameObject.layer == LayerMask.NameToLayer("Light")){
            chargingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

}
