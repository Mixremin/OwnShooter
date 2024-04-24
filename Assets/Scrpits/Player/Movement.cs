using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Movement : MonoBehaviour
{
   


    /// <summary>
    /// Wall run done, here comes the rest of the movement script
    /// </summary>

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
    public Transform cam; //отдельный трансформ вращения

    float HorKBInput, VerKBInput; 

    Vector3 moveDir;

    Rigidbody rb;
    public MovementState state;

    //---------------
    /// <summary>
    /// Wall run Tutorial stuff, scroll down for full movement
    /// </summary>

    //Wallrunning
    public LayerMask whatIsWall;
    public float wallrunForce, maxWallrunTime, maxWallSpeed;
    bool isWallRight, isWallLeft;
    bool isWallRunning;
    public float maxWallRunCameraTilt, wallRunCameraTilt;

    private void WallRunInput() //make sure to call in void Update
    {
        //Wallrun
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

            //Make sure char sticks to wall
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
    private void CheckForWall() //make sure to call in void Update
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);

        //leave wall run
        if (!isWallLeft && !isWallRight) StopWallRun();
        //reset double jump (if you have one :D)
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

    }
    private void FixedUpdate()
    {
        PlayerMovement();
    }
    private void Update()
    {
        //Проверяем землю под ногами рейкастом
        Grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, WhatIsGround);

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
}
