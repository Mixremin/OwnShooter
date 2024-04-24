using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MovementWithCam : MonoBehaviour
{
    public bool isLocked = false;

    [Header("HP")]

    public int maxHealth;
    public int currHealth;
    public GameController gameController;
    public HpBarMax healthBar;


    [Header("Movement")]
    //Скорость
    private float MoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    //----------------------------------------
    public float GroundDrag;
    //Прыжок
    public float JumpForce, JumpCD, AirMulti;
    bool JumpReady = true;
    [Header("Binds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    [Header("Ground Check")] 
    public float PlayerHeight;
    public LayerMask WhatIsGround;
    bool Grounded;

    public Transform orientation;
    public Transform camOrientation;
    public Transform cam; //отдельный трансформ вращения

    float HorKBInput, VerKBInput; 

    Vector3 moveDir;

    Rigidbody rb;
    public MovementState state;

    [Header("Cam")]
    public float X;
    public float Y;
    private float xRot, yRot;
    //---------------
    /// <summary>
    /// Wall run Tutorial stuff, scroll down for full movement
    /// </summary>
    [Header("Wall Run")]
    //Wallrunning
    public LayerMask whatIsWall;
    public float wallrunForce, maxWallrunTime, maxWallSpeed;
    bool isWallRight, isWallLeft;
    bool isWallRunning;
    public float maxWallRunCameraTilt, wallRunCameraTilt;

    private void WallRunInput() 
    {
        if (Input.GetKey(KeyCode.D) && isWallRight) StartWallrun();
        if (Input.GetKey(KeyCode.A) && isWallLeft) StartWallrun();
    }
    private void StartWallrun()
    {
        rb.useGravity = false;
        isWallRunning = true;

        if (rb.velocity.magnitude <= maxWallSpeed)
        {
            rb.AddForce(orientation.forward * wallrunForce * Time.deltaTime);

            if (isWallRight)
                rb.AddForce(orientation.right * wallrunForce / 5 * Time.deltaTime);
            else
                rb.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);
        }
    }
    private void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
    }
    private void CheckForWall()
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);

        if (!isWallLeft && !isWallRight) StopWallRun();
    }
    //-----------------


    public enum MovementState
    {
        walking,
        sprinting,
        air,
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        Cursor.lockState = CursorLockMode.Locked; //Лочим курсор в центре
        Cursor.visible = false; //Делаем его невидимым

    }
    private void FixedUpdate()
    {
        if (!isLocked)
        {
            PlayerMovement();
        }
    }
    private void Update()
    {
        if (!isLocked)
        {
            Grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, WhatIsGround); //Проверяем землю под ногами рейкастом

            Look();
            MoveInput();

            SpeedSet();
            StateHandler();
            CheckForWall();
            WallRunInput();

            //Тянем вниз
            if (Grounded)
            {
                rb.drag = GroundDrag;
            }
            else
            {
                rb.drag = 0;
            }
        }
    }

    private void MoveInput()
    {
        HorKBInput = Input.GetAxisRaw("Horizontal");
        VerKBInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && JumpReady && Grounded)
        {
            JumpReady = false;
            Jump();

            Invoke(nameof(JumpReset), JumpCD);
        }
    }

    private void StateHandler()
    {
        //Спринт
        if (Grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            MoveSpeed = sprintSpeed;
        }
        else if (Grounded)
        {
            state = MovementState.walking;
            MoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }
    private void PlayerMovement()
    {
        //Настраиваем направление движения  
        moveDir = orientation.forward * VerKBInput + orientation.right * HorKBInput; //Всегда идём по направлению движения
        //На земле
        if (Grounded)
        {
            rb.AddForce(moveDir.normalized * MoveSpeed * 10f, ForceMode.Force);
        }
        //В воздухе
        else if (!Grounded)
        {
            rb.AddForce(moveDir.normalized * MoveSpeed * 10f * AirMulti, ForceMode.Force);
        }
    }
    private void SpeedSet()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x,0f, rb.velocity.z);

        if (flatVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * MoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //Ставим скорость по Y на ноль чтобы высота прыжка не менялась
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }
    private void JumpReset()
    {
        JumpReady = true;
    }
    private void Look()
    {
        //Получаем данные от мыши
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * X;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * Y;

        yRot += mouseX;
        xRot -= mouseY;
        //Лочим чтобы не поднималось выше или ниже 90 градусов
        xRot = Mathf.Clamp(xRot, -90, 90);
        //Обеспечиваем вращение модельки
        cam.transform.rotation = Quaternion.Euler(xRot, yRot, wallRunCameraTilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRot, 0);

        if (Mathf.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallRight)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
        if (Mathf.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;

        //Tilts camera back again
        if (wallRunCameraTilt > 0 && !isWallRight && !isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
        if (wallRunCameraTilt < 0 && !isWallRight && !isWallLeft)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
    }

    public void Restart()
    {
        gameController.Restart();
    }
    public void TakeDamage(int damage)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            isLocked = true;
            gameController.LoseGame();
            Invoke("Restart", 2f);
        }
        healthBar.SetHealth(currHealth);
    }
}
