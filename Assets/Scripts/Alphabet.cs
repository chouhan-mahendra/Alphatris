using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Alphabet : MonoBehaviour
{
    public char character;
    public Color onSelectColor;
    public int id;

    private Color naturalColor;
    private bool isSelected;

    public Material extraPointsMaterial;
    public Material timeStopMaterial;
    public Material explosiveMaterial;

    public enum TYPE {NORMAL, EXTRA_POINTS, TIME_STOP, BOMBERMAN};
    public TYPE alphabetType;

    private void Start()
    {
        isSelected = false;
        foreach (Transform child in transform)
        {
            TextMeshPro text = child.gameObject.GetComponent<TextMeshPro>();
            if (text != null)
                text.SetText(character.ToString());
            else Debug.Log("TextMeshPro not found");
        }

        //Fetch the Event Trigger component from your GameObject
        EventTrigger trigger = GetComponent<EventTrigger>();
        //Create a new entry for the Event Trigger
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //Add a Drag type event to the Event Trigger
        entry.eventID = EventTriggerType.Drag;
        //call the OnDragDelegate function when the Event System detects dragging
        entry.callback.AddListener((data) => {
            //Debug.Log("Alphabet Dragging");
            MenuController.Instance.OnDrag(((PointerEventData)data).position);
        });
        //Add the trigger entry
        trigger.triggers.Add(entry);

        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener((data) => {
            Debug.Log("End Drag");
            MenuController.Instance.onSubmitClicked(true);
        });
        trigger.triggers.Add(endDrag);
    }

    public void setMaterial(Material m) {
        GetComponent<MeshRenderer>().material = m;
        naturalColor = m.color;
    }

    public void SetIsSelected(bool isSelected)
    {
        Debug.Log("in selected");
        this.isSelected = isSelected;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = isSelected ? onSelectColor : naturalColor;
    }

    void OnMouseDown()
    {
        Debug.Log("MouseDown"+ gameObject.name);
    }

    void OnMouseUp()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(ray, out hit)) {
            Debug.Log("MouseUP " + hit.transform.gameObject.name);
            if (gameObject.name.Equals(hit.transform.gameObject.name))
            {
                MenuController.Instance.OnSelectAlphabet(gameObject);
            }
            //else let ondragend take care of everything
        }
    }

    public void Explode(float time = 0.1f)
    {
        GetComponent<Destructible>().Explode(time);
    }

    public void makeSpecial(TYPE type) {
        switch(type) {
            case TYPE.EXTRA_POINTS:
                GetComponent<MeshRenderer>().material = extraPointsMaterial;
                naturalColor = extraPointsMaterial.color;
                break;
            case TYPE.TIME_STOP:
                GetComponent<MeshRenderer>().material = timeStopMaterial;
                naturalColor = timeStopMaterial.color;
                break;
            case TYPE.BOMBERMAN:
                GetComponent<MeshRenderer>().material = explosiveMaterial;
                break;
        }
    }

    public void replaceMaterial(Material material) {
        GetComponent<MeshRenderer>().material = material;
    }

    public bool isSpecial() {
        if(!(this.alphabetType != TYPE.NORMAL))
            return true;
        return false;
    }

    public bool GetIsSelected() { return isSelected; }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log(gameObject.name +"-"+ col.gameObject.name);
        if(transform.position.y > 3) {
            GameController.Instance.EndGame();
        }
    }

    internal List<Alphabet> FindNeighbours()
    {
        List<Alphabet> neighbours = new List<Alphabet>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
        foreach (Collider nearby in colliders)
        {
            Alphabet neighbour = nearby.GetComponent<Alphabet>();
            if (neighbour != null )
            {
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }
}