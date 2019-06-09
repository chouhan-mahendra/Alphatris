using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : MonoBehaviour
{
    public static NetworkController Instance;
    public string URL_Http = "http://localhost:8080/check";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

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
        socket.On("updateScore", OnUpdateScore);
        //StartCoroutine(GetRequest("hello"));
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
        GameController.Instance.StartGame(1);
        GameController.Instance.CreateAlphabet(position, ch[1]);
    }

    private void OnInitAlphabet(SocketIOEvent e) {
        Debug.Log(e.data["x"].ToString() + e.data["char"].ToString()); 
        int x = int.Parse(e.data["x"].ToString());
        Vector3 position = new Vector3(x, 5, 0);
        string ch = (e.data["char"].ToString());
        GameController.Instance.CreateAlphabet(position, ch[1]);
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

    public void OnWordSelected(string word)
    {
        string jsonString = string.Format(@"{{ ""word"" : ""{0}"" }}", word);
        socket.Emit("wordSelected", new JSONObject(jsonString));
    }

    private void OnUpdateScore(SocketIOEvent e) {
        int scoreDelta = int.Parse(e.data["score"].ToString());
        Debug.Log("ScoreDelta " + scoreDelta);
        GameController.Instance.UpdateScore(scoreDelta);
    }

    public IEnumerator GetRequest(string word, System.Action<int> done) {
        using (UnityWebRequest webRequest = 
                    UnityWebRequest.Get(URL_Http + "?word="+word)) {
            yield return webRequest.SendWebRequest();
            if(webRequest.isNetworkError)
            {
                Debug.Log("Error : "+ webRequest.error);
                done(0);
            }
            else
            {
                Debug.Log("Received : "+ webRequest.downloadHandler.text);
                done(int.Parse(webRequest.downloadHandler.text));
            }
        }
    }
}
