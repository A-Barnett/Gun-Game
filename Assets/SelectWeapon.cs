using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SelectWeapon : MonoBehaviour
{
    public GameObject M4;
    public GameObject Sniper;
    public GameObject Saw;
    public GameObject Pistol;
    public GameObject SVD;
    public bool GunGameOn;
    public int killsToSwap;
    private int random;
    private bool isSwapping;
    private void Awake()
    {
         RandomGun();
    }

    public void RandomGun()
    {
        if (GunGameOn)
        {
            PlayerPrefs.SetInt("Gun",Random.Range(0,5) ); 
            PlayerPrefs.SetInt("GameMode",1);
        }
        else
        {
            SwitchGun(Random.Range(0,5));
            PlayerPrefs.SetInt("GameMode",0);
        }
    }
    public void SwitchGun(int gun)
    {
        if (gun == 0)
        {
            M4.gameObject.SetActive(true);
            Sniper.gameObject.SetActive(false);
            Saw.gameObject.SetActive(false);
            Pistol.gameObject.SetActive(false);
            SVD.gameObject.SetActive(false);
        }else if (gun == 1)
        {
            M4.gameObject.SetActive(false);
            Sniper.gameObject.SetActive(true);
            Saw.gameObject.SetActive(false);
            Pistol.gameObject.SetActive(false);
            SVD.gameObject.SetActive(false);
        }else if (gun  == 2)
        {
            M4.gameObject.SetActive(false);
            Sniper.gameObject.SetActive(false);
            Saw.gameObject.SetActive(true);
            Pistol.gameObject.SetActive(false);
            SVD.gameObject.SetActive(false);
        }else if (gun == 3)
        {
            M4.gameObject.SetActive(false);
            Sniper.gameObject.SetActive(false);
            Saw.gameObject.SetActive(false);
            Pistol.gameObject.SetActive(true);
            SVD.gameObject.SetActive(false);
        }else if (gun == 4)
        {
            M4.gameObject.SetActive(false);
            Sniper.gameObject.SetActive(false);
            Saw.gameObject.SetActive(false);
            Pistol.gameObject.SetActive(false);
            SVD.gameObject.SetActive(true);
        }else if(gun ==-1)
        {
            M4.gameObject.SetActive(false);
            Sniper.gameObject.SetActive(false);
            Saw.gameObject.SetActive(false);
            Pistol.gameObject.SetActive(false);
            SVD.gameObject.SetActive(false);
        }
    }

    public void ResetGun()
    {
        M4.gameObject.SetActive(false);
        Sniper.gameObject.SetActive(false);
        Saw.gameObject.SetActive(false);
        Pistol.gameObject.SetActive(false);
        SVD.gameObject.SetActive(false);
        SwitchGun(PlayerPrefs.GetInt("Gun"));
    }
    private void Update()
    {
        if (GunGameOn)
        {
            GunGame();
            SwitchGun(PlayerPrefs.GetInt("Gun"));
        }
    }
    private void GunGame()
    {
        int kills = PlayerPrefs.GetInt("Kills");
        int currentGun = PlayerPrefs.GetInt("Gun");
        if (kills % killsToSwap == 0 && kills != 0 && !isSwapping)
        {
            int newGun = currentGun;
            while (newGun == currentGun)
            {
                newGun = UnityEngine.Random.Range(0, 5);
            }
            PlayerPrefs.SetInt("Gun", newGun);
            isSwapping = true;
        }
        else if (kills % killsToSwap != 0)
        {
            isSwapping = false;
        }
    }
}
