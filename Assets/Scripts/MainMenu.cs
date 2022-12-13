using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void onPlay(){
        SceneManager.LoadScene("Intro");
    }

    public void onInstructions(){
        SceneManager.LoadScene("Instructions");
    }

    public void onExit(){
        Application.Quit();
    }
}
