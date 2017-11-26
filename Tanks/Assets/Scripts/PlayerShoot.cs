using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    public Rigidbody _bulletPrefab;

    public Transform _bulletSpawn;

    public int _shotsPerBurst = 2;

    int _shotsLeft;

    bool _isReloading;

    public float _reloadTime = 1f;


    // Use this for initialization
    void Start()
    {
        _shotsLeft = _shotsPerBurst;
        _isReloading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (_isReloading || _bulletPrefab == null)
        {
            return;
        }

        Bullet bullet = null;
        bullet = _bulletPrefab.GetComponent<Bullet>();

        Rigidbody rbody = Instantiate(_bulletPrefab, _bulletSpawn.position, _bulletSpawn.rotation) as Rigidbody;

        if (rbody != null)
        {
            rbody.velocity = bullet._speed * _bulletSpawn.transform.forward;
        }

        _shotsLeft--;

        if (_shotsLeft <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        _shotsLeft = _shotsPerBurst;
        _isReloading = true;

        yield return new WaitForSeconds(_reloadTime);

        _isReloading = false;
    }
}
