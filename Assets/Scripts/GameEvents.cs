using UnityEngine;
using System;
public class GameEvents : MonoBehaviour
{
    //methods that can be subscribed and unsubscribed to.
    public static Action<int> AddScore;
    public static Action<int> UpdateCombo;
    public static Action CanPlaceShape;
    public static Action MoveShapeToStartPosition;
    public static Action RequestNewShapes;
    public static Action DiminishShapeControls;
    public static Action<bool> GameOver;
    public static Action<Transform> SaveTrophyState;
    public static Action<string> PlaceNewTrophy;

}
