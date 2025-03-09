using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGunController : MonoBehaviour
{
    [SerializeField] private int gun;
    [SerializeField] private SelectWeapon selectWeapon;
    public void ChangeWeapon()
    {
        selectWeapon.SwitchGun(gun);
    }
}
