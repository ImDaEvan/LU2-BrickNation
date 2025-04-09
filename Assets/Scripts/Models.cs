using UnityEngine;
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
    public class LoginModel
    {
        public string Username {get;set;}
        public string Password {get;set;}
    }
    [System.Serializable]
    public class RegisterModel
    {
        public string Username {get;set;}
        public string Email {get;set;}
        public string Password {get;set;}
    }
}