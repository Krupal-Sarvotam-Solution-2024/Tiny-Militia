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
using System.Threading.Tasks;

public class Menu : MonoBehaviour
{
    [SerializeField] private Transform battlebutton, customBattle, Servivalmode;

    [SerializeField] private Transform Playbutton;
    [SerializeField] private TMP_InputField Player_name;
    [SerializeField] private TextMeshProUGUI player_nametext;
    [SerializeField] private GameObject Playername_panel;
    [SerializeField] private GameObject nameErrorText;
    

    private void Awake()
    {
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

    // Update is called once per frame
    void Update()
    {

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
        Debug.Log("Login Sucessfull");
    }

}