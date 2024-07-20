using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviourPunCallbacks
{

    [SerializeField] List<GameObject> LoadingList;
    [SerializeField] GameObject Canvas;

    private void Awake()
    {
        Random.Range(0, LoadingList.Count);
        Instantiate(LoadingList[LoadingList.Count],Vector3.zero,Quaternion.identity,Canvas.transform);

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
