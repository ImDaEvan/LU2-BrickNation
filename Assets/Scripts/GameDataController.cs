using System.Collections;
using Models;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
public class GameDataController : MonoBehaviour
{
    public HighScoreData savedHighScoreData {get;private set;}
    public Trophies trophiesAvailable {get;private set;}
    private ApiController apiController;
    private Trophies trophiesToPlace;
     private TrophyState[] trophiesPlaced;
    private string Username { get {return PlayerPrefs.GetString("Username");} }
    private int Environment { get {return PlayerPrefs.GetInt("Environment");} }
    public List<TrophyState> trophiesInEnvironment = new List<TrophyState>();
    void Start()
    {
        apiController = GetComponent<ApiController>();
        PlayerPrefs.SetInt("Environment",1);
    }
    public void SaveHighscore(HighScoreData highScoreData)
    {
        savedHighScoreData = highScoreData;
        StartCoroutine(SaveHighscoreCoroutine());
        
    }
    public HighScoreData GetHighscore()
    {

        // Use await here to make sure the function waits for the result.
        StartCoroutine(GetHighScoreAsync());
        return savedHighScoreData;
    }
    private IEnumerator SaveHighscoreCoroutine()
    {
        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(savedHighScoreData);
        var url = "api/game/highscore/post/" + Username;
        Task<string> task = apiController.PerformApiCall<string>(url, "POST", jsonString);
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
     private IEnumerator GetHighScoreAsync()
    {
        string baseURL = "api/game/highscore/get/";
        HighscoreRequest req = new HighscoreRequest{ username = Username };
        Debug.Log(Username);
        Debug.Log(req.username);
       string jsonString = JsonUtility.ToJson(req); 
       Debug.Log(jsonString);
       
        Task<string> task = apiController.PerformApiCall<string>(baseURL, "POST", jsonString);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task != null)
        {
            HighScoreData gotHighScore = JsonUtility.FromJson<HighScoreData>(task.Result);
            Debug.Log(gotHighScore.score);
            savedHighScoreData = gotHighScore;
            PlayerPrefs.SetInt("Highscore",gotHighScore.score);
        }
        else
        {
            Debug.LogError("Get failed.");
        }
    }

    public void GetTrophies(System.Action<Trophies> callback)
    {
        StartCoroutine(GetTrophiesCoroutine(callback));
    }
    public void SaveTrophies( Trophies trophies)
    {
        StartCoroutine(SaveTrophiesCoroutine(trophies));
    }
    public void SaveTrophyState(TrophyState trophy)
    {
        StartCoroutine(SaveTrophyStateCoroutine(trophy));
    }
    public TrophyState[] GetTrophyStates(System.Action<TrophyState[]> callback)
    {
        StartCoroutine(GetTrophyStateCoroutine(callback));
        return trophiesPlaced;
    }
    private IEnumerator GetTrophyStateCoroutine(System.Action<TrophyState[]> callback)
    {
        string baseURL = "api/game/trophies/get/state/";
        baseURL += Username + "/";
        baseURL += Environment.ToString();
        string jsonString = ""; //$"\"{username}\"";
        Task<string> task = apiController.PerformApiCall<string>(baseURL, "POST", jsonString);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Result != null)
        {
            Debug.Log(task.Result);
            TrophyStateList trophyStateList = JsonUtility.FromJson<TrophyStateList>(task.Result);
            TrophyState[] trophiesPlaced = trophyStateList.trophies;
            callback?.Invoke(trophiesPlaced);

        }
        else
        {
            Debug.LogError("Get failed.");
            callback?.Invoke(null);
        }
    }
    private IEnumerator GetTrophiesCoroutine(System.Action<Trophies> callback)
    {
        string baseURL = "api/game/trophies/get/";
        if (Username != null || Username != "")
        {
            baseURL += Username+"/";
            baseURL += PlayerPrefs.GetInt("Environment").ToString();
        }
        
        string jsonString = ""; //$"\"{username}\"";
        Task<string> task = apiController.PerformApiCall<string>(baseURL, "POST", jsonString);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Result != null)
        {
            Debug.Log(task.Result);
            Trophies gotTrophies = JsonUtility.FromJson<Trophies>(task.Result);
            Debug.Log(gotTrophies);
            Debug.Log(Username);
            trophiesAvailable = new Trophies(){t= new int[6]{1,2,2,1,2,1}};//gotTrophies;
            callback?.Invoke(gotTrophies);
        }
        else
        {
            Debug.LogError("Get failed.");
            callback?.Invoke(null);
        }
    }
    private IEnumerator SaveTrophyStateCoroutine(TrophyState trophy)
    {
        string baseURL = "api/game/trophies/state/save/";
        if (Username != null || Username != "")
        {
            baseURL += Username;
        }
        TrophyState trophyToSave = trophy;
        trophyToSave.EnvironmentID = PlayerPrefs.GetInt("Environment");
        string jsonString = JsonUtility.ToJson(trophy);
        Debug.Log("Sending Trophy JSON: " + jsonString);
        Debug.Log(" URL: " +baseURL);
        Task<string> task = apiController.PerformApiCall<string>(baseURL, "POST", jsonString);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Result != null)
        {
            Debug.Log("saved");
        }
        else
        {
            Debug.LogError("Save failed.");

        }
    }
    private IEnumerator SaveTrophiesCoroutine(Trophies trophy)
     {
        string baseURL = "api/game/trophies/save/";
        if (Username != null || Username != "")
        {
            baseURL += Username;
        }
        string jsonString = JsonUtility.ToJson(trophy);
        Debug.Log("Sending Trophy JSON: " + jsonString);
        Debug.Log(" URL: " +baseURL);
        Task<string> task = apiController.PerformApiCall<string>(baseURL, "POST", jsonString);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Result != null)
        {
            Debug.Log(task.Result);
        }
        else
        {
            Debug.LogError("Save failed.");

        }
    }
}
