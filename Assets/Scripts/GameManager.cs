using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float scrollSpeed;
    private float scrollSpeedMultiplier = 1f;
    public float accelerationOverTime = 1f;
    public float fallingSpeedMultiplier = 1.4f;
    public float scrollSpeedMultiplierOnHit = 0.6f;

    public GameObject[] sections;
    private float offset;

    private GameObject currentSection = null;
    private GameObject newSection = null;

    public PlayerMovement player;

    private bool canScroll = true;



    void Awake()
    {
        //get length of a section
        offset = sections[0].transform.localScale.z;

        //whenever the player enters a new section, spawn another section ahead of it and delete the previous one
        NewSectionTriggerScript.onNewSectionTriggerEnter += SpawnSection;
        HpScript.onHit += OnHit;
        HpScript.onDeath += OnDeath;
        SpawnSection();
    }



    void Update()
    {
        scrollSpeed += Time.deltaTime * accelerationOverTime; //increase scroll speed the longer the game is played

        if(player.flying){
            scrollSpeedMultiplier = 1f;
        }else{
            scrollSpeedMultiplier = fallingSpeedMultiplier;
        }


        //move sections
        if(canScroll){
            currentSection.transform.position -= new Vector3(0f, 0f, scrollSpeed * scrollSpeedMultiplier * Time.deltaTime);
            newSection.transform.position -= new Vector3(0f, 0f, scrollSpeed * scrollSpeedMultiplier * Time.deltaTime);
        }
    }



    private void SpawnSection(){

        //currentSection is null at the beginning of the game
        if(currentSection != null){
            Destroy(currentSection);
            currentSection = newSection;
        }else{
            currentSection = Instantiate(sections[Random.Range(0, sections.Length)], new Vector3(0f, 0f, 0f), Quaternion.identity);
        }
        
        //create new section ahead of the current one
        newSection = Instantiate(sections[Random.Range(0, sections.Length)], new Vector3(0f, 0f, currentSection.transform.position.z + offset), Quaternion.identity);
    
    }



    //decrease scroll speed whenever the player gets hit
    private void OnHit(){
        scrollSpeed *= scrollSpeedMultiplierOnHit;
    }

    //stop movement on death
    private void OnDeath(){
        canScroll = false;
        player.enabled = false;
    }
}