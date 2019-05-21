using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyEffect;

    private void OnMouseDown()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        // change color to red
        meshRenderer.material.color = Color.blue;
        StartCoroutine(Destroy(1f));
    }

    private IEnumerator Destroy(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject destroyEffectInst = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(500, transform.position, 4);
            }
        }
        Destroy(this.gameObject);
        Destroy(destroyEffectInst, 1f);
    }
}
