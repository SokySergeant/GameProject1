using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HpScript : MonoBehaviour
{
    private int currentHp;
    private int maxHp = 3;

    public delegate void OnHit();
    public static event OnHit onHit;

    public delegate void OnDeath();
    public static event OnDeath onDeath;

    public TextMeshProUGUI hpText;



    void Awake()
    {
        currentHp = maxHp;
        hpText.text = "HP: " + currentHp;
    }



    //collision with an obstacle
    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Obstacle")){

            //subtract hp
            ChangeHp(-1);

            //destroy obstacle so player can continue ahead after getting hit
            other.gameObject.GetComponent<Obstacle>().BlowUp();
            hpText.text = "HP: " + currentHp;
            onHit?.Invoke();
        }
    }



    public void ChangeHp(int hp){
        currentHp += hp;

        if(currentHp <= 0){ //if hp is below or equal to 0, the player died
            hpText.text = "HP: X";
            onDeath?.Invoke();
        }
    }
    


    //
    void OnDisable()
    {
        onHit = null;
        onDeath = null;
    }

    
}
