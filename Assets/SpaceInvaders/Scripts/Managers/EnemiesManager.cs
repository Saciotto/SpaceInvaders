using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private GameObject _squidPrefab;

    private float _spawnTimer = 0.0f;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += this.OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= this.OnGameStateChanged;
    }

    private void StartSpawningEnemies()
    {
        _spawnTimer = 0.0f;
    }

    private void SpawningEnemies()
    {
        Vector3 initialPosition = _spawnPosition.position - new Vector3(4, 0, 0);
        for (int i = 0; i < 11; i++) {
            Instantiate(_squidPrefab, initialPosition + Vector3.right * i * 0.8f, Quaternion.identity);
        }
        GameManager.Instance.SetGameState(GameState.Playing);
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.SpawningEnimies) {
            StartSpawningEnemies();
        }
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.SpawningEnimies) {
            SpawningEnemies();
        }
    }
}
