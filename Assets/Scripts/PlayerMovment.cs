using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    private CharacterController characterController;
    public float walkSpeed = 10f; //移动速度
    public float runSpeed = 15f;//奔跑速度
    public float speed;//移动速度
    public Vector3 moveDirction;//移动方向

    private Transform goundCheck;
    private float groundDistance = 0.1f;
    public LayerMask groundMask;
    public float jumpForce = 3f;
    public Vector3 velocity; //设置玩家Y轴冲量
    public float gravity = -98f;//设置重力


    private bool isGround;
    private  bool isJump;
    public bool isWalk;
    public bool isRun;




    [Header("声音设置")]
    [SerializeField] private AudioSource audioSource;
    public AudioClip walkingSound;
    public AudioClip runingSound;




    [Header("键位设置")]
    [SerializeField][Tooltip("奔跑按键")] private KeyCode runInputName; //奔跑键
    [SerializeField][Tooltip("跳跃按键")] public string jumpInputName = "Jump"; //这里也可以使用KeyCode


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        runInputName = KeyCode.LeftShift;
        goundCheck = GameObject.Find("Player/CheckGround").GetComponent<Transform>();
    }


    private void Update()
    {
        CheckGround();
        Move();
    }

    public void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        isRun = Input.GetKey(runInputName);
        isWalk = (Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0) ? true : false;

        speed = isRun ? runSpeed : walkSpeed;

        moveDirction = (transform.right * h + transform.forward * v).normalized;

        characterController.Move(moveDirction * speed * Time.deltaTime);

        if (isGround == false)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
        Jump();
        PlayFootStepSound();
    }

    public void PlayFootStepSound()
    {
        if (isGround && moveDirction.sqrMagnitude > 0.9f)
        {
            audioSource.clip = isRun ? runingSound : walkingSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    public void Jump()
    {
        isJump = Input.GetButtonDown(jumpInputName);//Better than GetKey
        if (isJump && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    public void CheckGround()
    {
        isGround = Physics.CheckSphere(goundCheck.position, groundDistance, groundMask);
        if (isGround && velocity.y <= 0)
        {
            velocity.y = -2f;
        }
    }
}
