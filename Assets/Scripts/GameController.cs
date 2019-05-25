using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum GameState { STARTED, PAUSED, IN_LOBBY }
    public GameState currentState;

    public static GameController INSTANCE;

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

    }

    internal void toggleMusic()
    {
        throw new NotImplementedException();
    }

    internal void startGame()
    {
        throw new NotImplementedException();
    }
}