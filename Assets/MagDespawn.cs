using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagDespawn : MonoBehaviour
{
    [SerializeField] private float timeToDespawn;
    private float despawnTimer;
    void Update()
    {
        despawnTimer += Time.deltaTime;
        if (despawnTimer >= timeToDespawn)
        {
            Destroy(gameObject);
        }
    }
}
