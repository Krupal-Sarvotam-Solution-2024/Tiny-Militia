using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public List<Gun> AllGunData;
    public List<Transform> RespawnPoint;
    public GameObject PlayerPrefeb;
    public Camera MainCamera;
    public PlayerController PlayerManager;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MainCamera = Camera.main;
        playerSpawn();
    }

    public void playerSpawn()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("InRoom");
            GameObject Temp = PhotonNetwork.Instantiate(PlayerPrefeb.name, RespawnPoint[Random.Range(0, RespawnPoint.Count)].position, Quaternion.identity);
            if (Temp.GetComponent<PhotonView>().IsMine)
            {
                MainCamera.transform.position = new Vector3(Temp.transform.position.x, Temp.transform.position.y, Temp.transform.position.z - 10);
                PlayerManager = Temp.GetComponent<PlayerController>();
                UIManager.instance.playerController = PlayerManager;
                MainCamera.GetComponent<CameraController>().PlayerTransform = Temp.transform;
            }
        }
        else
        {
            GameObject Temp = Instantiate(PlayerPrefeb, RespawnPoint[Random.Range(0, RespawnPoint.Count)].position, Quaternion.identity);

            MainCamera.transform.position = new Vector3(Temp.transform.position.x, Temp.transform.position.y, Temp.transform.position.z - 10);
            PlayerManager = Temp.GetComponent<PlayerController>();
            UIManager.instance.playerController = PlayerManager;
            MainCamera.GetComponent<CameraController>().PlayerTransform = Temp.transform;

        }

    }
}
