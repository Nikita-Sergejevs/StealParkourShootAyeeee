using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYscale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crounchKey = KeyCode.C;

    [Header("References")]
    public Climbing climbingScript;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public float groundDrag;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYscale = transform.localScale.y;
    }

    private void Update()
    {
        // GroundCheck
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        if(grounded)
        {
            rb.drag = groundDrag;
        }else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer(); 
    }

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    public MovementState state;

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        verticalInput = Input.GetAxisRaw("Vertical") * Time.deltaTime;

        // ����� � ������
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump),jumpCooldown);
        }
        // ����� ������
        if(Input.GetKeyDown(crounchKey) && grounded)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5, ForceMode.Impulse);   
        }
        // ���������� �������
        if(Input.GetKeyUp(crounchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);

        }
    }

    private void StateHandler()
    {
        // ��� - ������
        if(Input.GetKey(crounchKey) && grounded)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        // ��� - ���
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // ��� - ������
        else if(grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // ��� - ������
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // ��������� ��� ���������� ���� ������� �� �����
        if(climbingScript.exitingWall)
        {
            return;
        }

        // ����������� ����������� ��������
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // ����� �� �����
        if(grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10, ForceMode.Force);
        }
        // ����� � �������
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10 * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // ������������ �������� ���� ����
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // ���������� �������� Y
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}