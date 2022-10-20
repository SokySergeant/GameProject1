using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 15f;
    public float fallingSpeed = 1f;
    public float ascendSpeed = 50f;
    public float ascendMaxSpeed = 10f;
    private float currentFallingSpeed = 1f;
    public float fallingSpeedOnDownHold = 2f;
    public Vector2 horizontalInput;

    [HideInInspector] public CharacterController controller;

    [HideInInspector] public Vector3 velocity;
    private Vector3 moveVector;

    [HideInInspector] public bool flying;
    private bool falling = false;

    [HideInInspector] public float currentEnergy;
    private float maxEnergy = 200f;
    public float energyFlyingUsage = 1f;
    public float energyDepletionRate = 20f;
    public Slider energyBar;
    private float isDepletingMultiplier = -1f;

    private FMOD.Studio.EventInstance verticalEngineSound;
    private float tempVerticalEngineRpm = 0.2f;
    private float targetVerticalEngineRpm;
    public float verticalEngineSoundRampupSpeed = 0.5f;

    public FMOD.Studio.EventInstance horizontalEngineSound;
    private float tempHorizontalEngineRpm = 0.8f;
    private float targetHorizontalEngineRpm;
    public float horizontalEngineSoundRampupSpeed = 0.5f;

    private FMOD.Studio.EventInstance windHorizontalSound;
    private float tempWindHorizontal = 0.5f;
    private float targetWindHorizontal;
    public float windHorizontalRampupSpeed = 0.5f;

    private FMOD.Studio.EventInstance chargingSound;

    private FMOD.Studio.EventInstance fallingSound;
    private float tempFallingSoundVol = 0f;
    private float targetFallingSoundVol;
    public float fallingSoundVolRampupSpeed = 0.5f;

    private Animator animator;
    public Material boardMat;

    

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        currentEnergy = maxEnergy;
        currentFallingSpeed = fallingSpeed;

        //engine left right sound
        horizontalEngineSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/EngineState");
        horizontalEngineSound.start();

        //flying sound
        verticalEngineSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/SolarPowerFly");

        //wind left right sound
        windHorizontalSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Wind/WindMove");
        windHorizontalSound.start();
        windHorizontalSound.setParameterByName("WindLR", 0.5f);

        //light charging sound
        chargingSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/EngineCharging");

        //falling sound
        fallingSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Wind/WindFlying");
        fallingSound.start();
        fallingSound.setParameterByName("WindFlyVol", 0f);
    }

    private void Update()
    {
        // Physics and misc
        Vector3 gravity = Physics.gravity;
        Vector3 gravityDir = gravity.normalized;
        
        currentEnergy -= energyDepletionRate * Time.deltaTime * isDepletingMultiplier;
        if (flying && currentEnergy > 0f)
        {
            currentEnergy -= energyFlyingUsage * Time.deltaTime; //lose energy whenever you fly upwards
            
            velocity += -gravityDir * (ascendSpeed * Time.deltaTime);
            velocity = Vector3.ClampMagnitude(velocity, ascendMaxSpeed);
            
            targetVerticalEngineRpm = 1f;
        }
        else
        {
            velocity += gravity * Time.deltaTime * currentFallingSpeed;

            targetFallingSoundVol = controller.isGrounded ? 0f : 1f;
        }

        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        
        moveVector = new Vector3(horizontalInput.x, 0f, 0f);
        
        controller.Move((velocity + moveVector * playerSpeed) * Time.deltaTime);
        
        // Visuals
        energyBar.value = currentEnergy / maxEnergy;
        //Glowiness of board
        boardMat.SetColor("_EmissionColor", new Color(0f, 13f, 191f) * (currentEnergy / maxEnergy));
        
        animator.SetBool("Flying", flying);
        animator.SetFloat("Horizontal", horizontalInput.x);
        animator.SetBool("Grounded", controller.isGrounded);
        animator.SetBool("Falling", falling);
        
        // Audio
        if (horizontalInput.x > 0f)
        {
            targetHorizontalEngineRpm = 1f;
            targetWindHorizontal = 1f;
        }
        else if (horizontalInput.x < 0f)
        {
            targetHorizontalEngineRpm = 1f;
            targetWindHorizontal = 0f;
        }
        else
        {
            targetHorizontalEngineRpm = 0.8f;
            targetWindHorizontal = 0.5f;
            tempWindHorizontal = Mathf.Lerp(tempWindHorizontal, targetWindHorizontal, windHorizontalRampupSpeed * 10f * Time.deltaTime); //lerp here as well so the return to 0.5f is 10 times faster
        }
        
        tempVerticalEngineRpm = Mathf.Lerp(tempVerticalEngineRpm, targetVerticalEngineRpm, verticalEngineSoundRampupSpeed * Time.deltaTime);
        verticalEngineSound.setParameterByName("RPM SP", tempVerticalEngineRpm);

        tempHorizontalEngineRpm = Mathf.Lerp(tempHorizontalEngineRpm, targetHorizontalEngineRpm, horizontalEngineSoundRampupSpeed * Time.deltaTime);
        horizontalEngineSound.setParameterByName("RPM", tempHorizontalEngineRpm);

        tempWindHorizontal = Mathf.Lerp(tempWindHorizontal, targetWindHorizontal, windHorizontalRampupSpeed * Time.deltaTime);
        windHorizontalSound.setParameterByName("WindLR", tempWindHorizontal);

        tempFallingSoundVol = Mathf.Lerp(tempFallingSoundVol, targetFallingSoundVol, fallingSoundVolRampupSpeed * Time.deltaTime);
        fallingSound.setParameterByName("WindFlyVol", tempFallingSoundVol);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Vector3.Dot(velocity, hit.normal) < 0)
        {
            Vector3 flatVelocity = Vector3.ProjectOnPlane(velocity, hit.normal);
            velocity = flatVelocity;
        }
    }

    //get horizontal input
    public void OnMove(InputAction.CallbackContext input){
        horizontalInput = input.ReadValue<Vector2>();

        if (input.started){
            tempWindHorizontal = 0.5f;
            windHorizontalSound.setParameterByName("WindLR", 0.5f);
        }
    }



    public void OnJump(InputAction.CallbackContext input){
        if(input.started || input.canceled){
            flying = !flying;
        }

        if(input.started && currentEnergy > 0f){
            verticalEngineSound.start();
            tempVerticalEngineRpm = 0.2f;
        }else if(input.canceled){
            verticalEngineSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }



    public void OnFall(InputAction.CallbackContext input){
        if(input.started){
            currentFallingSpeed = fallingSpeedOnDownHold;
            falling = true;
        }else if(input.canceled){
            currentFallingSpeed = fallingSpeed;
            falling = false;
        }
    }



    //gain energy in light
    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Light")){ //if inside light

            /*
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
            */

            isDepletingMultiplier = -1f;
            

        }else if(other.gameObject.layer == LayerMask.NameToLayer("Dark")){
            isDepletingMultiplier = 1f;
        }
    }


    private void OnTriggerExit(Collider other){
        if (other.gameObject.layer == LayerMask.NameToLayer("Light")){
            chargingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

}
