using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("SpawnPoint")]
    public Transform player;
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = Color.green;

        spawnPoint.gameObject.transform.position = gameObject.transform.position;
    }
}