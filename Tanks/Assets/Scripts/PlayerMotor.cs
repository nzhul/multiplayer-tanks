using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{

    public Rigidbody _rigidBody;

    public Transform _chassis;

    public Transform _turret;

    public float _moveSpeed = 100f;

    public float _chassisRotateSpeed = 1f;

    public float _turretRotateSpeed = 3f;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void MovePlayer(Vector3 dir)
    {
        Vector3 moveDirection = dir * _moveSpeed * Time.deltaTime;
        _rigidBody.velocity = moveDirection;
    }

    public void FaceDirection(Transform xform, Vector3 dir, float rotSpeed)
    {
        if (dir != Vector3.zero && xform != null)
        {
            Quaternion desiredRot = Quaternion.LookRotation(dir);
            xform.rotation = Quaternion.Slerp(xform.rotation, desiredRot, rotSpeed * Time.deltaTime);
        }


    }

    public void RotateChassis(Vector3 dir)
    {
        FaceDirection(_chassis, dir, _chassisRotateSpeed);
    }

    public void RotateTurret(Vector3 dir)
    {
        FaceDirection(_turret, dir, _turretRotateSpeed);
    }

}
