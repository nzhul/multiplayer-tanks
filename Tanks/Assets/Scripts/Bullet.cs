using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    Rigidbody _rigidbody;

    Collider _collider;

    List<ParticleSystem> _allParticles;

    public int _speed = 100;

    public float _lifetime = 5f;

    public ParticleSystem _explosionFx;

    public List<string> _bounceTags;

    public int _bounces = 2;


    private void Start()
    {
        _allParticles = GetComponentsInChildren<ParticleSystem>().ToList();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        StartCoroutine(SelftDestruct());
    }

    private IEnumerator SelftDestruct()
    {
        yield return new WaitForSeconds(_lifetime);
        Explode();

    }

    private void Explode()
    {
        _collider.enabled = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.Sleep();

        foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
        {
            m.enabled = false;
        }

        foreach (ParticleSystem ps in _allParticles)
        {
            ps.Stop();
        }

        if (_explosionFx != null)
        {
            _explosionFx.transform.parent = null;
            _explosionFx.Play();
        }

        Destroy(gameObject);
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
        if (_bounceTags.Contains(collision.gameObject.tag))
        {
            if (_bounces <= 0)
            {
                Explode();
            }

            _bounces--;
        }
    }



}
