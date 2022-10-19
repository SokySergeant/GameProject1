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

    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public HpScript playerHp;

    private bool canScroll = true;

    public GameObject exitBtn;

    private bool isPaused = false;

    [SerializeField] private SegmentManager _segmentManager;

    void Awake()
    {
        player = GameObject.Find("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerHp = player.GetComponent<HpScript>();

        _segmentManager.AppendSegment();
        
        //whenever the player enters a new section, spawn another section ahead of it and delete the previous one
        HpScript.onHit += OnHit;
        HpScript.onDeath += OnDeath;

        //Reset time scale incase the user previously exited through the pause menu
        Time.timeScale = 1f;
    }



    void Update()
    {
        if(scrollSpeed < maxScrollSpeed){
            scrollSpeed += Time.deltaTime * accelerationOverTime; //increase scroll speed the longer the game is played
        }

        if(playerMovement.flying){
            Debug.Log(playerMovement, playerMovement);
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
        playerMovement.enabled = false;

        //show exit button
        exitBtn.SetActive(true);
    }
    
    //on death button functions 
    public void Exit(){
        playerMovement.hoverEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SceneManager.LoadScene("MainMenu");
    }
    
    //pause menu
    public void OnPause(InputAction.CallbackContext input){
        if(isPaused){
            isPaused = false;
            playerMovement.enabled = true;
            exitBtn.SetActive(false);
            playerMovement.hoverEngine.start();
            Time.timeScale = 1f;
        }else{
            isPaused = true;
            playerMovement.enabled = false;
            exitBtn.SetActive(true);
            playerMovement.hoverEngine.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Time.timeScale = 0f;
        }
        
    }



}
