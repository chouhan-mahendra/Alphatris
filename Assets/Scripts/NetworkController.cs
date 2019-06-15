using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class NetworkController : MonoBehaviour
{
    public static NetworkController Instance;
    public string URL_Http = "http://localhost:8080/check";
    //public string URL_Socket = "ws://127.0.0.1:3000/socket.io/?EIO=4&transport=websocket";

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
        socket.On("spawnAlphabet", OnSpawnAlphabet);
        socket.On("updateScore", OnUpdateScore);
        socket.On("destroyAlphabet", OnDestroyAlphabet);

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
        int id = int.Parse(e.data["id"].ToString()); 
        int x = int.Parse(e.data["x"].ToString());
        Vector3 position = new Vector3(x, 5, 0);
        string ch = (e.data["char"].ToString());
        Debug.Log("Players are online, Starting new game from " + position + "," + ch[1]);
        GameController.Instance.StartGame(1);
        GameController.Instance.CreateAlphabet(position, ch[1], id, false);
    }

    private void OnSpawnAlphabet(SocketIOEvent e) {
        Debug.Log(e.data["x"].ToString() + e.data["char"].ToString());
        int id = int.Parse(e.data["id"].ToString()); 
        int x = int.Parse(e.data["x"].ToString());
        bool isSpecial = bool.Parse(e.data["isSpecial"].ToString());
        Vector3 position = new Vector3(x, 5, 0);
        string ch = (e.data["char"].ToString());
        GameController.Instance.CreateAlphabet(position, ch[1], id, isSpecial);
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

    public void OnWordSelected(string word, List<int> idList)
    {
        string jsonArray = "[";
        for (int i = 0; i < idList.Count; ++i)
        {
            jsonArray += idList[i];
            if (i < idList.Count - 1)
                jsonArray += ",";
        }
        jsonArray += "]";
        string jsonString = string.Format(@"{{ ""word"" : ""{0}"" , ""idList"" : ""{1}""}}", word, jsonArray);
        socket.Emit("wordSelected", new JSONObject(jsonString));
    }

    public void submitSelection(string word, List<Tuple<int, bool>> idList, bool isDrag)
    {
        string jsonArray = "[";
        for (int i = 0; i < idList.Count; ++i)
        {
            jsonArray += "{";
            jsonArray += ("id:" + idList[i].Item1 + "," + "special:"+idList[i].Item2);
            jsonArray += "}";
            if (i < idList.Count - 1)
                jsonArray += ",";
        }
        jsonArray += "]";
        string jsonString = string.Format(@"{{ ""word"" : ""{0}"" , ""idList"" : ""{1}"", ""isDrag"" : ""{2}""}}", word, jsonArray, isDrag);
        socket.Emit("submitSelection", new JSONObject(jsonString));
    }

    private void OnUpdateScore(SocketIOEvent e) {
        int scoreDelta = int.Parse(e.data["score"].ToString());
        Debug.Log("ScoreDelta " + scoreDelta);
        GameController.Instance.UpdateScore(scoreDelta);
    }

    private void OnDestroyAlphabet(SocketIOEvent e) {
        Debug.Log("onDestroyAlphabet : " + e.data["idList"].ToString());
        var array = JSON.Parse(e.data["idList"].ToString().Replace("\"", ""));

        List<int> list = new List<int>();
        for (int i = 0; i < array.Count; ++i)
            list.Add(array[i]);
        
        Debug.Log("onDestroyAlphabet : " + array[0] + "," + array.Count);
        GameController.Instance.DestroyAlphabet(list);
    }

    public IEnumerator GetRequest(string word, System.Action<int> done) {
        using (UnityWebRequest webRequest = 
                    UnityWebRequest.Get(URL_Http + "?word="+word)) {
            yield return webRequest.SendWebRequest();
            if(webRequest.isNetworkError)
            {
                Debug.Log("GetRequest Error : "+ webRequest.error);
                done(0);
            }
            else
            {
                Debug.Log("GetRequest Received : "+ webRequest.downloadHandler.text);
                done(int.Parse(webRequest.downloadHandler.text));
            }
        }
    }
}
