using System.Collections;
using Models;
using UnityEngine;
using System.Threading.Tasks;
public class GameDataController : MonoBehaviour
{
    public HighScoreData savedHighScoreData {get;private set;}
    private ApiController apiController;
    void Start()
    {
        apiController = GetComponent<ApiController>();
    }
    public void SaveGameData(HighScoreData highScoreData)
    {
        savedHighScoreData = highScoreData;
        StartCoroutine(SaveHighscoreCoroutine());
        
    }
    public void GetGameData(string username)
    {
        if (username==null)
        {
            username = "";
        } 
        StartCoroutine(GetHighScoreCoroutine(username));
    }
    private IEnumerator SaveHighscoreCoroutine()
    {
        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(savedHighScoreData);

        Task<string> task = apiController.PerformApiCall<string>("api/game/highscore", "POST", jsonString);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result != null)
        {
            Debug.Log("Save successful: " + task.Result);
        }
        else
        {
            Debug.LogError("Save failed.");
        }
    }
    private IEnumerator GetHighScoreCoroutine(string username)
    {

        Task<string> task = apiController.PerformApiCall<string>("api/game/highscore", "GET", username);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Result != null)
        {
            HighScoreData gotHighScore = JsonUtility.FromJson<HighScoreData>(task.Result);
            Debug.Log(gotHighScore.score);
            savedHighScoreData = gotHighScore;
        }
        else
        {
            Debug.LogError("Get failed.");

        }
    }
}
