using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCube : MonoBehaviour
{
    public bool isTouched;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter() {
        Debug.Log("tourched");
        this.isTouched = true;
    }
}
