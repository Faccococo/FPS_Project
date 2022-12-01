using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//
public class WeaponController : MonoBehaviour
{
    public PlayerMovment PM;
    public Transform shooterPoint;
    public Transform GunCamera;
    
    public int bulletsMag = 31; //一个弹匣子弹数量
    public int range = 100;//武器射程
    public int bulletLeft = 62;//子弹总数量
    public int currentBullet; //当前子弹数

    private bool GunShootInput;
    public float fireRate = 0.1f; //越小射击速度越快
    private float fireTimer;


    public AnimationCurve recoilCurve;//子弹散射曲线
    private float RecoilAngel = 0; //当前散射大小
    public float recoil;// 用于散射效果的减弱
    [HideInInspector]
    private float currentRecoilTime; // 当前后坐力曲线曲线时间

    private float lightTimer;
    private float lightTime;

    public ParticleSystem muzzleFlash;
    public Light muzzleFlashLight;
    public GameObject hitParticle;
    public GameObject bulletHole;

    private AudioSource audioSource;
    public AudioClip AK47SoundClip;
    public AudioClip reloadAmmoLeftClip; // 换子弹音效1
    public AudioClip reloadOutOfAmmoLeftClip; // 换子弹音效1

    private bool isReload;//判断是否在换弹
    private bool isAiming;//判断是否在瞄准


    [Header("键位设置")]
    [SerializeField][Tooltip("填装子弹按键")] private KeyCode reloadInputName;
    [SerializeField][Tooltip("检查武器按键")] private KeyCode InspectInputName;

    [Header("UI设置")]
    public Image crossHairUI;
    public Text AmmoTextUI;

    private Animator anim;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        reloadInputName = KeyCode.R;
        InspectInputName = KeyCode.F;
        currentBullet = bulletsMag;
        UpdateAmmoUI();
    }

    // Update is called once per frame
    
    void Update()
    {
        //Debug.Log(mainCamera.fieldOfView);
        //检测鼠标左键是否按下
        GunShootInput = Input.GetMouseButton(0);
        if (GunShootInput && currentBullet > 0)
        {
            currentRecoilTime += Time.deltaTime;
            float recoilFraction = currentRecoilTime * recoil * 0.1f;
            RecoilAngel = recoilCurve.Evaluate(recoilFraction);
            //Debug.Log(RecoilAngel);
            GunFire();
        }
        else
        {
            currentRecoilTime = 0;
            muzzleFlashLight.enabled = false;
        }

        

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        //两种换子弹动画
        if (info.IsName("reload_ammo_left")||info.IsName("reload_out_of_ammo"))
        {
            isReload = true;
        }
        else
        {
            isReload = false;
        }

        if ((Input.GetKeyDown(reloadInputName) && currentBullet < bulletsMag && bulletLeft > 0) || (currentBullet == 0 && bulletLeft > 0))
        {
            Reload();
        }
        DoingAim();
        if (Input.GetKeyDown(InspectInputName))
        {
            //设置查看武器动画
            anim.SetTrigger("Inspect");
        }
        anim.SetBool("Run",PM.isRun);
        anim.SetBool("Walk", PM.isWalk);

        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    //射击
    public void GunFire()
    {
        if (fireTimer < fireRate || currentBullet <= 0 || isReload||PM.isRun)
        {
            //muzzleFlashLight.enabled = false;
            return;
        }

        RaycastHit hit;
        Vector3 spread = Random.insideUnitCircle * RecoilAngel * mainCamera.fieldOfView * 0.005f;
        if (isAiming) spread = spread * 0.1f;
        Debug.Log(spread.x + " " + spread.y);
        Vector3 shootDirection = shooterPoint.forward + spread ;//射击方向
        if (Physics.Raycast(shooterPoint.position, shootDirection, out hit, range))
        {
            //Debug.Log("命中" + hit.transform.name + " 剩余弹药：" + currentBullet + "/" + bulletLeft);
            GameObject hitParticleEffect = Instantiate(hitParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)); //创造火光特效
            GameObject bulletHoleEffect = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            Destroy(bulletHoleEffect, 3f);
            Destroy(hitParticleEffect, 1f);
        }

        if (!isAiming)
        {
            //播放普通开火动画
            anim.CrossFadeInFixedTime("fire", 0.1f);
        }
        else
        {
            //播放瞄准开火动画
            anim.CrossFadeInFixedTime("aim_fire", 0.1f);

        }
        PlayerShootSound();
        muzzleFlash.Play();
        muzzleFlashLight.enabled = true;

        currentBullet--;
        UpdateAmmoUI();
        fireTimer = 0f; //重置计时器
    }

    //播放换弹动画
    public void DoReloadAnimation()
    {
        if (currentBullet > 0) 
        {
            anim.Play("reload_ammo_left", 0, 0);
            audioSource.clip = reloadAmmoLeftClip;
            audioSource.Play();
        }
        if (currentBullet == 0)
        {
            anim.Play("reload_out_of_ammo", 0, 0);
            audioSource.clip = reloadOutOfAmmoLeftClip ;
            audioSource.Play();
        }

    }

    public void Reload()
    {
        if (bulletLeft <= 0) return;
        DoReloadAnimation();
        int bulletToLoad = bulletsMag - currentBullet;
        int bulletToReduce = (bulletLeft >= bulletToLoad) ? bulletToLoad : bulletLeft;
        currentBullet += bulletToReduce;
        bulletLeft -= bulletToReduce;
        UpdateAmmoUI();
    }

    public void PlayerShootSound()
    {
        audioSource.clip = AK47SoundClip;
        audioSource.Play();
    }

    public void DoingAim()
    {
        if (Input.GetMouseButton(1) && !isReload && !PM.isRun) 
        {
            //瞄准
            isAiming = true;
            anim.SetBool("Aim", true);
            crossHairUI.gameObject.SetActive(false);
            mainCamera.fieldOfView = 5; //瞄准时的摄像机视野
        }
        else
        {
            //非瞄准
            isAiming = false;
            anim.SetBool("Aim", false);
            crossHairUI.gameObject.SetActive(true);
            mainCamera.fieldOfView = 60; //瞄准时的摄像机视野
        }
    }

    public void UpdateAmmoUI()
    {
        AmmoTextUI.text = currentBullet + "/" + bulletLeft;
    }

    public void WeaponRecoil()
    {
        
    }

}
