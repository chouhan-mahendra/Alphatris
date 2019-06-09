using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public enum GameState { STARTED, PAUSED, IN_LOBBY }
    public enum Mode { LOCAL, MULTIPLAYER }

    public GameState currentState;
    public Mode currentGameMode;

    public GameObject alphabetPrefab;
    public GameObject specialAlphaPrefab;
    
    public List<GameObject> alphabets;
    public int ROWS = 4;
    public float WIDTH = 10;
    public int SCORE = 0;

    private float SCALE = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        Time.timeScale = 1.5f;
        currentState = GameState.IN_LOBBY;
    }

    public void RequestConnection() {
        NetworkController.Instance.RequestConnection();
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
                MenuController.Instance.DisableWaitingForPlayersMenu();
                break;
        }

        //Fancy animation on start game :)
        for(int i = 0;i < alphabets.Count; ++i) 
        {
            alphabets[i].GetComponent<Destructible>().Explode(i*0.1f);
        }
        alphabets = new List<GameObject>();
    }

    public void SetState(GameState nextState)
    {
        Instance.currentState = nextState;
    }

    public void EndGame()
    {
        Debug.Log("********EndGame********");
        Time.timeScale = 0f;
        MenuController.Instance.EndGame(SCORE);
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
        alphabetGO.GetComponent<Alphabet>().clickListener = MenuController.Instance;
        alphabets.Add(alphabetGO);
    }

    public void UpdateScore(string word)
    {
        if (currentGameMode == Mode.LOCAL) {
            StartCoroutine(NetworkController.Instance.GetRequest(word, UpdateScore));
        }
        else NetworkController.Instance.OnWordSelected(word);
    }

    public void UpdateScore(int scoreDelta) {
        if (scoreDelta > 0)
        {
            SCORE += scoreDelta;
            MenuController.Instance.DestroySelection();
        }
        else MenuController.Instance.UnSelectAll();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public GameState GetState()
    {
        return Instance.currentState;
    }
}