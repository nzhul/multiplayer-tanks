using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public Text _messageText;

    public int _minPlayers = 2;
    int _maxPlayers = 4;

    [SyncVar]
    public int _playerCount = 0;

    public Color[] _playerColors = { Color.red, Color.blue, Color.green, Color.magenta };

    static GameManager _instance;

    public List<PlayerController> _allPlayers;

    public List<Text> _nameLabelText;

    public List<Text> _playerScoreText;

    public int _maxScore = 3;

    [SyncVar]
    bool _gameOver = false;

    PlayerController _winner;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine("GameLoopRoutine");
    }

    private IEnumerator GameLoopRoutine()
    {
        yield return StartCoroutine(EnterLobby());
        yield return StartCoroutine(PlayGame());
        yield return StartCoroutine(EndGame());

        StartCoroutine(GameLoopRoutine());
    }

    IEnumerator EnterLobby()
    {
        while (_playerCount < _minPlayers)
        {
            UpdateMessage("waiting for players");
            DisablePlayers();
            yield return null;
        }
    }

    void UpdateMessage(string message)
    {
        if (isServer)
        {
            RpcUpdateMessage(message);
        }
    }

    [ClientRpc]
    private void RpcUpdateMessage(string message)
    {
        if (_messageText != null)
        {
            _messageText.gameObject.SetActive(true);
            _messageText.text = message;
        }
    }

    IEnumerator PlayGame()
    {
        yield return new WaitForSeconds(2f);
        UpdateMessage("3");
        yield return new WaitForSeconds(1f);
        UpdateMessage("2");
        yield return new WaitForSeconds(1f);
        UpdateMessage("1");
        yield return new WaitForSeconds(1f);
        UpdateMessage("FIGHT");
        yield return new WaitForSeconds(1f);

        EnablePlayers();
        UpdateScoreboard();

        UpdateMessage("");

        PlayerController winner = null;

        while (!_gameOver)
        {
            yield return null;
        }
    }

    IEnumerator EndGame()
    {
        DisablePlayers();

        UpdateMessage("GAME OVER \n " + _winner._pSetup._playerNameText.text + " wins!");
        Reset();

        yield return new WaitForSeconds(3f);
        UpdateMessage("Restarting ...");
        yield return new WaitForSeconds(3f);
    }

    [ClientRpc]
    void RpcSetPlayerState(bool state)
    {
        PlayerController[] allPlayers = GameObject.FindObjectsOfType<PlayerController>();
        foreach (PlayerController p in allPlayers)
        {
            p.enabled = state;
        }
    }

    void EnablePlayers()
    {
        if (isServer)
        {
            RpcSetPlayerState(true);
        }
    }

    void DisablePlayers()
    {
        if (isServer)
        {
            RpcSetPlayerState(false);
        }
    }

    public void AddPlayer(PlayerSetup pSetup)
    {
        if (_playerCount < _maxPlayers)
        {
            _allPlayers.Add(pSetup.GetComponent<PlayerController>());
            pSetup._playerColor = _playerColors[_playerCount];
            pSetup._playerNum = _playerCount + 1;
        }
    }

    [ClientRpc]
    void RpcUpdateScoreBoard(string[] playerNames, int[] playerScores)
    {
        for (int i = 0; i < _playerCount; i++)
        {
            if (playerNames[i] != null)
            {
                _nameLabelText[i].text = playerNames[i];
            }

            _playerScoreText[i].text = playerScores[i].ToString();
        }
    }

    public void UpdateScoreboard()
    {
        if (isServer)
        {
            _winner = GetWinner();
            if (_winner != null)
            {
                _gameOver = true;
            }

            string[] names = new string[_playerCount];
            int[] scores = new int[_playerCount];

            for (int i = 0; i < _playerCount; i++)
            {
                names[i] = _allPlayers[i].GetComponent<PlayerSetup>()._playerNameText.text;
                scores[i] = _allPlayers[i]._score;
            }

            RpcUpdateScoreBoard(names, scores);
        }
    }

    PlayerController GetWinner()
    {
        if (isServer)
        {
            for (int i = 0; i < _playerCount; i++)
            {
                if (_allPlayers[i]._score >= _maxScore)
                {
                    return _allPlayers[i];
                }
            }
        }

        return null;
    }

    private void Reset()
    {
        if (isServer)
        {
            RpcReset();
            UpdateScoreboard();
            _winner = null;
            _gameOver = false;
        }
    }

    [ClientRpc]
    private void RpcReset()
    {
        PlayerController[] allPlayers = GameObject.FindObjectsOfType<PlayerController>();
        foreach (PlayerController p in allPlayers)
        {
            p._score = 0;
        }
    }
}
