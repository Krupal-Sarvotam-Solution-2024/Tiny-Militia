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
    }

    void OnDataRecived(GetUserDataResult result)
    {
        Debug.Log("Recived user data!");
        if(result.Data != null && result.Data.ContainsKey("HighScore"))
        {
            DataShow.Instance. SetApperanceHighScore(int.Parse(result.Data["HighScore"].Value));
        }

    }

    public void SaveApperance(int HighScore)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
                {
                    {"HighScore", HighScore.ToString() }
                }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("Data didn't Send Successfully");
        throw new NotImplementedException();
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Data send successfully");
    }


}
