using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillCounter : MonoBehaviour
{
    public TextMeshProUGUI killCounter;
    public TextMeshProUGUI ammoCounter;
    public int ammo;
    void Update()
    {
        killCounter.text = PlayerPrefs.GetInt("Kills").ToString();
        int gun = PlayerPrefs.GetInt("Gun");
        if (PlayerPrefs.GetInt("GameMode") == 1 && gun <= 1)
        {
            ammoCounter.text = ammo.ToString();
        }
        else
        {
            ammoCounter.text = "";
        }
    }
}
