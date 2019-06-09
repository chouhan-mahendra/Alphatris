using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour ,IClickable
{
    public static MenuController Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public GameObject pauseMenu;
    public GameObject startMenu;
    public GameObject settingsMenu;
    public GameObject waitingForPlayersMenu;
    public GameObject inGameMenu;
    public GameObject gameOverMenu;

    public TextMeshProUGUI score;
    public TextMeshProUGUI selection;
    public TextMeshProUGUI gameOverText;

    private List<Alphabet> selectedItems = new List<Alphabet>();

    // Update is called once per frame
    void Update()
    {
        GameController.GameState state = GameController.Instance.GetState();
        switch(state) {
            case GameController.GameState.STARTED:
                score.SetText("SCORE " + GameController.Instance.SCORE);
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
        GameController.Instance.SetState(GameController.GameState.PAUSED);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameController.Instance.SetState(GameController.GameState.STARTED);
    }

    public void EndGame(int score)
    {
        gameOverText
            .SetText("Your Final Score is "+ score);
        gameOverMenu.SetActive(true);
        inGameMenu.SetActive(false);
    }

    public void DisableWaitingForPlayersMenu() {
        waitingForPlayersMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }

    public void OnClick(Alphabet alphabet)
    {
        if(alphabet.GetIsSelected()) {
            selection.text = (selection.text + alphabet.character);
            selectedItems.Add(alphabet);
        }
    }

    public void OnUpdateScoreClicked()
    {
        GameController.Instance.UpdateScore(selection.text);
    }

    internal void DestroySelection()
    {
        selection.text = "";
        for (int i = 0; i < selectedItems.Count; ++i)
            selectedItems[i].Explode(i * 0.05f);
        selectedItems.Clear();
    }

    internal void UnSelectAll()
    {
        selection.text = "";
        for (int i = 0; i < selectedItems.Count; ++i)
            selectedItems[i].SetIsSelected(false);
        selectedItems.Clear();
    }
}
