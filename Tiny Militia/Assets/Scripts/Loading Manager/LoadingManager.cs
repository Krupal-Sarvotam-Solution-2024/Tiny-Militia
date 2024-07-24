using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class LoadingManager : MonoBehaviourPunCallbacks
{

    [SerializeField] List<GameObject> LoadingList;
    [SerializeField] GameObject Canvas;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        int Temp = Random.Range(0, LoadingList.Count);
        GameObject Temo1 = Instantiate(LoadingList[Temp]);
        Temo1.transform.parent = Canvas.transform;
        Temo1.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void OnConnectedToServer()
    {
        Debug.Log("Connected");

    }

   

    public override void OnConnectedToMaster()
    {
        
        SceneManager.LoadScene("Menu");
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.");

      
    }
}
