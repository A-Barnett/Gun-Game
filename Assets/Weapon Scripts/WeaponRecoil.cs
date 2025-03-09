using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;

    public float rotIntensity;
    public float posIntensity, adsPosIntensity;
    public float posMax, adsPosMax;
    public float rotSmooth, posSmoth, adsPosSmooth;
    private void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    public void ApplyRecoil()
    {
        RecoilRot();
        RecoilPos();
    }

    public void ApplyRecoilADS()
    {
        RecoilPosADS();
    }

    private void RecoilPos()
    {
        float moveZ = Mathf.Clamp(-posIntensity, -posMax, posMax);
        Vector3 finalPos = new Vector3(0,0, moveZ);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + startPos, Time.deltaTime * posSmoth);
    }
    private void RecoilRot()
    {
         Quaternion adjustmentY = Quaternion.AngleAxis(-rotIntensity,Vector3.right);
         Quaternion targetRotation = adjustmentY * startRot;
         transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * rotSmooth);
    }

    private void RecoilPosADS()
    {
        float moveZAds = Mathf.Clamp(-adsPosIntensity, -adsPosMax, adsPosMax);
        Vector3 finalPosAds = new Vector3(0,0, moveZAds);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosAds + startPos, Time.deltaTime * adsPosSmooth);
    }
}
