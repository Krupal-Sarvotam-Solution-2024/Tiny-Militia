using System;
using UnityEngine;
using System.Collections;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
/// <summary>
/// This script automatically connects to Photon (using the settings file),
/// tries to join a random room and creates one if none was found (which is ok).
/// </summary>
public class ConnectAndJoinRandom : MonoBehaviourPunCallbacks
{
    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = true;
    byte maxPlayer = 2;
    public byte Version = 1;
    public TextMeshProUGUI contingtext;
    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;


    public virtual void Start()
    {
     
    }

    public virtual void Update()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.IsConnected)
        {
            Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");

            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings();
           
         //   PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, "1", "cluster3");       // connecting to a specific cluster may be necessary, when regions get sharded and you support friends
        }

        Debug.Log(PhotonNetwork.InRoom);
    }


    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage

    private void OnConnectedToServer()
    {
        Debug.Log("Connecte");
    }
    public override void OnConnectedToMaster()
    {
       PhotonNetwork.JoinRandomRoom();
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.");
      
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }



    public void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayer);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {

        /// relod will occur;
        Debug.LogError("Cause: " + cause);
    }

    public  override void OnJoinedRoom()
    {

        Debug.Log("player joined room");
    }
    public void Battle()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("player is trying to join room");
    }
    private void OnPlayerConnected()
    {
        Debug.Log("player entered");
    }
}
