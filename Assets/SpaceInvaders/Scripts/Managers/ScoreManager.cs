using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score;
    public int Lives;
    public int Stage;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void IncrementScore()
    {
        Score += 20 * (Stage + 1);
    }

    public void DecrementLives()
    {
        if (Lives == 0) {
            GameManager.Instance.SetGameState(GameState.GameOver);
        } else {
            Lives--;
        }
    }
}
