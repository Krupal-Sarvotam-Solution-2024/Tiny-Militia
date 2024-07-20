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

    public Image bomb_inage;

    public Image ReloadImage;
    
    public Image CurrentGunImage;
    
    public Button ReloadButton;
    
    public Button GunChangeButton;

    public Button PauseExitButton;
    
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

    public TextMeshProUGUI RespawnTime_Text;

   
    public TextMeshProUGUI BombAmount;

    [HideInInspector]
    public GunsData changingGunData;




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
    public void BombSwitch()
    {
        int crrent_bombtype = (int)playerController.selectedbomb;
        switch (crrent_bombtype)
        {
            case 0:
                playerController.selectedbomb = Bomb.bombtype.timebomb;
                break;
            case 1:
                playerController.selectedbomb = Bomb.bombtype.poisionbomb;
                break;
            case 2:
                playerController.selectedbomb = Bomb.bombtype.explodebomb;
                break;

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

    public void GunSwitch()
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

    public void GunChange()
    {
        //changingGunData.GunSprite = playerController.guns[playerController.currentGunIndex].GunSprite;
        changingGunData.GetComponent<SpriteRenderer>().sprite = playerController.guns[playerController.currentGunIndex].GunSprite;

        Gun TempGunData;

        TempGunData = playerController.guns[playerController.currentGunIndex];
        
        playerController.guns[playerController.currentGunIndex] = changingGunData.currentData;
        
        changingGunData.currentData = TempGunData;

        Gun currentGun = playerController.guns[playerController.currentGunIndex];
        
        playerController.leftgunTransform.GetComponent<SpriteRenderer>().sprite = currentGun.GunSprite;

        GunChangeButton.transform.GetChild(0).GetComponent<Image>().sprite = changingGunData.currentData.GunSprite;
        
        UI_Updates();
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
        PauseExitButton.gameObject.SetActive(true);
        RespawnTime_Text.gameObject.SetActive(false);
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

    public void UI_Updates()
    {
        AmmoInfo_text.text = playerController.guns[playerController.currentGunIndex].currentAmmoInMagazine.ToString() + " / " + playerController.guns[playerController.currentGunIndex].currentTotalAmmo.ToString();
        CurrentGunImage.GetComponent<Image>().sprite = playerController.guns[playerController.currentGunIndex].GunSprite;
        ScopeText.text = (Camera.main.orthographicSize - 4).ToString() + "x";
        Camera.main.orthographicSize = playerController.guns[playerController.currentGunIndex].maxScope;
    }

    IEnumerator PunchingCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Punched Successfully");
        playerController.isPunching = false;
        playerController.leftgunboneTransform.parent.transform.GetComponent<PolygonCollider2D>().enabled = false;
    }

}