using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance; // Make Script Static

    public List<PlayerController> allplayer = new List<PlayerController>();

    public List<Gun> AllGunData; // All Guns Data

    public GameObject[] bombspawn,gunspawn;

    public Gun firstdefaltgun, seconddefaltgun;

    public Transform[] allpostion;

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

    GameObject privousgunspawn;
    IEnumerator GunSpawn()
    {
        yield return new WaitForSeconds(24f);
        if (PhotonNetwork.IsMasterClient)
        {
            int randomno = Random.Range(0, allpostion.Length);
            PhotonNetwork.Destroy(privousgunspawn);
            privousgunspawn = PhotonNetwork.Instantiate(gunspawn[Random.Range(0, gunspawn.Length)].name, allpostion[randomno].position, Quaternion.identity);
            StartCoroutine(GunSpawn());
        }
    }
    private void Start()
    {
        MainCamera = Camera.main;
        if (PhotonNetwork.InRoom)
        {
            Timer = DataShow.Instance.GameTime;
            StartCoroutine(GameTimer(Timer));
        }
        playerSpawn();
        StartCoroutine(GunSpawn());
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("other player left the room");
        
        base.OnPlayerLeftRoom(otherPlayer);
    }
    // Player Respawn after Death
    IEnumerator PlayerRespawn(PlayerController PlayerObject)
    { 
        if (PhotonNetwork.InRoom && PlayerObject.view.IsMine)
        {
            UIManager.instance.Pause.gameObject.SetActive(true);
        }
        else if(!PhotonNetwork.InRoom)
        {
            UIManager.instance.Pause.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(4);
        if (PhotonNetwork.InRoom)
        {
            UIManager.instance.Pause.gameObject.SetActive(false);


            GameObject Temp = PlayerObject.gameObject;

            Temp.transform.position = RespawnPoint[Random.Range(0, RespawnPoint.Count)].position;
            Temp.SetActive(true);
            firstdefaltgun.Initialize();
            seconddefaltgun.Initialize();
            PlayerObject.guns[0] = firstdefaltgun;
            
            PlayerObject.guns[1] = seconddefaltgun;

            PlayerObject.currentHealth = PlayerObject.maxHealth;
            PlayerObject.bombsamount[0] = 3;
            PlayerObject.bombsamount[1] = 3;
        
            PlayerObject.guns[1] = seconddefaltgun;
            UIManager.instance.UI_Updates();
            PlayerObject.UpdateHealthImage();
           // PlayerObject.Kill_Count = DataShow.Instance.This_Match_Kill_Count;
           // PlayerObject.death_count = DataShow.Instance.This_match_death_count;
           // Temp.GetComponent<PhotonView>().RPC("GettingData", RpcTarget.Others, Temp.GetComponent<PlayerController>().Kill_Count, Temp.GetComponent<PlayerController>().death_count);
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

            //  GameObject Temp1 = GameObject.FindGameObjectWithTag("Player");

            //  Destroy(Temp1);

            // GameObject Temp = Instantiate(PlayerPrefeb, RespawnPoint[Random.Range(0, RespawnPoint.Count)].position, Quaternion.identity);

            GameObject Temp = PlayerObject.gameObject;
            Temp.transform.position = RespawnPoint[Random.Range(0, RespawnPoint.Count)].position;
            Temp.SetActive(true);
            //Temp.tag = "Player";

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
        DataShow.Instance.This_Match_Kill_Count = 0;
        DataShow.Instance.This_match_death_count = 0;
        isTiming = false;
        PlayerManager.SoringPlayerBoard();
        PlayerManager.onGameOver();
        //SceneManager.LoadScene("Menu");
    }
   
    // Timer Showing in UI Manageer
    void TimerShowing()
    {
        if (isTiming)
        {
            Timer -= Time.deltaTime;
            float minutes = Mathf.FloorToInt(Timer / 60);
            float seconds = Mathf.FloorToInt(Timer % 60);
            UIManager.instance.Timer.text = string.Format("{00:00}:{01:00}",minutes,seconds);
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