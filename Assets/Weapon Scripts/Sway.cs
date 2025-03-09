using System;
using UnityEngine;
public class Sway : MonoBehaviour
{
    public float rotateIntensityX, rotateIntensityY, rotateIntensityZ;
    public float rotateSmooth;
    public float moveAmount;
    public float maxMoveAmount;
    public float moveSmooth;
    private Quaternion origRotation;
    private Vector3 origPos;
    private float Xmouse, Ymouse;
    private PlayerController playerController;
    
    public AnimationCurve speedToFrequencyCurve;
    [SerializeField, Range(0,10f)]private float amplitudeX = 0.015f;
    [SerializeField, Range(0,10f)]private float amplitudeY = 0.015f;
    [SerializeField, Range(0,100)]private float frequency = 10f;
    [SerializeField]private CharacterController player;
    

    private void Start()
    {
        playerController = gameObject.GetComponentInParent<PlayerController>();
        origRotation = transform.localRotation;
        origPos = transform.localPosition;
     
    }

    private void FixedUpdate()
    { 
        Xmouse = Input.GetAxis("Mouse X");
        Ymouse = Input.GetAxis("Mouse Y");
        if (!playerController.isAiming)
        {
            UpdateSwayRotate();
            UpdateSwayMove();
        }


    }

    private void UpdateSwayRotate()
    {
        Quaternion weaponTargetRotation;
        Quaternion weaponAdjustmentX = Quaternion.AngleAxis(rotateIntensityX * Xmouse, Vector3.up);
        Quaternion weaponAdjustmentY = Quaternion.AngleAxis(-rotateIntensityY * Ymouse, Vector3.right);
        Quaternion weaponAdjustmentZ = Quaternion.AngleAxis(-rotateIntensityZ * Xmouse, Vector3.forward);
        Quaternion weaponRotation = FootStep();
        weaponTargetRotation = weaponRotation * weaponAdjustmentX * weaponAdjustmentY *weaponAdjustmentZ* origRotation;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, weaponTargetRotation, Time.deltaTime * rotateSmooth);
    }

    private void UpdateSwayMove()
    {
        float moveX = Mathf.Clamp(Xmouse * moveAmount, -maxMoveAmount, maxMoveAmount);
        float moveY = Mathf.Clamp(Ymouse * moveAmount, -maxMoveAmount, maxMoveAmount);
        Vector3 finalPos = new Vector3(moveX, moveY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + origPos, Time.deltaTime * moveSmooth);
    }

    private Quaternion FootStep()
    {
        float speed = new Vector3(player.velocity.x, 0, player.velocity.z).magnitude;
        float frequencyNow = speedToFrequencyCurve.Evaluate(speed) * frequency;
        Quaternion posadjustmentX = Quaternion.AngleAxis(Mathf.Cos(Time.time *frequencyNow/ 2) * amplitudeX * 2, Vector3.up);
        Quaternion posadjustmentY = Quaternion.AngleAxis(Mathf.Sin(Time.time*frequencyNow) * amplitudeY, Vector3.right);
        Quaternion targetRotation = posadjustmentX * posadjustmentY;
        return targetRotation;
    }

}
