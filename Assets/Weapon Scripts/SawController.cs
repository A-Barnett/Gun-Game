using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour
{
    [SerializeField]private float adsTime,adsTimeReturn,aimYPos,aimYRotate,fireRate,shotForce,bulletSpread,aimSpreadReduction;
    [SerializeField]private Camera cam;
    [SerializeField]private GameObject bulletPrefab;
    [SerializeField]private GameObject bulletSpawn;
    [SerializeField]private ParticleSystem muzzleFlash;
    [SerializeField]private GameObject CrossHairTargetPos;
    [SerializeField]private Animator hipfireCrossHairAnimation;
    [SerializeField]private float sensitivity;
    [SerializeField]private float adsSensitivityMulti;
    [SerializeField]private Animator SawAnimation;


    private float originalFOV = 90;
    private float targetFOV = 50;
    private float smoothTime = 0.3f;
    private float smoothVelocity = 1;
    private float nextFire;
    private Vector3 bulletDirection;
    private Vector3 origWeaponPos;
    private CamRecoil Recoil_Script;
    private PlayerController PlayerController;
    private WeaponRecoil weaponRecoil;
    private  Quaternion targetRotation = Quaternion.Euler(new Vector3(25f, 180, 0));

    void Start()
    {
        origWeaponPos = transform.localPosition;
        cam = gameObject.GetComponentInParent<Camera>();
        Recoil_Script = GetComponentInParent<CamRecoil>();
        PlayerController = GetComponentInParent<PlayerController>();
        weaponRecoil = gameObject.GetComponent<WeaponRecoil>();
    }
    
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            PlayerController.isFire = true;
            PlayerController.isSprinting = false;
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Fire();
            }
        }
        else
        {
            PlayerController.isFire = false;
        }
        HandleAiming();
        HandleSprinting();
        HandleLooking();
    }
    
    private void HandleAiming()
    {
        if (Input.GetMouseButton(1))
        {
            hipfireCrossHairAnimation.gameObject.SetActive(false);
            PlayerController.isAiming = true;
            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFOV, ref smoothVelocity, smoothTime);
            transform.localPosition = Vector3.Lerp(transform.localPosition, origWeaponPos + new Vector3(-0.1505973f, aimYRotate, -0.24f), Time.deltaTime * adsTime);
            transform.localRotation = Quaternion.Euler(new Vector3(aimYPos, 180, 0));
        }
        else
        {
            hipfireCrossHairAnimation.gameObject.SetActive(true);
            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, originalFOV, ref smoothVelocity, smoothTime);
            PlayerController.isAiming = false;
            if (!PlayerController.isSprinting)
            { 
                transform.localPosition = Vector3.Lerp(transform.localPosition, origWeaponPos, Time.deltaTime * adsTimeReturn);
            }
        }
    }
    private void Fire()
    {
        SawAnimation.Play("SawFire",0,0f);
        Vector3 bulletPos = bulletSpawn.transform.position;
        Quaternion bulletRot = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
        RaycastHit hit;
        
        float spread = PlayerController.isAiming ? bulletSpread * aimSpreadReduction : bulletSpread;
        Vector2 randomCircle = Random.insideUnitCircle * spread;
        Vector3 randomSpread = new Vector3(randomCircle.x, randomCircle.y, 0);

        if (Physics.Raycast(bulletPos, CrossHairTargetPos.transform.forward, out hit))
        {
            Vector3 targetPos = hit.point;
            bulletDirection = (targetPos - bulletPos);
        }
        else
        {
            bulletDirection = cam.transform.forward;
        }
        bulletDirection = (bulletDirection + randomSpread).normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletPos, bulletRot);
        bullet.transform.Rotate(Vector3.right, 90);
        Rigidbody rbB = bullet.GetComponent<Rigidbody>();
        rbB.AddForce(bulletDirection.normalized*shotForce, ForceMode.Impulse);
        if (PlayerController.isAiming)
        {
            weaponRecoil.ApplyRecoilADS();
            Recoil_Script.RecoilFireAim();
        }
        else
        {
            weaponRecoil.ApplyRecoil();
            muzzleFlash.Play();
            Recoil_Script.RecoilFire();
            hipfireCrossHairAnimation.Play("Fire", 0, 0f);
        }
    }
    private void HandleSprinting()
    {
        if (!PlayerController.isAiming && PlayerController.isSprinting && !PlayerController.isFire)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime*5);
            transform.localPosition = Vector3.Lerp(transform.localPosition, origWeaponPos, Time.deltaTime * 5f);
        }
    }
    private void HandleLooking()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float clampedY;
        if (PlayerController.isAiming)
        {
            PlayerController.gameObject.transform.Rotate(Vector3.up, mouseX * sensitivity * adsSensitivityMulti);
            clampedY = cam.transform.localEulerAngles.x - mouseY * sensitivity * adsSensitivityMulti;
        }
        else
        {
            PlayerController.gameObject.transform.Rotate(Vector3.up, mouseX * sensitivity);
            clampedY = cam.transform.localEulerAngles.x - mouseY * sensitivity;
        }
     
        cam.transform.localEulerAngles = new Vector3(clampedY, 0, 0);
    }

}

