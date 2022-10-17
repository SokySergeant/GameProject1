using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button startBtn;

    void Awake()
    {
        //set the start button as selected on startup so navigation without mouse can occur
        startBtn.Select();
    }



    public void StartGame(){
        SceneManager.LoadScene("MainGame");
    }

    public void Exit(){
        Application.Quit();
    }
}
