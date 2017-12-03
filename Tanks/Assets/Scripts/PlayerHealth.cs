using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = "UpdateHealthBar")]
    float _currentHealth;
    public float _maxHealth = 3;

    [SyncVar]
    public bool _isDead = false;

    public RectTransform _healthBar;

    public GameObject _deathPrefab;

    public PlayerController _lastAttacker;

    private void Start()
    {
        Reset();
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1f);
        Damage(1);
        UpdateHealthBar(_currentHealth);

        yield return new WaitForSeconds(1f);
        Damage(1);
        UpdateHealthBar(_currentHealth);

        yield return new WaitForSeconds(1f);
        Damage(1);
        UpdateHealthBar(_currentHealth);
    }

    public void Damage(float damage, PlayerController pc = null)
    {
        if (!isServer)
        {
            return;
        }

        if (pc != null && pc != this.GetComponent<PlayerController>())
        {
            _lastAttacker = pc;
        }

        _currentHealth -= damage;
        UpdateHealthBar(_currentHealth);
        if (_currentHealth <= 0 && !_isDead)
        {
            if (_lastAttacker != null)
            {
                _lastAttacker._score++;
                _lastAttacker = null;
            }

            GameManager.Instance.UpdateScoreboard();
            _isDead = true;
            RpcDie();
        }
    }

    void UpdateHealthBar(float value)
    {
        if (_healthBar != null)
        {
            _healthBar.sizeDelta = new Vector2(value / _maxHealth * 150f, _healthBar.sizeDelta.y);
        }
    }

    [ClientRpc]
    private void RpcDie()
    {
        if (_deathPrefab != null)
        {
            GameObject deathFX = Instantiate(_deathPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            GameObject.Destroy(deathFX, 3f);
        }

        SetActiveState(false);

        gameObject.SendMessage("Disable");
    }

    private void SetActiveState(bool state)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = state;
        }

        foreach (Canvas c in GetComponentsInChildren<Canvas>())
        {
            c.enabled = state;
        }

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = state;
        }


    }

    public void Reset()
    {
        _currentHealth = _maxHealth;
        SetActiveState(true);
        _isDead = false;
    }
}
