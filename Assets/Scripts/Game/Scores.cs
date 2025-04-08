using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text scoreText;
    public TMP_Text comboText;
    private int score;
    private int combo;
    public void OnEnable()
    {
        GameEvents.AddScore += AddScore;
        GameEvents.UpdateCombo += UpdateCombo;
    }
    public void OnDisable()
    {
        GameEvents.AddScore -= AddScore;
        GameEvents.UpdateCombo -= UpdateCombo;
    }
    void Start()
    {
        score = 0;
        UpdateScoreText();
    }
    private void UpdateCombo(int comboToSet)
    {
        combo = comboToSet;
        comboText.text = "Combo: \n"+combo.ToString();
    }
    private void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}
