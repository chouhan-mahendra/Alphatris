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
        socket.On("initAlphabet", OnInitAlphabet);
    }

    //TODO : write reconnection logic
    public void RequestConnection()
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

    private void OnStartGame(SocketIOEvent e)
    {
        Debug.Log(e.data["x"].ToString() + e.data["char"].ToString()); 
        int x = int.Parse(e.data["x"].ToString());
        Vector3 position = new Vector3(x, 5, 0);
        string ch = (e.data["char"].ToString());
        Debug.Log("Players are online, Starting new game from " + position + "," + ch[1]);
        GameController.INSTANCE.StartGame(1);
        GameController.INSTANCE.CreateAlphabet(position, ch[1]);
    }

    private void OnInitAlphabet(SocketIOEvent e) {
        Debug.Log(e.data["x"].ToString() + e.data["char"].ToString()); 
        int x = int.Parse(e.data["x"].ToString());
        Vector3 position = new Vector3(x, 5, 0);
        string ch = (e.data["char"].ToString());
        GameController.INSTANCE.CreateAlphabet(position, ch[1]);
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
