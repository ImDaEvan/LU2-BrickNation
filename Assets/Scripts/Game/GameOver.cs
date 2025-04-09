using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverScreen,highScore;
    public TMP_Text score;
    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }
    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }
    void Start()
    {
        gameOverScreen.SetActive(false);
    }
    public void OnGameOver(bool highscore)
    {  
        gameOverScreen.SetActive(true);
        highScore.SetActive(highscore);
    }
}
