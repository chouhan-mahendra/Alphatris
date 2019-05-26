using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject startMenu;
    public GameObject settingsMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (GameController.INSTANCE.currentState == GameController.GameState.PAUSED)
            {
                resume();
            }
            else if(GameController.INSTANCE.currentState == GameController.GameState.STARTED)
            {
                pause();
            }
        }
    }

    public void pause()
    {
        pauseMenu.SetActive(true);
        GameController.INSTANCE.currentState = GameController.GameState.PAUSED;
        Time.timeScale = 0.05f;
    }

    public void resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameController.INSTANCE.currentState = GameController.GameState.STARTED;
    }
}
