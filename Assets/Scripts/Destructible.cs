using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Destructible : MonoBehaviour
{
    public GameObject onDestroyEffect;

    public void Explode(float time)
    {
        StartCoroutine(Destroy(time));
    }

    private IEnumerator Destroy(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject destroyEffectInst = Instantiate(onDestroyEffect, transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(200, transform.position, 4);
            }
        }
        Destroy(this.gameObject);
        Destroy(destroyEffectInst, 1.5f);
        CameraShaker.Instance.ShakeOnce(2f,2f,0.05f,0.5f);
    }
}
