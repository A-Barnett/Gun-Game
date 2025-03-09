using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TargetSpawner : MonoBehaviour
{
  [SerializeField] private float modeLegnth = 30;
  public bool spawnTargetsOn;
  public GameObject target;
  public float spawnTimer = 1;
  public float startSpawnTime;
  private float modeTimer;
  public bool modeOn;
  public TextMeshProUGUI timerText;
  public TextMeshProUGUI scoreText;
  private float despawnTimer;
  public int range;
  public int movement;

  private void Start()
  {
    startSpawnTime = spawnTimer;
  }

  private void Update()
  {
    if (modeOn)
    {
      spawnTargetsOn = true;
      modeTimer += Time.deltaTime;
     
    }
    if (modeTimer >= modeLegnth)
    {
      modeOn = false;
      spawnTargetsOn = false;
      modeTimer = 0;
    }
    
    if (spawnTargetsOn)
    {
      spawnTimer -= Time.deltaTime;
      if (spawnTimer <= 0)
      {
        SpawnTarget();
      }
      scoreText.text = PlayerPrefs.GetInt("Kills").ToString();
      timerText.text = String.Format("{0:00.00}", modeLegnth - modeTimer).PadLeft(5, '0');
    }
    else
    {
      timerText.text = "00.00";
    }
  }

  public void ChangeMode(int modeSelect)
  {
    if (modeSelect == 1)
    {
      despawnTimer = 5f;
    }else if (modeSelect == 2)
    {
      despawnTimer = 3f;
    }else if (modeSelect == 3)
    {
      despawnTimer = 1.5f;
    }
  }
  private void SpawnTarget()
  {
    spawnTimer = startSpawnTime;
    GameObject newTarget = Instantiate(target, new Vector3(0, 0, 0),new Quaternion(0f, 180f, 0f,0f));
    newTarget.transform.parent = gameObject.transform;
    newTarget.transform.localPosition = SetRange();
    newTarget.gameObject.GetComponentInChildren<TargetController>().SetDespawnTimer(despawnTimer);
    newTarget.gameObject.GetComponent<TargetMoves>().SetMoveMode(movement);
  }

  private Vector3 SetRange()
  {
    if (range == 4)
    {
      return new Vector3(Random.Range(-7f, 7f), Random.Range(-0.9f, 0f), Random.Range(-7f, 7f));
    }
    if (range == 3)
    {
      return new Vector3(Random.Range(-13f, 0f), Random.Range(-1f, 1f), Random.Range(-10f, 10f));
    }
    if (range == 2)
    {
      return new Vector3(Random.Range(-3f, 3f), Random.Range(-0.9f, -0.3f), Random.Range(-6f, 6f));
    }
    if (range == 1)
    {
      return new Vector3(Random.Range(2f, 7f), Random.Range(-0.9f, -0.6f), Random.Range(-4f, 4f));
    }
    return Vector3.zero;
  }
}
