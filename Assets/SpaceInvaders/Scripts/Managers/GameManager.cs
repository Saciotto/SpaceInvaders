using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    StartingGame,
    SpawningEnimies,
    Playing,
    Die,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Transform _playerSpawn;
    [SerializeField] Player _playerPrefab;

    public GameState CurrentGameState { get; private set; } = GameState.StartingGame;
    public event Action<GameState> OnGameStateChanged = null;
    public Transform TopLimit;
    public Transform BottomLimit;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start()
    {
        SetGameState(GameState.SpawningEnimies);
    }

    public void SetGameState(GameState newState)
    {
        if (CurrentGameState != newState) {
            CurrentGameState = newState;
            OnGameStateChanged?.Invoke(CurrentGameState);

            if (newState == GameState.Die) {
                ScoreManager.Instance.DecrementLives();
                if (ScoreManager.Instance.Lives > 0) {
                    StartCoroutine(Respawn());
                } else {
                    SetGameState(GameState.GameOver);
                }
            }
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.2f);
        Instantiate(_playerPrefab, _playerSpawn.position, Quaternion.identity);
        SetGameState(GameState.Playing);
    }
}
