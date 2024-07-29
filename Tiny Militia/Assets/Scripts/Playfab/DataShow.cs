using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataShow : MonoBehaviour
{
    public static DataShow Instance;

    public int High_Score_Count;    // Set to the Server Successfully
    public int Total_Kill_Count;    // Not Set Yet
    public int Total_Death_Count;   // Not Set Yet
    public int Win_Matches_Count;   // Not Set Yet
    public int Total_Matches_Count; // Set to the Server Successfully
    public int KD_Count;            // Not Set Yet
    public int Rank_Count;          // Not Set Yet
    public float GameTime;


    private void Awake()
    {
        Instance = this;
        if(Instance != null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // For Showing High Score
    public void Set_and_Show_ApperanceHighScore(int HighScore)
    {
        Menu.Instance.High_Score.text = HighScore.ToString();
        High_Score_Count = HighScore;
    }

    // For Showing Total Kills
    public void Set_and_Show_ApperanceTotalKills(int TotalKills)
    {
        Menu.Instance.Total_KillCount.text = TotalKills.ToString();
        Total_Kill_Count = TotalKills;
    }

    // For Showing Total Deaths
    public void Set_and_Show_ApperanceTotalDeaths(int TotalDeaths)
    {
        Menu.Instance.Total_DeathCount.text = TotalDeaths.ToString();
        Total_Death_Count = TotalDeaths;
    }

    // For Showing Total Win Matches
    public void Set_and_Show_ApperanceTotalWinMatches(int TotalWinMatches)
    {
        Menu.Instance.Win_Matches.text = TotalWinMatches.ToString();
        Win_Matches_Count = TotalWinMatches;
    }

    // For Showing Total Matches
    public void Set_and_Show_ApperanceTotalMatches(int TotalMatches)
    {
        Menu.Instance.Total_Matches.text = TotalMatches.ToString();
        Total_Matches_Count = TotalMatches;
    }

    // For Showing KD Count
    public void Set_and_Show_ApperanceKD(int KD)
    {
        Menu.Instance.KD_ratio.text = KD.ToString();
        KD_Count = KD;
    }

    // For Showing Player Name
    public void Set_and_Show_ApperancePlayerName()
    {
        Menu.Instance.PlayerName.text = Menu.Instance.player_nametext.text;
    }

}