using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public enum GameState { STARTED, PAUSED, IN_LOBBY, WAITING_FOR_PLAYERS }
    public enum Mode { LOCAL, MULTIPLAYER }

    public GameState currentState;
    public Mode currentGameMode;
    public GameObject dummyObject;
    public GameObject alphabetPrefab;
    
    public Dictionary<int, GameObject> alphabets;
    public int ROWS = 4;
    public float WIDTH = 10;
    public int SCORE = 0;
    public float SPAWN_RATE;

    public Material specialMaterial;

    private float SCALE = 1;
    private List<Alphabet> currentSelection;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        Time.timeScale = 1.8f;
        currentState = GameState.IN_LOBBY;
    }

    public void StartGame(int mode)
    {
        currentGameMode = (Mode)mode;
        Time.timeScale = 1f;
        SCALE = WIDTH / ROWS;
        switch (currentGameMode) {
            case Mode.LOCAL: 
                //Keep instantiating new aplhabets
                currentState = GameState.STARTED;
                NetworkController.Instance.RequestConnection();
                InvokeRepeating("SpawnAlphabetLocal", 1.0f, SPAWN_RATE);
                break;
            case Mode.MULTIPLAYER:
                currentState = GameState.WAITING_FOR_PLAYERS;
                NetworkController.Instance.RequestConnection();
                break;
        }

        //Fancy animation on start game :)
        int i = 0; 
        foreach(Transform child in dummyObject.transform) { 
            child.GetComponent<Destructible>().Explode(i++ *0.1f);
        }
        alphabets = new Dictionary<int, GameObject>();
        currentSelection = new List<Alphabet>();
    }

    public void chooseMultiplayerId(string multiplayerId) {
        if(currentGameMode == Mode.MULTIPLAYER && currentState == GameState.WAITING_FOR_PLAYERS) {
            Debug.Log(multiplayerId);
            Debug.Log(multiplayerId.Length);
            if(multiplayerId != "-1") {
                currentState = GameState.STARTED;
                MenuController.Instance.DisableWaitingForPlayersMenu();
                NetworkController.Instance.establishMultiplayerConnection(multiplayerId);
            } else {
                NetworkController.Instance.addToPool();
            }
        }
    }

    public void SetState(GameState nextState)
    {
        Instance.currentState = nextState;
    }

    public void EndGame()
    {
        Time.timeScale = 0f;
        MenuController.Instance.EndGame(SCORE);
    }

    void SpawnAlphabetLocal()
    {
        int x = (int) (Random.Range(0, ROWS) * SCALE);
        Vector3 position = new Vector3(x , 5, 0);
        char character = (char)(Random.Range(0, 26) + 65);
        bool isSpecial = Random.Range(0, 10) == 1;
        CreateAlphabet(position, character, Random.Range(0,100), isSpecial);
    }

    public void CreateAlphabet(Vector3 position, char character, int id, bool isSpecial) {
        GameObject alphabet = Instantiate(alphabetPrefab, position, Quaternion.identity);
        alphabet.name = character + "_" + id.ToString();
        alphabet.transform.localScale = Vector3.one * SCALE;
        alphabet.GetComponent<Alphabet>().id = id;
        alphabet.GetComponent<Alphabet>().character = character;
        alphabets[id] = alphabet;
        if(isSpecial) {
            alphabet.GetComponent<Alphabet>().makeSpecial(specialMaterial);
        }
    }

    public void UpdateScore(string word, List<int> idList, bool isDrag)
    {
        if (currentGameMode == Mode.LOCAL) {
            NetworkController.Instance.submitSelection(word, getLetterList(idList), isDrag);
        }
        // else NetworkController.Instance.OnWordSelected(word, idList);
    }

    private List<Tuple<int, bool>> getLetterList(List<int> idList) {
        var list = new List<Tuple<int, bool>>();
        foreach(int id in idList) {
            list.Add(Tuple.Create(id, alphabets[id].GetComponent<Alphabet>().isSpecial()));
        }
        return list;
    }

    public void UpdateScore(int scoreDelta) {
        if (scoreDelta > 0)
        {
            SCORE += scoreDelta;
            MenuController.Instance.DestroySelection();
        }
        else MenuController.Instance.UnSelectAll();
    }

    public void DestroyAlphabet(List<int> list) {
        foreach(int id in list) {
            alphabets[id].GetComponent<Alphabet>().Explode();
        }
    }

    public void resetSelection() {
        MenuController.Instance.UnSelectAll();
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