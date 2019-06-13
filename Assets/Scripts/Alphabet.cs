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

    private Color naturalColor;
    private bool isSelected;

    private void Start()
    {
        naturalColor = GetComponent<MeshRenderer>().material.color;
        isSelected = false;
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
        //Debug.Log(gameObject.name +"-"+ col.gameObject.name);
        if(transform.position.y > 3) {
            GameController.Instance.EndGame();
        }
    }
}