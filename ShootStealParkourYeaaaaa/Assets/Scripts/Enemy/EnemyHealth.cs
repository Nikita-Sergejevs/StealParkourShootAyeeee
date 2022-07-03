using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Parametrs")]
    public float maxHP = 100;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHP;
    }

    public void ApplyDamage(float dmg)
    {
        currentHealth -= dmg;
        if(currentHealth < 0)
            KillEnemy();
    }

    public void KillEnemy()
    {
        currentHealth = 0;
        Debug.Log("Enemy DEAD");
    }
}
