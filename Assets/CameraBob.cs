using System;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    
    [SerializeField]private bool BobEnable;
    [SerializeField, Range(0,0.1f)]private float amplitude = 0.015f;
    [SerializeField, Range(0,30)]private float frequency = 10f;
    [SerializeField]private Transform cam;
    [SerializeField]private Transform camHolder;
    [SerializeField]private CharacterController player;
    private float toggleSpeed = 3f;
    private Vector3 startPos;

    private void Awake()
    {
        startPos = cam.localPosition;
    }
    void Update()
    {
        if(!BobEnable) return;
        CheckMotion();
        cam.LookAt(FocusTarget());
    }

    private void CheckMotion()
    {
        float speed = new Vector3(player.velocity.x, 0, player.velocity.z).magnitude;
        ResetPos();
        if(speed< toggleSpeed) return;
        if(!player.isGrounded) return;
        PlayMotion(FootStep());
    }
    private void PlayMotion(Vector3 motion){
        cam.localPosition += motion; 
    }

    private Vector3 FootStep()
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x = Mathf.Cos(Time.time * frequency/2) * amplitude*2;
        return pos;
    }

    private void ResetPos()
    {
        if (cam.localPosition == startPos) return;
        cam.localPosition = Vector3.Lerp(cam.localPosition, startPos, Time.deltaTime);
    }

    private Vector3 FocusTarget()
    {
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + camHolder.localPosition.y, transform.position.z);
        targetPos += camHolder.forward * 15f;
        return targetPos;
    }


}
