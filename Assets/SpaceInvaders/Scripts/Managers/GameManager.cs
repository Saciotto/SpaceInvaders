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
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentGameState { get; private set; } = GameState.StartingGame;
    public event Action<GameState> OnGameStateChanged = null;

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
        }
    }
}
