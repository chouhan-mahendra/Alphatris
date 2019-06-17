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
    public string URL_Http = "http://172.16.0.222:8080/check";

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
        socket.On("playerDisconnected", OnPlayerDisconnected);
        socket.On("spawnAlphabet", OnSpawnAlphabet);
        socket.On("updateScore", OnUpdateScore);
        socket.On("destroyAlphabet", OnDestroyAlphabet);
        socket.On("multiplayerConnectionEstablished", multiplayerConnectionEstablished);
        socket.On("invalidSelection", invalidSelection);
        socket.On("playerReady", playerReady);
        socket.On("checkAndDestroyAlphabet", checkAndDestroyAlphabet);
        socket.On("opponentScore", opponentScore);

        //var array2 = JSON.Parse("[1,2,3]");
        //var array3 = JSON.Parse("\"[1,2,3]\"".Replace("\"",""));
        //Debug.Log("2 :" + array2.Count);
        //Debug.Log("3 :" + array3.Count);
    }

    public void restart()
    {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open",OnConnected);
        socket.On("init", OnInit);
        socket.On("playerDisconnected", OnPlayerDisconnected);
        socket.On("spawnAlphabet", OnSpawnAlphabet);
        socket.On("updateScore", OnUpdateScore);
        socket.On("destroyAlphabet", OnDestroyAlphabet);
        socket.On("multiplayerConnectionEstablished", multiplayerConnectionEstablished);
        socket.On("invalidSelection", invalidSelection);
        socket.On("playerReady", playerReady);
        socket.On("checkAndDestroyAlphabet", checkAndDestroyAlphabet);

        //var array2 = JSON.Parse("[1,2,3]");
        //var array3 = JSON.Parse("\"[1,2,3]\"".Replace("\"",""));
        //Debug.Log("2 :" + array2.Count);
        //Debug.Log("3 :" + array3.Count);
    }

    //TODO : write reconnection logic
    public void RequestConnection()
    {
        socket.enabled = true;
    }

    public void DisableNetwork()
    {
        socket.enabled = false;
        socket.Emit("disconnect");
    }

    public bool socketState() {
        return socket.enabled;
    }
    private void OnPlayerDisconnected(SocketIOEvent e)
    {
        Debug.Log("Player disconnected {" + e.data["id"] + ","+ e.data["name"] + "}");
    }

    private void playerReady(SocketIOEvent e) {
        GameController.Instance.playerReady();
    }

    private void OnSpawnAlphabet(SocketIOEvent e) {
        int id = int.Parse(e.data["id"].ToString()); 
        int x = int.Parse(e.data["x"].ToString());
        int type = int.Parse(e.data["type"].ToString());
        Vector3 position = new Vector3(x, 5, 0);
        string ch = (e.data["char"].ToString());
        GameController.Instance.CreateAlphabet(position, ch[1], id, type);
        timer.isPaused = false;
    }

    private void OnInit(SocketIOEvent e)
    {
        id = this.getParsedResponse(e.data["id"].ToString());
        playerName = this.getParsedResponse(e.data["name"].ToString());
        string multiplayerId = this.getParsedResponse(e.data["multiplayer_id"].ToString().Replace("\\", ""));
        GameController.Instance.chooseMultiplayerId(multiplayerId);
        Debug.Log("Init {" + id + "," + playerName + "}");
    }

    public void initializeSinglePlayerGame() {
        socket.Emit("initializeSinglePlayerGame");
    }

    private void multiplayerConnectionEstablished(SocketIOEvent e) {
        string id1 = this.getParsedResponse(e.data["id1"].ToString());
        string id2 = this.getParsedResponse(e.data["id2"].ToString());
        string multiId = "";
        if(id1 == this.id) {
            multiId = id2;
        } else if(id2 == this.id) {
            multiId = id1;
        }
        if(multiId != "") {
            GameController.Instance.setMultiplayerId(multiId);
        }
    }

    public void addToPool() {
        socket.Emit("addToPool", new JSONObject(string.Format(@"{{ ""id"" : ""{0}""}}", this.id)));
    }

    public void establishMultiplayerConnection(string multiplayerId) {
        socket.Emit("establishMultiplayerConnection", new JSONObject(string.Format(@"{{ ""multiplayerId"" : ""{0}""}}", multiplayerId)));
    }

    public void reset() {
        Debug.Log("in reset");
        socket.Emit("reset");
    }

    private void OnConnected(SocketIOEvent obj)
    {
        Debug.Log("connected to server");
    }

    public void submitSelection(string word, List<Tuple<int, int>> idList, bool isDrag, int specialPointsCount = 0)
    {
        string jsonArray = "[";
        int specialTimeCount = 0;
        for (int i = 0; i < idList.Count; ++i)
        {
            jsonArray += idList[i].Item1;
            if (i < idList.Count - 1)
                jsonArray += ",";
            if(idList[i].Item2 == 1) {
                ++specialPointsCount;
            } else if(idList[i].Item2 == 2) {
                ++specialTimeCount;
            }
        }
        jsonArray += "]";
        string jsonString = string.Format(@"{{ ""word"" : ""{0}"" , ""wordList"" : ""{1}"", ""isDrag"" : ""{2}"", ""specialPointsCount"" : ""{3}"", ""specialTimeCount"" : ""{4}""}}", word, jsonArray, isDrag, specialPointsCount, specialTimeCount);
        socket.Emit("submitSelection", new JSONObject(jsonString));
    }

    private void OnUpdateScore(SocketIOEvent e) {
        int scoreDelta = int.Parse(e.data["score"].ToString());
        int timeToStop = int.Parse(e.data["timeToStop"].ToString());
        Debug.Log(timeToStop);
        GameController.Instance.UpdateScore(scoreDelta);
        if(timeToStop > 0) {
            Debug.Log("in time to stop");
            timer.isPaused = true;
        }
    }

    private void opponentScore(SocketIOEvent e) {
        int scoreDelta = int.Parse(e.data["score"].ToString());
        GameController.Instance.UpdateOpponentScore(scoreDelta);
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

    private void checkAndDestroyAlphabet(SocketIOEvent e) {
        Debug.Log("onCheckAndDestroyAlphabet : " + e.data["idList"].ToString());

        var array = JSON.Parse(e.data["idList"].ToString().Replace("\"", ""));

        List<int> list = new List<int>();
        for (int i = 0; i < array.Count; ++i)
            list.Add(array[i]);
        GameController.Instance.checkAndDestroyAlphabet(list);
    }

    private void invalidSelection(SocketIOEvent e) {
        MenuController.Instance.UnSelectAll();
    }

    public string getParsedResponse(string s) {
        return s.Replace('"', ' ').Trim();
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
