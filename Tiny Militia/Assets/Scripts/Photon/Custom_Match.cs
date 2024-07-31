using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class Custom_Match : MonoBehaviourPunCallbacks
{
    string RoomCode = "1234567";
    string characters = "ABCDEFGHIJKLMOPQRSTUVWXYZ";
    public TMP_InputField inputField_RoomCode;
    public void onGenerateRoom_Code()
    {
        char[] chars = RoomCode.ToCharArray();
        char[] characters2 = characters.ToCharArray();
        RoomCode = null;

        for(int k = 0; k < chars.Length; k++)
        {
            chars[k] = characters2[Random.Range(0, characters2.Length)];
            RoomCode += chars[k];
        }
        
        Debug.Log(RoomCode);

        List<RoomInfo> roomList = new List<RoomInfo>();

        for(int p = 0;p < PhotonNetwork.CountOfRooms;p++)
        {
            if (roomList[p].Name == RoomCode)
            {
                onGenerateRoom_Code();
                return;
            }   
        }

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 6;
        //options.IsOpen = false;
        //options.IsVisible = false;
        PhotonNetwork.CreateRoom(RoomCode,options);
        ConnectAndJoinRandom.Instance.view.RPC("PlayerJoined", RpcTarget.All);
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("Room Creation Successfully");
        base.OnCreatedRoom();   
    }

    public void onJoinedRoom_Code()
    {
        PhotonNetwork.JoinRoom(inputField_RoomCode.text);
        ConnectAndJoinRandom.Instance.view.RPC("PlayerJoined", RpcTarget.All);
    }
}
