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

    private FMOD.Studio.EventInstance damagedSound;
    private FMOD.Studio.EventInstance music;

    private float tempST1Vol = 0f;
    private float tempST2Vol = 0f;
    private float ST1TargetVol = 0f;
    private float ST2TargetVol = 0f;

    public float musicSwitchSpeed = 5f;


    void Awake()
    {
        currentHp = maxHp;
        hpText.text = "HP: " + currentHp;

        damagedSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/EngineDamaged");
        music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BackgroundMusic");
        music.start();
        music.setParameterByName("HealthST1", 0f);
        music.setParameterByName("HealthST2", 0f);
    }



    private void Update()
    {
        tempST1Vol = Mathf.Lerp(tempST1Vol, ST1TargetVol, musicSwitchSpeed * Time.deltaTime);
        tempST2Vol = Mathf.Lerp(tempST2Vol, ST2TargetVol, musicSwitchSpeed * Time.deltaTime);

        music.setParameterByName("HealthST1", tempST1Vol);
        music.setParameterByName("HealthST2", tempST2Vol);
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

            if(currentHp == 2){
                ST1TargetVol = 1f;
                ST2TargetVol = 0f;
            }
            else if(currentHp == 1){
                damagedSound.start();
                ST1TargetVol = 1f;
                ST2TargetVol = 1f;
            }
            else{
                damagedSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                ST1TargetVol = 0f;
                ST2TargetVol = 0f;
            }

            if (currentHp <= 0){ //if hp is below or equal to 0, the player died
                hpText.text = "HP: X";

                music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                onDeath?.Invoke();
            }

            if (hp < 0){ //if the given hp is below 0, the player is taking damage
                onHit?.Invoke();
                invulnerable = true;
                yield return new WaitForSeconds(invulnerabilityTime);
                invulnerable = false;
            }else{
                yield return null;
            }

        }
    }
    


    //empty events for next playthrough
    void OnDisable(){
        onHit = null;
        onDeath = null;
    }

    
}
