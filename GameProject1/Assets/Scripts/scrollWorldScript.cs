using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollWorldScript : MonoBehaviour
{
    public float scrollSpeed;
    public GameObject[] sections;
    private float offset;

    private GameObject currentSection = null;
    private GameObject newSection = null;



    void Start()
    {
        //get length of a section
        offset = sections[0].transform.localScale.x;

        //whenever the player enters a new section, spawn another section ahead of it and delete the previous one
        newSectionTriggerScript.onNewSectionTriggerEnter += SpawnSection;
        SpawnSection();
    }



    void Update()
    {
        //move sections
        currentSection.transform.position = new Vector3(currentSection.transform.position.x + scrollSpeed * Time.deltaTime, currentSection.transform.position.y, currentSection.transform.position.z);
        newSection.transform.position = new Vector3(newSection.transform.position.x + scrollSpeed * Time.deltaTime, newSection.transform.position.y, newSection.transform.position.z);
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
        newSection = Instantiate(sections[Random.Range(0, sections.Length)], new Vector3(currentSection.transform.position.x - offset, 0f, 0f), Quaternion.identity);
    
    }
}
