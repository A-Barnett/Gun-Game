using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


public class TargetController : MonoBehaviour
{
    public int health = 4;
    private TargetSpawner TargetSpawner;
    public ParticleSystem deathParticle;
    private int startHealth;
    public GameObject stick;
    public GameObject sprite;
    public SpriteRenderer spriteImage;
    private float despawnTimer = 5f;
    private bool despawnKill;
    void Start()
    {
        startHealth = health;
        TargetSpawner = GetComponentInParent<TargetSpawner>();
    }

    public void SetDespawnTimer(float time)
    {
        despawnTimer = time;
    }

    private void Update()
    {
        if (!TargetSpawner.spawnTargetsOn)
        {
            despawnKill = true;
            health = 0;
            Death();
        }
        despawnTimer -= Time.deltaTime;
        if (despawnTimer <= 0)
        {
            despawnKill = true;
            health = 0;
            Death();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= 1;
        }else if (collision.gameObject.CompareTag("Sniper Bullet"))
        {
            health -= 4;
        }else if (collision.gameObject.CompareTag("Saw Bullet"))
        {
            health -= 2;
        }
        spriteImage.color = Color.Lerp(Color.white,Color.red,(float)(startHealth-health)/startHealth);
        Death();
    }
    private void Death()
    {
        if (health <= 0)
        {
            ParticleSystem death = Instantiate(deathParticle, transform.position, Quaternion.identity);
            death.Play();
            if (!despawnKill)
            { 
                PlayerPrefs.SetInt("Kills",PlayerPrefs.GetInt("Kills")+1);
            }
            Destroy(stick);
            Destroy(sprite);
            Destroy(gameObject);
        }
    }
}
