using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController INSTANCE;
    public enum GameState { STARTED, PAUSED, IN_LOBBY }
    public GameState currentState;

    public GameObject alphabetPrefab;
    public List<GameObject> alphabets;

    public int ROWS = 4;
    public float WIDTH = 10;
    public int SCORE = 0;
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

    public void StartGame()
    {
        currentState = GameState.STARTED;
        Time.timeScale = 1f;
        SCALE = WIDTH / ROWS;
        //Keep instantiating new aplhabets
        InvokeRepeating("createAlphabet", 2.0f, 1f);
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

    internal void toggleMusic()
    {
        throw new NotImplementedException();
    }

    void createAlphabet()
    {
        float x = Random.Range(0, ROWS) * SCALE;
        Vector3 position = new Vector3(x , 5, 0);
        GameObject go = Instantiate(alphabetPrefab, position, Quaternion.identity);
        go.transform.localScale = Vector3.one * SCALE;
        alphabets.Add(go);
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