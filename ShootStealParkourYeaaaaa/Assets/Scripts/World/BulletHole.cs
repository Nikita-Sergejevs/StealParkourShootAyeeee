using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [SerializeField] private GameObject _bulletHolePrefabe;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit))
            {
                GameObject obj = Instantiate(_bulletHolePrefabe, hit.point, Quaternion.LookRotation(hit.normal));   

            }
        }
    }
}
