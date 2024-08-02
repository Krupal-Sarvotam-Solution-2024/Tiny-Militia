using System;
using UnityEngine;
using System.Collections;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class ConnectAndJoinRandom : MonoBehaviourPunCallbacks
{

    public int selectedtime;
    public bool customRoom_selected;
    public static ConnectAndJoinRandom Instance;
    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = true;
    byte maxPlayer = 2;
    public byte Version = 1;
    public TextMeshProUGUI contingtext;
    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;
    public PhotonView view;
    public GameObject PlayerInformation;
    public GameObject PlayersList;
    public TextMeshProUGUI PlayerCount;
    bool isMatchMaking;
    float MatchMakingTime = 5f;
    public Custom_Match cumstomMatch;

    public void CustionRoom_selected()
    {
        customRoom_selected = true;
    }
    public void RandomJoin_selected()
    {
        customRoom_selected = false;
    }

    private void Awake()
    {
        Instance = this;
    }

    public virtual void Update()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.IsConnected)
        {
            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings();
        }
        if (isMatchMaking == true)
        {
            MatchMakingTime -= Time.deltaTime;
            Menu.Instance.MatchmakingTime_text.text = "Match Start in Just " + MatchMakingTime.ToString("00") + " Second";
        }
        else
        {
            MatchMakingTime = 4;
            Menu.Instance.MatchmakingTime_text.text = "Finding other players";
        }

    }


    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage



    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayer);
    }

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {

        /// relod will occur;
        Debug.LogError("Cause: " + cause);
    }

    public override void OnJoinedRoom()
    {
        view.RPC("PlayerJoined", RpcTarget.All);
    }

    public void CreateRoom()
    {
        //   PhotonNetwork.CreateRoom();
    }


    [PunRPC]
    void PlayerJoined()
    {
        for (int d = PlayersList.transform.childCount; d > 0; d--)
        {
            Destroy(PlayersList.transform.GetChild(d - 1).gameObject);
        }

        for (int k = 0; k < PhotonNetwork.PlayerList.Length; k++)
        {
            GameObject Temp = Instantiate(PlayerInformation);

            Temp.transform.parent = PlayersList.transform;

            Temp.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            Temp.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[k].NickName;

            PlayerCount.text = "Total Players : " + PhotonNetwork.PlayerList.Length.ToString();
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartCoroutine("GoToFight");
        }

        if(customRoom_selected && PhotonNetwork.IsMasterClient)
        {
            view.RPC("GetingMasterTime", RpcTarget.Others, DataShow.Instance.GameTime);
        }
    }



    [PunRPC]
    void GetingMasterTime(float time)
    {
        DataShow.Instance.GameTime = time;
    }

    public void Battle()
    {
        if (!customRoom_selected)
        {
            PhotonNetwork.JoinRandomRoom();
            DataShow.Instance.GameTime = 600;
        }
        else
        {
            
            cumstomMatch.onGenerateRoom_Code();


        }
    }

        IEnumerator GoToFight()
    {
        isMatchMaking = true;
        yield return new WaitForSeconds(3f);
        isMatchMaking = false;
        DataShow.Instance.Total_Matches_Count++;
        PlayfabManager.Instance.SaveApperance_TotalMatches(DataShow.Instance.Total_Matches_Count);
        SceneManager.LoadScene("Survival_PVP");
    }

    public void SurvivalOpen()
    {
        SceneManager.LoadScene("Survival_Bot");
    }

}
