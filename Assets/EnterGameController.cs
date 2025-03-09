using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterGameController : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject crossHair;
    [SerializeField] private Switch _switch;
    [SerializeField] private GameObject selectWeapon;
    [SerializeField] private GameObject enterCanvas;
    [SerializeField] private DeathControl deathControl;
    [SerializeField] private GameObject playerPostProcessing;
    public bool enteredGame;
    private bool escLock;
    private float origFov;
    [SerializeField] private Camera cam;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        origFov = cam.fieldOfView;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscMenu();
        }
    }

    public void StartTestButton()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        escLock = false;
        _switch.SwaptoTest();
        SetUpGame(true);
        enterCanvas.SetActive(false);
        enteredGame = true;
        player.GetComponent<PlayerController>().ResetDeath();
    }
    public void StartGameButton()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        escLock = false;
        Time.timeScale = 1f;
        _switch.SwaptoGame();
        SetUpGame(true);
        enterCanvas.SetActive(false);
        enteredGame = true;
        player.GetComponent<PlayerController>().ResetDeath();
    }

    public void QuitGame()
    { 
        Application.Quit();
    }

    private void EscMenu()
    {
        if (enteredGame)
        {
            if (!escLock)
            {
                cam.fieldOfView = origFov;
                Cursor.visible = true;
                playerPostProcessing.SetActive(false);
                enterCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                SetUpGame(false);
                Time.timeScale = 0;
                escLock = true;
            }
            else
            {
                Cursor.visible = false;
                playerPostProcessing.SetActive(true);
                enterCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                SetUpGame(true);
                escLock = false;
            }
        }
    }

    public void SetUpGame(bool gameOn)
    {
        selectWeapon.SetActive(gameOn);
        player.GetComponent<PlayerController>().enabled = gameOn;
        player.GetComponent<CharacterController>().enabled = gameOn;
        playerUI.SetActive(gameOn);
        crossHair.SetActive(gameOn);
    }
    
}
