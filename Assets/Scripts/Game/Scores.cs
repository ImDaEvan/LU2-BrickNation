using System.Globalization;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text scoreText;
    public TMP_Text comboText;
    private int score;
    private bool gotHighscore = false;
    private HighScoreData highscore = new HighScoreData();
    private int combo;
    public GameObject gameDataControllerObject;
    private GameDataController gameDataController;
    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        GameEvents.AddScore += AddScore;
        GameEvents.UpdateCombo += UpdateCombo;
        GameEvents.GameOver += SaveHighScore;
    }
    private void OnDisable()
    {
        GameEvents.AddScore -= AddScore;
        GameEvents.UpdateCombo -= UpdateCombo;
        GameEvents.GameOver -= SaveHighScore;
    }
    public void SaveHighScore(bool scoredHighScore)
    {
        if (scoredHighScore)
        {
            HighScoreData highScoreData = new HighScoreData()
            {
                score = 0
            };
            gameDataController.SaveGameData(highScoreData);
        }
    }

    void Start()
    {
        gameDataController = gameDataControllerObject.GetComponent<GameDataController>();
        score = 0;
        gotHighscore = false;
        UpdateScoreText();
        gameDataController.GetGameData("evan");
        Debug.Log(gameDataController.savedHighScoreData);
    }
    private void UpdateCombo(int comboToSet)
    {
        combo = comboToSet;
        comboText.text = "Combo: \n"+combo.ToString();
    }
    private void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        if (score > highscore.score)
        {
            gotHighscore = true;
            highscore.score = score;
        }
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}
