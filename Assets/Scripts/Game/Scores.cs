using System.Globalization;
using System.Threading.Tasks;
using Models;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Scores : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text highscoreText;
    
    private int score =0 ;
    private bool gotHighscore = false;
    private int scoreToReach = -1;
    private HighScoreData highscore = new HighScoreData{score=0};
    private int getHighScore {get{return PlayerPrefs.GetInt("Highscore");}}
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
            gameDataController.SaveHighscore(highScoreData);
        }
    }

    void Start()
    {
        gameDataController = gameDataControllerObject.GetComponent<GameDataController>();
        score = 0;
        gotHighscore = false;
        UpdateScoreText();
        
        int goalScore = PlayerPrefs.GetInt("GoalScore",-1);
        if (goalScore > 0)
        {
            scoreToReach = goalScore;
        }
        highscore = gameDataController.GetHighscore();
        
    }
    private void UpdateCombo(int comboToSet)
    {
        combo = comboToSet;
        comboText.text = "Combo: \n"+combo.ToString();
    }
    private void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        if (score > (getHighScore>=-1? getHighScore:0))
        {
            gotHighscore = true;
            var data = new HighScoreData(){score = score};
            gameDataController.SaveHighscore(data);
        }
        if (score>scoreToReach && scoreToReach > 0)
        {
            PlayerPrefs.SetInt("WonObject",1);
            //GameEvents.GameOver(false);
        }
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        highscoreText.text = "Highscore: "+getHighScore.ToString();
        scoreText.text = score.ToString();
    }
}
