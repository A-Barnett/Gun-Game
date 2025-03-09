using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtonInputController : MonoBehaviour
{
    [SerializeField]private Camera buttonCamera;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(buttonCamera.transform.position, buttonCamera.transform.forward, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Button"))
                {
                    hit.collider.gameObject.GetComponent<ButtonController>().RunButton();
                }else if (hit.collider.gameObject.CompareTag("Range"))
                {
                    hit.collider.gameObject.GetComponent<RangeController>().ChangeRange();
                }else if (hit.collider.gameObject.CompareTag("GunSelect"))
                {
                    hit.collider.gameObject.GetComponent<SelectGunController>().ChangeWeapon();
                }else if (hit.collider.gameObject.CompareTag("MoveSelect"))
                {
                    hit.collider.gameObject.GetComponent<TargetMoveSelect>().ChangeMoveMode();
                }
            }
        }
    }
}
