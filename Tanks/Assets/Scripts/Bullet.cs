using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : NetworkBehaviour
{
    Rigidbody _rigidbody;

    Collider _collider;

    List<ParticleSystem> _allParticles;

    public int _speed = 100;

    public float _lifetime = 5f;

    public ParticleSystem _explosionFx;

    public List<string> _bounceTags;

    public List<string> _collisionTags;

    public int _bounces = 2;

    public float _damage = 1f;

    public PlayerController _owner;

    public float _delay = 0.04f;

    private void Start()
    {
        _allParticles = GetComponentsInChildren<ParticleSystem>().ToList();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        StartCoroutine(SelftDestruct());
    }

    private IEnumerator SelftDestruct()
    {
        _collider.enabled = false;

        yield return new WaitForSeconds(_delay);

        _collider.enabled = true;

        yield return new WaitForSeconds(_lifetime);
        Explode();

    }

    private void Explode()
    {
        _collider.enabled = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.Sleep();

        foreach (ParticleSystem ps in _allParticles)
        {
            ps.Stop();
        }

        if (_explosionFx != null)
        {
            _explosionFx.transform.parent = null;
            _explosionFx.Play();
            Destroy(_explosionFx.gameObject, 2f); // TODO: use object pool here.
        }

        if (isServer)
        {
            Destroy(gameObject);
            foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
            {
                m.enabled = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_rigidbody.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollisions(collision);

        if (_bounceTags.Contains(collision.gameObject.tag))
        {
            if (_bounces <= 0)
            {
                Explode();
            }

            _bounces--;
        }
    }

    void CheckCollisions(Collision collision)
    {
        if (_collisionTags.Contains(collision.collider.tag))
        {
            Explode();
            PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Damage(_damage, _owner);
            }
        }
    }



}
