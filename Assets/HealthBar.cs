using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image Healthbar;
    private float fillamount = 0.5f;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Healthbar.fillAmount = fillamount;
    }
}
