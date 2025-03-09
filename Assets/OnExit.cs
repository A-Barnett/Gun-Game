using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnExit : MonoBehaviour
{
  private void OnApplicationQuit()
  {
    PlayerPrefs.SetInt("Kills",0);
  }

  private void OnDestroy()
  {
    OnApplicationQuit();
  }
}
