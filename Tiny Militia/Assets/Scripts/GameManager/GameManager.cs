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
            GameObject Temp1 = Instantiate(PlayerPrefeb, RespawnPoint[Random.Range(0, RespawnPoint.Count)].position, Quaternion.identity);

            MainCamera.transform.position = new Vector3(Temp1.transform.position.x, Temp1.transform.position.y, Temp1.transform.position.z - 10);
            PlayerManager = Temp1.GetComponent<PlayerController>();
            UIManager.instance.playerController = PlayerManager;
            MainCamera.GetComponent<CameraController>().PlayerTransform = Temp1.transform;

        }
    }

    IEnumerator PlayerRespawn()
    {
        yield return new WaitForSeconds(4);
        Debug.Log("In Coroutine");
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
            GameObject Temp1 = GameObject.FindGameObjectWithTag("Player");
            Destroy(Temp1);
            GameObject Temp = Instantiate(PlayerPrefeb, RespawnPoint[Random.Range(0, RespawnPoint.Count)].position, Quaternion.identity);
            Temp.tag = "Player";
            MainCamera.transform.position = new Vector3(Temp.transform.position.x, Temp.transform.position.y, Temp.transform.position.z - 10);
            PlayerManager = Temp.GetComponent<PlayerController>();
            UIManager.instance.playerController = PlayerManager;
            MainCamera.GetComponent<CameraController>().PlayerTransform = Temp.transform;
        }
    }
}