using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour ,IClickable
{
    public GameObject pauseMenu;
    public GameObject startMenu;
    public GameObject settingsMenu;
    public GameObject waitingForPlayersMenu;

    public TextMeshProUGUI score;
    public TextMeshProUGUI selection;
    private List<Alphabet> selectedItems = new List<Alphabet>();

    // Update is called once per frame
    void Update()
    {
        GameController.GameState state = GameController.GetState();

        switch(state) {
            case GameController.GameState.STARTED:
                score.SetText("SCORE " + GameController.GetScore().ToString());
                break;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (state == GameController.GameState.PAUSED)
            {
                Resume();
            }
            else if(state == GameController.GameState.STARTED)
            {
                Pause();
            }
        }

    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        GameController.SetState(GameController.GameState.PAUSED);
        Time.timeScale = 0.1f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameController.SetState(GameController.GameState.STARTED);
    }

    public void End()
    {
        //Debug.Log("Game Ended in " + time.ToString());
        //finalScoreText
        //    .SetText("You Finished the Game in " +
        //        time.ToString() + " seconds");
        //finalScoreMenu.SetActive(true);
        //inGameMenu.SetActive(false);
    }

    public void DisableWaitingForPlayersMenu() {
        waitingForPlayersMenu.SetActive(false);
    }

    public void OnClick(Alphabet alphabet)
    {
        if(alphabet.GetIsSelected()) {
            selection.SetText(selection.text + alphabet.character);
            selectedItems.Add(alphabet);
        }
    }

    public void OnUpdateScoreClicked()
    {
        GameController.INSTANCE.UpdateScore(selection.text.Length);
        selection.SetText("");
        for(int i = 0;  i < selectedItems.Count; ++i) 
            selectedItems[i].Explode(i * 0.05f);
        selectedItems.Clear();
    }
}
