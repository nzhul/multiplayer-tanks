using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
    public string _basename = "PLAYER";
    public int _playerNum = 1;
    public Text _playerNameText;

    public Color _playerColor;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (_playerNameText != null)
        {
            _playerNameText.enabled = false;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in meshes)
        {
            r.material.color = _playerColor;
        }

        if (_playerNameText != null)
        {
            _playerNameText.enabled = true;
            _playerNameText.text = _basename + _playerNum.ToString();
        }
    }

}
