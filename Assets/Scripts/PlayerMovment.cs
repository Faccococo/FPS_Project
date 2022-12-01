using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    private CharacterController characterController;
    public float walkSpeed = 10f; //�ƶ��ٶ�
    public float runSpeed = 15f;//�����ٶ�
    public float speed;//�ƶ��ٶ�
    public Vector3 moveDirction;//�ƶ�����

    private Transform goundCheck;
    private float groundDistance = 0.1f;
    public LayerMask groundMask;
    public float jumpForce = 3f;
    public Vector3 velocity; //�������Y�����
    public float gravity = -98f;//��������


    private bool isGround;
    private  bool isJump;
    public bool isWalk;
    public bool isRun;




    [Header("��������")]
    [SerializeField] private AudioSource audioSource;
    public AudioClip walkingSound;
    public AudioClip runingSound;




    [Header("��λ����")]
    [SerializeField][Tooltip("���ܰ���")] private KeyCode runInputName; //���ܼ�
    [SerializeField][Tooltip("��Ծ����")] public string jumpInputName = "Jump"; //����Ҳ����ʹ��KeyCode


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
