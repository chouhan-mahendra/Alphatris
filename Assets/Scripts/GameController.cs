using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController INSTANCE;
    public enum GameState { STARTED, PAUSED, IN_LOBBY }
    public GameState currentState;

    public GameObject alphabet;
    public List<GameObject> alphabets;

    private void Awake()
    {
        if (INSTANCE == null)
            INSTANCE = this;
        else if (INSTANCE != this)
            Destroy(gameObject);
        Time.timeScale = 1.5f;
        currentState = GameState.IN_LOBBY;
    }

    public void StartGame()
    {
        currentState = GameState.STARTED;
        Time.timeScale = 1f;
        // Start is called before the first frame update
        InvokeRepeating("createAlphabet", 2.0f, 1f);

        for(int i = 0;i < alphabets.Count; ++i) 
        {
            alphabets[i].GetComponent<Destructible>().Explode(i*0.1f);
        }

        alphabets = new List<GameObject>();
    }

    internal void toggleMusic()
    {
        throw new NotImplementedException();
    }

    internal void startGame()
    {
        throw new NotImplementedException();
    }

    void createAlphabet()
    {
        Vector3 position = new Vector3((int)Random.Range(-5.0f, 5.0f), 5, 0);
        GameObject go = Instantiate(alphabet, position, Quaternion.identity);
        alphabets.Add(go);
    }
}