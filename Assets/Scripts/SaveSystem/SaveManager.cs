using System;
using GameManagement;
using UnityEngine;

namespace SaveSystem
{
   public class SaveManager: ManagerBase
   {
       [field: SerializeField]
       public GameData GameData { get; private set; }

       private const string HighScoreKey = "HighScore";
       private const string CoinKey = "Coins";
       private const string LevelKey = "Level";
       private const string LastPlayDateKey = "LastPlayDate";
       private const string SoundEnabledKey = "SoundEnabled";

       public override void SetUp()
       {
           base.SetUp();
           
           if (GameData == null)
           {
               GameData = new GameData();
           }
           
           LoadGameData();
       }

       private void LoadGameData()
       {
           GameData.HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
           GameData.Coins = PlayerPrefs.GetInt(CoinKey, 0);
           GameData.Level = PlayerPrefs.GetInt(LevelKey, 1);
           GameData.LastPlayDate = PlayerPrefs.GetString(LastPlayDateKey, DateTime.Now.ToString());
           GameData.SoundEnabled = PlayerPrefs.GetInt(SoundEnabledKey, 1) == 1;
       }

       private void OnApplicationPause(bool pauseStatus)
       {
           if (pauseStatus)
           {
               SaveGameData();
           }
       }

       private void OnApplicationQuit()
       {
           SaveGameData();
       }

       public void SaveGameData()
       {
           PlayerPrefs.SetInt(HighScoreKey, GameData.HighScore);
           PlayerPrefs.SetInt(CoinKey, GameData.Coins);
           PlayerPrefs.SetInt(LevelKey, GameData.Level);
           PlayerPrefs.SetString(LastPlayDateKey, GameData.LastPlayDate);
           PlayerPrefs.SetInt(SoundEnabledKey, GameData.SoundEnabled ? 1 : 0);
           
           PlayerPrefs.Save();
       }
       
       public void ResetGameData()
       {
           GameData = new GameData();
           SaveGameData();
       }
   }

   [Serializable]
   public class GameData
   {
       [field: SerializeField]
       public int HighScore { get; set; }
       
       [field: SerializeField]
       public int Coins { get; set; }
       
       [field: SerializeField]
       public int Level { get; set; }
       
       [field: SerializeField]
       public string LastPlayDate { get; set; }
       
       [field: SerializeField]
       public bool SoundEnabled { get; set; } = true;
   }
}