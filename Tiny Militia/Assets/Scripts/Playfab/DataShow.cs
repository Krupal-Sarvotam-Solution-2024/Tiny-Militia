using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataShow : MonoBehaviour
{
    public static DataShow Instance;

    public int High_Score_Count;//
    public int Total_Kill_Count;//
    public int Total_Death_Count;//
    public int Win_Matches_Count;
    public int Total_Matches_Count;
    public int KD_Count;
    public int Rank_Count;


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

   
    // For High Score
    public void SetApperanceHighScore(int HighScore)
    {
        Menu.Instance.High_Score.text = HighScore.ToString();
        High_Score_Count = HighScore;
    }

    // For Total Kills
    public void SetApperanceTotalKills(int TotalKills)
    {
        Menu.Instance.Total_KillCount.text = TotalKills.ToString();
        Total_Kill_Count = TotalKills;
    }

    // For Total Deaths
    public void SetApperanceTotalDeaths(int TotalDeaths)
    {
        Menu.Instance.High_Score.text = TotalDeaths.ToString();
        Total_Death_Count = TotalDeaths;
    }

    // For Total Win Matches
    public void SetApperanceTotalWinMatches(int TotalWinMatches)
    {
        Menu.Instance.Win_Matches.text = TotalWinMatches.ToString();
        Win_Matches_Count = TotalWinMatches;
    }

    // For Total Matches
    public void SetApperanceTotalMatches(int TotalMatches)
    {
        Menu.Instance.Total_Matches.text = TotalMatches.ToString();
        Total_Matches_Count = TotalMatches;
    }

    // For KD Count
    public void SetApperanceKD(int KD)
    {
        Menu.Instance.KD_ratio.text = KD.ToString();
        KD_Count = KD;
    }

    // For Player Name
    public void SetApperancePlayerName()
    {
        Menu.Instance.PlayerName.text = Menu.Instance.player_nametext.text;
    }
}
