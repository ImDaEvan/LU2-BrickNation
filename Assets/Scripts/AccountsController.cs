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
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
public class AccountsController : MonoBehaviour
{

    public GameObject introCanvas, loginCanvas, registerCanvas;
    public GameObject loginButton, registerButton, backButton1, backButton2;
    public GameObject loginUserNameField, loginPasswordField, loginConfirm;
    public GameObject registerEmailField, registerUserNameField, registerPasswordField, registerConfirm;
    public ApiController apiController;
    public string Username;
    
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
    public void ToggleUiElement(GameObject element, bool on)
    {
        element.SetActive(on);
    }
    public void ShowRegister()
    {
        ToggleUiElement(introCanvas, false);
        ToggleUiElement(registerCanvas, true);
    }
    public void ShowLogin()
    {
        Debug.Log(" CLICKCIKCICK");
        ToggleUiElement(introCanvas, false);
        ToggleUiElement(loginCanvas, true);
    }
    public void ShowIntro()
    {
        ToggleUiElement(introCanvas, true); ;
        ToggleUiElement(loginCanvas, false);
        ToggleUiElement(registerCanvas, false);
    }

    public void ChangeToEnvironmentScreen()
    {
        SceneManager.LoadScene("MainScreen");
    }

    public void SubmitLogin()
    {
        StartCoroutine(SubmitLoginCoroutine());
    }
    public void SubmitRegister()
    {
        StartCoroutine(SubmitRegisterCoroutine());
    }

   private IEnumerator SubmitLoginCoroutine()
    {
        string username = loginUserNameField.GetComponent<TMP_InputField>().text;
        string password = loginPasswordField.GetComponent<TMP_InputField>().text;

        var model = new LoginModel { username = username, password = password };
        string json = JsonUtility.ToJson(model);
        Task<string> task = apiController.PerformApiCall<string>("api/auth/login", "POST", json);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Result != null)
        {

                var result = JsonUtility.FromJson<AuthResponse>(task.Result);
                apiController.Token = result.accessToken;
                PlayerPrefs.SetString("Username",username);
                ChangeToEnvironmentScreen();
            } 
        else{
            Debug.LogError("Registration failed: " + task.Result);
        }
    }

    private IEnumerator SubmitRegisterCoroutine()
    {
        string email = registerEmailField.GetComponent<TMP_InputField>().text;
        string name = registerUserNameField.GetComponent<TMP_InputField>().text;
        string password = registerPasswordField.GetComponent<TMP_InputField>().text;

        var model = new RegisterModel { email = email, name = name, password = password };
        string json = JsonUtility.ToJson(model);

        Task<string> task = apiController.PerformApiCall<string>("api/auth/register", "POST", json);
        yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log(json);
        if (task.Result != null)
        {

            Debug.Log("Registration success: " + task.Result);
                ShowLogin();
            } 
        else{
            Debug.LogError("Registration failed: " + task.Result);
        }
    }

}
