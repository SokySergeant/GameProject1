using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDControllerScript : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text highScoreText;

    public float scoreMultiplier = 10f;

    public float distanceUnit = 1f;
    [HideInInspector] public float highScore = 0f;
    private float distanceTravelled;



    void Awake()
    {
        distanceTravelled = 0f;
        highScore = 0f;
    }



    void Update()
    {
        //increase score
        distanceTravelled += Time.deltaTime * scoreMultiplier;

        //show score on hud
        distanceText.text = "SCORE: " + Mathf.Round(distanceTravelled).ToString();

        //set highscore
        if(distanceTravelled > highScore){
            highScore = distanceTravelled;
        }

        //show highscore on hud
            highScoreText.text = "HIGH SCORE: " + Mathf.Round(highScore).ToString();

        
    }



}
