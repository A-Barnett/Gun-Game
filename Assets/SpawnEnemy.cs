using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemy : MonoBehaviour
{
   public GameObject enemyPrefab;
   private float timer;
   public float timeLimit;
   public Transform playerpos;
   public float spawnSize;

   private void Update()
   {
      timer += 1;
      if (timer > timeLimit)
      {
         SpawnLocation(spawnSize);
         timer = 0;
      }
   }

   public void SpawnLocation(float spawnSize)
   {
      float x = Random.Range(playerpos.position.x-spawnSize, playerpos.position.x+spawnSize);
      float z = Random.Range(playerpos.position.z-spawnSize, playerpos.position.z+spawnSize);
      Vector3 spawnPos = new Vector3(x, 50f, z); 
      
      RaycastHit hit;
      if (Physics.Raycast(spawnPos, Vector3.down, out hit))
      {
         Vector3 spawnpoint = hit.point;
         spawnpoint.y += 0.1f;
         GameObject enemy =Instantiate(enemyPrefab, spawnpoint, Quaternion.identity);
      }
   }
}
