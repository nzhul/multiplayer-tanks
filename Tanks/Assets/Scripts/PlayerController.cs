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
    PlayerSetup _pSetup;
    PlayerShoot _pShoot;

    private void Start()
    {
        _pHealth = GetComponent<PlayerHealth>();
        _pMotor = GetComponent<PlayerMotor>();
        _pSetup = GetComponent<PlayerSetup>();
        _pShoot = GetComponent<PlayerShoot>();
    }

    Vector3 GetInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        Vector3 inputDirection = GetInput();
        _pMotor.MovePlayer(inputDirection);
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Vector3 inputDirection = GetInput();
        if (inputDirection.sqrMagnitude > 0.25f)
        {
            _pMotor.RotateChassis(inputDirection);
        }

        Vector3 turretDir = Utility.GetWorldPointFromScreenPoint(Input.mousePosition, _pMotor._turret.position.y) - _pMotor._turret.position;
        _pMotor.RotateTurret(turretDir);
    }

}
