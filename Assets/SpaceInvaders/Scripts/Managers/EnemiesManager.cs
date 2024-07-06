using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class DificultyLevel
{
    public float MovimentInterval;
    public int Enemies;
}

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance { get; private set; }

    [SerializeField] private GameObject _squidPrefab;
    [SerializeField] private GameObject _crabPrefab;
    [SerializeField] private GameObject _octopusPrefab;
    [SerializeField] private Projectile[] _projectilePrefabs;

    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private Transform _leftLimit;
    [SerializeField] private Transform _rightLimit;

    [SerializeField] private float _spawnInterval;
    [SerializeField] private float _horizontalDistance;
    [SerializeField] private float _verticalDistance;
    [SerializeField] private int _spawnSquidRows;
    [SerializeField] private int _spawnCrabRows;
    [SerializeField] private int _spawnOctopusRows;
    [SerializeField] private int _spawnCols;
    [SerializeField] private float _movimentInterval;
    [SerializeField] private float _movimentDistance;

    [SerializeField] private float _shootSpeed;
    [SerializeField] private float _shootInterval;
    [SerializeField] private DificultyLevel[] _dificulties;

    private float _spawnTimer = 0.0f;
    private float _movimentTimer = 0.0f;
    private float _shootTimer = 0.0f;
    private List<EnemyData> _enemiesData;
    private List<GameObject> _enemies;
    private int _spawnedEnemies = 0;
    private int _spawnRows = 0;
    private int _movimentDirection = 1;
    private GameObject _deadEnemy = null;

    private class EnemyData
    {
        public GameObject Prefab;
        public Vector3 Position;
    }

    private void Awake()
    {
        Instance = this;
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

    private void UpdateMovimentInterval()
    {
        foreach (DificultyLevel level in _dificulties) {
            if (_enemies.Count <= level.Enemies && _movimentInterval > level.MovimentInterval) {
                _movimentInterval = level.MovimentInterval;
            }
        }
    }

    private void StartSpawningEnemies()
    {
        _spawnTimer = 0.0f;
        _spawnedEnemies = 0;
        _movimentDirection = 1;
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

    private void StopEnemiesAnimations()
    {
        foreach (GameObject enemy in _enemies) {
            enemy.GetComponent<Animator>().speed = 0;
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
            GameManager.Instance.SetGameState(GameState.Playing);
        }
    }

    private void StartEnemiesMoviment()
    {
        _movimentTimer = 0.0f;
        _shootTimer = 0.0f;
        StartEnemiesAnimations();
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.SpawningEnimies) {
            StartSpawningEnemies();
        } else if (newState == GameState.Playing) {
            StartEnemiesMoviment();
        }
    }

    private bool ShouldAdvanceLine()
    { 
        foreach (GameObject enemy in _enemies) {
            var position = enemy.transform.position;
            if (_movimentDirection > 0 && position.x > _rightLimit.position.x) {
                return true;
            } else if (_movimentDirection < 0 && position.x < _leftLimit.position.x) {
                return true;
            }
        }
        return false;
    }

    private void MoveEnemies()
    {
        _movimentTimer += Time.deltaTime;
        if (_movimentTimer < _movimentInterval) {
            return;
        }
        _movimentTimer -= _movimentInterval;

        if (_deadEnemy != null) {
            _enemies.Remove(_deadEnemy);
            Destroy(_deadEnemy);
            _deadEnemy = null;
        }

        if (ShouldAdvanceLine()) {
            _movimentDirection *= -1;
            float movimentY = _octopusPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
            foreach (GameObject enemy in _enemies) {
                enemy.GetComponent<Rigidbody2D>().MovePosition(enemy.transform.position + new Vector3(0, -movimentY, 0));
            }
        } else {
            foreach (GameObject enemy in _enemies) {
                enemy.GetComponent<Rigidbody2D>().MovePosition(enemy.transform.position + new Vector3(_movimentDistance * _movimentDirection, 0, 0));
            }
        }
    }

    private void EnemyShoot(GameObject enemy)
    {
        int index = UnityEngine.Random.Range(0, _projectilePrefabs.Length - 1);
        Projectile prefab = _projectilePrefabs[index];
        float y = enemy.GetComponent<SpriteRenderer>().bounds.size.y / 2 + prefab.GetComponent<SpriteRenderer>().bounds.size.y;
        Projectile projectile = Instantiate(prefab, enemy.transform.position - Vector3.up * y, Quaternion.identity);
        projectile.Speed = _shootSpeed;
        projectile.Direction = -1;
    }

    private void EnemiesShoot()
    {
        _shootTimer += Time.deltaTime;
        if (_shootTimer < _shootInterval) {
            return;
        }
        _shootTimer -= _shootInterval;

        List<float> cols = new List<float>();
        List<GameObject> canShoot = new List<GameObject>();

        for (int i = _enemies.Count; i > 0; i--) {
            GameObject enemy = _enemies[i - 1];
            if (cols.Contains(enemy.transform.position.x)) {
                continue;
            }
            cols.Add(enemy.transform.position.x);
            canShoot.Add(enemy);
        }

        if (canShoot.Count == 0)
            return;

        int index = UnityEngine.Random.Range(0, canShoot.Count);
        GameObject shootingEnemy = canShoot[index];
        EnemyShoot(shootingEnemy);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState == GameState.SpawningEnimies) {
            SpawningEnemies();
        } else if (GameManager.Instance.CurrentGameState == GameState.Playing) {
            MoveEnemies();
            EnemiesShoot();
        } else {
            StopEnemiesAnimations();
        }
    }

    public void Kill(GameObject enemy)
    {
        if (_deadEnemy != null) {
            return;
        }
        UpdateMovimentInterval();
        _deadEnemy = enemy;
        enemy.GetComponent<Animator>().SetBool("Dead", true);
        _movimentTimer = _movimentInterval * 0.5f;
        _shootTimer = 0;
    }
}
