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

    public Material extraPointsMaterial;
    public Material timeStopMaterial;

    public enum TYPE {NORMAL, EXTRA_POINTS, TIME_STOP};
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
}