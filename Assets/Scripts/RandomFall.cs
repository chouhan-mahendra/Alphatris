using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFall : MonoBehaviour
{
    public GameObject alphabet;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("createAlphabet", 2.0f, 1f);
    }

    void createAlphabet()
    {
        Vector3 position = new Vector3((int)Random.Range(-5.0f, 5.0f), 5, 0); 
        Instantiate(alphabet,position,Quaternion.identity);
    }
}
