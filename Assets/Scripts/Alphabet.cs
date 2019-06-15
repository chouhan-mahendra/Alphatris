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
    public static float AudioPitch = 0.8f;
    private Color naturalColor;
    private bool isSelected;
    private AudioSource audioSource;

    private bool special;

    private void Start()
    {
        naturalColor = GetComponent<MeshRenderer>().material.color;
        isSelected = false;
        audioSource = GetComponent<AudioSource>();
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
        AudioPitch += (isSelected) ? 0.1f : -0.1f;
        audioSource.pitch = AudioPitch;
        audioSource.Play();
    }

    public void Explode(float time = 0.1f)
    {
        GetComponent<Destructible>().Explode(time);
    }

    public void makeSpecial(Material material) {
        GetComponent<MeshRenderer>().material = material;
        this.special = true;
    }
    public void replaceMaterial(Material material) {
        GetComponent<MeshRenderer>().material = material;
    }

    public bool isSpecial() {
        return this.special;
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