using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPade : MonoBehaviour
{
    [Header("JumpPade")]
    [Range(0, 500)]
    public float bouncheHeight;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject jumpPade = collision.gameObject;
        Rigidbody rb = jumpPade.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * bouncheHeight * 5);
    }
}
