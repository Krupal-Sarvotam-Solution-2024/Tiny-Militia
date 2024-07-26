using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class Menu : MonoBehaviour
{
    public static Menu Instance;


    /* Home Page Variables */
    [SerializeField] private Transform battlebutton, customBattle, Servivalmode;
    [SerializeField] private Transform Playbutton;
    [SerializeField] private TMP_InputField Player_name;
    [SerializeField] private TextMeshProUGUI player_nametext;
    [SerializeField] private GameObject Playername_panel;
    [SerializeField] private GameObject nameErrorText;

    /* Profile Page Variables */
    public Image PlayerRankImage;
    public TextMeshProUGUI PlayerRank;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI Total_KillCount;
    public TextMeshProUGUI High_Score;
    public TextMeshProUGUI Total_Matches;
    public TextMeshProUGUI Total_DeathCount;
    public TextMeshProUGUI Win_Matches;
    public TextMeshProUGUI KD_ratio;
    

    private void Awake()
    {
        Instance = this;
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true,
            }

        };
        PlayFabClientAPI.LoginWithCustomID(request, loginSuccess, PlayFab_Error);
    }

    public void playButton()
    {
        Playbutton.DOScale(Vector3.zero, .5f);
        battlebutton.DOMoveY(Playbutton.position.y, 1f);
        customBattle.DOMoveY(Playbutton.position.y, 1f);
        Servivalmode.DOMoveY(Playbutton.position.y, 1f);

    }

    public void NamingUser()
    {
        if (Player_name.text.Length > 3 || Player_name.text.Length < 15)
        {
            PhotonNetwork.NickName = Player_name.text;
            Playername_panel.SetActive(false);
            player_nametext.text = PhotonNetwork.NickName;
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = Player_name.text

            }; PlayFabClientAPI.UpdateUserTitleDisplayName(request, NameData_addedSuccessfully, PlayFab_Error);
            nameErrorText.SetActive(false);

        }
        else
        {

            nameErrorText.SetActive(true);
        }

    }

    private void NameData_addedSuccessfully(UpdateUserTitleDisplayNameResult obj)
    {
        Debug.Log("Name Successfully Saved");
        throw new NotImplementedException();
    }

    private void PlayFab_Error(PlayFabError obj)
    {
        Debug.Log("ERRORS");

        throw new NotImplementedException();
    }

    private void loginSuccess(LoginResult obj)
    {
        string name;
        name = null;
        Debug.Log(obj.InfoResultPayload);
        if (obj.InfoResultPayload.PlayerProfile != null)
        {
            name = obj.InfoResultPayload.PlayerProfile.DisplayName;
        }

        if (name == null)
        {
            Playername_panel.SetActive(true);
            PhotonNetwork.NickName = name;

        }
        else
        {
            PhotonNetwork.NickName = name;
            Playername_panel.SetActive(false);
            player_nametext.text = name;
        }
    }

    public void ProfilePageOpen()
    {
        PlayfabManager.Instance.ProfileData_Showing();
    }

}