using System;
using System.Collections.Generic;
using UnityEngine;

namespace VContainerApp.Services
{
    [Serializable]
    public class SaveData
    {
        public int Score;
        public int ClickPower;
        public List<int> ItemPurchaseCounts;
    }
    
    public class SaveService
    {
        private const string SAVE_KEY = "GameSaveData";
        
        public void Save(ScoreService scoreService, ItemService itemService)
        {
            var saveData = new SaveData
            {
                Score = scoreService.Score,
                ClickPower = scoreService.ClickPower,
                ItemPurchaseCounts = itemService.GetPurchaseCounts()
            };
            
            var json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
            
            Debug.Log("Game saved!");
        }
        
        public bool Load(ScoreService scoreService, ItemService itemService)
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY))
            {
                Debug.Log("No save data found.");
                return false;
            }

            try
            {
                var json = PlayerPrefs.GetString(SAVE_KEY);
                var saveData = JsonUtility.FromJson<SaveData>(json);
            
                scoreService.LoadData(saveData.Score, saveData.ClickPower);
                itemService.LoadData(saveData.ItemPurchaseCounts);
            
                Debug.Log("Game loaded!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save data: {e.Message}");
                return true;
            }

            return true;
        }
    }
}