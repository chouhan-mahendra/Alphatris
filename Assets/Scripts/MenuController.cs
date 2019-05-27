using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject startMenu;
    public GameObject settingsMenu;
    public TextMeshProUGUI scoreText;
    // Update is called once per frame
    void Update()
    {
        GameController.GameState state = GameController.GetState();

        switch(state) {
            case GameController.GameState.STARTED:
                scoreText.SetText("SCORE " + GameController.GetScore().ToString());
                break;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (state == GameController.GameState.PAUSED)
            {
                resume();
            }
            else if(state == GameController.GameState.STARTED)
            {
                pause();
            }
        }

    }

    public void pause()
    {
        pauseMenu.SetActive(true);
        GameController.SetState(GameController.GameState.PAUSED);
        Time.timeScale = 0.05f;
    }

    public void resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameController.SetState(GameController.GameState.STARTED);
    }

    public void end()
    {
        //Debug.Log("Game Ended in " + time.ToString());
        //finalScoreText
        //    .SetText("You Finished the Game in " +
        //        time.ToString() + " seconds");
        //finalScoreMenu.SetActive(true);
        //inGameMenu.SetActive(false);
    }
}
