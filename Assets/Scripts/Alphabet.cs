using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alphabet : MonoBehaviour
{
    public char alpha;
    public Color onSelectColor;
    private Color naturalColor;
    private bool isSelected;

    public void OnMouseDown()
    {
        isSelected = !isSelected;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = isSelected ? onSelectColor : naturalColor;
    }

    private void Start()
    {
        naturalColor = GetComponent<MeshRenderer>().material.color;
        isSelected = false;
        foreach (Transform child in transform)
        {
            Debug.Log(child.gameObject.name);
            TextMeshPro text = child.gameObject.GetComponent<TextMeshPro>();
            if (text != null)
                text.SetText(alpha.ToString());
            else Debug.Log("TextMeshPro not found");
        }
    }
}
