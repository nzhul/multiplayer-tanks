using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
    [SyncVar(hook = "UpdateColor")]
    public Color _playerColor;
    public string _basename = "PLAYER";

    [SyncVar(hook = "UpdateName")]
    public int _playerNum = 1;
    public Text _playerNameText;

    private void Start()
    {
        UpdateName(_playerNum);
        UpdateColor(_playerColor);
    }

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
        CmdSetupPlayer();
    }

    private void UpdateName(int pNum)
    {
        if (_playerNameText != null)
        {
            _playerNameText.enabled = true;
            _playerNameText.text = _basename + pNum.ToString();
        }
    }

    private void UpdateColor(Color pColor)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in meshes)
        {
            r.material.color = pColor;
        }
    }

    [Command]
    void CmdSetupPlayer()
    {
        GameManager.Instance.AddPlayer(this);
        GameManager.Instance._playerCount++;
    }
}
