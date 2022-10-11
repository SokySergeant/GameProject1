using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpScript : MonoBehaviour
{
    public float currentEnergy;
    private float maxEnergy = 200f;
    private float energyRate = 10f;
    private float energyMultiplier = -1;


    public Slider energyBar;


    void Start()
    {
        currentEnergy = maxEnergy;
    }



    void FixedUpdate()
    {
        currentEnergy += energyRate * energyMultiplier * Time.fixedDeltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy); //clamp current energy value so it doesn't go below 0 or above max
        energyBar.value = currentEnergy / maxEnergy;
    }


    
    private void DepleteEnergy(){
        energyMultiplier = -1;
    }

    private void IncreaseEnergy(){
        energyMultiplier = 1;
    }



    void OnTriggerStay(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Light")){ //if inside light
            Vector3 lightSourcePos = other.transform.parent.transform.position; //get position of light source
            Vector3 dir = transform.position - lightSourcePos; //get vector from lightsource to player

            RaycastHit hit;
            if(Physics.Raycast(lightSourcePos, dir, out hit)){ //shoot a ray towards the player 
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Player")){ //check if nothing is obstructing the space between the lightsource and player, meaning nothing is creating shade
                    IncreaseEnergy();
                }else{
                    DepleteEnergy();
                }
            }
            

        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Light")){ //go back to depleting the energy when you exit a light
            DepleteEnergy();
        }
    }

    
}
