using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Destructible : MonoBehaviour
{
    public GameObject onDestroyEffect;
    public GameObject onDestroyBig;
    //0 ==> none, 1 == normal, 2 == big
    public void Explode(float time, int type = 1)
    {
        StartCoroutine(Destroy(time, type));
    }

    private IEnumerator Destroy(float time, int type)
    {
        yield return new WaitForSeconds(time);
        if (gameObject == null) yield break;
        if(type == 0)
        {
            Destroy(this.gameObject);
            yield break;
        }

        GameObject destroyEffectInst = Instantiate( type == 1 ? onDestroyEffect : onDestroyBig, 
                                                    transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(100, transform.position, 4);
            }
        }
        Destroy(this.gameObject);
        Destroy(destroyEffectInst, type == 1 ? 0.6f : 1.5f);
        if(type == 1)
            CameraShaker.Instance.ShakeOnce(1.5f,1.5f,0.05f,0.5f);
        else CameraShaker.Instance.ShakeOnce(3f, 3f, 0.1f, 1f);
    }
}
