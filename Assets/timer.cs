 using UnityEngine;
 using System.Collections;
 using UnityEngine.UI;
 using TMPro;
 
 public class timer : MonoBehaviour {

     public static timer Instance;
     private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }
     public TextMeshProUGUI timerText;
 
     private float time;

     public static bool isPaused;

     void Update() {
         if((!(GameController.Instance.currentState == GameController.GameState.STARTED)) || isPaused) {
             return;
         }
         time += Time.deltaTime;
 
         var minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
         var seconds = time % 60;//Use the euclidean division for the seconds.
         var fraction = (time * 100) % 100;
 
         //update the label value
         timerText.text = string.Format ("{0:00} : {1:00}", minutes, seconds);
     }

     public void reset() {
         time = 0;
         timerText.text = "0";
     }
 }