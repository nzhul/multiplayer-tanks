using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerShoot))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerSetup))]
public class PlayerController : NetworkBehaviour
{

    PlayerHealth _pHealth;
    PlayerMotor _pMotor;
    public PlayerSetup _pSetup;
    PlayerShoot _pShoot;

    Vector3 _originalPosition;
    NetworkStartPosition[] _spawnPoints;

    public GameObject _spawnFx;

    public int _score;

    private void Start()
    {
        _pHealth = GetComponent<PlayerHealth>();
        _pMotor = GetComponent<PlayerMotor>();
        _pSetup = GetComponent<PlayerSetup>();
        _pShoot = GetComponent<PlayerShoot>();

        GameManager gm = GameManager.Instance;
    }

    public override void OnStartLocalPlayer()
    {
        _spawnPoints = GameObject.FindObjectsOfType<NetworkStartPosition>();
        _originalPosition = transform.position;
    }

    Vector3 GetInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer || _pHealth._isDead)
        {
            return;
        }
        Vector3 inputDirection = GetInput();
        _pMotor.MovePlayer(inputDirection);
    }

    private void Update()
    {
        if (!isLocalPlayer || _pHealth._isDead)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _pShoot.Shoot();
        }

        Vector3 inputDirection = GetInput();
        if (inputDirection.sqrMagnitude > 0.25f)
        {
            _pMotor.RotateChassis(inputDirection);
        }

        Vector3 turretDir = Utility.GetWorldPointFromScreenPoint(Input.mousePosition, _pMotor._turret.position.y) - _pMotor._turret.position;
        _pMotor.RotateTurret(turretDir);
    }

    void Disable()
    {
        StartCoroutine("RespawnRoutine");
    }

    IEnumerator RespawnRoutine()
    {
        transform.position = GetRandomSpawnPosition();
        _pMotor._rigidBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(3f);
        _pHealth.Reset();

        if (_spawnFx != null)
        {
            GameObject spawnFx = Instantiate(_spawnFx, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            Destroy(spawnFx, 3f);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (_spawnPoints != null)
        {
            if (_spawnPoints.Length > 0)
            {
                NetworkStartPosition startPos = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
                return startPos.transform.position;
            }
        }

        return _originalPosition;
    }

}
