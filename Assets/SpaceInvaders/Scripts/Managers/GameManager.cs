using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform _spawnReference;
    [SerializeField] private GameObject _squidPrefab;

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
        Vector3 initialPosition = _spawnReference.position - new Vector3(4, 0, 0);
        for (int i = 0; i < 11; i++) {
            Instantiate(_squidPrefab, initialPosition + Vector3.right * i * 0.8f, Quaternion.identity);
        }
    }
}
