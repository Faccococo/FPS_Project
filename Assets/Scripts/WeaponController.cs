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
    
    public int bulletsMag = 31; //һ����ϻ�ӵ�����
    public int range = 100;//�������
    public int bulletLeft = 62;//�ӵ�������
    public int currentBullet; //��ǰ�ӵ���

    private bool GunShootInput;
    public float fireRate = 0.1f; //ԽС����ٶ�Խ��
    private float fireTimer;


    public AnimationCurve recoilCurve;//�ӵ�ɢ������
    private float RecoilAngel = 0; //��ǰɢ���С
    public float recoil;// ����ɢ��Ч���ļ���
    [HideInInspector]
    private float currentRecoilTime; // ��ǰ��������������ʱ��

    private float lightTimer;
    private float lightTime;

    public ParticleSystem muzzleFlash;
    public Light muzzleFlashLight;
    public GameObject hitParticle;
    public GameObject bulletHole;

    private AudioSource audioSource;
    public AudioClip AK47SoundClip;
    public AudioClip reloadAmmoLeftClip; // ���ӵ���Ч1
    public AudioClip reloadOutOfAmmoLeftClip; // ���ӵ���Ч1

    private bool isReload;//�ж��Ƿ��ڻ���
    private bool isAiming;//�ж��Ƿ�����׼


    [Header("��λ����")]
    [SerializeField][Tooltip("��װ�ӵ�����")] private KeyCode reloadInputName;
    [SerializeField][Tooltip("�����������")] private KeyCode InspectInputName;

    [Header("UI����")]
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
        //����������Ƿ���
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
        //���ֻ��ӵ�����
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
            //���ò鿴��������
            anim.SetTrigger("Inspect");
        }
        anim.SetBool("Run",PM.isRun);
        anim.SetBool("Walk", PM.isWalk);

        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    //���
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
        Vector3 shootDirection = shooterPoint.forward + spread ;//�������
        if (Physics.Raycast(shooterPoint.position, shootDirection, out hit, range))
        {
            //Debug.Log("����" + hit.transform.name + " ʣ�൯ҩ��" + currentBullet + "/" + bulletLeft);
            GameObject hitParticleEffect = Instantiate(hitParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)); //��������Ч
            GameObject bulletHoleEffect = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            Destroy(bulletHoleEffect, 3f);
            Destroy(hitParticleEffect, 1f);
        }

        if (!isAiming)
        {
            //������ͨ���𶯻�
            anim.CrossFadeInFixedTime("fire", 0.1f);
        }
        else
        {
            //������׼���𶯻�
            anim.CrossFadeInFixedTime("aim_fire", 0.1f);

        }
        PlayerShootSound();
        muzzleFlash.Play();
        muzzleFlashLight.enabled = true;

        currentBullet--;
        UpdateAmmoUI();
        fireTimer = 0f; //���ü�ʱ��
    }

    //���Ż�������
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
            //��׼
            isAiming = true;
            anim.SetBool("Aim", true);
            crossHairUI.gameObject.SetActive(false);
            mainCamera.fieldOfView = 5; //��׼ʱ���������Ұ
        }
        else
        {
            //����׼
            isAiming = false;
            anim.SetBool("Aim", false);
            crossHairUI.gameObject.SetActive(true);
            mainCamera.fieldOfView = 60; //��׼ʱ���������Ұ
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
