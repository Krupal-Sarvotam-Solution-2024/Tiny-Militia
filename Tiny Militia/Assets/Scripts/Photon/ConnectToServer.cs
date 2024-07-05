using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    byte maxPlayer = 2;
    public float version;
    public float timetoLoad;
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
        // player joined the loby can see the room
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
 
    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.CountOfPlayersInRooms == maxPlayer)
        {

        }
   
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }



}
