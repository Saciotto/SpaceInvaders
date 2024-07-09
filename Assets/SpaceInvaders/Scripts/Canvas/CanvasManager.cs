using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TextMeshProUGUI _stageText;

    private void Awake()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateCanvas;
        ScoreManager.Instance.OnLivesChanged += UpdateCanvas;
        ScoreManager.Instance.OnStageChanged += UpdateCanvas;
    }

    private void Start()
    {
        UpdateCanvas();
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateCanvas;
        ScoreManager.Instance.OnLivesChanged -= UpdateCanvas;
        ScoreManager.Instance.OnStageChanged -= UpdateCanvas;
    }

    private void UpdateCanvas()
    {
        _scoreText.text = $"Score: {ScoreManager.Instance.Score}";
        _livesText.text = $"Lives: {ScoreManager.Instance.Lives}";
        _stageText.text = $"Stage: {ScoreManager.Instance.Stage}";
    }
}
