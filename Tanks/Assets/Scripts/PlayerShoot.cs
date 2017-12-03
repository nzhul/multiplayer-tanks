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

    }

    public void Shoot()
    {
        if (_isReloading || _bulletPrefab == null)
        {
            return;
        }

        CmdShoot();

        _shotsLeft--;

        if (_shotsLeft <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    [Command]
    private void CmdShoot()
    {
        Bullet bullet = null;
        // bullet = _bulletPrefab.GetComponent<Bullet>();

        Rigidbody rbody = Instantiate(_bulletPrefab, _bulletSpawn.position, _bulletSpawn.rotation) as Rigidbody;
        bullet = rbody.gameObject.GetComponent<Bullet>();

        if (rbody != null)
        {
            rbody.velocity = bullet._speed * _bulletSpawn.transform.forward;
            bullet._owner = GetComponent<PlayerController>();
            NetworkServer.Spawn(rbody.gameObject);
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
