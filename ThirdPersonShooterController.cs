using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using System.Runtime.CompilerServices;
public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    //  ObjectPooler objectPooler;
    private Vector3 mouseWorldPosition = Vector3.zero;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }
    public void Start()
    {
        //        objectPooler = ObjectPooler.Instance;
    }
    private void Update()
    {
        if (starterAssetsInputs.fps)
        {
            //Debug.Log("ENTERED FPS STATE");
            FirstPersonState();
        }
        else if (!starterAssetsInputs.fps)
        {
            //Debug.Log("ENTERED TPS STATE");
            ThirdPersonState();
        }

    }
    private void FirstPersonState()
    {
        firstPersonCamera.gameObject.SetActive(true);
        AimAndShoot();
        
    }
    private void ThirdPersonState() 
    {
        firstPersonCamera.gameObject.SetActive(false);
        if (starterAssetsInputs.aim)
        {
            AimAndShoot();
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            firstPersonCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
        }
    }
 
    private void AimAndShoot()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit rayCastHit, 9999f, aimColliderLayerMask))
        {
            debugTransform.position = rayCastHit.point;
            mouseWorldPosition = rayCastHit.point;
            hitTransform = rayCastHit.transform;
        }

        aimVirtualCamera.gameObject.SetActive(true);
        thirdPersonController.SetSensitivity(aimSensitivity);
        thirdPersonController.SetRotateOnMove(false);
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

        if (starterAssetsInputs.shoot)
        {
            if (hitTransform != null)
            {
                if (hitTransform.GetComponent<Target>() != null)
                {
                    Instantiate(vfxHitRed, mouseWorldPosition, Quaternion.identity);
                }
                else
                {
                    Instantiate(vfxHitGreen, mouseWorldPosition, Quaternion.identity);
                }
            }
             Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;

            Debug.Log("aim dir: " + aimDir);
             GameObject bullet = ObjectPool.instance.GetPooledObjectOne();
             if (bullet != null) 
             {
                 bullet.transform.position = spawnBulletPosition.position;
                 bullet.transform.rotation = Quaternion.LookRotation(aimDir,Vector3.up);
                 bullet.SetActive(true);

             }
            starterAssetsInputs.shoot = false;
        }
    }
}


//            Instantiate(pfBulletProjectile, spawnBulletPosition.position , Quaternion.LookRotation(aimDir, Vector3.up));
//            objectPooler.SpawnFromPool("bullet",spawnBulletPosition.position, Quaternion.LookRotation(aimDir,Vector3.up));