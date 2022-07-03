using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public float fireRate = 0.2f;
    public int clipsize = 12;
    public int reservedAmmoCapacity = 270;
    public float damage;
    public int maxDistance = 100;
    public GameObject _bulletHolePrefabe;
    public ParticleSystem muzzleFlash;
    public LayerMask Enemy;

    bool _canShoot;
    int _currentAmmoInClip;
    int _ammonInReserve;

    private void Start()
    {
        _currentAmmoInClip = clipsize;
        _ammonInReserve = reservedAmmoCapacity;
        _canShoot = true;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
        {
            _canShoot = false;
            _currentAmmoInClip--;
            StartCoroutine(ShootGun());
            muzzleFlash.Play();

            RaycastHit hit;
            if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit))
            {
                GameObject obj = Instantiate(_bulletHolePrefabe, hit.point, Quaternion.LookRotation(hit.normal));
                obj.transform.position += obj.transform.forward / 10;
                Destroy(obj, 5);

            }

        }
        else if(Input.GetKeyDown(KeyCode.R) && _currentAmmoInClip < clipsize && _ammonInReserve > 0)
        {
            int amountNeed = clipsize - _currentAmmoInClip;
            if (amountNeed >= _ammonInReserve)
            {
                _currentAmmoInClip += _ammonInReserve;
                _ammonInReserve -= amountNeed;
            }
            else
            {
                _currentAmmoInClip = clipsize;
                _ammonInReserve -= amountNeed;
            }
        }
    }

    IEnumerator ShootGun()
    {
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }
}