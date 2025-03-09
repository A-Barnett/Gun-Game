
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathControl : MonoBehaviour
{
  
  public Canvas deathCanvas;
  public Canvas UIOverlay;
  public Switch switch1;
  public Button respawn;
  public TextMeshProUGUI score;
  public EnterGameController enterGameController;
  public PlayerController playerController;
  public GameObject playerPostProcessing;
  public void Death()
  {
    playerPostProcessing.SetActive(false);
    Cursor.lockState = CursorLockMode.Confined;
    Cursor.visible = true;
    enterGameController.enteredGame = false;
    deathCanvas.enabled = true;
    UIOverlay.gameObject.SetActive(false);
    score.text = PlayerPrefs.GetInt("Kills").ToString();
    Time.timeScale = 0;
  }

  public void RespawnButton()
  {
    Reset();
  }

  public void Reset()
  {
    Time.timeScale = 1;
    playerController.health = playerController.maxHealth;
    PlayerPrefs.SetInt("Kills",0);
    deathCanvas.enabled = false;
    switch1.SwaptoMenu();
  }
}
