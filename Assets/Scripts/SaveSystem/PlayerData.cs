using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // Player's Current Level - Load From This Field
    [HideInInspector]
    public List<GameManager.Level> levelStatistics = new List<GameManager.Level>();

    public PlayerData()
    {
        levelStatistics = GameManager.GameManagerInstance.GetStatistics();
    }
}
