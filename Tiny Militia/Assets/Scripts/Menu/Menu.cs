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


    [Header("HomePage Variables")]
    /* Home Page Variables */
    [SerializeField] private Transform Playbutton;
    [SerializeField] private Transform BattleButton, CustomButton, SurvivalMode;
    [SerializeField] private TMP_InputField Player_name;
    [SerializeField] public TextMeshProUGUI player_nametext;
    [SerializeField] private GameObject Playername_panel;
    [SerializeField] private GameObject nameErrorText;

    [Space(20)]
    [Header("ProfilePage Variables")]
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

    [Space(20)]
    [Header("MatchmakingPage Variables")]
    /* Matchmaking Page Variables */
    public TextMeshProUGUI MatchmakingTime_text;

    [Space(20)]
    [Header("Map Selection Page Variables")]
    public Slider TimerSlider;
    public TextMeshProUGUI TimerText;

    [Space(20)]
    [Header("Room Selection Page Variables")]
    public TextMeshProUGUI RoomCodeText;


    private void Awake()
    {
        Instance = this;
    }

    private void LoginWithCustomID()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, loginSuccess, PlayFab_Error);
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
        PlayfabManager.Instance.GetApperance();
    }

    public void playButton()
    {
        Playbutton.DOScale(Vector3.zero, .5f);
        BattleButton.DOMoveY(Playbutton.position.y, 1f);
        CustomButton.DOMoveY(Playbutton.position.y, 1f);
        SurvivalMode.DOMoveY(Playbutton.position.y, 1f);

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
                DisplayName = Player_name.text,

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

    public void SliderValueChange()
    {
        DataShow.Instance.GameTime = TimerSlider.value * 60;
        TimerText.text = TimerSlider.value.ToString() + " Minutes";
        Debug.Log(DataShow.Instance.GameTime);
    }

    public void ProfilePageOpen()
    {
        PlayfabManager.Instance.GetApperance();
        DataShow.Instance.Set_and_Show_ApperancePlayerName();
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

}