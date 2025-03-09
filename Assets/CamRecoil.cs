using UnityEngine;

public class CamRecoil : MonoBehaviour
{
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    [SerializeField] private float adsRecoilX;
    [SerializeField] private float asdRecoilY;
    [SerializeField] private float adsRecoilZ;
    [SerializeField] private float snap;
    [SerializeField] private float returnSpeed;
    
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    
    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation,targetRotation,snap*Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
    public void RecoilFireAim()
    {
        targetRotation += new Vector3(adsRecoilX, Random.Range(-asdRecoilY, asdRecoilY), Random.Range(-adsRecoilZ, adsRecoilZ));
    }
}
