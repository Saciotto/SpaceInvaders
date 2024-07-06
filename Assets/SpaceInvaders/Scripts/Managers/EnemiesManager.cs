using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemiesManager : MonoBehaviour
{
    [SerializeField] private GameObject _squidPrefab;
    [SerializeField] private GameObject _crabPrefab;
    [SerializeField] private GameObject _octopusPrefab;

    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private float _spawnInterval;
    [SerializeField] private float _horizontalDistance;
    [SerializeField] private float _verticalDistance;
    [SerializeField] private int _spawnSquidRows;
    [SerializeField] private int _spawnCrabRows;
    [SerializeField] private int _spawnOctopusRows;
    [SerializeField] private int _spawnCols;

    private float _spawnTimer = 0.0f;
    private List<EnemyData> _enemiesData;
    private List<GameObject> _enemies;
    private int _spawnedEnemies = 0;
    private int _spawnRows = 0;

    private class EnemyData
    {
        public GameObject Prefab;
        public Vector3 Position;
    }

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += this.OnGameStateChanged;
    }

    private void Start()
    {
        _enemies = new List<GameObject>(_spawnRows * _spawnCols);
        _enemiesData = new List<EnemyData>(_spawnRows * _spawnCols);
        _spawnRows = _spawnSquidRows + _spawnCrabRows + _spawnOctopusRows;

        GameObject prefab = _squidPrefab;
        float yOffset = 0;

        int squidRows = _spawnSquidRows;
        int crabRows = _spawnCrabRows;
        int octopusRows = _spawnOctopusRows;

        float enemyWidth = _octopusPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        for (int row = 0; row < _spawnRows; row++) {
            if (squidRows > 0) {
                prefab = _squidPrefab;
                squidRows--;
            } else if (crabRows > 0) {
                prefab = _crabPrefab;
                crabRows--;
            } else if (octopusRows > 0) {
                prefab = _octopusPrefab;
                octopusRows--;
            } else {
               break;
            }
            float enemyHeight = prefab.GetComponent<SpriteRenderer>().bounds.size.y;
            float totalRowWidth = enemyWidth * _spawnCols + _horizontalDistance * Mathf.Max(_spawnCols - 1, 0);
            float xOffset = -totalRowWidth / 2.0f + enemyWidth / 2.0f;
            float xMultiplier = enemyWidth + _horizontalDistance;
            for (int col = 0; col < _spawnCols; col++) {
                EnemyData data = new EnemyData();
                data.Prefab = prefab;
                data.Position = _spawnPosition.position + new Vector3(xOffset + xMultiplier * col, -yOffset, 0);
                _enemiesData.Add(data);
            }
            yOffset += enemyHeight + _verticalDistance;
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= this.OnGameStateChanged;
    }

    private void StartSpawningEnemies()
    {
        _spawnTimer = 0.0f;
        _spawnedEnemies = 0;
    }

    private void SpawnEnemy(EnemyData data)
    {
        GameObject enemy = Instantiate(data.Prefab, data.Position, Quaternion.identity);
        enemy.GetComponent<Animator>().speed = 0;
        _enemies.Add(enemy);
        _spawnedEnemies++;
    }

    private void StartEnemiesAnimations()
    {
        foreach (GameObject enemy in _enemies) {
            enemy.GetComponent<Animator>().speed = 1;
        }
    }

    private void SpawningEnemies()
    {
        _spawnTimer += Time.deltaTime;
        while (_spawnTimer > _spawnedEnemies * _spawnInterval && _spawnedEnemies < _enemiesData.Count) {
            EnemyData data = _enemiesData[_spawnedEnemies];
            SpawnEnemy(data);
        }
        if (_spawnedEnemies == _enemiesData.Count) {
            StartEnemiesAnimations();
            GameManager.Instance.SetGameState(GameState.Playing);
        }
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
