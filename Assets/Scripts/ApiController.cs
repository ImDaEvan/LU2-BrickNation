using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;
public class ApiController : MonoBehaviour
{
    private string baseUrl = "https://localhost:7220/";
    public string Token {private get; set;}
    public string Username;
    public async Task<string> PerformApiCall<T>(string url, string method, string jsonString)
    {
        
        using (UnityWebRequest request = new UnityWebRequest(baseUrl+url, method))
        {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(Token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + Token);
            }

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API Call Successful: " + request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError("API Call Failed: " + request.error);
                return null;
            }
        }
    }
}
