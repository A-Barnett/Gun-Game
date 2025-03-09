using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class M4Controller : MonoBehaviour
{
   
    [SerializeField]private float adsTime,adsTimeReturn,aimYPos,aimYRotate,fireRate,shotForce,bulletSpread,aimSpreadReduction, reloadWait;
    [SerializeField] private int ammoCount;
    [SerializeField]private Camera cam;
    [SerializeField]private GameObject bulletPrefab;
    [SerializeField]private GameObject bulletSpawn;
    [SerializeField]private ParticleSystem muzzleFlash;
    [SerializeField]private GameObject CrossHairTargetPos;
    [SerializeField]private Animator hipfireCrossHairAnimation;
    [SerializeField]private float sensitivity;
    [SerializeField]private float adsSensitivityMulti;
    [SerializeField]private Animator M4Animation;
    [SerializeField]private GameObject mag;
    [SerializeField]private GameObject UIControllerHolder;


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
    private  Quaternion targetRotation = Quaternion.Euler(new Vector3(35f, 180, 0));
    private  CharacterController ch;
    private int maxAmmo;
    private KillCounter UIController;
    private bool isReloading;
    private bool inGameMode;


    void Awake()
    {
        UIController = UIControllerHolder.GetComponent<KillCounter>();
        ch = gameObject.GetComponentInParent<CharacterController>();
        origWeaponPos = transform.localPosition;
        cam = gameObject.GetComponentInParent<Camera>();
        Recoil_Script = GetComponentInParent<CamRecoil>();
        PlayerController = GetComponentInParent<PlayerController>();
        weaponRecoil = gameObject.GetComponent<WeaponRecoil>();
        maxAmmo = ammoCount;
    }

    private void OnEnable()
    {
        inGameMode = PlayerPrefs.GetInt("GameMode") == 1;
        ammoCount = maxAmmo;
        isReloading = false;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0)&& (ammoCount>0 || !inGameMode))
        {
            PlayerController.isFire = true;
            PlayerController.isSprinting = false;
            if (Time.time > nextFire && !isReloading )
            {
                nextFire = Time.time + fireRate;
                Fire();
            }
        }
        else
        {
            PlayerController.isFire = false;
        }

        if (inGameMode)
        {
            if (Input.GetAxis("Reload") > 0 || (Input.GetMouseButton(0) && ammoCount == 0))
            {
                if (ammoCount != maxAmmo && !isReloading)
                {
                    isReloading = true;
                    M4Animation.ResetTrigger("Fire");
                    M4Animation.Play("Idle");
                    Reload();
                    StartCoroutine(ReloadWaitCoroutine());
                }
            }else
            {
                M4Animation.ResetTrigger("Reload");
            }
            UIController.ammo = ammoCount;
        }
        HandleAiming();
        HandleSprinting();
        HandleLooking();
    }
    
    private void HandleAiming()
    {
        if (Input.GetMouseButton(1)&& !isReloading)
        {
            hipfireCrossHairAnimation.gameObject.SetActive(false);
            PlayerController.isAiming = true;
            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFOV, ref smoothVelocity, smoothTime);
            transform.localPosition = Vector3.Lerp(transform.localPosition, origWeaponPos + new Vector3(-0.1520973f, aimYRotate, -0.22f), Time.deltaTime * adsTime);
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
        M4Animation.SetTrigger("Fire");
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
        ammoCount--;
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
    private void Reload()
    {
        M4Animation.SetTrigger("Reload");
        GameObject newMag = Instantiate(mag, mag.transform.position,ch.gameObject.transform.localRotation);
        newMag.transform.localScale = gameObject.transform.localScale *0.0025f;
        newMag.AddComponent<Rigidbody>().velocity = ch.velocity;
        newMag.AddComponent<BoxCollider>();
        newMag.layer = 7;
        newMag.SetActive(true);
    }

    private IEnumerator ReloadWaitCoroutine()
    {
        yield return new WaitForSeconds(reloadWait);
        ammoCount = maxAmmo;
        isReloading = false;
    }
}
