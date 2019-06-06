using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController INSTANCE;
    public enum GameState { STARTED, PAUSED, IN_LOBBY }
    public enum Mode { LOCAL, MULTIPLAYER }

    public GameState currentState;
    public Mode currentGameMode;

    public GameObject alphabetPrefab;
    public List<GameObject> alphabets;
    public int ROWS = 4;
    public float WIDTH = 10;
    public int SCORE = 0;
    public MenuController menuController;
    public NetworkController networkController;

    private float SCALE = 1;

    private void Awake()
    {
        if (INSTANCE == null)
            INSTANCE = this;
        else if (INSTANCE != this)
            Destroy(gameObject);
        Time.timeScale = 1.5f;
        currentState = GameState.IN_LOBBY;
    }

    public static int GetScore()
    {
        return INSTANCE.SCORE;
    }

    public void RequestConnection() {
        networkController.RequestConnection();
    }

    public void StartGame(int mode)
    {
        currentGameMode = (Mode)mode;
        currentState = GameState.STARTED;
        Time.timeScale = 1f;
        SCALE = WIDTH / ROWS;
        switch (currentGameMode) {
            case Mode.LOCAL: 
                //Keep instantiating new aplhabets
                InvokeRepeating("SpawnAlphabetLocal", 2.0f, 1f);
                break;
            case Mode.MULTIPLAYER:
                menuController.DisableWaitingForPlayersMenu();
                break;
        }

        //Fancy animation on start game :)
        for(int i = 0;i < alphabets.Count; ++i) 
        {
            alphabets[i].GetComponent<Destructible>().Explode(i*0.1f);
        }
        alphabets = new List<GameObject>();
    }

    public static void SetState(GameState nextState)
    {
        INSTANCE.currentState = nextState;
    }

    public void EndGame()
    {
        SceneManager.LoadScene("LobbyScene");
    }


    void SpawnAlphabetLocal()
    {
        int x = (int) (Random.Range(0, ROWS) * SCALE);
        Vector3 position = new Vector3(x , 5, 0);
        char character = (char)(Random.Range(0, 26) + 65);
        CreateAlphabet(position, character);
    }

    public void CreateAlphabet(Vector3 position, char character) {
        GameObject alphabetGO = Instantiate(alphabetPrefab, position, Quaternion.identity);
        alphabetGO.transform.localScale = Vector3.one * SCALE;
        alphabetGO.GetComponent<Alphabet>().character = character;
        alphabetGO.GetComponent<Alphabet>().clickListener = menuController;
        alphabets.Add(alphabetGO);
    }

    public bool UpdateScore(string word)
    {
        if (currentGameMode == Mode.LOCAL) {
            SCORE += word.Length;
            return true;
        }
        networkController.OnWordSelected(word);
        return false;
    }

    public void UpdateScore(int scoreDelta) {
        SCORE += scoreDelta;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public static GameState GetState()
    {
        return INSTANCE.currentState;
    }
}