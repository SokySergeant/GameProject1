using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HpScript : MonoBehaviour
{
    private int currentHp;
    public int maxHp = 15;

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

    public GameObject hpParent;
    public float hpRadius = 1f;
    private GameObject[] hpOrbs;
    public GameObject orbPrefab;



    void Awake()
    {
        currentHp = maxHp;
        hpText.text = "HP: " + currentHp;

        damagedSound = FMODUnity.RuntimeManager.CreateInstance("event:/Hoverboard/Engine/EngineDamaged");

        music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BackgroundMusic");
        music.start();
        music.setParameterByName("HealthST1", 0f);
        music.setParameterByName("HealthST2", 0f);

        //create health orbs
        hpOrbs = new GameObject[currentHp];
        UpdateHealthOrbs();
    }



    private void Update()
    {
        //get new sound volumes
        tempST1Vol = Mathf.Lerp(tempST1Vol, ST1TargetVol, musicSwitchSpeed * Time.deltaTime);
        tempST2Vol = Mathf.Lerp(tempST2Vol, ST2TargetVol, musicSwitchSpeed * Time.deltaTime);

        music.setParameterByName("HealthST1", tempST1Vol);
        music.setParameterByName("HealthST2", tempST2Vol);

        //make orbs face player
        for (int i = 0; i < hpOrbs.Length; i++){
            hpOrbs[i].transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f));
        }
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

            float tempRatio = (float)currentHp / (float)maxHp;

            if(tempRatio < 0.3f){
                damagedSound.start();
                ST1TargetVol = 1f;
                ST2TargetVol = 1f;
            }else if(tempRatio < 0.5f){
                ST1TargetVol = 1f;
                ST2TargetVol = 0f;
            }else{
                damagedSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                ST1TargetVol = 0f;
                ST2TargetVol = 0f;
            }

            if (currentHp <= 0){ //if hp is below or equal to 0, the player died
                hpText.text = "HP: X";

                music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                onDeath?.Invoke();

            }else if (hp < 0){ //if the given hp is below 0, the player is taking damage
                onHit?.Invoke();
                invulnerable = true;
                yield return new WaitForSeconds(invulnerabilityTime);
                invulnerable = false;
            }else{
                yield return null;
            }

        }
    }



    private void UpdateHealthOrbs(){
        //reset orbs
        for (int i = 0; i < hpOrbs.Length; i++){
            if(hpOrbs[i] != null){
                Destroy(hpOrbs[i]);
            }
        }

        hpOrbs = new GameObject[currentHp];

        //create orbs
        for (int i = 0; i < currentHp; i++){

            //get position in a circle, take into account how many orbs are supposed to be spawned
            float radians = 2 * Mathf.PI / currentHp * i;
            float vertical = Mathf.Sin(radians);
            float horizontal = Mathf.Cos(radians);
            Vector3 spawnDir = new Vector3(horizontal, 0f, vertical);
            Vector3 spawnPos = hpParent.transform.position + spawnDir * hpRadius;

            //create orb
            GameObject tempOrb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
            tempOrb.transform.parent = hpParent.transform;
            tempOrb.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            hpOrbs[i] = tempOrb;
        }
    }



    //empty events for next playthrough
    void OnDisable(){
        onHit = null;
        onDeath = null;
    }


}
