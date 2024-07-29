using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance; // Make Script Static

    public List<Gun> AllGunData; // All Guns Data

    public List<Transform> RespawnPoint; // Respawn Point For Players

    public GameObject PlayerPrefeb; // Player's Prefeb for Respwaning

    public Camera MainCamera; // Main Camera For Following

    public PlayerController PlayerManager; // Player Controller

    public float Timer; // Timer which Selected by User For Multiplaying

    float RespawnTime = 4; // Float for Respawn time text

    public GameObject direction_Sound_Arrow;// the place where the opponet is shooting shows the direction

    bool isTiming; // Booolen for checking the timing method is true or not

    [HideInInspector]
    public bool isRespawning; // Boolen for reducing the timing of respawn time for text

    [HideInInspector]
    public bool isLifeLineOver; // Boolen for checking that player life line is over not
    
    [HideInInspector]
    public int isDead = 0; // Int for player death only 1 time

    [HideInInspector]
    public int Lifes = 3; // When players is in Survival Mode

    public GameObject TempPlayer;
    /* *-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----*-----* */

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Timer = DataShow.Instance.GameTime;
        MainCamera = Camera.main;
        if (PhotonNetwork.InRoom)
        {
            StartCoroutine(GameTimer(Timer));
        }
        playerSpawn();
    }

    private void Update()
    {
        TimerShowing();
        RespawningTimeShowing();
    }

    // Player Spawn First Time
    public void playerSpawn()
    {
        if (PhotonNetwork.InRoom)
        {
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

    // Player Respawn after Death
    IEnumerator PlayerRespawn(PlayerController PlayerObject)
    {
        if (PhotonNetwork.InRoom)
        {
            UIManager.instance.Pause.gameObject.SetActive(true);
        }
        else
        {
            UIManager.instance.Pause.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(4);
        if (PhotonNetwork.InRoom)
        {
            UIManager.instance.Pause.gameObject.SetActive(false);

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
            UIManager.instance.Pause.gameObject.SetActive(false);

            GameObject Temp1 = GameObject.FindGameObjectWithTag("Player");

            Destroy(Temp1);

            GameObject Temp = Instantiate(PlayerPrefeb, RespawnPoint[Random.Range(0, RespawnPoint.Count)].position, Quaternion.identity);

            Temp.tag = "Player";

            MainCamera.transform.position = new Vector3(Temp.transform.position.x, Temp.transform.position.y, Temp.transform.position.z - 10);

            PlayerManager = Temp.GetComponent<PlayerController>();

            MainCamera.GetComponent<CameraController>().PlayerTransform = Temp.transform;

            UIManager.instance.playerController = PlayerManager;

            isDead = 0;
        }
    }

    // Enumerator For Timing
    IEnumerator GameTimer(float time)
    {
        isTiming = true;
        yield return new WaitForSeconds(time);
        PhotonNetwork.LeaveRoom();
        isTiming = false;
        SceneManager.LoadScene("Menu");
    }

    // Timer Showing in UI Manageer
    void TimerShowing()
    {
        if (isTiming)
        {
            float minutes = Timer / 60;

            float seconds = Timer % 60;

            UIManager.instance.Timer.text = minutes.ToString("00") + " : " + seconds.ToString("00");
            Timer -= Time.deltaTime;

        }
    }

    void RespawningTimeShowing()
    {
        if (isRespawning)
        {
            UIManager.instance.RespawnTime_Text.text = "Respawn in " + RespawnTime.ToString("00") + " Second";
            RespawnTime -= Time.deltaTime;
            if (RespawnTime <= 0)
            {
                isRespawning = false;
                RespawnTime = 4;
            }
        }

        if (isLifeLineOver)
        {
            UIManager.instance.RespawnTime_Text.text = "Open Menu in " + RespawnTime.ToString("00") + " Second";
            RespawnTime -= Time.deltaTime;
            if (RespawnTime <= 0)
            {
                isLifeLineOver = false;
                SceneManager.LoadScene("Menu");
                RespawnTime = 4;
            }
        }
    }
}