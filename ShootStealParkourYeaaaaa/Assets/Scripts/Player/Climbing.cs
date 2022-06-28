using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerCam cam;
    public PlayerMovement pm;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    [Header("ClimbJumping")]
    public float climbJumpJumpUpForce;
    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;

    public int climbJumps;
    private int climbjumpsLeft;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Detecrion")]
    public float detectionlenght;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private bool climbing;
    private bool wallFront;

    private RaycastHit frontWallHit;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;  

    private void Update()
    {
        WallCheck();
        StateMachine();

        if(climbing && !exitingWall)
        {
            ClimbingMovement();
        }
    }

    private void StateMachine()
    {
        // Ползем
        if(wallFront && Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if(!climbing && climbTimer > 0) StartClimbing();
            // Таймер
            if(climbTimer > 0) climbTimer -= Time.deltaTime;
            if(climbTimer < 0) StopClimbing();
        }
        // Отходим от стены
        else if(exitingWall)
        {
            StopClimbing();

            if(exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if(exitWallTimer < 0) exitingWall = false;
        }
        // Не ползем
        else
        {
            if(climbing)
            {
                StopClimbing();
            }
        }
        // Прыжки от стен
        if(wallFront && Input.GetKeyDown(jumpKey) && climbjumpsLeft > 0)
        {
            ClimbJump();
        }
    }

    private void WallCheck()
    {
        // Проверяем находимся ли мы у стены и делаем ограничения радиуса обзора для того что бы пониматься по стене
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionlenght, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal,frontWallHit.normal)) > minWallNormalAngleChange;

        if((wallFront && newWall) || pm.grounded)
        {
            climbTimer = maxClimbTime;
            climbjumpsLeft = climbJumps;
        }
    }

    private void StartClimbing()
    {
        climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;

        // Fov
        cam.DoFov(65);
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        climbing= false;

        // Убираем Fov
        cam.DoFov(75);
    }

    private void ClimbJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 forceToApply = transform.up * climbJumpJumpUpForce + frontWallHit.normal * climbJumpBackForce;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbjumpsLeft--;
    }
}
