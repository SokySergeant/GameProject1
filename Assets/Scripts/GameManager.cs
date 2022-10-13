using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public float scrollSpeed;
    public float maxScrollSpeed = 200f;
    private float scrollSpeedMultiplier = 1f;
    public float accelerationOverTime = 1f;
    public float fallingSpeedMultiplier = 1.4f;
    public float scrollSpeedMultiplierOnHit = 0.6f;

    public PlayerMovement player;

    private bool canScroll = true;

    public GameObject exitBtn;

    private bool isPaused = false;

    [SerializeField] private SegmentManager _segmentManager;
    [SerializeField] private Segment _spawnSegment;
    

    void Awake()
    {
        _segmentManager.ExpandSegment(_spawnSegment);
        
        //whenever the player enters a new section, spawn another section ahead of it and delete the previous one
        HpScript.onHit += OnHit;
        HpScript.onDeath += OnDeath;

        //Reset time scale incase the user previously exited through the main menu
        Time.timeScale = 1f;
    }



    void Update()
    {
        if(scrollSpeed < maxScrollSpeed){
            scrollSpeed += Time.deltaTime * accelerationOverTime; //increase scroll speed the longer the game is played
        }

        if(player.flying){
            scrollSpeedMultiplier = 1f;
        }else{
            scrollSpeedMultiplier = fallingSpeedMultiplier;
        }


        //move sections
        if(canScroll)
        {
            Vector3 moveVector = new Vector3(0f, 0f, scrollSpeed * scrollSpeedMultiplier * Time.deltaTime);
            _segmentManager.MoveSegments(-moveVector);
        }
    }
    
    //decrease scroll speed whenever the player gets hit
    private void OnHit(){
        scrollSpeed *= scrollSpeedMultiplierOnHit;
    }

    //stop movement on death
    private void OnDeath(){
        canScroll = false;
        player.enabled = false;

        //show exit button
        exitBtn.SetActive(true);
    }
    
    //on death button functions 
    public void Exit(){
        SceneManager.LoadScene("MainMenu");
    }
    
    //pause menu
    public void OnPause(InputAction.CallbackContext input){
        if(isPaused){
            isPaused = false;
            player.enabled = true;
            exitBtn.SetActive(false);
            Time.timeScale = 1f;
        }else{
            isPaused = true;
            player.enabled = false;
            exitBtn.SetActive(true);
            Time.timeScale = 0f;
        }
        
    }
}
