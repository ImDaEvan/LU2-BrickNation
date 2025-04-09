using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft;
using TMPro;
using System;
using Models;
public class AccountsController : MonoBehaviour
{

    public GameObject introCanvas, loginCanvas, registerCanvas;
    public GameObject loginButton, registerButton, backButton1, backButton2;
    public GameObject loginUserNameField, loginPasswordField,loginConfirm;
    public GameObject registerEmailField, registerUserNameField, registerPasswordField,registerConfirm;
    public ApiController apiController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loginCanvas.SetActive(false);
        registerCanvas.SetActive(false);
        loginButton.GetComponent<Button>().onClick.AddListener(ShowLogin);
        registerButton.GetComponent<Button>().onClick.AddListener(ShowRegister);
        backButton1.GetComponent<Button>().onClick.AddListener(ShowIntro);
        backButton2.GetComponent<Button>().onClick.AddListener(ShowIntro);
        loginConfirm.GetComponent<Button>().onClick.AddListener(SubmitLogin);
        registerConfirm.GetComponent<Button>().onClick.AddListener(SubmitRegister);
        apiController = GetComponent<ApiController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleUiElement(GameObject element)
    {
        element.SetActive(!element.activeInHierarchy);
    }
    public void ToggleUiElement(GameObject element,bool on)
    {
        element.SetActive(on);
    }
    public void ShowRegister()
    {
        ToggleUiElement(introCanvas,false); 
        ToggleUiElement(registerCanvas,true);
    }
    public void ShowLogin()
    {
        Debug.Log(" CLICKCIKCICK");
        ToggleUiElement(introCanvas,false); 
        ToggleUiElement(loginCanvas,true);
    }
    public void ShowIntro()
    {
        ToggleUiElement(introCanvas,true);;
        ToggleUiElement(loginCanvas,false); 
        ToggleUiElement(registerCanvas,false);
    }

    public void ChangeToEnvironmentScreen()
    {
        introCanvas.SetActive(false);
        loginCanvas.SetActive(false);
        registerCanvas.SetActive(false);
    }
    
    public void SubmitLogin()
    {
        StartCoroutine(SubmitLoginCoroutine());
    }

    private IEnumerator SubmitLoginCoroutine()
    {
        string username = loginUserNameField.GetComponent<TMP_InputField>().text;
        string password = loginPasswordField.GetComponent<TMP_InputField>().text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            LoginModel loginData = new LoginModel { Username = username, Password = password };
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
            
            Task<string> task = apiController.PerformApiCall<string>("api/auth/login", "POST", jsonString);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Result != null)
            {
                Debug.Log("Login successful: " + task.Result);
                ChangeToEnvironmentScreen();
            }
            else
            {
                Debug.LogError("Login failed.");
            }
        }
    }
    // public async Task<string> SubmitLogin()
    // {
    //     string username = loginUserNameField.GetComponent<Text>().text;
    //     string password = loginPasswordField.GetComponent<Text>().text;

    //     if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
    //     {
    //         LoginModel loginData = new LoginModel { Username = username, Password = password };
    //         string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
    //         string response = await PerformApiCall(API_BASE_URL + "api/auth/login","POST", "POST", jsonString);

    //         if (response != null)
    //         {
    //             Debug.Log("Login successful: " + response);
    //             ChangeToEnvironmentScreen();
    //             return response;
    //         }
    //     }
    //     return "404";
    //}
    public void SubmitRegister()
    {
        string email = registerUserNameField.GetComponent<Text>().text;
        string username = registerUserNameField.GetComponent<Text>().text;
        string password = registerPasswordField.GetComponent<Text>().text; 

        if (email.Contains("@") && email.Contains(".") && username != "Username" && username !="Password")
        {
            // send register call
        }

    }
}
