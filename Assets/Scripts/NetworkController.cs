using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    private SocketIOComponent socket;
    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open",OnConnected);
        socket.On("init", OnInit);
        socket.On("startGame", OnStartGame);
        socket.On("playerConnected", OnPlayerConnected);
        socket.On("playerDisconnected", OnPlayerDisconnected);
    }

    private void OnPlayerDisconnected(SocketIOEvent obj)
    {
        throw new NotImplementedException();
    }

    private void OnPlayerConnected(SocketIOEvent obj)
    {
        throw new NotImplementedException();
    }

    private void OnStartGame(SocketIOEvent obj)
    {
        throw new NotImplementedException();
    }

    private void OnInit(SocketIOEvent obj)
    {

    }

    private void OnConnected(SocketIOEvent obj)
    {
        Debug.Log("connected to server");
    }
}
