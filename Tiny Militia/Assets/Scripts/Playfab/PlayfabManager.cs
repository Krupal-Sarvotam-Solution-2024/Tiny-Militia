using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using System;
using JetBrains.Annotations;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager Instance;



    private void Awake()
    {
        Instance = this;
        if (Instance != null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GetApperance()
    {

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecived, OnError);
        //SaveApperance_KD(5 / DataShow.Instance.Total_Death_Count);// Call this Mwthod Where Kill and Death Count Is Plus 1
    }

    void OnDataRecived(GetUserDataResult result)
    {
        Debug.Log("Recived user data!");
        if (result.Data != null && result.Data.ContainsKey("HighScore"))
        {
            DataShow.Instance.Set_and_Show_ApperanceHighScore(int.Parse(result.Data["HighScore"].Value));
        }

        if (result.Data != null && result.Data.ContainsKey("Total_Matches"))
        {
            DataShow.Instance.Set_and_Show_ApperanceTotalMatches(int.Parse(result.Data["Total_Matches"].Value));

        }

        if (result.Data != null && result.Data.ContainsKey("Total_Deaths"))
        {
            DataShow.Instance.Set_and_Show_ApperanceTotalDeaths(int.Parse(result.Data["Total_Deaths"].Value));

        }

        if (result.Data != null && result.Data.ContainsKey("KD_Ratio"))
        {
            DataShow.Instance.Set_and_Show_ApperanceKD(float.Parse(result.Data["KD_Ratio"].Value));
        }

        if (result.Data != null && result.Data.ContainsKey("Total_Kills"))
        {
            DataShow.Instance.Set_and_Show_ApperanceTotalKills(int.Parse(result.Data["Total_Kills"].Value));

        }
    }

    // Save the HighScore Data to the Server
    public void SaveApperance_HighScore(int HighScore)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
                {
                    {"HighScore", HighScore.ToString() },
                }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    // Save the Total Match Played by User in Server
    public void SaveApperance_TotalMatches(int TotalMatches)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
                {
                    {"Total_Matches", TotalMatches.ToString() },
                }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    // Save the Total Death Count of User
    public void SaveApperance_TotalDeath(int TotalDeaths)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
                {
                    {"Total_Deaths", TotalDeaths.ToString() },
                }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    // Save the Total Death Count of User
    public void SaveApperance_KD(float KD)
    {
        Debug.Log("in Save KD");
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
                {
                    {"KD_Ratio", KD.ToString() },
                }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void SaveApperance_KillCount(int TotalKills)
    {
        Debug.Log("Saved");
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Total_Kills",TotalKills.ToString() },
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    // When Getting Error on Get Data From Server
    private void OnError(PlayFabError error)
    {
        throw new NotImplementedException();
    }

    // When Get Data Successfully From Server
    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Result of Getting Data :\n" + result);
    }
}