using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float walljumpUpForce;
    public float walljumpSideForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    private float horiznotalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exeting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce; 

    [Header("Refences")]
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if(pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        horiznotalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Начинает бег по стене
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if(!pm.wallrunning)
            {
                StarWallRun();
            }
            if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime; 
            }
            if(wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                WallJump();
            }
        }
        // Покидает стену для бега
        else if(exitingWall)
        {
            if(pm.wallrunning)
            {
                StopWallRun();
            }
            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }
            if(exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        // Заканчивает бежать по стене
        else
        {
            if(pm.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    private void StarWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;
        
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if(useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 forceToApplay = transform.up * walljumpUpForce + wallNormal * walljumpSideForce;

        // Убирает скорость у и добовляет силу
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApplay, ForceMode.Impulse);
    }
}
