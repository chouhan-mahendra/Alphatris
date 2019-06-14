using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public GameObject pauseMenu;

    public GameObject startMenu;
    public GameObject settingsMenu;
    public GameObject waitingForPlayersMenu;
    public GameObject inGameMenu;
    public GameObject gameOverMenu;

    public TextMeshProUGUI score;
    public TextMeshProUGUI selection;
    public TextMeshProUGUI gameOverText;

    private List<Alphabet> currentSelection = new List<Alphabet>();
    private IDisposable clickSub;

    private bool isDrag;

    // Update is called once per frame
    void Update()
    {
        GameController.GameState state = GameController.Instance.GetState();
        switch(state) {
            case GameController.GameState.STARTED:
                score.SetText("SCORE " + GameController.Instance.SCORE);
                break;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (state == GameController.GameState.PAUSED)
            {
                Resume();
            }
            else if(state == GameController.GameState.STARTED)
            {
                Pause();
            }
        }
    }

    private void Start()
    {
        clickSub = this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .Select(_ => {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();
                return (Physics.Raycast(ray, out hit)) ? hit.transform.gameObject : null;
            })
            .Where(gameObject => gameObject != null && gameObject.tag.Equals("Cube"))
            .DistinctUntilChanged(gameObject => gameObject.name)
            .Subscribe(item => {
                //Debug.Log("unirx : " + item.name);
                Alphabet alphabet = item.GetComponent<Alphabet>();
                int index = currentSelection.FindIndex(it => it.name.Equals(item.name));
                if (index != -1) {
                    //Debug.Log("item already present, removing all proceeding indexes");
                    for (int it = index + 1; it < currentSelection.Count; ++it) {
                        currentSelection[it].SetIsSelected(false);
                        this.isDrag = false;
                    }
                    currentSelection
                        .RemoveRange(index + 1, currentSelection.Count - index - 1);
                }
                else {
                    currentSelection.Add(alphabet);
                    alphabet.SetIsSelected(true);
                    this.isDrag = true;
                }
                string currentText = "";
                foreach (Alphabet alp in currentSelection)
                    currentText += alp.character;
                selection.text = currentText;
            });
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        GameController.Instance.SetState(GameController.GameState.PAUSED);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameController.Instance.SetState(GameController.GameState.STARTED);
    }

    public void EndGame(int score)
    {
        gameOverText
            .SetText("Your Final Score is "+ score);
        gameOverMenu.SetActive(true);
        inGameMenu.SetActive(false);
    }

    public void DisableWaitingForPlayersMenu() {
        waitingForPlayersMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }

    public void onSubmitClicked()
    {
        Debug.Log("in on submit clicked");
        List<int> idlist = new List<int>();
        currentSelection.ForEach(alphabet => idlist.Add(alphabet.id));
        GameController.Instance.UpdateScore(selection.text, idlist, isDrag);
    }

    internal void DestroySelection()
    {
        selection.text = "";
        for (int i = 0; i < currentSelection.Count; ++i)
            currentSelection[i].Explode(i * 0.05f);
        currentSelection.Clear();
    }

    internal void UnSelectAll()
    {
        selection.text = "";
        for (int i = 0; i < currentSelection.Count; ++i)
            currentSelection[i].SetIsSelected(false);
        currentSelection.Clear();
    }

    internal void submitSelection() {
        
    }

    private void OnDestroy()
    {
        clickSub.Dispose();
    }
}
