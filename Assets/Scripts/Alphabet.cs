using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alphabet : MonoBehaviour
{
    public char character;
    public Color onSelectColor;
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

    public bool GetIsSelected() { return isSelected; }
}