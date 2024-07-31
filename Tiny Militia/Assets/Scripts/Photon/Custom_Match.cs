using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Custom_Match : MonoBehaviourPunCallbacks
{
    string RoomCode = "1234567";
    string characters = "ABCDEFGHIJKLMOPQRSTUVWXYZ0123456789";

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
        options.IsOpen = false;
        options.IsVisible = false;
        PhotonNetwork.CreateRoom(RoomCode,options);

    }
}
