using System;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject enemySpawn;
    public GameObject MapGenerator;
    public GameObject TestWorld;
    public GameObject Player;
    public SelectWeapon SelectWeapon;
    public GameObject globalLight;
    private bool foundPoint;
    private Vector3 startPos;
    public GameObject enterCanvas;
    public GameObject playerPostProcessing;
    public EnterGameController enterGameController;
    public Camera cam;
    private float origFov;

    private void Start()
    {
        startPos = Player.transform.position;
        origFov = cam.fieldOfView;
    }

    public void SwaptoMenu()
    {
        
        cam.fieldOfView = origFov;
        Cursor.visible = true;
        enterGameController.SetUpGame(false);
        playerPostProcessing.SetActive(false);
        Time.timeScale = 0;
        SelectWeapon.SwitchGun(-1);
        Player.transform.position = startPos;
        enemySpawn.SetActive(false);
        MapGenerator.SetActive(true);
        TestWorld.SetActive(false);
        globalLight.SetActive(true);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        enterCanvas.SetActive(true);
    }

    public void SwaptoTest()
    {
        Cursor.visible = false;
        playerPostProcessing.SetActive(false);
        SelectWeapon.GunGameOn = false;
        PlayerPrefs.SetInt("GameMode",0);
        SelectWeapon.ResetGun();
        Player.transform.position = new Vector3(-0.89134f, 23f, -2.6748f);
        Player.transform.rotation = new Quaternion(0, 0, 0, 0);
        enemySpawn.SetActive(false);
        MapGenerator.SetActive(false);
        TestWorld.SetActive(true);
        globalLight.SetActive(false);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
    public void SwaptoGame()
    {
        Cursor.visible = false;
        playerPostProcessing.SetActive(true);
        SelectWeapon.GunGameOn = true;
        PlayerPrefs.SetInt("GameMode",1);
        SelectWeapon.ResetGun();
        Player.transform.position = new Vector3(-0.89134f, 23f, -2.6748f);
        Player.transform.rotation = new Quaternion(0, 0, 0, 0);
        enemySpawn.SetActive(true);
        MapGenerator.SetActive(true);
        TestWorld.SetActive(false);
        globalLight.SetActive(true);
        RaycastHit ray;
        int layerMask = ~LayerMask.GetMask("Player","TestWorld");
        int i = 0;
        while (!foundPoint)
        {
            Vector3 playerPos = Player.transform.position;
            playerPos.x += i;
            if (Physics.Raycast(playerPos, -Vector3.up, out ray, Mathf.Infinity, layerMask))
            {
                if (ray.point.y >= -3.93)
                {
                    Vector3 pos = ray.point;
                    pos.y += 1f;
                    Player.transform.position = pos;
                    foundPoint = true;
                }
            }
            i++;
        }
        foundPoint = false;
    }
}
