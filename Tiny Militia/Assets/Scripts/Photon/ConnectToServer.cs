using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    byte maxPlayer = 2;
    void Start()
    {
        // Connecting To Server at Starting
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Server Successfully");

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayer);
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnConnected()
    {
        base.OnConnected();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.CountOfPlayersInRooms == maxPlayer)
        {

        }
        base.OnJoinedRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }



}
