using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    public float speedF, speedB, speedS;
    public float shotForce;
    public float fireRate;
    public float stopDistance;
    public float gravity;
    public float shotRandomnessX, shotRandomnessY, bobAmount;
    public int health = 4; 
    public GameObject player;
    public CharacterController ch;
    public GameObject bulletPrefab;
    public GameObject bulletSpawn;
    public ParticleSystem muzzleFlash;
    public ParticleSystem deathParticle;
    public float timeToDespawn;
    

    private float nextFire = 0.0f;
    private Vector3 playerpos; 
    private Vector3 bulletDirection;
    private Vector3 origWeaponPos;
    private Quaternion originalWeaponRotation;
    private int startHealth;
    private MeshRenderer meshRenderer;
    private float despawnTimer;
    private bool despawns;


    void Start()
    {
        player = GameObject.Find("Player (1)");
        ch = gameObject.GetComponent<CharacterController>();
        meshRenderer =gameObject.GetComponent<MeshRenderer>();
        startHealth = health;
    }
    void FixedUpdate()
    {
        despawnTimer += Time.deltaTime;
        if (despawnTimer >= timeToDespawn)
        {
            health = 0;
            despawns = true;
            Death();
        }
        playerpos = player.transform.position;
        float distance = Vector3.Distance(transform.position, playerpos);
        transform.LookAt(playerpos);
        if (distance > stopDistance+0.3f)
        {
            AIMove();
        }else if (distance < stopDistance-0.3f)
        {
            AIBackAway();
        }
        else
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Fire();
            }
        }

        if (transform.position.y >= -3)
        {
            ch.Move(Vector3.down * gravity * Time.deltaTime);
        }
        else if (transform.position.y > -3.2)
        {
            ch.Move(Vector3.down * bobAmount * Time.deltaTime);
        }else if (transform.position.y < -3.42)
        {
            ch.Move(-Vector3.down * bobAmount * Time.deltaTime);
        }
        
    }

    private void AIMove()
    {
        Vector3 direction = (playerpos - transform.position).normalized;
        direction.y = 0;
        ch.Move(direction * speedF * Time.deltaTime);
    }
    private void AIBackAway()
    {
        Vector3 direction = (transform.position - playerpos).normalized;
        direction.y = 0;
        ch.Move(direction * speedB * Time.deltaTime);
    }
    private void Fire()
    {
        Vector3 bulletPos = bulletSpawn.transform.position;
        Quaternion bulletRot = Quaternion.LookRotation(ch.transform.forward, ch.transform.up);
        RaycastHit hit;
        if (Physics.Linecast(bulletPos, playerpos, out hit))
        {
            Vector3 targetPos = hit.point;
            bulletDirection = (targetPos - bulletPos);
        }
        else
        {
            bulletDirection = ch.transform.forward;
        }
        bulletDirection += new Vector3(shotRandomnessX * Random.insideUnitCircle.normalized.x,shotRandomnessY * Random.insideUnitCircle.normalized.y);
        GameObject bullet = Instantiate(bulletPrefab, bulletPos, bulletRot);
        bullet.transform.Rotate(Vector3.right, 90);
        Rigidbody rbB = bullet.GetComponent<Rigidbody>();
        rbB.AddForce(bulletDirection.normalized*shotForce, ForceMode.Impulse);
        muzzleFlash.Play();
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
        meshRenderer.material.color = Color.Lerp(Color.white,Color.red,(float)(startHealth-health)/startHealth);
        Death();
    }

    private void Death()
    {
        if (health <= 0)
        {
            if (!despawns)
            { 
                ParticleSystem death = Instantiate(deathParticle, transform.position, Quaternion.identity);
                death.Play();
                PlayerPrefs.SetInt("Kills",PlayerPrefs.GetInt("Kills")+1);
            }
            Destroy(gameObject);
        }
    }
}
