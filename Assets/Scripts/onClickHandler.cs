using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onClickHandler : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetButtonDown("Fire1"))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                Debug.Log("Selected  " + hitObject.tag);
                if(hitObject.tag.Equals("Cube")) {
                    Destructible destructible = hitObject.GetComponent<Destructible>();
                    destructible.Destroy();
                }
            }
        }
        */
    }
}
