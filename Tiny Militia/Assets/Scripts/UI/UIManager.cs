using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Variables For Showing Data in Canvas
    public GameObject Player_Life_Information; // Life Line Information of Player
    public Image boosterLevelImage; // Player Booster
    public Image healthImage; // Player Health
    public Image GunImage; // 
    public Image ReloadImage;
    public Image CurrentGunImage;
    public Button ReloadButton;
    public Button GunChange;
    public int GunIndex;
    public PlayerController playerController;
    public Canvas Pause;
    public TextMeshProUGUI AmmoInfo_text;
    public TextMeshProUGUI ScopeText;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI LifeCount;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Kill;
    public TextMeshProUGUI High_Score;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        playerController = GameManager.Instance.PlayerManager;
        if (SceneManager.GetActiveScene().name == "Survival_Bot")
        {
            Timer.gameObject.SetActive(false);
            Player_Life_Information.SetActive(true);
        }
        else
        {
            Timer.gameObject.SetActive(true);
            Player_Life_Information.SetActive(false);
        }
    }

    public void ReloadGun()
    {
        Gun currentgun;
        currentgun = playerController.guns[GunIndex];
        if (currentgun.currentAmmoInMagazine < currentgun.magazineSize)
        {
            playerController.reload_co = StartCoroutine(playerController.Reload(currentgun));
        }
    }

    public void Zoom()
    {
        Camera cam = Camera.main;
        Gun currentgun = playerController.guns[GunIndex];
        if (cam.orthographicSize < currentgun.maxScope)
        {
            cam.orthographicSize += 1;

        }
        else
        {
            cam.orthographicSize = 5;
        }
        ScopeText.text = (cam.orthographicSize - 4).ToString() + "x";
    }

    public void ThrowBomb()
    {
        PhotonView view = playerController.view;
        //playerController.ThrowBomb();

        if (PhotonNetwork.InRoom)
        {
            view.RPC("ThrowBomb", RpcTarget.All, view.ViewID);
        }
        else
        {
            playerController.ThrowBomb(view.ViewID);
        }
    }

    public void GunChange()
    {
        PhotonView view = playerController.view;
        //  playerController.


        if (PhotonNetwork.InRoom)
        {
            view.RPC("HandleGunSwitching", RpcTarget.All, view.ViewID);
        }
        else
        {
            playerController.HandleGunSwitching();
        }
    }

    public void PauseGame()
    {
        
        Score.text = playerController.Score_Count.ToString();
        Kill.text = playerController.Kill_Count.ToString();
        if (PlayerPrefs.GetInt("HighScore") > playerController.Score_Count)
        {
            PlayerPrefs.SetInt("HighScore", playerController.Score_Count);
            High_Score.text = PlayerPrefs.GetInt("HighScore").ToString();
        }

        Time.timeScale = 0;
        Pause.gameObject.SetActive(true);
    }

    public void PunchButton()
    {
        PhotonView view = playerController.view;
        playerController.isPunching = true;
        playerController.leftgunboneTransform.parent.transform.GetComponent<PolygonCollider2D>().enabled = true;
        StartCoroutine("PunchingCoroutine", 2f);
        if (PhotonNetwork.InRoom)
        {
            //playerController.TakeDamage(playerController.guns[playerController.currentGunIndex].damagePerBullet * 2,);
        }
        else
        {
            //playerController.TakeDamage(playerController.guns[playerController.currentGunIndex].damagePerBullet * 2,);
        }
    }

    public void BombChange()
    {

    }

    public void LeaveMatchButton()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }

    public void ContinueMatch()
    {
        Time.timeScale = 1f;
        Pause.gameObject.SetActive(false);
    }

    IEnumerator PunchingCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Punched Successfully");
        playerController.isPunching = false;
        playerController.leftgunboneTransform.parent.transform.GetComponent<PolygonCollider2D>().enabled = false;
    }

}