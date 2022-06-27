using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] GameObject player;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnValue;

    void Update()
    {
        if(player.transform.position.y < -spawnValue)
        {
            RespawnPoint();
        }
    }

    void RespawnPoint()
    {
        transform.position = spawnPoint.position;
    }
}
