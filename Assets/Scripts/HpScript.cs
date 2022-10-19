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

    private bool invulnerable = false;
    public float invulnerabilityTime = 1.5f;


    void Awake()
    {
        currentHp = maxHp;
        hpText.text = "HP: " + currentHp;
    }



    public void ChangeHp(int hp){
        StartCoroutine(ChangeHpRoutine(hp));
    }

    private IEnumerator ChangeHpRoutine(int hp){
        if(!invulnerable){
            currentHp += hp;

            //make sure hp doesn't go above max or below 0
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);

            //update hud
            hpText.text = "HP: " + currentHp;
            
            if (hp < 0){ //if the given hp is below 0, the player is taking damage
                onHit?.Invoke();
                invulnerable = true;
                yield return new WaitForSeconds(invulnerabilityTime);
                invulnerable = false;
            }else{
                yield return null;
            }

            if(currentHp <= 0){ //if hp is below or equal to 0, the player died
                hpText.text = "HP: X";
                onDeath?.Invoke();
            }
        }
    }
    


    //empty events for next playthrough
    void OnDisable(){
        onHit = null;
        onDeath = null;
    }

    
}
