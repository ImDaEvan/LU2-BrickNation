using UnityEngine;
using System;
namespace Models{
    public class GridSize
    {
        public int columns {get;set;}
        public int rows {get;set;}
        public GridSize(int ccolumns, int crows)
        {
            columns = ccolumns;
            rows = crows;
        }
    }
    public class GameSettings
    {
        public int scorePerLine;
        public float scoreMultiplier2Lines;
        public float scoreMultiplier3Lines;
        public float initialComboMultiplier;
        public float comboProgression;
        public int comboDeter;
    }
    [System.Serializable]
    public class HighScoreData
    {
        public int score=0;
    }
    [System.Serializable]
    public class RegisterModel
    {
        public string name;
        public string email;
        public string password;
    }

    [System.Serializable]
    public class LoginModel
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class AuthResponse
    {
        public string accessToken;
    }
    public class Trophies
    {
        public int[] t;

    }
    public class GameObjectServer
    {
        public Guid ID { get; set; }
        public Guid EnvironmentID { get; set; }
        public int PrefabID { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int ScaleX { get; set; }
        public int ScaleY { get; set; }
        public int RotationZ { get; set; }
        public int SortingLayer { get; set; }
    }
    [System.Serializable]
    public class TrophyState
    {
        public float PosX;
        public float PosY;
        public int PrefabID;
        public int EnvironmentID;
    }
    [System.Serializable]
    public class HighscoreRequest
    {
        public string username;
    }
    [System.Serializable]
    public class TrophyStateList
    {
        public TrophyState[] trophies;
    }
}