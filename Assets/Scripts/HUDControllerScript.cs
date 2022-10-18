using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDControllerScript : MonoBehaviour
{
    public GameObject spawnSegmentObject;
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text highScoreText;

    public float distanceUnit = 1f;
    public float highScore = 100f;

    private float distanceTravelled = 0f;
    private float startZ;


    // Start is called before the first frame update
    void Start()
    {
        startZ = spawnSegmentObject.transform.position.z;
        distanceText.text = "Score: " + Mathf.RoundToInt(spawnSegmentObject.transform.position.z - startZ).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelled = -Mathf.RoundToInt(spawnSegmentObject.transform.position.z - startZ) * distanceUnit;
        distanceText.text = "Score: " + distanceTravelled.ToString();
        if(distanceTravelled > highScore)
        {
            highScore = distanceTravelled;
        }

        highScoreText.text = "High score: " + highScore.ToString();
    }
}
