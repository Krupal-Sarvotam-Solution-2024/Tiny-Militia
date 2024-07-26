using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void ProfileData_Showing()
    {
        var request = new GetAccountInfoRequest();
        Debug.Log(request.PlayFabId);
        Menu.Instance.PlayerName.text = request.Username;
    }


}
