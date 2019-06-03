using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    private SocketIOComponent socket;
    private string playerName;
    private string id;

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

    public void EnableNetwork()
    {
        socket.enabled = true;
    }

    public void DisableNetwork()
    {
        socket.enabled = false;
    }

    private void OnPlayerDisconnected(SocketIOEvent e)
    {
        Debug.Log("Player disconnected {" + e.data["id"] + ","+ e.data["name"] + "}");
    }

    private void OnPlayerConnected(SocketIOEvent e)
    {
        string id = e.data["id"].ToString();
        string name = e.data["name"].ToString();
        Debug.Log("Player connected {"+ id +"," + name + "}");
    }

    private void OnStartGame(SocketIOEvent obj)
    {
        Debug.Log("Starting new game");
    }

    private void OnInit(SocketIOEvent e)
    {
        id = e.data["id"].ToString();
        playerName = e.data["name"].ToString();
        Debug.Log("Init {" + id + "," + playerName + "}");
    }

    private void OnConnected(SocketIOEvent obj)
    {
        Debug.Log("connected to server");
    }

}
