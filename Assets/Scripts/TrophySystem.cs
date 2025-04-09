using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Models;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TrophySystem : MonoBehaviour
{
    public GameObject[] trophies;
    public GameObject trophySelection,trophyDropField;
    public TMP_InputField environmentSelection;
    private Trophies trophyAmounts;
    private TrophyState[] trophiesToPlace;
    private string username = "evan";
    private int EnvironmentID {get{return PlayerPrefs.GetInt("Environment");}}
    private GameDataController controller;
    private List<GameObject> trophiesInSelectionField = new List<GameObject>();
    private void OnEnable()
    {
        GameEvents.SaveTrophyState += SaveTrophyState;
        GameEvents.PlaceNewTrophy += PlacedTrophy;
    }
    private void OnDisable()
    {
        GameEvents.SaveTrophyState -= SaveTrophyState;
         GameEvents.PlaceNewTrophy -= PlacedTrophy;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveObjects(new Vector3(0,-0.5f,0));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveObjects(new Vector3(0,.5f,0));  
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveObjects(new Vector3(-.5f,0,0));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveObjects(new Vector3(.5f,0,0));
        }
    }
    private void MoveObjects(Vector3 move)
    {
        trophyDropField.transform.position = trophyDropField.transform.position - move;
    }

    void Start()
    {
        Debug.Log(" Getting trophies...");
        controller = GetComponent<GameDataController>();
        GetTrophies();
        GetPlacedTrophies();
    }
    private void GetPlacedTrophies()
    {
        controller.GetTrophyStates((trophies)=>
        {
            if (trophies != null)
            {
                trophiesToPlace = trophies;
                PlaceSavedTrophy(trophiesToPlace,false);
            }
            else 
            {
                Debug.Log("TRophy data laoding failed");
            }
        });
    }
    private void GetTrophies()
    {
        
        controller.GetTrophies((trophies)=>
        {
            if (trophies != null)
            {
                trophyAmounts = trophies;
                SpawnTrophiesToPlace();
            }
            else 
            {
                Debug.Log("TRophy data laoding failed");
            }
        });
    }
    private void SaveTrophies()
    {
        controller.SaveTrophies(trophyAmounts);
    }
    public void SaveTrophyState(UnityEngine.Transform transform)
    {
        TrophyState state = new TrophyState()
        {
            PosX = transform.position.x,
            PosY = transform.position.y,
            PrefabID = transform.gameObject.GetComponent<TrophyController>().PrefabID,
            EnvironmentID = EnvironmentID

        };
        
        controller.SaveTrophyState(state);
    }
    private void SpawnTrophiesToPlace()
    {
        List<GameObject> visualTrophies = new List<GameObject>();
        foreach (var visualTrophy in trophies)
        {
            visualTrophies.Add(Instantiate(visualTrophy));
        }
        var index = 0;
        trophiesInSelectionField = visualTrophies;
        foreach (var trophy in visualTrophies)
        {
            PlaceTrophy(trophy,index,false);
            index++;
        }
    }
    private void PlaceSavedTrophy(TrophyState[] states,bool draggable)
    {
        foreach (var trophy in states)
        {
            if (trophy.EnvironmentID == EnvironmentID)
            {

                var placedTrophy = Instantiate(trophies[trophy.PrefabID-1]);
                placedTrophy.GetComponent<TrophyController>().draggable = draggable;
                placedTrophy.transform.position = new Vector2 (trophy.PosX,trophy.PosY);
                placedTrophy.transform.SetParent(trophyDropField.transform);
                placedTrophy.transform.localScale = trophies[trophy.PrefabID-1].transform.localScale;
            }
        }
    }
    private void PlaceTrophy(GameObject trophy,int index,bool again)
    {
            trophy.transform.SetParent(trophySelection.transform);

            var ShapeContainer = trophySelection.transform.GetChild(index+1);
            trophy.transform.localPosition = ShapeContainer.transform.localPosition;

            var amount = ShapeContainer.transform.Find("Amount");
            var tmpText = amount.GetComponent<TMP_Text>();
            Debug.Log(tmpText.text);
            var decrease = again?1:0;
            trophyAmounts.t[index] -= decrease;
            
            tmpText.text = trophyAmounts.t[index].ToString();

            TrophyController trophyController = trophy.GetComponent<TrophyController>();
            trophy.transform.localScale = trophyController._trophyStartScale;
            trophyController.trophySelection = trophySelection;
            trophyController.trophyDropField = trophyDropField;
            trophy.name = index.ToString();
            if (trophyAmounts.t[index] == 0)
            {
                trophyController.draggable = false;
            }
            SaveTrophies();
    }
    private void PlacedTrophy(string name)
    {
        var index = int.Parse(name);
        var newTrophy = Instantiate(trophies[index]);
        PlaceTrophy(newTrophy,index,true);
    }
 
    public void OnUpdate()
    {
        // Get the input from the environment selection text
        string inputText = environmentSelection.text;

        // Try to parse the input to an integer
        if (int.TryParse(inputText, out int environmentIDSelected))
        {
            // Check if the parsed value is within the valid range
            if (environmentIDSelected >= 1 && environmentIDSelected <= 5)
            {
                // Save the valid value to PlayerPrefs
                PlayerPrefs.SetInt("Environment", environmentIDSelected);
                ReloadTrophies();
            }
            else
            {
                // Handle the case where the value is out of range
                Debug.LogWarning("Please enter a value between 1 and 5.");
            }
        }
        else
        {
            // Handle the case where the input is not a valid integer
            Debug.LogWarning("Invalid input. Please enter a valid integer.");
        }
    }

    private void ReloadTrophies()
    {
        foreach (Transform child in trophyDropField.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (GameObject child in trophiesInSelectionField)
        {
            Destroy(child);
        }
        GetPlacedTrophies();
        GetTrophies();
    }
}
