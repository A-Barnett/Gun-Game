using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBulletController : MonoBehaviour
{
    private float timer;
    public ParticleSystem explosionPrefab;
    public bool bounce;
    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.Play();
        if (bounce)
        {
             if (!collision.gameObject.CompareTag("Enemy"))
             {
                 Destroy(gameObject);
             }
        }
        else
        {
            Destroy(gameObject); 
        }
       
    }
    private void Update()
    {
        timer += 1;
        if (timer > 300)
        {
            Destroy(gameObject);
        }
    }
}
