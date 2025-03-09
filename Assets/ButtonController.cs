using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonController : MonoBehaviour
{

    [SerializeField] private TargetSpawner targetSpawner;
    private Vector3 sphereStartPos;
    public int modeSelect;
    private float pressedTimer;
    private bool pressed;
   
    private void Start()
    {
        sphereStartPos = gameObject.transform.position;
    }
    private void Update()
    {
        if (pressed)
        {
            pressedTimer += Time.deltaTime;
        }
        if (!targetSpawner.spawnTargetsOn && pressedTimer >1f)
        {
            StartCoroutine(PressButton(false));
            pressed = false;
            pressedTimer = 0;
        }
       
    }

    public void RunButton()
    {
        if (!targetSpawner.spawnTargetsOn)
        {
            pressedTimer = 0;
            pressed = true;
            targetSpawner.spawnTargetsOn = true;
            PlayerPrefs.SetInt("Kills",0);
            StartCoroutine(PressButton(true));
            if (modeSelect == 1)
            {
                EasyMode();
            }
            else if (modeSelect == 2)
            {
                MediumMode();
            }
            else if (modeSelect == 3)
            {
                HardMode();
            }
            targetSpawner.ChangeMode(modeSelect);
            targetSpawner.modeOn = true;
        }
    }

    private IEnumerator PressButton(bool isPressed)
    {
        float duration = 1.0f;
        Vector3 startPos = sphereStartPos;
        Vector3 endPos = sphereStartPos;
        endPos.y -= 0.01f;
        Color startColor = Color.white;
        Color endColor = Color.red;
        if (!isPressed)
        {
            startPos = endPos;
            endPos = sphereStartPos;
            startColor = endColor;
            endColor = Color.white;
        }

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = endPos;
        gameObject.GetComponent<Renderer>().material.color = endColor;
    }
    private void EasyMode()
    {
        targetSpawner.startSpawnTime = 2;
        targetSpawner.spawnTimer = 2;
    }

    private void MediumMode()
    {
        targetSpawner.startSpawnTime = 1;
        targetSpawner.spawnTimer = 1;
    }
    private void HardMode()
    {
        targetSpawner.startSpawnTime = 0.5f;
        targetSpawner.spawnTimer = 0.5f;
    }
    
}
