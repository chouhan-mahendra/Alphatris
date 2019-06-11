using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alphabet : MonoBehaviour
{
    public char character;
    public Color onSelectColor;
    public int id;
    
    public IClickable clickListener = null;

    private Color naturalColor;
    private bool isSelected;

    public void OnMouseDown()
    {
        SetIsSelected(!isSelected);
        if(clickListener != null) 
            clickListener.OnClick(this);
    }

    private void Start()
    {
        naturalColor = GetComponent<MeshRenderer>().material.color;
        isSelected = false;
        if (clickListener == null)
            Debug.Log("No IClickListener for " + this);
        foreach (Transform child in transform)
        {
            TextMeshPro text = child.gameObject.GetComponent<TextMeshPro>();
            if (text != null)
                text.SetText(character.ToString());
            else Debug.Log("TextMeshPro not found");
        }
    }

    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = isSelected ? onSelectColor : naturalColor;
    }

    public void Explode(float time = 0.1f)
    {
        GetComponent<Destructible>().Explode(time);
    }

    public bool GetIsSelected() { return isSelected; }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log(gameObject.name +"-"+ col.gameObject.name);
        if(transform.position.y > 3) {
            GameController.Instance.EndGame();
        }
    }
}