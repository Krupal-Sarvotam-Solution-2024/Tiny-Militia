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
        GameObject Temp1 = Instantiate(LoadingList[Temp],Canvas.transform);
        Temp1.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void OnConnectedToServer()
    {
        
    }

   

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Menu");
    }
}
